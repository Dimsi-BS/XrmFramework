using Deploy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Utils
{
    public class AssemblyFactory
    {
        private readonly IAssemblyImporter _importer;

        public AssemblyFactory(IAssemblyImporter importer)
        {
            _importer = importer;
        }

        public IAssemblyContext CreateFromLocalAssemblyContext(Type TPlugin)
        {
            var Assembly = TPlugin.Assembly;
            var pluginType = Assembly.GetType("XrmFramework.Plugin");
            var customApiType = Assembly.GetType("XrmFramework.CustomApi");
            var workflowType = Assembly.GetType("XrmFramework.Workflow.CustomWorkflowActivity");

            var plugins = Assembly.GetTypes()
                                  .Where(t =>    pluginType.IsAssignableFrom(t)
                                              && !customApiType.IsAssignableFrom(t)
                                              && t.IsPublic
                                              && !t.IsAbstract)
                                  .Select(t => _importer.CreatePluginFromType(t))
                                  .ToList();

            var workflows = Assembly.GetTypes()
                                    .Where(t =>    workflowType.IsAssignableFrom(t)
                                                && !t.IsAbstract
                                                && t.IsPublic)
                                    .Select(t => _importer.CreateWorkflowFromType(t)) 
                                    .ToList();

            var customApis = Assembly.GetTypes()
                                     .Where(t =>    customApiType.IsAssignableFrom(t)
                                                 && t.IsPublic
                                                 && !t.IsAbstract)
                                     .Select(t => _importer.CreateCustomApiFromType(t))
                                     .ToList();

            var assembly = _importer.CreateAssemblyFromLocal(Assembly);

            foreach(var plugin in plugins)
            {
                plugin.ParentId = assembly.Id;
            }
            foreach(var workflow in workflows)
            {
                workflow.ParentId = assembly.Id;
            }

            var localAssembly = new AssemblyContext
            {
                Assembly = assembly,
                Plugins = plugins,
                Workflows = workflows,
                CustomApis = customApis,
            };
            return localAssembly;
        }

        public IAssemblyContext CreateFromRemoteAssemblyContext(IRegistrationService service, string assemblyName)
        {
            var assembly = service.GetAssemblyByName(assemblyName);
            AssemblyContext registeredAssembly;

            if (assembly != null)
            {
                Console.WriteLine("Remote Assembly Exists, Fetching Components...");
                var registeredPluginTypes = service.GetRegisteredPluginTypes(assembly.Id);

                var registeredSteps = service.GetRegisteredSteps(assembly.Id);
                var registeredStepImages = service.GetRegisteredImages(assembly.Id);


                var registeredCustomApis = service.GetRegisteredCustomApis(assembly.Id);

                var registeredRequestParameters = service.GetRegisteredCustomApiRequestParameters(assembly.Id);
                var registeredResponseProperties = service.GetRegisteredCustomApiResponseProperties(assembly.Id);

                Console.WriteLine("Parsing...");

                registeredPluginTypes = registeredPluginTypes.Where(p => !registeredCustomApis.Any(c => c.PluginTypeId.Id == p.Id)).ToList();

                var steps = registeredSteps.Select(s => _importer.CreateStepFromRemote(s, registeredStepImages));
                var pluginsAndWorkflows = registeredPluginTypes.Select(p => _importer.CreatePluginFromRemote(p, steps));

                var plugins = pluginsAndWorkflows.Where(p => !p.IsWorkflow).ToList();
                var workflows = pluginsAndWorkflows.Where(p => p.IsWorkflow).ToList();


                var customApis = registeredCustomApis
                    .Select(c => _importer.CreateCustomApiFromRemote(c, registeredRequestParameters, registeredResponseProperties))
                    .ToList();

                registeredAssembly = new AssemblyContext()
                {
                    Assembly = assembly,
                    Plugins = plugins,
                    CustomApis = customApis,
                    Workflows = workflows

                };
            }
            else
            {
                registeredAssembly = new AssemblyContext()
                {
                    Assembly = assembly,
                    Plugins = new List<Plugin>(),
                    Workflows = new List<Plugin>(),
                    CustomApis = new List<CustomApi>(),
                };
            }
            return registeredAssembly;
        }

        public IFlatAssemblyContext CreateFlatAssemblyContextFromAssemblyContext(IAssemblyContext from)
        {
            var assembly = from.Assembly;
            var plugins = from.Plugins;
            var steps = new List<Step>();
            foreach (var plugin in plugins)
            {
                steps.AddRange(plugin.Steps);
            }

            var stepImages = new List<StepImage>();
            foreach (var step in steps)
            {
                stepImages.Add(step.PreImage);
                stepImages.Add(step.PostImage);
            }

            var workflows = from.Workflows;
            var customApis = from.CustomApis;
            var customApiRequestParameters = new List<CustomApiRequestParameter>();
            var customApiResponseProperties = new List<CustomApiResponseProperty>();
            foreach (var customApi in customApis)
            {
                customApiRequestParameters.AddRange(customApi.InArguments);
                customApiResponseProperties.AddRange(customApi.OutArguments);
            }

            var flatAssembly = new FlatAssemblyContext()
            {
                Assembly = assembly,
                Plugins = plugins,
                Steps = steps,
                StepImages = stepImages,
                Workflows = workflows,
                CustomApis = customApis,
                CustomApiRequestParameters = customApiRequestParameters,
                CustomApiResponseProperties = customApiResponseProperties
            };
            return flatAssembly;
        }
    }
}