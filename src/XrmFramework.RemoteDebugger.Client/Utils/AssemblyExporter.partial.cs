using System;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public partial class AssemblyExporter
    {
        /// <summary>
        /// Creates a <see cref="ICrmComponent"/> on the Crm
        /// </summary>
        /// <remarks>
        /// This method is in a partial file because it is implemented differently in the DeployUtils project
        /// </remarks>
        /// <param name="component"> The component to export</param>
        public void CreateComponent(ICrmComponent component)
        {
            int? entityTypeCode = component.DoFetchTypeCode
                ? _registrationService.GetIntEntityTypeCode(component.EntityTypeName)
                : null;

            component.Id = Guid.Empty;
            var registeringComponent = _converter.ToRegisterComponent(component);
            component.Id = _registrationService.Create(registeringComponent);
            registeringComponent.Id = component.Id;

            if (!component.DoAddToSolution) return;

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
        }

    }
}