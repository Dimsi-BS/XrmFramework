using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XrmFramework.AzureFunctions;
using XrmFramework.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class XrmFrameworkOptionBuilderExtensions
    {
        public static XrmFrameworkOptionBuilder AddAzureFunctions(this XrmFrameworkOptionBuilder builder, Action<IFunctionBuilder> functionBuilder)
        {
            builder
                .ServiceCollection
                .Replace(new ServiceDescriptor(
                    typeof(ICommandDispatcher),
                    typeof(LoggingCommandDispatcher),
                    ServiceLifetime.Transient));

            return builder;
        }
    }
}
