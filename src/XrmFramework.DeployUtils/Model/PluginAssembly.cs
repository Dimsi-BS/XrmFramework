using System;
using System.Collections.Generic;
using XrmFramework;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Model;

namespace Deploy
{
    partial class PluginAssembly : ICrmComponent
    {
        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

        public Guid ParentId { get; set; }
        public string EntityTypeName => PluginAssemblyDefinition.EntityName;

        public string UniqueName => Name;

        public IEnumerable<ICrmComponent> Children => new List<ICrmComponent>();
        public void AddChild(ICrmComponent child) => throw new ArgumentException("PluginAssembly doesn't take children");

        public int Rank { get; } = 0;
        public bool DoAddToSolution { get; } = true;
        public bool DoFetchTypeCode { get; } = false;
    }
}
