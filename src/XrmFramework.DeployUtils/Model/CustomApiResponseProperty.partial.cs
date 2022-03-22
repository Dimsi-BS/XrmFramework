using Microsoft.Xrm.Sdk;
using XrmFramework;

namespace Deploy
{
    partial class CustomApiResponseProperty : ICustomApiComponent
    {
        public bool IsOptional { get; set; }
        public RegistrationState RegistrationState { get; set; }

    }
}
