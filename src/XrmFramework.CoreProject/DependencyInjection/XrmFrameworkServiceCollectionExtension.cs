#if !DISABLE_DI

using BoDi;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using XrmFramework;
using XrmFramework.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class XrmFrameworkServiceCollectionExtension
    {
        public static IServiceCollection AddXrmFramework(this IServiceCollection serviceCollection, Action<IXrmFrameworkOptionBuilder> optionsBuilderAction = null)
        {
            var optionsBuilder = new XrmFrameworkOptionBuilder(serviceCollection);
            optionsBuilderAction?.Invoke(optionsBuilder);

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IOrganizationService), sp =>
                {
#if NETCOREAPP
                    var service = new Microsoft.PowerPlatform.Dataverse.Client.ServiceClient(optionsBuilder.ConnectionString);

                    if (optionsBuilder.UseWebApiForced)
                    {
                        service.UseWebApi = optionsBuilder.WebApiUsage;
                    }

                    return service;
#else
                    return new Xrm.Tooling.Connector.CrmServiceClient(optionsBuilder.ConnectionString);
#endif
                }

                , ServiceLifetime.Singleton));
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IServiceContext), sp =>
            {
                var orgService = sp.GetService<IOrganizationService>();
                return new ServiceContextBase(orgService);
            }, ServiceLifetime.Singleton));

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IObjectContainer), sp =>
            {
                var serviceContext = sp.GetRequiredService<IServiceContext>();

                var objectContainer = new ObjectContainer();

                objectContainer.RegisterInstanceAs(serviceContext);

                InternalDependencyProvider.RegisterDefaults(objectContainer);

                return objectContainer;
            }, ServiceLifetime.Scoped));

            RegisterServices(serviceCollection);

            return serviceCollection;
        }

        static partial void RegisterServices(IServiceCollection serviceCollection);

        private static void RegisterService<TIService>(IServiceCollection serviceCollection)
            where TIService : IService
        {
            var serviceDescriptor = new ServiceDescriptor(typeof(TIService), sp =>
           {
               var objectContainer = sp.GetRequiredService<IObjectContainer>();

               return objectContainer.Resolve(typeof(TIService));
           }, ServiceLifetime.Scoped);

            serviceCollection.Add(serviceDescriptor);
        }
    }
}

#endif