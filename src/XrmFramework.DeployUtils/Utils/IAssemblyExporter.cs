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

        void CreateAllComponents(IEnumerable<ICrmComponent> componentsToCreate);
        void DeleteAllComponents(IEnumerable<ICrmComponent> componentsToDelete);
        void UpdateAllComponents(IEnumerable<ICrmComponent> componentsToUpdate);

        void InitExportMetadata(IEnumerable<Step> steps);
    }
}