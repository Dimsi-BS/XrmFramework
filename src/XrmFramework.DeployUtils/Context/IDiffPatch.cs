using System;
using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    public interface IDiffPatch
    {
        PluginAssembly PluginAssembly { get; }
        IEnumerable<ICrmComponent> GetComponentsWhere(Func<DiffComponent, bool> predicate);
        void SetComputedWhere(Func<DiffComponent, bool> predicate);
    }
}
