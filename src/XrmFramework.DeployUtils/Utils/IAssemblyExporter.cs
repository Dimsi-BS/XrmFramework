using Deploy;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public interface IAssemblyExporter
    {
        PluginType ToRegisterPluginType(Guid pluginAssemblyId, string pluginFullName);
        AddSolutionComponentRequest CreateAddSolutionComponentRequest(EntityReference objectRef, int? objectTypeCode = null);

        void CreateAllComponents(IEnumerable<ICrmComponent> components);
        void DeleteAllComponents(IEnumerable<ICrmComponent> components);
        void UpdateAllComponents(IEnumerable<ICrmComponent> components);

        void InitExportMetadata(IEnumerable<Step> steps);
    }
}