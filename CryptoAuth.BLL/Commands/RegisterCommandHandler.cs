using System.Text;
using CryptoAuth.BLL.DTOs;
using CryptoAuth.BLL.Validations;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace CryptoAuth.BLL.Commands.RegisterCommandHandler;

public record RegisterCommand(RegisterRequest registerRequest) : IRequest<Result<string>>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RegisterValidator _validator;
    private readonly IEmailSender _sender;
    private readonly FrontEndOptions _options;

    public RegisterCommandHandler(UserManager<IdentityUser> userManager, RegisterValidator validator, IEmailSender sender, IOptions<FrontEndOptions> options)
    {
        _userManager = userManager;
        _validator = validator;
        _sender = sender;
        _options = options.Value;
    }
    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request.registerRequest);

        if (!validation.IsValid)
        {
            var errors = string.Join("\n", validation.Errors.Select(e => e.ErrorMessage)); 
            
            return Result<string>.Failure(errors);
        }
        
        var user = new IdentityUser
        {
            UserName = request.registerRequest.Username,
            Email = request.registerRequest.Email
        };

        var result = await _userManager.CreateAsync(user, request.registerRequest.Password);
        
        if (!result.Succeeded)
        {
            var errors = string.Join("\n", result.Errors.Select(e => e.Description));
            
            return Result<string>.Failure(errors);
            
        }
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var frontEndUrl = _options.Host;        
        var confirmationUrl = $"{frontEndUrl}/confirm-email?userId={user.Id}&token={token}";
        await _sender.SendEmailAsync(user.Email, "Підтвердження пошти", confirmationUrl);
        
        return Result<string>.Success(user.Id);
    }
}

public class FrontEndOptions
{
    public string Host { get; set; }
}