// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Deploy.Plugins;
using Deploy.Utils;
using Workflows;

namespace Deploy.Workflows
{
    class Program
    {
        static void Main(string[] args)
        {
            var prog = new Program();
            prog.RegisterPlugins();
        }

        private void RegisterPlugins()
        {
            var assemblyPath = "Workflows.dll";

            RegistrationHelper.Register<global::Plugins.Plugin, CustomWorkflowActivity>("Workflows", assemblyPath, PluginConverter.Convert, PluginConverter.Convert);
        }
    }
}
