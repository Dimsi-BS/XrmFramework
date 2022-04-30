using System;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public partial class AssemblyExporter
    {
        public void CreateComponent(ICrmComponent component)
        {
            int? entityTypeCode = component.DoFetchTypeCode
                ? _registrationService.GetIntEntityTypeCode(component.EntityTypeName)
                : null;

            component.Id = Guid.Empty;
            var registeringComponent = _converter.ToRegisterComponent(component);
            component.Id = _registrationService.Create(registeringComponent);
            registeringComponent.Id = component.Id;

            if (component.DoAddToSolution)
            {
                var addSolutionComponentRequest = CreateAddSolutionComponentRequest(registeringComponent.ToEntityReference(), entityTypeCode);

                if (addSolutionComponentRequest != null)
                {
                    _registrationService.Execute(addSolutionComponentRequest);
                }
            }
        }
    }
}