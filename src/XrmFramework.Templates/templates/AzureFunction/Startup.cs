using $safeprojectname$.Commands;
using $safeprojectname$.Extensions;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;

namespace $safeprojectname$
{
    public class Startup : IFunctionAppConfiguration
{
    public void Build(IFunctionHostBuilder builder) => builder
            .Setup((serviceCollection, commandRegistry) =>
            {
                commandRegistry.Discover<Startup>();

                serviceCollection
                    .AddValidatorsFromAssemblyContaining<Startup>();

                serviceCollection
                    .Replace(new ServiceDescriptor(
                        typeof(ICommandDispatcher),
                        typeof(LoggingCommandDispatcher),
                        ServiceLifetime.Transient));

                serviceCollection.AddXrmFramework(opt =>
                    opt
                        .UseConnectionString(Environment.GetEnvironmentVariable("Xrm"))
                        .UseWebApi(false));
            })
            .DefaultHttpResponseHandler<XrmFrameworkHttpResponseHandler>()
            .AddFluentValidation()
            .OpenApiEndpoint(openApi => openApi
                .Title("$safeprojectname$")
                .Version(ThisAssembly.AssemblyVersion)
                .UserInterface("/swagger")
            )
            .Functions(functions => functions
                .Timer<EveryTenMinutesTimerCommand>("0 */10 * * * *")
                .HttpRoute("/v1", function => function

                    .HttpFunction<HealthTestCommand>("/health", AuthorizationTypeEnum.Anonymous, HttpMethod.Post)
                )
            );
}
}
