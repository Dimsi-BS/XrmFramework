using Deploy;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public interface IAssemblyExporter
    {
        PluginType ToRegisterPluginType(Guid pluginAssemblyId, string pluginFullName);
        Entity ToRegisterComponent(ISolutionComponent component);
        AddSolutionComponentRequest CreateAddSolutionComponentRequest(EntityReference objectRef, int? objectTypeCode = null);
    }
}