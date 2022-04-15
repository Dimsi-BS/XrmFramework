using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    public partial class PluginAssembly : ICrmComponent
    {
        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;
        public Guid Id { get; set; }

        public Guid ParentId { get; set; }
        public string EntityTypeName => PluginAssemblyDefinition.EntityName;

        public string UniqueName { get; set; }

        public IEnumerable<ICrmComponent> Children => new List<ICrmComponent>();
        public void AddChild(ICrmComponent child) => throw new ArgumentException("PluginAssembly doesn't take children");

        public int Rank { get; } = 0;
        public bool DoAddToSolution { get; } = true;
        public bool DoFetchTypeCode { get; } = false;
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
