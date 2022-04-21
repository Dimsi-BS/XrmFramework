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
        public string FullName { get; }

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

        public IEnumerable<ICrmComponent> Children => Steps;

        public void AddChild(ICrmComponent child)
        {
            if (child is not Step step) throw new ArgumentException("Plugin doesn't take this type of children");
            step.ParentId = _id;
            Steps.Add(step);
        }

        public void RemoveChild(ICrmComponent child)
        {
            if (child is not Step step) throw new ArgumentException("Plugin doesn't have this type of children");
            Steps.Remove(step);
        }

        public void CleanChildrenWithState(RegistrationState state)
        {
            foreach (var child in Children)
            {
                child.CleanChildrenWithState(state);
                if (!child.Children.Any() && child.RegistrationState == state)
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
