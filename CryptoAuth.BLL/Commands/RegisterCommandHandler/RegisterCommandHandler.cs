using CryptoAuth.BLL.DTOs;
using CryptoAuth.BLL.Validations;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CryptoAuth.BLL.Commands.RegisterCommandHandler;

public record RegisterCommand(RegisterRequest registerRequest) : IRequest<Result<string>>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RegisterValidator _validator;

    public RegisterCommandHandler(UserManager<IdentityUser> userManager, RegisterValidator validator)
    {
        _userManager = userManager;
        _validator = validator;
    }
    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request.registerRequest);

        if (!validation.IsValid)
        {
            var errors = string.Join("\n", validation.Errors.Select(e => e.ErrorMessage)); 
            
            return Result<string>.Failure(errors);
        }
        
        var user = new IdentityUser()
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
        return Result<string>.Success(user.Id);
    }
}