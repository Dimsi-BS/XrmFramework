#if !DISABLE_DI

using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xrm.Sdk;
using XrmFramework;
using XrmFramework.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class XrmFrameworkServiceCollectionExtension
    {
        public static IServiceCollection AddXrmFramework(this IServiceCollection serviceCollection, Action<XrmFrameworkOptionBuilder> optionsBuilderAction = null)
        {
            var optionsBuilder = new XrmFrameworkOptionBuilder();
            optionsBuilderAction?.Invoke(optionsBuilder);

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IOrganizationService), sp =>
#if NETCOREAPP
                new Microsoft.PowerPlatform.Dataverse.Client.ServiceClient(optionsBuilder.ConnectionString)
#else
                new Xrm.Tooling.Connector.CrmServiceClient(optionsBuilder.ConnectionString)
#endif

                , ServiceLifetime.Singleton));
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IServiceContext), sp =>
            {
                var orgService = sp.GetService<IOrganizationService>();
                return new ServiceContextBase(orgService);
            }, ServiceLifetime.Singleton));

            RegisterServices(serviceCollection);

            return serviceCollection;
        }

        static partial void RegisterServices(IServiceCollection serviceCollection);
    }
}

#endif