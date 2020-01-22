// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using XrmFramework;
using XrmFramework.Utils;

namespace Plugins
{
    public sealed class ServiceFactory
    {
        private static readonly IDictionary<IServiceContext, IDictionary<Type, object>> LoadedServices = new Dictionary<IServiceContext, IDictionary<Type, object>>();

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
                        LoadedServices.Add(localContext, new Dictionary<Type, object>());
                    }
                }
            }

            var loadedServicesTemp = LoadedServices[localContext];

            var isNewService = false;
            object implementation = null;

            if (!loadedServicesTemp.ContainsKey(t))
            {
                lock (SyncRoot)
                {
                    if (!loadedServicesTemp.ContainsKey(t))
                    {
                        Type loggedServiceType = null, implementationType = null;

                        foreach (var assembly in loadedAssemblies)
                        {
                            foreach (var module in assembly.GetModules())
                            {
                                foreach (var type in module.GetTypes())
                                {
                                    if (t.IsAssignableFrom(type) && typeof(ILoggedService).IsAssignableFrom(type)
                                                                 && (t == typeof(IService) && type.Name == "LoggedService" || t != typeof(IService)))
                                    {
                                        loggedServiceType = type;
                                    }

                                    if (t.IsAssignableFrom(type) && !type.IsAbstract && type.IsClass && !typeof(ILoggedService).IsAssignableFrom(type)
                                        && (t == typeof(IService) && type == typeof(DefaultService) || t != typeof(IService)))
                                    {
                                        implementationType = type;
                                    }
                                }
                            }
                        }

                        var implementationConstructor = implementationType?.GetConstructor(new[] { typeof(LocalContext) });
                        var loggedConstructor = loggedServiceType?.GetConstructor(new[] { typeof(LocalContext), t });
                        if (loggedConstructor == null || implementationConstructor == null)
                        {
                            throw new InvalidPluginExecutionException($"Service '{t.Name}' not found");
                        }

                        implementation = implementationConstructor.Invoke(new object[] { localContext });

                        if (implementation is IServiceWithSettings implementationWithSettings)
                        {
                            implementationWithSettings.InitSettings();
                        }

                        service = loggedConstructor.Invoke(new[] { localContext, implementation });
                        loadedServicesTemp.Add(t, service);
                        isNewService = true;
                    }
                }
            }

            if (!loadedServicesTemp.TryGetValue(t, out service))
            {
                throw new InvalidPluginExecutionException($"Service '{t.Name}' not found");
            }

            if (isNewService && implementation != null)
            {
                foreach (var property in implementation.GetType().GetProperties())
                {
                    if (property.GetCustomAttribute<DependencyInjectionAttribute>() == null)
                    {
                        continue;
                    }

                    if (typeof(IService).IsAssignableFrom(property.PropertyType))
                    {
                        var dependingService = GetService(property.PropertyType, localContext);

                        property.SetValue(implementation, dependingService);
                    }
                }
            }

            return service;
        }
    }
}
