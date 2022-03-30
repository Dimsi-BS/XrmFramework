// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Deploy;
using Microsoft.Xrm.Sdk;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.DeployUtils.Model
{
    public class Plugin : ISolutionComponent
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

        public IEnumerable<ISolutionComponent> Children => Steps;
        public void AddChild(ISolutionComponent child)
        {
            if(!child.GetType().IsAssignableFrom(typeof(Step))) throw new ArgumentException("Plugin doesn't take this type of children");
            Steps.Add((Step)child);
        }

    }
}
