// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Deploy.Utils;
using Workflows;
using XrmFramework.Debugger;

namespace Deploy.Plugins
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
            var assemblyPath = "Plugins.dll";

            RegistrationHelper.Register<global::Plugins.Plugin, CustomWorkflowActivity>("Plugins", assemblyPath, PluginConverter.Convert, PluginConverter.Convert);
        }
    }   
}
