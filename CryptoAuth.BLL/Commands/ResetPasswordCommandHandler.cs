using CryptoAuth.BLL.DTOs;
using CryptoAuth.DAL.Entities;
using CryptoAuth.DAL.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CryptoAuth.BLL.Commands;

public record ResetPasswordCommand(RessetPasswordRequest ressetPassword) : IRequest<Result<string>>; 

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<string>>
{
    private readonly IRepository<ResetPasswordCode> _repository;
    private readonly UserManager<IdentityUser> _userManager;

    public ResetPasswordCommandHandler(IRepository<ResetPasswordCode> repository, UserManager<IdentityUser> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }
    public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.ressetPassword.Email);
        
        if (user is null) return Result<string>.Failure("User not found");
        
        var code = await _repository.GetByIdAsync(request.ressetPassword.Code);

        if (code.UserEmail != request.ressetPassword.Email || code.Code != request.ressetPassword.Code || code.Expire < DateTime.UtcNow) return Result<string>.Failure("Not correct code or expered!");

       await _repository.DeleteAsync(code.Code);

        var removePassword = await _userManager.RemovePasswordAsync(user);
        if (!removePassword.Succeeded) return Result<string>.Failure("Remove password failed!");

        var addPassword = await _userManager.AddPasswordAsync(user, request.ressetPassword.NewPassword);
        if (!addPassword.Succeeded) return Result<string>.Failure("Add password failed!");
        
        return Result<string>.Success($"Reset password for {user.Email} - successfully!");
    }
}