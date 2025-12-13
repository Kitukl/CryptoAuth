using CryptoAuth.DAL.Entities;
using CryptoAuth.DAL.Repositories;
using MediatR;

namespace CryptoAuth.BLL.Commands;

public record LogoutCommand(string refreshToken) : IRequest<Result<string>>;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<string>>
{
    private readonly IRepository<RefreshToken> _repository;

    public LogoutCommandHandler(IRepository<RefreshToken> repository)
    {
        _repository = repository;
    }
    public async Task<Result<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.refreshToken);
        return Result<string>.Success(result);
    }
}