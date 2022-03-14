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

        IEnumerable<CustomApiRequestParameter> GetRegisteredCustomApiRequestParameters(Guid assemblyId);

        IEnumerable<CustomApiResponseProperty> GetRegisteredCustomApiResponseProperties(Guid assemblyId);

        IEnumerable<CustomApi> GetRegisteredCustomApis(Guid assemblyId);

        ICollection<SdkMessageProcessingStepImage> GetRegisteredImages(Guid assemblyId);

        IEnumerable<PluginType> GetRegisteredPluginTypes(Guid pluginAssemblyId);

        ICollection<SdkMessageProcessingStep> GetRegisteredSteps(Guid assemblyId);

        IList<Entity> RetrieveAll(QueryExpression query, bool cleanLinks = true);

        Guid Create(IRegisteredAssemblyContext context);
        void Delete(IRegisteredAssemblyContext context);
        void Update(IRegisteredAssemblyContext context);

        ICollection<Guid> CreateMany<T>(ICollection<T> entities) where T : Entity;
        void DeleteMany<T>(ICollection<T> entities) where T : Entity;
        void UpdateMany<T>(ICollection<T> entities) where T : Entity;

        void AddSolutionComponentToSolution(string solutionUniqueName, EntityReference objectRef, int? objectTypeCode = null);

        int GetIntEntityTypeCode(string logicalName);
    }
}