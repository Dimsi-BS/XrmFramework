using Deploy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    public interface ILocalAssemblyContext : IAssemblyContext
    {
        Assembly Assembly { get; }

        ICollection<Plugin> Plugins { get; set; }
        PluginAssembly ToPluginAssembly();
        PluginAssembly ToPluginAssembly(Guid Id);

    }
}
