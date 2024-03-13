//HintName: DependencyInjection.cs
using BoDi;
using ClientNamespace.Core;
using XrmFramework;
#if !DISABLE_DI && PLUGIN

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

#if !DISABLE_DI

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
