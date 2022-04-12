using System;
using System.Collections.Generic;
using XrmFramework;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Model;

namespace Deploy
{
    partial class CustomApiResponseProperty : ICustomApiComponent
    {
        public string EntityTypeName => CustomApiResponsePropertyDefinition.EntityName;

        public bool IsOptional { get; set; }
        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;
        public Guid ParentId { get => CustomApiId.Id; set => CustomApiId.Id = value; }
        public IEnumerable<ICrmComponent> Children => new List<ICrmComponent>();
        public void AddChild(ICrmComponent child) => throw new ArgumentException("CustomApiResponseProperty doesn't take children");

        public int Rank { get; } = 3;
        public bool DoAddToSolution { get; } = true;
        public bool DoFetchTypeCode { get; } = true;

    }
}
