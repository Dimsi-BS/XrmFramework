// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using XrmProject;

namespace Deploy.WebResources
{
    class Program
    {
        static void Main(string[] args)
        {
            WebResourceHelper.SyncWebResources(@"..\..\..\..\WebResources\", "Webresources");
        }
    }
}
