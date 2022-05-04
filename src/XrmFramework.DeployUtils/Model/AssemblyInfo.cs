using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    /// <summary>
    /// Metadata of the Assembly
    /// </summary>
    public class AssemblyInfo : ICrmComponent
    {
        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ParentId { get; set; } = Guid.NewGuid();
        public string EntityTypeName => PluginAssemblyDefinition.EntityName;
        public string UniqueName => Name;
        public IEnumerable<ICrmComponent> Children => new List<ICrmComponent>();
        public void AddChild(ICrmComponent child) => throw new ArgumentException("AssemblyInfo doesn't take children");
        public void CleanChildrenWithState(RegistrationState state) { }
        public int Rank => 0;
        public bool DoAddToSolution => true;
        public bool DoFetchTypeCode => false;

        /// <summary>Common Name of the Assembly</summary>
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
