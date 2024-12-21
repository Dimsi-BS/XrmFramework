using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Newtonsoft.Json;

namespace $safeprojectname$.Extensions;

public class LoggingCommandPipelineBehavior<TRequest, TResponse>(
    ILogger<LoggingCommandPipelineBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received payload {Name} : {SerializedObject}", request.GetType().Name, JsonConvert.SerializeObject(request));

        var result = await next();
            
        logger.LogInformation("Result sent : {SerializedObject}", JsonConvert.SerializeObject(result));

        return result;
    }
}
