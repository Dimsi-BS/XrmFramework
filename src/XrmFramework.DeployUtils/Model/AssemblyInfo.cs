using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    /// <summary>
    /// Metadata of the Assembly
    /// </summary>
    public class AssemblyInfo : BaseCrmComponent
    {
        /// <summary>Common Name of the Assembly</summary>
        public string Name { get; set; }
        public OptionSetValue SourceType { get; set; }
        public OptionSetValue IsolationMode { get; set; }
        public string Culture { get; set; }
        public string PublicKeyToken { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public byte[] Content { get; set; }

        #region ICrmComponents
        public override string EntityTypeName => PluginAssemblyDefinition.EntityName;
        public override string UniqueName { get => Name; set => Name = value; }
        public override IEnumerable<ICrmComponent> Children => Enumerable.Empty<ICrmComponent>();
        public override void AddChild(ICrmComponent child) => throw new ArgumentException("AssemblyInfo doesn't take children");
        protected override void RemoveChild(ICrmComponent child) { }
        public override int Rank => 0;
        public override bool DoAddToSolution => true;
        public override bool DoFetchTypeCode => false;
        #endregion
    }
}
