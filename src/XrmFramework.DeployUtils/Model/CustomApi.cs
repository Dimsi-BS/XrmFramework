using System;
using System.Collections.Generic;
using XrmFramework;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Model;

namespace Deploy
{
    partial class CustomApi : ICrmComponent
    {
        public List<CustomApiRequestParameter> InArguments { get; } = new List<CustomApiRequestParameter>();

        public List<CustomApiResponseProperty> OutArguments { get; } = new List<CustomApiResponseProperty>();
        public IEnumerable<ICrmComponent> Children
        {
            get
            {
                var children = new List<ICrmComponent>();
                children.AddRange(InArguments);
                children.AddRange(OutArguments);
                return children;
            }
        }
        public void AddChild(ICrmComponent child)
        {
            switch (child)
            {
                case CustomApiRequestParameter parameter:
                    InArguments.Add(parameter);
                    break;
                case CustomApiResponseProperty property:
                    OutArguments.Add(property);
                    break;
                default:
                    throw new ArgumentException("CustomApi doesn't take this type of children");
            }
        }

        public int Rank { get; } = 2;
        public bool DoAddToSolution { get; } = true;
        public bool DoFetchTypeCode { get; } = true;
        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

        public Guid ParentId { get => PluginTypeId.Id; set => PluginTypeId.Id = value; }

        public Guid AssemblyId { get; set; }

        public override Guid Id
        {
            get => base.Id;
            set
            {
                foreach (var req in InArguments)
                {
                    req.CustomApiId.Id = value;
                }
                foreach (var res in OutArguments)
                {
                    res.CustomApiId.Id = value;
                }
                base.Id = value;
            }
        }
        public string FullName
        {
            get;
            set;
        }

        public string Prefix
        {
            set => UniqueName = $"{value}{Name}";
        }

        public string EntityTypeName => CustomApiDefinition.EntityName;

    }
}
