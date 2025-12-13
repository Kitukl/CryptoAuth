using CryptoAuth.BLL.DTOs;
using MediatR;

namespace CryptoAuth.BLL.Commands.RefreshTokenCommandHandler;

public record RefreshTokenCommand(string token) : IRequest<Result<LoginResponse>>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
{
    private readonly JWTProvider _provider;

    public RefreshTokenCommandHandler(JWTProvider provider)
    {
        _provider = provider;
    }
    public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _provider.ValidateRefreshToken(request.token);
    }
}