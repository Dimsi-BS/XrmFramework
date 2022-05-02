using Deploy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace XrmFramework.DeployUtils.Service
{
    /// <summary>
    /// Additional layer of <see cref="IOrganizationService"/> to simplify some redundant calls
    /// </summary>
    public interface IRegistrationService : IOrganizationService
    {
        IEnumerable<PluginAssembly> GetAssemblies();

        /// <summary>
        /// Retrieves a registered assembly by name
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns><see cref="PluginAssembly"/></returns>
        PluginAssembly GetAssemblyByName(string assemblyName);

        /// <summary>
        /// Retrieves all <see cref="CustomApiRequestParameter"/> that can be linked with the Remote Assembly of Id <paramref name="assemblyId"/>
        /// </summary>
        /// <param name="assemblyId"></param>
        /// <returns></returns>
        ICollection<CustomApiRequestParameter> GetRegisteredCustomApiRequestParameters(Guid assemblyId);

        /// <summary>
        /// Retrieves all <see cref="CustomApiResponseProperty"/> that can be linked with the Remote Assembly of Id <paramref name="assemblyId"/>
        /// </summary>
        /// <param name="assemblyId"></param>
        /// <returns></returns>
        ICollection<CustomApiResponseProperty> GetRegisteredCustomApiResponseProperties(Guid assemblyId);

        /// <summary>
        /// Retrieves all <see cref="CustomApi"/> that can be linked with the Remote Assembly of Id <paramref name="assemblyId"/>
        /// </summary>
        /// <param name="assemblyId"></param>
        /// <returns></returns>
        ICollection<CustomApi> GetRegisteredCustomApis(Guid assemblyId);

        /// <summary>
        /// Retrieves all <see cref="SdkMessageProcessingStepImage"/> that can be linked with the Remote Assembly of Id <paramref name="assemblyId"/>
        /// </summary>
        /// <param name="assemblyId"></param>
        /// <returns></returns>
        ICollection<SdkMessageProcessingStepImage> GetRegisteredImages(Guid assemblyId);

        /// <summary>
        /// Retrieves all <see cref="PluginType"/> that can be linked with the Remote Assembly of Id <paramref name="assemblyId"/>
        /// </summary>
        /// <param name="assemblyId"></param>
        /// <returns></returns>
        ICollection<PluginType> GetRegisteredPluginTypes(Guid assemblyId);

        /// <summary>
        /// Retrieves all <see cref="SdkMessageProcessingStep"/> that can be linked with the Remote Assembly of Id <paramref name="assemblyId"/>
        /// </summary>
        /// <param name="assemblyId"></param>
        /// <returns></returns>
        ICollection<SdkMessageProcessingStep> GetRegisteredSteps(Guid assemblyId);

        /// <summary>
        /// Retrieves all <see cref="Entity"/> that match the given <paramref name="query"/> <see cref="QueryExpression"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cleanLinks"></param>
        /// <returns></returns>
        IList<Entity> RetrieveAll(QueryExpression query, bool cleanLinks = true);

        /// <summary>
        /// Retrieves the EntityTypeCode of a CrmComponent given its <paramref name="logicalName"/>
        /// </summary>
        /// <param name="logicalName"></param>
        /// <returns></returns>
        int GetIntEntityTypeCode(string logicalName);
    }
}