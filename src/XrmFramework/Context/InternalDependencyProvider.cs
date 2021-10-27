
using System;
using BoDi;

namespace XrmFramework
{
    public static partial class InternalDependencyProvider
    {
        static partial void RegisterServices(IObjectContainer container);

        public static void RegisterDefaults(IObjectContainer container)
        {
            RegisterServices(container);
        }

        private static void RegisterService<TIService, TImplementation, TLoggedService>(IObjectContainer container)
            where TIService : IService
            where TImplementation : class, TIService
            where TLoggedService : class, TIService
        {
            container.RegisterTypeAs<TImplementation, TImplementation>();

            container.RegisterFactoryAs(objectContainer =>
            {
                var context = objectContainer.Resolve<IServiceContext>();
                var service = objectContainer.Resolve<TImplementation>();

                if (service is IServiceWithSettings serviceWithSettings)
                {
                    serviceWithSettings.InitSettings();
                }

                return (TIService)Activator.CreateInstance(typeof(TLoggedService), context, service);
            });
        }
    }
}
