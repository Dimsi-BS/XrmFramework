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
            RegistrationState = RegistrationState.NotComputed;
        }

        public Plugin(string fullName, string displayName) : this(fullName)
        {
            DisplayName = displayName;
            RegistrationState = RegistrationState.NotComputed;
        }

        public bool IsWorkflow => !string.IsNullOrWhiteSpace(DisplayName);

        public string FullName { get; }

        public string DisplayName { get; }


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

        public RegistrationState RegistrationState { get; set; }

        public Entity ToRegisterComponent(ISolutionContext context)
        {
            return AssemblyBridge.ToRegisterPluginType(this);
        }
    }
}
