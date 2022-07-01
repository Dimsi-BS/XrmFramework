//HintName: DependencyInjection.cs
#if PLUGIN || CORE_PROJECT
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
