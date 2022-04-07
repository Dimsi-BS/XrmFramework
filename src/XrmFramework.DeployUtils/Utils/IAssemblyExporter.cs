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

        void CreateAllComponents<T>(IEnumerable<T> components, bool doAddToSolution = false, bool doFetchCode = false) where T : ISolutionComponent;
        void DeleteAllComponents<T>(IEnumerable<T> components) where T : ISolutionComponent;
        void UpdateAllComponents<T>(IEnumerable<T> components) where T : ISolutionComponent;

        void InitExportMetadata(IEnumerable<Step> steps);
    }
}