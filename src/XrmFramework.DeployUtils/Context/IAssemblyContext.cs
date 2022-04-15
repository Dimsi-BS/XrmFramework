using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    public interface IAssemblyContext
    {
        PluginAssembly Assembly { get; }

        ICollection<Plugin> Plugins { get; }
        ICollection<Plugin> Workflows { get; }
        ICollection<CustomApi> CustomApis { get; }

        IReadOnlyCollection<ICrmComponent> ComponentsOrderedPool { get; }
    }
}