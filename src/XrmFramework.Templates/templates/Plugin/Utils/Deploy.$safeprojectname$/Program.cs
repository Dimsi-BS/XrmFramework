// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Deploy.Utils;
using XrmFramework.Workflow;
using XrmFramework.DeployUtils;

namespace Deploy.Plugins
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var prog = new Program();
            prog.RegisterPlugins();
        }

        private void RegisterPlugins()
        {
            var assemblyPath = "$safeprojectname$.dll";

            RegistrationHelper.Register<global::XrmFramework.Plugin, CustomWorkflowActivity>("$safeprojectname$", assemblyPath, PluginConverter.Convert, PluginConverter.Convert);
        }
    }   
}
