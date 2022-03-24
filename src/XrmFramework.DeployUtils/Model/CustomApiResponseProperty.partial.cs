using Microsoft.Xrm.Sdk;
using XrmFramework;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;

namespace Deploy
{
    partial class CustomApiResponseProperty : ICustomApiComponent
    {
        public string EntityTypeName => CustomApiResponsePropertyDefinition.EntityName;

        public bool IsOptional { get; set; }
        public RegistrationState RegistrationState { get; set; }

        public Entity ToRegisterComponent(IRegistrationContext context)
        {
            return this;
        }
    }
}
