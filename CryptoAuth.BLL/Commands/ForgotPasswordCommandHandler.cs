using System.Security.Cryptography;
using CryptoAuth.DAL.Entities;
using CryptoAuth.DAL.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CryptoAuth.BLL.Commands;

public record ForgotPasswordCommand(string email) : IRequest<Result<string>>;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<string>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IRepository<ResetPasswordCode> _repository;

    public ForgotPasswordCommandHandler(UserManager<IdentityUser> userManager, IEmailSender emailSender, IRepository<ResetPasswordCode> repository)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _repository = repository;
    }
    public async Task<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.email);
        if (user is null) return Result<string>.Failure("User not found!");

        var code = RandomNumberGenerator.GetInt32(900000) + 100000;
        string message = $"Ваш код для відновлення паролю: {code}, дійсний протягом 2 хвилин";
        
        await _emailSender.SendEmailAsync(user.Email, "Відновлення паролю", message);

        var codeEntity = new ResetPasswordCode()
        {
            Code = code.ToString(),
            UserEmail = user.Email,
            Expire = DateTime.UtcNow.AddMinutes(2)
        };
        await _repository.CreateAsync(codeEntity);
        
        return Result<string>.Success("Code sent!");
    }
}