// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    public class Plugin : ICrmComponent
    {
        private Guid _id;
        public string EntityTypeName => PluginTypeDefinition.EntityName;
        public Plugin(string fullName)
        {
            FullName = fullName;
        }

        public Plugin(string fullName, string displayName) : this(fullName)
        {
            DisplayName = displayName;
        }

        public bool IsWorkflow => !string.IsNullOrWhiteSpace(DisplayName);

        public string FullName { get; }

        public string DisplayName { get; }

        public string UniqueName => FullName;

        public Guid Id
        {
            get => _id;
            set
            {
                foreach (var step in Steps)
                {
                    step.ParentId = value;
                }
                _id = value;
            }
        }

        public Guid ParentId { get; set; }

        public StepCollection Steps { get; } = new StepCollection();

        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

        public IEnumerable<ICrmComponent> Children => Steps.ToList<ICrmComponent>();

        public void AddChild(ICrmComponent child)
        {
            if (child is not Step step) throw new ArgumentException("Plugin doesn't take this type of children");
            Steps.Add(step);
        }

        public int Rank => 1;
        public bool DoAddToSolution => false;
        public bool DoFetchTypeCode => false;
    }
}
