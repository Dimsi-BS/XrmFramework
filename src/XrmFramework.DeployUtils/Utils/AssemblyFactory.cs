using Deploy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Utils
{
    public class AssemblyFactory
    {
        private readonly IRegistrationContext _registrationContext;

        public AssemblyFactory(IRegistrationContext context)
        {
            _registrationContext = context;
        }

        public IAssemblyContext CreateFromLocalAssemblyContext(Type TPlugin)
        {
            var Assembly = TPlugin.Assembly;
            var pluginType = Assembly.GetType("XrmFramework.Plugin");
            var customApiType = Assembly.GetType("XrmFramework.CustomApi");
            var workflowType = Assembly.GetType("XrmFramework.Workflow.CustomWorkflowActivity");

            var PluginTypes = Assembly.GetTypes()
                                     .Where(t => pluginType.IsAssignableFrom(t)
                                              && !customApiType.IsAssignableFrom(t)
                                              && t.IsPublic
                                              && !t.IsAbstract)
                                     .ToList();

            var WorkFlowTypes = Assembly.GetTypes()
                                       .Where(t => workflowType.IsAssignableFrom(t)
                                                && !t.IsAbstract
                                                && t.IsPublic)
                                       .ToList();

            var CustomApiTypes = Assembly.GetTypes()
                                        .Where(t => customApiType.IsAssignableFrom(t)
                                                 && t.IsPublic
                                                 && !t.IsAbstract)
                                        .ToList();

            var plugins = AssemblyBridge.CreateInstanceOfTypeList<Plugin>(PluginTypes, PluginRegistrationType.Plugin, _registrationContext);

            var workflows = AssemblyBridge.CreateInstanceOfTypeList<Plugin>(WorkFlowTypes, PluginRegistrationType.Workflow, _registrationContext);

            var customApis = AssemblyBridge.CreateInstanceOfTypeList<CustomApi>(CustomApiTypes, PluginRegistrationType.CustomApi, _registrationContext);

            var localAssembly = new AssemblyContext
            {
                Assembly = AssemblyBridge.ToPluginAssembly(Assembly),
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

                var pluginsAndWorkflows = AssemblyBridge.FromCrmPlugins(registeredPluginTypes, registeredSteps, registeredStepImages, _registrationContext);

                var plugins = pluginsAndWorkflows.Where(p => !p.IsWorkflow).ToList();
                var workflows = pluginsAndWorkflows.Where(p => p.IsWorkflow).ToList();


                var customApis = AssemblyBridge.FromCrmCustomApis(registeredCustomApis, registeredRequestParameters, registeredResponseProperties, _registrationContext);


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
