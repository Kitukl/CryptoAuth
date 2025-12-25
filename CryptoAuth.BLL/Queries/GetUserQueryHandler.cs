using CryptoAuth.BLL.DTOs;
using CryptoAuth.DAL.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CryptoAuth.BLL.Queries;

public record GetUserQuery(string email) : IRequest<Result<UserResponse>>;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserResponse>>
{
    private readonly UserManager<User> _userManager;

    public GetUserQueryHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.email);
        if (user is not null)
        {
            var result = new UserResponse
            {
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                UserName = user.UserName
            };
            return Result<UserResponse>.Success(result);
        }
        
        return Result<UserResponse>.Failure("User not found!");
    }
}