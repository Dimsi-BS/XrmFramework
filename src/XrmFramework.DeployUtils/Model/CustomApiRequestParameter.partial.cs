using Microsoft.Xrm.Sdk;
using System;
using XrmFramework;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;

namespace Deploy
{
    partial class CustomApiRequestParameter : ICustomApiComponent
    {
        public string EntityTypeName => CustomApiRequestParameterDefinition.EntityName;

        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

        public Guid ParentId { get => CustomApiId.Id; set => CustomApiId.Id = value; }
    }
}
