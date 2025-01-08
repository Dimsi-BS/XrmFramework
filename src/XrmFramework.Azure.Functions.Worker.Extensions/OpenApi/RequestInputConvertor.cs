using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Azure.Functions.Worker.Converters;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace XrmFramework.Azure.Functions.Worker.Extensions.OpenApi;

public class RequestInputConvertor : IInputConverter
{
    public async ValueTask<ConversionResult> ConvertAsync(ConverterContext context)
    {
        if (!context.TargetType.IsAssignableTo(typeof(IBaseRequest)))
        {
            return ConversionResult.Unhandled();
        }

        if (context.FunctionContext.Items.TryGetValue("HttpRequestContext", out var item) && item is HttpContext httpContext)
        {
            try
            {
                var serializerSettings = (JsonSerializerSettings)httpContext.RequestServices.GetRequiredService(typeof(JsonSerializerSettings));
                var modelMetadataProvider = (IModelMetadataProvider)httpContext.RequestServices.GetRequiredService(typeof(IModelMetadataProvider));

                object? result;

                var hasBody = httpContext.Request.Method != "GET" && httpContext.Request.Method != "DELETE";

                if (hasBody)
                {
                    var stringContent = await httpContext.Request.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject(stringContent, context.TargetType, serializerSettings);
                }
                else
                {
                    result = Activator.CreateInstance(context.TargetType);
                }

                modelMetadataProvider
                    .GetMetadataForProperties(context.TargetType)
                    .OfType<DefaultModelMetadata>()
                    .ToList()
                    .ForEach(metadata =>
                {
                    if (
                        (!hasBody 
                         || 
                         metadata.Attributes.PropertyAttributes
                             .Any(p => p is FromRouteAttribute || p is FromQueryAttribute))
                        && result != null && context.FunctionContext.BindingContext.BindingData
                            .TryGetValue(metadata.PropertyName, out var value))
                    {
                        metadata.PropertySetter?.Invoke(result, Convert.ChangeType(value, metadata.ModelType));
                    }
                });

                return ConversionResult.Success(result);
            }
            catch (Exception ex)
            {
                return ConversionResult.Failed(ex);
            }
        }

        return ConversionResult.Unhandled();
    }
}
