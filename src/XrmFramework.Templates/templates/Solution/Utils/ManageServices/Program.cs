// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using XrmFramework;
using XrmFramework.DeployUtils.Generators;

namespace GenerateMocks
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var types = typeof(IService).Assembly.GetTypes().Where(t => typeof(IService).IsAssignableFrom(t) && t.IsInterface);
            
            var serviceUtilsProjFileName = "../../../../LoggedServices/LoggedServices.projitems";

            MockGenerator.GenerateMocks(serviceUtilsProjFileName, types, typeof(NullableAttribute));
        }
    }
}