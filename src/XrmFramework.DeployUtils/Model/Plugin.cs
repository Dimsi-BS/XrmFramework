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

        public Plugin(string fullName)
        {
            FullName = fullName;
        }

        public Plugin(string fullName, string displayName) : this(fullName)
        {
            DisplayName = displayName;
        }

        public string FullName { get; }
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

        /// <summary>Indicates whether this <see cref="Plugin"/> is a WorkFlow</summary>
        public bool IsWorkflow => !string.IsNullOrWhiteSpace(DisplayName);

        /// <summary>Collection of the <see cref="Step"/></summary>
        public StepCollection Steps { get; } = new StepCollection();


        public string DisplayName { get; }

        public string UniqueName => FullName;
        public string EntityTypeName => PluginTypeDefinition.EntityName;


        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

        public IEnumerable<ICrmComponent> Children => Steps;

        public void AddChild(ICrmComponent child)
        {
            if (child is not Step step) throw new ArgumentException("Plugin doesn't take this type of children");
            step.ParentId = _id;
            Steps.Add(step);
        }

        private void RemoveChild(ICrmComponent child)
        {
            if (child is not Step step) throw new ArgumentException("Plugin doesn't have this type of children");
            Steps.Remove(step);
        }

        public void CleanChildrenWithState(RegistrationState state)
        {
            var childrenWithStateSafe = Children
                .Where(c => c.RegistrationState == state)
                .ToList();
            foreach (var child in childrenWithStateSafe)
            {
                child.CleanChildrenWithState(state);
                if (!child.Children.Any())
                {
                    RemoveChild(child);
                }
            }
        }

        public int Rank => 1;
        public bool DoAddToSolution => false;
        public bool DoFetchTypeCode => false;
    }
}
