#if CORE_PROJECT

using BoDi;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using Microsoft.Xrm.Tooling.Connector;
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


            serviceCollection.TryAdd(new ServiceDescriptor(typeof(CrmServiceClient), _ =>
            {
                var service = new CrmServiceClient(optionsBuilder.ConnectionString);

                return service;
            }, ServiceLifetime.Scoped));

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IOrganizationService), sp => sp.GetRequiredService<CrmServiceClient>(), ServiceLifetime.Scoped));

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
