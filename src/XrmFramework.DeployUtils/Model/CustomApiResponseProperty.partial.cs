using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using XrmFramework;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;

namespace Deploy
{
    partial class CustomApiResponseProperty : ICustomApiComponent
    {
        public string EntityTypeName => CustomApiResponsePropertyDefinition.EntityName;

        public bool IsOptional { get; set; }
        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;
        public Guid ParentId { get => CustomApiId.Id; set => CustomApiId.Id = value; }
        public IEnumerable<ISolutionComponent> Children => new List<ISolutionComponent>();
        public void AddChild(ISolutionComponent child) => throw new ArgumentException("CustomApiResponseProperty doesn't take children");

    }
}
