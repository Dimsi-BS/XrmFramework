using System;
using System.Collections.Generic;
using XrmFramework;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Model;

namespace Deploy
{
    partial class CustomApiRequestParameter : ICustomApiComponent
    {
        public string EntityTypeName => CustomApiRequestParameterDefinition.EntityName;

        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

        public Guid ParentId { get => CustomApiId.Id; set => CustomApiId.Id = value; }

        public IEnumerable<ICrmComponent> Children => new List<ICrmComponent>();

        public void AddChild(ICrmComponent child) => throw new ArgumentException("CustomApiRequestParameter doesn't take children");

        public int Rank { get; } = 3;
        public bool DoAddToSolution { get; } = true;
        public bool DoFetchTypeCode { get; } = true;

    }
}