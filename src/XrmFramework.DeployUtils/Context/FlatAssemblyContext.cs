using Deploy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    public class FlatAssemblyContext : IFlatAssemblyContext
    {
        public string Name { get; }

        public PluginAssembly Assembly { get; set; }
        public ICollection<Plugin> Plugins { get; set; }
        public ICollection<Step> Steps { get; set; }
        public ICollection<StepImage> StepImages { get; set; }
        public ICollection<Plugin> Workflows { get; set; }
        public ICollection<CustomApi> CustomApis { get; set; }
        public ICollection<CustomApiRequestParameter> CustomApiRequestParameters { get; set; }

        public ICollection<CustomApiResponseProperty> CustomApiResponseProperties { get; set; }
    }
}
