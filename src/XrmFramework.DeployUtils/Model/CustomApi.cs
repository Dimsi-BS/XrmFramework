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
        public PluginType PluginType { get; set; }
        public List<CustomApiRequestParameter> InArguments { get; } = new List<CustomApiRequestParameter>();

        public List<CustomApiResponseProperty> OutArguments { get; } = new List<CustomApiResponseProperty>();
        public RegistrationState RegistrationState { get; set; }

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

        public Entity ToRegisterComponent(IRegistrationContext context)
        {
            return this;
        }
        public string EntityTypeName => CustomApiDefinition.EntityName;

        public PluginType ToPluginType(Guid assemblyId) => AssemblyBridge.ToRegisterPluginType(assemblyId, FullName);
    }
}
