#if !DISABLE_DI

using BoDi;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xrm.Sdk;
using System;
#if NET6_0_OR_GREATER
using Microsoft.PowerPlatform.Dataverse.Client;
#endif
using XrmFramework;
using XrmFramework.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class XrmFrameworkServiceCollectionExtension
    {
        public static IServiceCollection AddXrmFramework(this IServiceCollection serviceCollection,
            Action<IXrmFrameworkOptionBuilder> optionsBuilderAction = null)
        {
            var optionsBuilder = new XrmFrameworkOptionBuilder(serviceCollection);
            optionsBuilderAction?.Invoke(optionsBuilder);

#if NET6_0_OR_GREATER
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IOrganizationServiceAsync), sp =>
                {
                    var serviceClient = sp.GetRequiredService<ServiceClient>();

                    return serviceClient;
                }, ServiceLifetime.Scoped));

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IOrganizationServiceAsync2), sp =>
                {
                    var serviceClient = sp.GetRequiredService<ServiceClient>();

                    return serviceClient;
                }, ServiceLifetime.Scoped));

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(ServiceClient), sp =>
                {
                    var service = new ServiceClient(optionsBuilder.ConnectionString);

                    if (optionsBuilder.UseWebApiForced)
                    {
                        service.UseWebApi = optionsBuilder.WebApiUsage;
                    }

                    return service;
                }, ServiceLifetime.Scoped));
#endif

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IOrganizationService), sp =>
                {
#if NET6_0_OR_GREATER
                    return sp.GetRequiredService<ServiceClient>();
#else
                    return new Xrm.Tooling.Connector.CrmServiceClient(optionsBuilder.ConnectionString);
#endif
                }, ServiceLifetime.Scoped));

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IServiceContext), sp =>
            {
                var orgService = sp.GetService<IOrganizationService>();
                return new ServiceContextBase(orgService);
            }, ServiceLifetime.Scoped));

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IObjectContainer), sp =>
            {
                var serviceContext = sp.GetRequiredService<IServiceContext>();

                var objectContainer = new ObjectContainer();

                objectContainer.RegisterInstanceAs(serviceContext);

                InternalDependencyProvider.RegisterDefaults(objectContainer);

                return objectContainer;
            }, ServiceLifetime.Scoped));

            RegisterServices(serviceCollection);

            RegisterCustomService(serviceCollection);
            return serviceCollection;
        }

        static partial void RegisterServices(IServiceCollection serviceCollection);
        static partial void RegisterCustomService(IServiceCollection serviceCollection);
        
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