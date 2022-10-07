using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public partial class AssemblyExporter
    {
        /// <summary>
        /// Creates a <see cref="ICrmComponent"/> on the Crm
        /// </summary>
        /// 
        /// This method is in a partial file because it is implemented differently in the RemoteDebugger.Client project
        public void CreateComponent(ICrmComponent component)
        {
            if (component is CustomApi customApi)
            {
                CreateCustomApiPluginType(customApi);
            }

            component.Id = component is IAssemblyContext
                ? Guid.NewGuid()
                : Guid.Empty;

            var registeringComponent = _converter.ToRegisterComponent(component);
            component.Id = _registrationService.Create(registeringComponent);
            registeringComponent.Id = component.Id;

            if (!component.DoAddToSolution) return;

            int? entityTypeCode = component.DoFetchTypeCode
                ? _registrationService.GetIntEntityTypeCode(component.EntityTypeName)
                : null;

            var addSolutionComponentRequest = CreateAddSolutionComponentRequest(registeringComponent.ToEntityReference(), entityTypeCode);

            if (addSolutionComponentRequest != null)
            {
                _registrationService.Execute(addSolutionComponentRequest);
            }
        }

        /// <summary>
        /// Deletes a <see cref="ICrmComponent"/> on the Crm
        /// </summary>
        /// 
        /// This method is in a partial file because it is implemented differently in the RemoteDebugger.Client project
        public void DeleteComponent(ICrmComponent component)
        {
            _registrationService.Delete(component.EntityTypeName, component.Id);

            if (component is CustomApi customApi)
            {
                _registrationService.Delete(PluginTypeDefinition.EntityName, customApi.ParentId);
            }
        }
        public IEnumerable<OrganizationRequest> ToDeleteRequestCollection(IEnumerable<ICrmComponent> componentsToDelete)
        {
            var sortedList = componentsToDelete.ToList();

            var customApiPluginTypes = sortedList.OfType<CustomApi>()
                .Select(c => new Plugin(c.UniqueName) { Id = c.ParentId })
                .ToList();
            sortedList.AddRange(customApiPluginTypes);

            sortedList.Sort((x, y) => -x.Rank.CompareTo(y.Rank));

            return sortedList.Select(ToDeleteRequest);
        }

        private void CreateCustomApiPluginType(CustomApi customApi)
        {
            var customApiPluginType = ToRegisterPluginType(customApi.AssemblyId, customApi.FullName);
            var id = _registrationService.Create(customApiPluginType);
            customApi.ParentId = id;
        }
    }
}
