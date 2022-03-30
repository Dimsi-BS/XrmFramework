using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using XrmFramework;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;

namespace Deploy
{
    partial class CustomApiRequestParameter : ICustomApiComponent
    {
        public string EntityTypeName => CustomApiRequestParameterDefinition.EntityName;

        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

        public Guid ParentId { get => CustomApiId.Id; set => CustomApiId.Id = value; }

        public IEnumerable<ISolutionComponent> Children => new List<ISolutionComponent>();

        public void AddChild(ISolutionComponent child) => throw new ArgumentException("CustomApiRequestParameter doesn't take children");

    }
}