using System.Text;
using CryptoAuth.BLL.DTOs;
using CryptoAuth.DAL.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CryptoAuth.BLL.Commands;

public record ConfirmEmailCommand(ConfirmEmailRequest request) : IRequest<Result<string>>;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<string>>
{
    private readonly UserManager<User> _userManager;

    public ConfirmEmailCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    public async Task<Result<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.request.userID);

        if (user == null)
        {
            return Result<string>.Failure("User not found");
        }

        if (user.EmailConfirmed)
        {
            return Result<string>.Failure("User already confirmed email");
        }

        var decodedToken = WebEncoders.Base64UrlDecode(request.request.token);
        var originalToken = Encoding.UTF8.GetString(decodedToken);
        
        var result = await _userManager.ConfirmEmailAsync(user, originalToken);

        if (!result.Succeeded)
        {
            return Result<string>.Failure(string.Join("\n", result.Errors.Select(e => e.Description)));
        }
        return Result<string>.Success(result.ToString());
    }
}