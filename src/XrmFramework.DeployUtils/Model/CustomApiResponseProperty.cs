using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    partial class CustomApiResponseProperty : ICustomApiComponent
    {
        public string EntityTypeName => CustomApiResponsePropertyDefinition.EntityName;

        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string LogicalEntityName => CustomApiResponsePropertyDefinition.EntityName;
        public bool IsOptional { get; set; }
        public OptionSetValue Type { get; set; }
        public string Name { get; set; }
        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public IEnumerable<ICrmComponent> Children => new List<ICrmComponent>();
        public void AddChild(ICrmComponent child) => throw new ArgumentException("CustomApiResponseProperty doesn't take children");

        public string UniqueName { get; set; }
        public int Rank => 2;
        public bool DoAddToSolution => true;
        public bool DoFetchTypeCode => true;
    }
}
