using Deploy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmFramework.DeployUtils.Context
{
    public interface IRegisteredAssemblyContext : IAssemblyContext
    {
        PluginAssembly Assembly { get; set; }
        Guid Id { get; set; }
        EntityReference EntityReference { get; }

        IEnumerable<PluginType> PluginTypes { get; set; }

        ICollection<SdkMessageProcessingStepImage> ImageSteps { get; }

        bool IsNull { get; }
    }
}
