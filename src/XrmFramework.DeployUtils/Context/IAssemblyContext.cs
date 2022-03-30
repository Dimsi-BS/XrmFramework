using Deploy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Reflection;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    public interface IAssemblyContext
    {
        PluginAssembly Assembly { get; }

        ICollection<Plugin> Plugins { get; }
        ICollection<Plugin> Workflows { get; }
        ICollection<CustomApi> CustomApis { get; }

    }
}