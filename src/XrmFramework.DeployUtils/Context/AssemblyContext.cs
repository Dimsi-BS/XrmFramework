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
        public string Name => Assembly.Name;
        public bool IsNull => Assembly == null;
        public Guid Id { get => Assembly.Id; set => Assembly.Id = value; }

        public PluginAssembly Assembly { get; set; }

        public ICollection<Plugin> Plugins { get; set; }
        public ICollection<CustomApi> CustomApis { get; set; }
        public ICollection<Plugin> Workflows { get; set; }
    }
}
