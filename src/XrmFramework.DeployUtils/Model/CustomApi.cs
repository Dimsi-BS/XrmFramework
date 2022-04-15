using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    public partial class CustomApi : ICrmComponent
    {
        public List<ICustomApiComponent> Arguments { get; } = new List<ICustomApiComponent>();
        public IEnumerable<ICrmComponent> Children => Arguments;

        public void AddChild(ICrmComponent child)
        {
            if (child is not ICustomApiComponent c)
                throw new ArgumentException("CustomApi doesn't take this type of children");
            Arguments.Add(c);
        }

        public int Rank { get; } = 2;
        public bool DoAddToSolution { get; } = true;
        public bool DoFetchTypeCode { get; } = true;
        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

        public Guid ParentId { get; set; }

        public Guid AssemblyId { get; set; }

        private Guid _id;

        public Guid Id
        {
            get => _id;
            set
            {
                foreach (var req in Arguments)
                {
                    req.ParentId = value;
                }

                _id = value;
            }
        }

        public string FullName { get; set; }
        public string UniqueName { get; set; }
        public string EntityTypeName => CustomApiDefinition.EntityName;
        public string DisplayName { get; set; }
        public string Name { get; set; }

        public OptionSetValue AllowedCustomProcessingStepType { get; set; }
        public string BoundEntityLogicalName { get; set; }
        public OptionSetValue BindingType { get; set; }
        public string Description { get; set; }
        public string ExecutePrivilegeName { get; set; }
        public bool IsFunction { get; set; }
        public bool IsPrivate { get; set; }
        public bool WorkflowSdkStepEnabled { get; set; }
    }
}