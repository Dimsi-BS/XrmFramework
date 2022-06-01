using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    /// <summary>
    /// Metadata of CustomApi
    /// </summary>
    /// <seealso cref="XrmFramework.DeployUtils.Model.ICrmComponent" />
    public class CustomApi : BaseCrmComponent
    {
        // Don't put these fields in readonly, AutoMapper wouldn't be able to map them
        // but if you know a way enjoy :D
        private List<ICustomApiComponent> _arguments = new();

        /// <summary>Id of the Assembly the PluginType is attached to</summary>
        public Guid AssemblyId { get; set; } = Guid.NewGuid();

        public override IEnumerable<ICrmComponent> Children => _arguments;

        public override void AddChild(ICrmComponent child)
        {
            if (child is not ICustomApiComponent apiComponent)
            {
                throw new ArgumentException("CustomApi doesn't take this type of children");
            }
            _arguments.Add(apiComponent);
            base.AddChild(child);
        }

        protected override void RemoveChild(ICrmComponent child)
        {
            if (child is not ICustomApiComponent apiComponent)
            {
                throw new ArgumentException("CustomApi doesn't have this type of children");
            }
            _arguments.Remove(apiComponent);
        }

        public override int Rank => 1;
        public override bool DoAddToSolution => true;
        public override bool DoFetchTypeCode => true;
        public override string UniqueName
        {
            get => $"{Prefix}_{Name}";
            set
            {
                var split = value.Split('_');
                Prefix = split[0];
                Name = split[1];
            }
        }
        public override string EntityTypeName => CustomApiDefinition.EntityName;

        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }

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