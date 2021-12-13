using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XrmFramework.AzureFunctions;
using XrmFramework.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace FunctionMonkey.FluentValidation
{
    public static class XrmFrameworkFunctionHostBuilderExtensions
    {
        public static IFunctionHostBuilder AddXrmFramework(
            this IFunctionHostBuilder functionHostBuilder, Action<IXrmFrameworkFunctionOptionBuilder> builder)
            => functionHostBuilder
                .Setup(serviceCollection =>
                    serviceCollection.AddXrmFramework((Action<IXrmFrameworkOptionBuilder>) builder))
                .AddFluentValidation();
    }
}

