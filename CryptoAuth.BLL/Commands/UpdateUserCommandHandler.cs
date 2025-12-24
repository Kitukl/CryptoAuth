using MediatR;

namespace CryptoAuth.BLL.Commands;

public record UpdateUserCommand() : IRequest<Result<string>>;

public class UpdateUserCommandHandler
{
    
}