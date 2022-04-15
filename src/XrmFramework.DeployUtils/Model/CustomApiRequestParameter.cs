using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    partial class CustomApiRequestParameter : ICustomApiComponent
    {
        public string EntityTypeName => CustomApiRequestParameterDefinition.EntityName;

        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;
        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public IEnumerable<ICrmComponent> Children => new List<ICrmComponent>();

        public void AddChild(ICrmComponent child) => throw new ArgumentException("CustomApiRequestParameter doesn't take children");

        public string UniqueName { get; set; }
        public int Rank { get; } = 3;
        public bool DoAddToSolution { get; } = true;
        public bool DoFetchTypeCode { get; } = true;

        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string LogicalEntityName => CustomApiRequestParameterDefinition.EntityName;
        public bool IsOptional { get; set; }
        public OptionSetValue Type { get; set; }
        public string Name { get; set; }
    }
}