using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    public partial class AssemblyInfo : ICrmComponent
    {
        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ParentId { get; set; } = Guid.NewGuid();
        public string EntityTypeName => PluginAssemblyDefinition.EntityName;

        public string UniqueName => Name;

        public IEnumerable<ICrmComponent> Children => new List<ICrmComponent>();
        public void AddChild(ICrmComponent child) => throw new ArgumentException("PluginAssembly doesn't take children");

        public int Rank => 0;
        public bool DoAddToSolution => true;
        public bool DoFetchTypeCode => false;
        public string Name { get; set; }
        public OptionSetValue SourceType { get; set; }
        public OptionSetValue IsolationMode { get; set; }
        public string Culture { get; set; }
        public string PublicKeyToken { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public byte[] Content { get; set; }
    }
}
