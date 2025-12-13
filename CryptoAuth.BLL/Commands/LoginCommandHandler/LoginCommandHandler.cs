using CryptoAuth.BLL.DTOs;
using CryptoAuth.DAL.Entities;
using CryptoAuth.DAL.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CryptoAuth.BLL.Commands.LoginCommandHandler;

public record LoginCommand(LoginRequest loginRequest) : IRequest<Result<LoginResponse>>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JWTProvider _provider;
    private readonly IRepository<RefreshToken> _repository;

    public LoginCommandHandler(UserManager<IdentityUser> userManager, JWTProvider provider, IRepository<RefreshToken> repository)
    {
        _userManager = userManager;
        _provider = provider;
        _repository = repository;
    }
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.loginRequest.Email);
        if (user is null) return Result<LoginResponse>.Failure("User not found!");
        if (await _userManager.IsLockedOutAsync(user)) return Result<LoginResponse>.Failure("User blocked!");

        var isEqual = await _userManager.CheckPasswordAsync(user, request.loginRequest.Password);

        if (!isEqual) return Result<LoginResponse>.Failure("Passwords not equal!");

        return Result<LoginResponse>.Success(new LoginResponse()
        {
            AccesToken = _provider.GenerateToken(user),
            RefreshToken = await _provider.GenerateRefreshToken(user)
        });
    }
}