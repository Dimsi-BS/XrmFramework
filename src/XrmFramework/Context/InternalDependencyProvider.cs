
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
    }
}
