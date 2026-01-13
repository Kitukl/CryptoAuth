using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CryptoAuth.BLL.Commands.RegisterCommandHandler;
using CryptoAuth.BLL.DTOs;
using CryptoAuth.DAL.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace CryptoAuth.BLL.Commands;

public record UpdateUserCommand(UpdateUserRequest request) : IRequest<Result<UserResponse>>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserResponse>>
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly UserManager<User> _userManager;
    private readonly IEmailSender _sender;
    private readonly FrontEndOptions _options;

    public UpdateUserCommandHandler(BlobServiceClient blobServiceClient, UserManager<User> userManager, IEmailSender sender, IOptions<FrontEndOptions> options)
    {
        _blobServiceClient = blobServiceClient;
        _userManager = userManager;
        _sender = sender;
        _options = options.Value;
    }
    public async Task<Result<UserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.request.CurrentEmail);
        if (user is null) return Result<UserResponse>.Failure("User not found!");

        if (user.Email != request.request.NewEmail)
        {
            user.Email = request.request.NewEmail;
            user.EmailConfirmed = false;
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var frontEndUrl = _options.Host;        
            var confirmationUrl = $"{frontEndUrl}/confirm-email?userId={user.Id}&token={token}";
            await _sender.SendEmailAsync(user.Email, "Підтвердження пошти", confirmationUrl);
        }
        
        if (request.request.AvatarUrl != null)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("avatars");
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            var blobClient = containerClient.GetBlobClient($"{user.Id}{Path.GetExtension(request.request.AvatarUrl.FileName)}");

            using var stream = request.request.AvatarUrl.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = request.request.AvatarUrl.ContentType }, cancellationToken: cancellationToken);

            user.AvatarUrl = blobClient.Uri.ToString().Replace("azurite_blob", "localhost");
        }
        
        user.UserName = request.request.UserName;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return Result<UserResponse>.Failure("Failed to update user: " + string.Join(", ", updateResult.Errors.Select(e => e.Description)));
        }
        
        var userResponse = new UserResponse
        {
            UserName = user.UserName,
            AvatarUrl = user.AvatarUrl,
            Email = user.Email
        };
        
        return Result<UserResponse>.Success(userResponse);
    }
}