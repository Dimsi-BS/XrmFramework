// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace XrmFramework.DeployUtils.Model
{
    public class Plugin
    {
        private ICollection<Step> _steps = new List<Step>();

        public Plugin(string fullName)
        {
            FullName = fullName;
        }

        public Plugin(string fullName, string displayName) :this(fullName)
        {
            DisplayName = displayName;
        }

        public bool IsWorkflow { get { return !string.IsNullOrEmpty(DisplayName); } }

        public string FullName { get; private set; }

        public string DisplayName { get; private set; }

        public ICollection<Step> Steps { get { return _steps; } }
    }
}
