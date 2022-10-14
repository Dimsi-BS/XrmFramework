// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using XrmFramework.DeployUtils;

namespace Deploy.WebResources
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            WebResourceHelper.SyncWebResources("Webresources", args);
        }
    }
}
