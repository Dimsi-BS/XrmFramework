using Deploy;
using System;
using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    public interface IDiffPatch
    {
        PluginAssembly PluginAssembly { get; }
        IEnumerable<ICrmComponent> RetrieveWhere(Func<DiffComponent, bool> predicate);
    }
}
