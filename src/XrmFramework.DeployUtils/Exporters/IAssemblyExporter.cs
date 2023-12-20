using Microsoft.Xrm.Sdk;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Exporters;

/// <summary>
///     Local to Crm <see cref="ICrmComponent" /> exporter
/// </summary>
public interface IAssemblyExporter
{
    /// <summary>
    ///     Creates an enumeration of <see cref="ICrmComponent" /> on the Crm
    /// </summary>
    /// <param name="componentsToCreate"></param>
    void CreateAllComponents(IEnumerable<ICrmComponent> componentsToCreate);

    /// <summary>
    ///     Creates a list that contains a request to delete each of the given components
    ///     <br />The list is ordered in the reverse order of <see cref="IAssemblyContext.ComponentsOrderedPool" />
    /// </summary>
    /// <param name="componentsToDelete"></param>
    IEnumerable<OrganizationRequest> ToDeleteRequestCollection(IEnumerable<ICrmComponent> componentsToDelete);


    /// <summary>
    ///     Creates a list that contains a request to update each of the given components
    ///     <br />The list is ordered in the same order of <see cref="IAssemblyContext.ComponentsOrderedPool" />
    /// </summary>
    /// <param name="componentsToUpdate"></param>
    IEnumerable<OrganizationRequest> ToUpdateRequestCollection(IEnumerable<ICrmComponent> componentsToUpdate);


    /// <summary>
    ///     Deletes an enumeration of <see cref="ICrmComponent" /> on the Crm
    /// </summary>
    /// <param name="componentsToDelete"></param>
    void DeleteAllComponents(IEnumerable<ICrmComponent> componentsToDelete);

    /// <summary>
    ///     Updates an enumeration of <see cref="ICrmComponent" /> on the Crm
    /// </summary>
    /// <param name="componentsToUpdate"></param>
    void UpdateAllComponents(IEnumerable<ICrmComponent> componentsToUpdate);

    /// <summary>
    ///     Creates a <see cref="ICrmComponent" /> on the Crm
    /// </summary>
    /// <param name="component"></param>
    Guid CreateComponent(ICrmComponent component);

    /// <summary>
    ///     Manually add a component to the solution
    /// </summary>
    /// <param name="component"></param>
    void AddComponentToSolution(ICrmComponent component);

    /// <summary>
    ///     Deletes a <see cref="ICrmComponent" /> on the Crm
    /// </summary>
    /// <param name="component"></param>
    void DeleteComponent(ICrmComponent component);

    /// <summary>
    ///     Updates a <see cref="ICrmComponent" /> on the Crm
    /// </summary>
    /// <param name="component"></param>
    void UpdateComponent(ICrmComponent component);

    /// <summary>
    ///     Fetches metadata required to export a given set of <see cref="Step" />
    /// </summary>
    /// <param name="steps"></param>
    void InitExportMetadata(IEnumerable<Step> steps);
}