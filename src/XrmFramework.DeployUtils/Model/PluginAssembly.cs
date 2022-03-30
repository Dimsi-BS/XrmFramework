using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;

namespace Deploy
{
    partial class PluginAssembly : ISolutionComponent
    {
        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

        public Guid ParentId { get; set; }
        public string EntityTypeName => PluginAssemblyDefinition.EntityName;

        public string UniqueName => Name;

        public IEnumerable<ISolutionComponent> Children => new List<ISolutionComponent>();
        public void AddChild(ISolutionComponent child) => throw new ArgumentException("PluginAssembly doesn't take children");
    }
}
