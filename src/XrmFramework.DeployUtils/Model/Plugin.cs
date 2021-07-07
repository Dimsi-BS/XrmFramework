// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework.DeployUtils.Model
{
    public class Plugin
    {
        public Plugin(string fullName, bool isCustomApi)
        {
            FullName = fullName;
            IsCustomApi = isCustomApi;
        }

        public Plugin(string fullName, string displayName) :this(fullName, false)
        {
            DisplayName = displayName;
        }

        public bool IsWorkflow => !string.IsNullOrEmpty(DisplayName);

        public string FullName { get; }

        public bool IsCustomApi { get; }

        public string DisplayName { get; }

        public StepCollection Steps { get; } = new StepCollection();


        public static Plugin FromXrmFrameworkPlugin(dynamic plugin, bool isWorkflow = false, bool isCustomApi = false)
        {
            var pluginTemp = !isWorkflow ? new Plugin(plugin.GetType().FullName, isCustomApi) : new Plugin(plugin.GetType().FullName, plugin.DisplayName);

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
