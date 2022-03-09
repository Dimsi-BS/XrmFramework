using Deploy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace XrmFramework.DeployUtils.Service
{
    public interface IRegistrationService : IOrganizationService
    {
        IEnumerable<PluginAssembly> GetAssemblies();

        PluginAssembly GetAssemblyByName(string assemblyName);

        PluginAssembly GetProfilerAssembly();

        IEnumerable<CustomApiRequestParameter> GetRegisteredCustomApiRequestParameters(Guid assemblyId);

        IEnumerable<CustomApiResponseProperty> GetRegisteredCustomApiResponseProperties(Guid assemblyId);

        IEnumerable<CustomApi> GetRegisteredCustomApis(Guid assemblyId);

        ICollection<SdkMessageProcessingStepImage> GetRegisteredImages(Guid assemblyId);

        IEnumerable<PluginType> GetRegisteredPluginTypes(Guid pluginAssemblyId);

        ICollection<SdkMessageProcessingStep> GetRegisteredSteps(Guid assemblyId);

        IList<Entity> RetrieveAll(QueryExpression query, bool cleanLinks = true);
    }
}