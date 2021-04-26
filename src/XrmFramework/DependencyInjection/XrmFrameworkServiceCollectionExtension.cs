
#if NETCOREAPP3_1

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.PowerPlatform.Dataverse.Client;
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

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IOrganizationService), sp => new ServiceClient(optionsBuilder.ConnectionString), ServiceLifetime.Singleton));
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IServiceContext), sp =>
            {
                var orgService = sp.GetService<IOrganizationService>();
                return new ServiceContextBase(orgService);
            }, ServiceLifetime.Singleton));

            var assembly = typeof(IService).Assembly;
            
            var serviceType = typeof(IService);

            var serviceTypes = assembly.GetModules().SelectMany(m => m.GetTypes().Where(t => serviceType.IsInstanceOfType(t) && t.IsInterface)).ToList();
            var serviceImplementationTypes = assembly.GetModules().SelectMany(m => m.GetTypes().Where(type => !type.IsAbstract && type.IsClass && serviceType.IsAssignableFrom(type))).ToList();

            var loggedServiceImplementationTypes = new List<Type>();
            if (optionsBuilder.LoggedServiceAssembly != null)
            {
                loggedServiceImplementationTypes = optionsBuilder.LoggedServiceAssembly.GetModules().SelectMany(m => m.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(ILoggedService).IsAssignableFrom(t))).ToList();
            }

            foreach (var type in serviceTypes)
            {
                var loggedServiceImplementation = loggedServiceImplementationTypes.FirstOrDefault(t => type.IsAssignableFrom(t));

                if (loggedServiceImplementation != null)
                {
                    serviceCollection.TryAdd(new ServiceDescriptor(type, loggedServiceImplementation, ServiceLifetime.Scoped));
                    continue;
                }

                var serviceImplementationType = serviceImplementationTypes.FirstOrDefault(t => type.IsAssignableFrom(t));

                if (serviceImplementationType != null)
                {
                    serviceCollection.TryAdd(new ServiceDescriptor(type, serviceImplementationType, ServiceLifetime.Scoped));
                    continue;
                }

                throw new NotImplementedException($"No implementation type found for service '{type.FullName}'");
            }

            return serviceCollection;
        }
    }
}

#endif