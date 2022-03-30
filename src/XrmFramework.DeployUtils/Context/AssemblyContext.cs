using Deploy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    public class AssemblyContext : IAssemblyContext
    {
        public PluginAssembly Assembly { get; set; }

        public ICollection<Plugin> Plugins { get; } = new List<Plugin>();
        public ICollection<CustomApi> CustomApis { get; } = new List<CustomApi>();
        public ICollection<Plugin> Workflows { get; } = new List<Plugin>();
    }
}
