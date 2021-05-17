
#if !DISABLE_DI

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xrm.Sdk;
using XrmFramework;
using XrmFramework.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class XrmFrameworkServiceCollectionExtension
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

            var assembly = typeof(IService).Assembly;
            var loggedServiceAssembly = assembly;

            var serviceType = typeof(IService);

            var serviceTypes = assembly.GetModules().SelectMany(m => m.GetTypes().Where(t => serviceType.IsAssignableFrom(t) && t.IsInterface)).ToList();
            var serviceImplementationTypes = assembly.GetModules().SelectMany(m => m.GetTypes().Where(type => !type.IsAbstract && type.IsClass && serviceType.IsAssignableFrom(type))).ToList();

            foreach (var type in serviceTypes)
            {
                var serviceImplementationType = serviceImplementationTypes.FirstOrDefault(t => type.IsAssignableFrom(t));

                if (serviceImplementationType != null)
                {
                    var descriptor = ServiceDescriptor.Describe(type,
                        (sp) =>
                            DynamicProxyLoggingDecorator.Decorate(type,
                                ActivatorUtilities.GetServiceOrCreateInstance(sp, serviceImplementationType)),
                        ServiceLifetime.Scoped);

                    serviceCollection.Add(descriptor);

                    continue;
                }

                throw new NotImplementedException($"No implementation type found for service '{type.FullName}'");
            }

            return serviceCollection;
        }
    }
}

#endif