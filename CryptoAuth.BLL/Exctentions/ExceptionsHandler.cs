using MediatR;

namespace CryptoAuth.BLL.Exctentions;

public class ExceptionsHandler<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TResponse : Result, new()
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception exception)
        {
            var result = new TResponse
            {
                isSuccess = false   
            };

            result.Errors += exception.Message;

            return result;
        }
    }
}