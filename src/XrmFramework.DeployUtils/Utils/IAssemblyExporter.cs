using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    /// <summary>
    /// Local to Crm <see cref="ICrmComponent"/> exporter
    /// </summary>
    public interface IAssemblyExporter
    {
        /// <summary>
        /// Creates an enumeration of <see cref="ICrmComponent"/> on the Crm
        /// </summary>
        /// <param name="componentsToCreate"></param>
        void CreateAllComponents(IEnumerable<ICrmComponent> componentsToCreate);

        /// <summary>
        /// Deletes an enumeration of <see cref="ICrmComponent"/> on the Crm
        /// </summary>
        /// <param name="componentsToDelete"></param>
        void DeleteAllComponents(IEnumerable<ICrmComponent> componentsToDelete);

        /// <summary>
        /// Updates an enumeration of <see cref="ICrmComponent"/> on the Crm
        /// </summary>
        /// <param name="componentsToUpdate"></param>
        void UpdateAllComponents(IEnumerable<ICrmComponent> componentsToUpdate);

        /// <summary>
        /// Creates a <see cref="ICrmComponent"/> on the Crm
        /// </summary>
        /// <param name="component"></param>
        void CreateComponent(ICrmComponent component);

        /// <summary>
        /// Deletes a <see cref="ICrmComponent"/> on the Crm
        /// </summary>
        /// <param name="component"></param>
        void DeleteComponent(ICrmComponent component);

        /// <summary>
        /// Updates a <see cref="ICrmComponent"/> on the Crm
        /// </summary>
        /// <param name="component"></param>
        void UpdateComponent(ICrmComponent component);

        /// <summary>
        /// Fetches metadata required to export a given set of <see cref="Step"/>
        /// </summary>
        /// <param name="steps"></param>
        void InitExportMetadata(IEnumerable<Step> steps);
    }
}