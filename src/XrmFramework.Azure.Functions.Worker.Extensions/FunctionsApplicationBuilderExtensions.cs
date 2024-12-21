using Azure.Functions.Worker.Extensions.HttpApi.Config;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using XrmFramework.Azure.Functions.Worker.Extensions.Configuration;
using XrmFramework.Azure.Functions.Worker.Extensions.Middlewares;
using XrmFramework.Azure.Functions.Worker.Extensions.OpenApi;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.Functions.Worker.Builder;

public static class FunctionsApplicationBuilderExtensions
{
    public static FunctionsApplicationBuilder? AddXrmFramework(this FunctionsApplicationBuilder? builder, Action<XrmFrameworkOptionsBuilder>? configureOptions = null)
    {
        var xrmFrameworkOptions = new XrmFrameworkOptions();
        var optionsBuilder = new XrmFrameworkOptionsBuilder(xrmFrameworkOptions);
        
        configureOptions?.Invoke(optionsBuilder);
        
        if (builder != null)
        {
            builder.AddHttpApi();
            builder.Services.AddMediatR(options =>
                options.RegisterServicesFromAssemblies(xrmFrameworkOptions.MediatRAssemblies ?? AppDomain.CurrentDomain.GetAssemblies()));

            builder.Services.AddFluentValidation(xrmFrameworkOptions.FluentValidationAssemblies ?? AppDomain.CurrentDomain.GetAssemblies());

            builder.UseWhen<RequestsValidationMiddleware>(context =>
                context.FunctionDefinition.InputBindings.Any(b => b.Value.Type == "httpTrigger"));
            
            builder.Services.RemoveAll(typeof(IOpenApiHttpTriggerContext));
            builder.Services.AddSingleton<IOpenApiHttpTriggerContext, CustomOpenApiHttpTriggerContext>();
        }

        return builder;
    }
}
