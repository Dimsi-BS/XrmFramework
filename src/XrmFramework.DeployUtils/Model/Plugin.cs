// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Deploy;

namespace XrmFramework.DeployUtils.Model
{
    public class Plugin
    {
        public Plugin(string fullName)
        {
            FullName = fullName;
            RegistrationState = RegistrationState.NotComputed;
        }

        public Plugin(string fullName, string displayName) :this(fullName)
        {
            DisplayName = displayName;
            RegistrationState = RegistrationState.NotComputed;
        }

        public bool IsWorkflow => !string.IsNullOrWhiteSpace(DisplayName);

        public string FullName { get; }

        public string DisplayName { get; }

        public Guid Id { get; set; }

        public Guid AssemblyId { get; set; }

        public StepCollection Steps { get; } = new StepCollection();

        public RegistrationState RegistrationState { get; set; }

    }
}
