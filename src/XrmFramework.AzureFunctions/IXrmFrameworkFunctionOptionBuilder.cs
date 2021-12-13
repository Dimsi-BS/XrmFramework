using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XrmFramework.DependencyInjection;

namespace XrmFramework.AzureFunctions
{
    public interface IXrmFrameworkFunctionOptionBuilder : IXrmFrameworkOptionBuilder
    {
        IXrmFrameworkOptionBuilder AddDefaultCommandLogging();

        IXrmFrameworkOptionBuilder AddCommandLogging<TCommandDispatcher>() where TCommandDispatcher : ICommandDispatcher;
    }

    public class XrmFrameworkFunctionOptionBuilder : XrmFrameworkOptionBuilder, IXrmFrameworkFunctionOptionBuilder
    {
        public XrmFrameworkFunctionOptionBuilder(IServiceCollection serviceCollection) : base(serviceCollection)
        {
        }

        public IXrmFrameworkOptionBuilder AddDefaultCommandLogging()
            => AddCommandLogging<LoggingCommandDispatcher>();

        public IXrmFrameworkOptionBuilder AddCommandLogging<TCommandDispatcher>() where TCommandDispatcher : ICommandDispatcher
        {
            ServiceCollection
                .Replace(new ServiceDescriptor(
                    typeof(ICommandDispatcher),
                    typeof(TCommandDispatcher),
                    ServiceLifetime.Transient));

            return this;
        }
    }
}
