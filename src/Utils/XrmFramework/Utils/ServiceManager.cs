// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Plugins
{
    public sealed class ServiceManager
    {
        private static readonly IDictionary<IServiceContext, List<object>> LoadedServices = new Dictionary<IServiceContext, List<object>>();

        private static readonly object SyncRoot = new object();

        public static object GetService(Type t, IServiceContext localContext)
        {
            var loadedAssemblies = new[] { localContext.GetType().Assembly };

            object service = null;

            if (!LoadedServices.ContainsKey(localContext))
            {
                lock (SyncRoot)
                {
                    if (!LoadedServices.ContainsKey(localContext))
                    {
                        LoadedServices.Add(localContext, new List<object>());
                    }
                }
            }

            var loadedServicesTemp = LoadedServices[localContext];

            foreach (var serviceTemp in loadedServicesTemp)
            {
                if (t.IsInstanceOfType(serviceTemp))
                {
                    service = serviceTemp;
                }
            }

            if (service == null)
            {
                foreach (var assembly in loadedAssemblies)
                {
                    foreach (var module in assembly.GetModules())
                    {
                        foreach (var type in module.GetTypes())
                        {
                            if (t.IsAssignableFrom(type) && typeof(ILoggedService).IsAssignableFrom(type))
                            {
                                var constructor = type.GetConstructor(new[] { typeof(LocalContext) });
                                if (constructor != null)
                                {
                                    service = constructor.Invoke(new object[] { localContext });
                                    loadedServicesTemp.Add(service);
                                }
                            }
                        }
                    }
                }
            }

            if (service != null)
            {
                return service;
            }
            throw new InvalidPluginExecutionException(string.Format("Service '{0}' not found", t.Name));
        }
    }
}
