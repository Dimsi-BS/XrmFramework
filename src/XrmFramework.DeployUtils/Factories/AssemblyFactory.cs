using System;
using System.Linq;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Utils
{
    /// <summary>
    /// Base implementation of <see cref="IAssemblyFactory"/>
    /// </summary>
    partial class AssemblyFactory : IAssemblyFactory
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
                                  .Where(t => pluginType.IsAssignableFrom(t)
                                              && !customApiType.IsAssignableFrom(t)
                                              && t.IsPublic
                                              && !t.IsAbstract)
                                  .Select(t => _importer.CreatePluginFromType(t))
                                  .ToList();

            var workflows = Assembly.GetTypes()
                                    .Where(t => workflowType.IsAssignableFrom(t)
                                                && !t.IsAbstract
                                                && t.IsPublic)
                                    .Select(t => _importer.CreateWorkflowFromType(t))
                                    .ToList();

            var customApis = Assembly.GetTypes()
                                     .Where(t => customApiType.IsAssignableFrom(t)
                                                 && t.IsPublic
                                                 && !t.IsAbstract)
                                     .Select(t => _importer.CreateCustomApiFromType(t))
                                     .ToList();


            var localAssembly = _importer.CreateAssemblyFromLocal(Assembly);


            plugins.ForEach(localAssembly.AddChild);
            customApis.ForEach(localAssembly.AddChild);
            workflows.ForEach(localAssembly.AddChild);
            return localAssembly;
        }

        public IAssemblyContext CreateFromRemoteAssemblyContext(IRegistrationService service, string assemblyName)
        {
            var assembly = service.GetAssemblyByName(assemblyName);

            var registeredAssembly = _importer.CreateAssemblyFromRemote(assembly);

            if (assembly == null)
            {
                return registeredAssembly;
            }


            FillRemoteAssemblyContext(service, registeredAssembly);

            return registeredAssembly;
        }

        /// <summary>
        /// Fills an <see cref="IAssemblyContext"/> which only contains an <see cref="Model.AssemblyInfo"/> from the Crm
        /// </summary>
        /// <param name="service">The Client used for communicating with the Crm</param>
        /// <param name="registeredAssembly">The Assembly to fill</param>
        private void FillRemoteAssemblyContext(IRegistrationService service,
            IAssemblyContext registeredAssembly)
        {
            Console.WriteLine("Remote Assembly Exists, Fetching Components...");

            var registeredPluginTypes = service.GetRegisteredPluginTypes(registeredAssembly.AssemblyInfo.Id);

            var registeredSteps = service.GetRegisteredSteps(registeredAssembly.AssemblyInfo.Id);
            var registeredStepImages = service.GetRegisteredImages(registeredAssembly.AssemblyInfo.Id);


            var registeredCustomApis = service.GetRegisteredCustomApis(registeredAssembly.AssemblyInfo.Id);
            var registeredRequestParameters = service.GetRegisteredCustomApiRequestParameters(registeredAssembly.AssemblyInfo.Id);
            var registeredResponseProperties = service.GetRegisteredCustomApiResponseProperties(registeredAssembly.AssemblyInfo.Id);

            // Why ReSharper ? .All() means you have to go through the whole list
            // Btw this filters PluginTypes that are not CustomApis
            registeredPluginTypes = registeredPluginTypes.Where(p => !registeredCustomApis.Any(c => c.PluginTypeId.Id == p.Id))
                .ToList();

            Console.WriteLine("Parsing...");

            var steps = registeredSteps.Select(s => _importer.CreateStepFromRemote(s, registeredStepImages));
            var pluginsAndWorkflows = registeredPluginTypes
                .Select(p => _importer.CreatePluginFromRemote(p, steps))
                .ToList();

            var plugins = pluginsAndWorkflows.Where(p => !p.IsWorkflow).ToList();
            var workflows = pluginsAndWorkflows.Where(p => p.IsWorkflow).ToList();


            var customApis = registeredCustomApis
                .Select(c => _importer.CreateCustomApiFromRemote(c, registeredRequestParameters, registeredResponseProperties))
                .ToList();

            plugins.ForEach(registeredAssembly.AddChild);
            customApis.ForEach(registeredAssembly.AddChild);
            workflows.ForEach(registeredAssembly.AddChild);
        }
    }
}