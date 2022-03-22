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
        string Name { get; }

        PluginAssembly Assembly { get; set; }

        ICollection<Plugin> Plugins { get; set; }
        ICollection<Plugin> Workflows { get; set; }
        ICollection<CustomApi> CustomApis { get; set; }

    }
}