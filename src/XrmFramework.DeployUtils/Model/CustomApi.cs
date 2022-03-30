using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using XrmFramework;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Utils;

namespace Deploy
{
    partial class CustomApi : ISolutionComponent
    {
        public List<CustomApiRequestParameter> InArguments { get; } = new List<CustomApiRequestParameter>();

        public List<CustomApiResponseProperty> OutArguments { get; } = new List<CustomApiResponseProperty>();
        public IEnumerable<ISolutionComponent> Children
        {
            get
            {
                var children = new List<ISolutionComponent>();
                children.AddRange(InArguments);
                children.AddRange(OutArguments);
                return children;
            }
        }
        public void AddChild(ISolutionComponent child)
        {
            switch (child)
            {
                case CustomApiRequestParameter:
                    InArguments.Add((CustomApiRequestParameter)child);
                    break;
                case CustomApiResponseProperty:
                    OutArguments.Add((CustomApiResponseProperty)child);
                    break;
                default:
                    throw new ArgumentException("CustomApi doesn't take this type of children");
            }
        }

        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

        public Guid ParentId { get => PluginTypeId.Id; set => PluginTypeId.Id = value; }

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

        public Entity ToRegisterComponent(ISolutionContext context)
        {
            return this;
        }
        public string EntityTypeName => CustomApiDefinition.EntityName;

    }
}
