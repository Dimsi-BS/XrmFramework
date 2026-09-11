using System.ServiceModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;

namespace $safeprojectname$.Extensions;

internal class XrmFrameworkHttpResponseHandler(
    ILogger<XrmFrameworkHttpResponseHandler> logger
) : IFunctionsWorkerMiddleware
{

    public Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            return next(context);
        }
        catch (Exception ex)
        {
            var innerMostException = ex;
            
            IActionResult response = null;

            while (innerMostException.InnerException != null)
            {
                innerMostException = innerMostException.InnerException;
            }

            switch (innerMostException)
            {
                case FaultException<OrganizationServiceFault> fault:
                    logger.LogError(ex, "Erreur 400 : {FaultDetail}", fault.Detail);

                    response = new BadRequestObjectResult(fault.Detail);
                    break;
            }

            logger.LogError(ex,
                "Exception : {Message} ({RequestType}) {StackTrace}, InnerException : {InnerMessage} ({InnerRequestType}) {InnerStackTrace}",
                ex.Message, ex.GetType().FullName, ex.StackTrace,
                innerMostException.Message,
                innerMostException.GetType().FullName,
                innerMostException.StackTrace);

            response ??= new BadRequestObjectResult(ex.Message);
            
            return Task.FromResult(response);
        }
    }
}


