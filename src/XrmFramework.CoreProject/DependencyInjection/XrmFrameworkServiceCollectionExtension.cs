#if CORE_PROJECT

using BoDi;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using Microsoft.PowerPlatform.Dataverse.Client;
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

            ValidateConnectionOptions(optionsBuilder);

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
                var service = optionsBuilder.UsesCallerManagedAuthentication
                    ? new ServiceClient(optionsBuilder.Url, optionsBuilder.AccessTokenProviderFunction, optionsBuilder.RequireNewServiceClientInstance)
                    : new ServiceClient(optionsBuilder.ConnectionString);

                if (optionsBuilder.UseWebApiForced)
                {
                    service.UseWebApi = optionsBuilder.WebApiUsage;
                }

                return service;
            }, ServiceLifetime.Scoped));

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IOrganizationService), sp =>
            {
                return sp.GetRequiredService<ServiceClient>();
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

        private static void ValidateConnectionOptions(XrmFrameworkOptionBuilder optionsBuilder)
        {
            if (!string.IsNullOrWhiteSpace(optionsBuilder.ConnectionString) && optionsBuilder.UsesCallerManagedAuthentication)
            {
                throw new InvalidOperationException(
                    "AddXrmFramework was given both a connection string and a managed identity / access token provider. Only one connection mode can be used, call either UseConnectionString or UseManagedIdentity / UseAccessTokenProvider.");
            }
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
