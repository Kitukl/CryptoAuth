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
    private readonly UserManager<User> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IRepository<ResetPasswordCode> _repository;

    public ForgotPasswordCommandHandler(UserManager<User> userManager, IEmailSender emailSender, IRepository<ResetPasswordCode> repository)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _repository = repository;
    }
    public async Task<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.email);
        if (user is null) return Result<string>.Failure("User not found!");

        var token = await _repository.GetByEmailAsync(user.Email);

        if (token is not null)
        {
            await _repository.DeleteAsync(token.Code);
        }

        var code = RandomNumberGenerator.GetInt32(900000) + 100000;
        
        await _emailSender.SendEmailAsync(user.Email, "Відновлення паролю", code.ToString());

        var codeEntity = new ResetPasswordCode()
        {
            Code = code.ToString(),
            UserEmail = user.Email,
            Expire = DateTime.UtcNow.AddMinutes(5)
        };
        await _repository.CreateAsync(codeEntity);
        
        return Result<string>.Success("Code sent!");
    }
}