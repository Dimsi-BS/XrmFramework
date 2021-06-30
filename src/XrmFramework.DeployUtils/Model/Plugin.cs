// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework.DeployUtils.Model
{
    public class Plugin
    {
        public Plugin(string fullName)
        {
            FullName = fullName;
        }

        public Plugin(string fullName, string displayName) :this(fullName)
        {
            DisplayName = displayName;
        }

        public bool IsWorkflow => !string.IsNullOrEmpty(DisplayName);

        public string FullName { get; }

        public string DisplayName { get; }

        public StepCollection Steps { get; } = new StepCollection();


        public static Plugin FromXrmFrameworkPlugin(dynamic plugin, bool isWorkflow = false)
        {
            var pluginTemp = !isWorkflow ? new Plugin(plugin.GetType().FullName) : new Plugin(plugin.GetType().FullName, plugin.DisplayName);

            if (!isWorkflow)
            {
                foreach (var step in plugin.Steps)
                {
                    pluginTemp.Steps.Add(Step.FromXrmFrameworkStep(step));
                }
            }

            return pluginTemp;
        }
    }
}
