using Deploy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using XrmFramework.DeployUtils.Context;

namespace XrmFramework.DeployUtils.Service
{
    public interface IRegistrationService : IOrganizationService, IDisposable
    {
        IEnumerable<PluginAssembly> GetAssemblies();

        PluginAssembly GetAssemblyByName(string assemblyName);

        ICollection<CustomApiRequestParameter> GetRegisteredCustomApiRequestParameters(Guid assemblyId);

        ICollection<CustomApiResponseProperty> GetRegisteredCustomApiResponseProperties(Guid assemblyId);

        ICollection<CustomApi> GetRegisteredCustomApis(Guid assemblyId);

        ICollection<SdkMessageProcessingStepImage> GetRegisteredImages(Guid assemblyId);

        ICollection<PluginType> GetRegisteredPluginTypes(Guid pluginAssemblyId);

        ICollection<SdkMessageProcessingStep> GetRegisteredSteps(Guid assemblyId);

        IList<Entity> RetrieveAll(QueryExpression query, bool cleanLinks = true);
        ICollection<Guid> CreateMany<T>(ICollection<T> entities) where T : Entity;
        void DeleteMany(string entityName, IEnumerable<Guid> guids);
        void UpdateMany<T>(IEnumerable<T> entities) where T : Entity;

        void AddSolutionComponentToSolution(string solutionUniqueName, EntityReference objectRef, int? objectTypeCode = null);

        int GetIntEntityTypeCode(string logicalName);
    }
}