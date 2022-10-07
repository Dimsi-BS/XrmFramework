// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    /// <summary>
    /// Metadata of a Plugin
    /// </summary>
    /// <seealso cref="XrmFramework.DeployUtils.Model.ICrmComponent" />
    public class Plugin : BaseCrmComponent
    {
        public Plugin(string fullName)
        {
            FullName = fullName;
        }

        public Plugin(string fullName, string displayName) : this(fullName)
        {
            DisplayName = displayName;
        }

        public string FullName { get; set; }
        public string DisplayName { get; }

        /// <summary>Collection of the <see cref="Step"/></summary>
        public StepCollection Steps { get; } = new StepCollection();

        /// <summary>Indicates whether this <see cref="Plugin"/> is a WorkFlow</summary>
        public bool IsWorkflow => !string.IsNullOrWhiteSpace(DisplayName);


        #region BaseCrmComponent overrides

        public override string UniqueName
        {
            get => FullName;
            set => FullName = value;
        }
        public override IEnumerable<ICrmComponent> Children => Steps;

        public override void AddChild(ICrmComponent child)
        {
            if (child is not Step step) throw new ArgumentException("Plugin doesn't take this type of children");
            base.AddChild(child);
            Steps.Add(step);
        }

        protected override void RemoveChild(ICrmComponent child)
        {
            if (child is not Step step) throw new ArgumentException("Plugin doesn't have this type of children");
            Steps.Remove(step);
        }
        public override string EntityTypeName => PluginTypeDefinition.EntityName;
        public override int Rank => 10;
        public override bool DoAddToSolution => false;
        public override bool DoFetchTypeCode => false;
        #endregion
    }
}
