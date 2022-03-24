using Microsoft.Xrm.Sdk;
using XrmFramework;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;

namespace Deploy
{
    partial class CustomApiRequestParameter : ICustomApiComponent
    {
        public string EntityTypeName => CustomApiRequestParameterDefinition.EntityName;

        public RegistrationState RegistrationState { get; set; }

        public Entity ToRegisterComponent(IRegistrationContext context)
        {
            return this;
        }
    }
}
