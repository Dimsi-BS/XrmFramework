//HintName: DependencyInjection.cs
#if !DISABLE_SERVICES && PLUGIN
using BoDi;
using ClientNamespace.Core;
using XrmFramework;

namespace XrmFramework
{
    partial class InternalDependencyProvider
    {
        static partial void RegisterServices(IObjectContainer container)
        {
            RegisterService<IService, DefaultService, LoggedIService>(container);

            RegisterService<ISub3Service, Sub3Service, LoggedISub3Service>(container);

        }
    }
}
#endif

#if !DISABLE_SERVICES

namespace Microsoft.Extensions.DependencyInjection
{
    partial class XrmFrameworkServiceCollectionExtension
    {
        static partial void RegisterServices(IServiceCollection serviceCollection)
        {
            RegisterService<IService>(serviceCollection);

            RegisterService<ISub3Service>(serviceCollection);

        }
    }
}
#endif
