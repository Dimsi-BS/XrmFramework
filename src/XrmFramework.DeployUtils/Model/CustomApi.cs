using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    public partial class CustomApi : ICrmComponent
    {
        private readonly List<CustomApiRequestParameter> _inArguments = new();
        private readonly List<CustomApiResponseProperty> _outArguments = new();

        public IEnumerable<ICrmComponent> Children
        {
            get
            {
                var args = new List<ICrmComponent>();
                args.AddRange(_inArguments);
                args.AddRange(_outArguments);
                return args;
            }
        }

        public void AddChild(ICrmComponent child)
        {

            switch (child)
            {
                case CustomApiRequestParameter req:
                    req.ParentId = _id;
                    _inArguments.Add(req);
                    break;
                case CustomApiResponseProperty rep:
                    rep.ParentId = _id;
                    _outArguments.Add(rep);
                    break;
                default:
                    throw new ArgumentException("CustomApi doesn't take this type of children");
            }
        }

        private void RemoveChild(ICrmComponent child)
        {
            switch (child)
            {
                case CustomApiRequestParameter req:
                    _inArguments.Remove(req);
                    break;
                case CustomApiResponseProperty rep:
                    _outArguments.Remove(rep);
                    break;
                default:
                    throw new ArgumentException("CustomApi doesn't have this type of children");
            }
        }

        public void CleanChildrenWithState(RegistrationState state)
        {
            var childrenSafe = Children.ToList();
            foreach (var child in childrenSafe)
            {
                child.CleanChildrenWithState(state);
                if (!child.Children.Any() && child.RegistrationState == state)
                {
                    RemoveChild(child);
                }
            }
        }


        public int Rank => 1;
        public bool DoAddToSolution => true;
        public bool DoFetchTypeCode => true;
        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

        public Guid ParentId { get; set; } = Guid.NewGuid();

        public Guid AssemblyId { get; set; }

        private Guid _id = Guid.NewGuid();

        public Guid Id
        {
            get => _id;
            set
            {
                foreach (var req in _inArguments)
                {
                    req.ParentId = value;
                }
                foreach (var rep in _outArguments)
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