// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xrm.Sdk;

namespace XrmFramework
{
    public sealed class ServiceFactory
    {
        private static readonly IDictionary<IServiceContext, IDictionary<Type, object>> LoadedServices = new Dictionary<IServiceContext, IDictionary<Type, object>>();

        private static readonly object SyncRoot = new object();

        public static T GetService<T>(IServiceContext localContext) where T : IService
            => (T)GetService(typeof(T), localContext);

        public static object GetService(Type t, IServiceContext localContext)
         => GetServiceInternal(t, localContext, new List<Type>());


        public static object GetServiceInternal(Type t, IServiceContext localContext, List<Type> callingTypes)
        {
            if (callingTypes.Contains(t))
            {
                throw new InvalidPluginExecutionException($"Circular service reference for Service {t.FullName}");
            }

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

                        var implementationConstructors = implementationType?.GetConstructors() ?? new ConstructorInfo[0];
                        if (!implementationConstructors.Any())
                        {
                            throw new InvalidPluginExecutionException($"No implementation found for Service '{t.FullName}'");
                        }

                        if (implementationConstructors.Length > 1)
                        {
                            throw new InvalidPluginExecutionException($"A unique constructor must be specified for '{t.FullName}' implementation '{implementationType.FullName}'");
                        }

                        var implementationConstructor = implementationConstructors[0];

                        var listParamValues = new List<object>();
                        foreach (var param in implementationConstructor.GetParameters())
                        {
                            if (typeof(IServiceContext).IsAssignableFrom(param.ParameterType))
                            {
                                listParamValues.Add(localContext);
                            }
                            else if (typeof(IService).IsAssignableFrom(param.ParameterType))
                            {
                                var obj = GetServiceInternal(param.ParameterType, localContext, callingTypes.Union(new[] { t }).ToList());
                                listParamValues.Add(obj);
                            }
                            else
                            {
                                throw new InvalidPluginExecutionException($"Invalid constructor parameter type {param.ParameterType.FullName} (param: {param.Name}) for {t.FullName} implementation {implementationType.FullName}");
                            }
                        }

                        implementation = implementationConstructor.Invoke(listParamValues.ToArray());

                        if (implementation is IServiceWithSettings implementationWithSettings)
                        {
                            implementationWithSettings.InitSettings();
                        }

                        var loggedConstructor = loggedServiceType?.GetConstructor(new[] { typeof(LocalContext), t });
                        if (loggedConstructor == null)
                        {
                            service = implementation;
                        }
                        else
                        {
                            service = loggedConstructor.Invoke(new[] { localContext, implementation });
                        }

                        loadedServicesTemp.Add(t, service);
                        isNewService = true;
                    }
                }
            }

            if (!loadedServicesTemp.TryGetValue(t, out service))
            {
                throw new InvalidPluginExecutionException($"Service '{t.Name}' not found");
            }

            return service;
        }
    }
}
