using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    public partial class CustomApi : ICrmComponent
    {
        private List<CustomApiRequestParameter> InArguments { get; } = new List<CustomApiRequestParameter>();
        private List<CustomApiResponseProperty> OutArguments { get; } = new List<CustomApiResponseProperty>();

        public IEnumerable<ICrmComponent> Children
        {
            get
            {
                var args = new List<ICrmComponent>();
                args.AddRange(InArguments);
                args.AddRange(OutArguments);
                return args;
            }
        }

        public void AddChild(ICrmComponent child)
        {
            switch (child)
            {
                case CustomApiRequestParameter req:
                    InArguments.Add(req);
                    break;
                case CustomApiResponseProperty rep:
                    OutArguments.Add(rep);
                    break;
                default:
                    throw new ArgumentException("CustomApi doesn't take this type of children");
            }
        }

        public int Rank => 1;
        public bool DoAddToSolution => true;
        public bool DoFetchTypeCode => true;
        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

        public Guid ParentId { get; set; }

        public Guid AssemblyId { get; set; }

        private Guid _id;

        public Guid Id
        {
            get => _id;
            set
            {
                foreach (var req in InArguments)
                {
                    req.ParentId = value;
                }
                foreach (var rep in OutArguments)
                {
                    rep.ParentId = value;
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