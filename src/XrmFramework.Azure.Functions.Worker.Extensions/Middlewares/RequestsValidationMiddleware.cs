using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Newtonsoft.Json;
using XrmFramework.Azure.Functions.Worker.Extensions.OpenApi;

namespace XrmFramework.Azure.Functions.Worker.Extensions.Middlewares;

public class RequestsValidationMiddleware(JsonSerializerSettings jsonSerializerSettings): IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (FluentValidation.ValidationException ex)
        {
            var httpReqData = await context.GetHttpRequestDataAsync();

            if (httpReqData != null)
            {
                // Create an instance of HttpResponseData with 400 status code.
                var newHttpResponse = httpReqData.CreateResponse();
                newHttpResponse.StatusCode = HttpStatusCode.BadRequest;
                newHttpResponse.Headers.Add("Content-Type", "application/json");

                var errorResult = new ValidationErrors
                {
                    Errors = ex.Errors.Select(x => new Error()
                    {
                        ErrorCode = x.ErrorCode,
                        Message = x.ErrorMessage,
                        Property = x.PropertyName
                    }).ToArray()
                };
                
                var errorContent = JsonConvert.SerializeObject(errorResult, jsonSerializerSettings);

                await newHttpResponse.WriteStringAsync(errorContent, context.CancellationToken);

                SetResponse(context, newHttpResponse);
            }
        }
    }

    private static void SetResponse(FunctionContext context, HttpResponseData newHttpResponse)
    {
        var invocationResult = context.GetInvocationResult();

        var httpOutputBindingFromMultipleOutputBindings = GetHttpOutputBindingFromMultipleOutputBinding(context);
        if (httpOutputBindingFromMultipleOutputBindings is not null)
        {
            httpOutputBindingFromMultipleOutputBindings.Value = newHttpResponse;
        }
        else
        {
            invocationResult.Value = newHttpResponse;
        }
    }
    
    private static OutputBindingData<HttpResponseData>? GetHttpOutputBindingFromMultipleOutputBinding(FunctionContext context)
    {
        // The output binding entry name will be "$return" only when the function return type is HttpResponseData
        var httpOutputBinding = context.GetOutputBindings<HttpResponseData>()
            .FirstOrDefault(b => b.BindingType == "http" && b.Name != "$return");

        return httpOutputBinding;
    }
}
