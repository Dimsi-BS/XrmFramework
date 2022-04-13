using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.BindingModel;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;
using XrmFramework.RemoteDebugger;
using XrmFramework.RemoteDebugger.Client.Configuration;

namespace XrmFramework.DeployUtils
{
    public partial class RegistrationHelper
    {
        private readonly Guid _debugSessionId;


        public RegistrationHelper(IRegistrationService service,
                                  IAssemblyExporter exporter,
                                  IAssemblyFactory assemblyFactory,
                                  AssemblyDiffFactory diffFactory,
                                  IOptions<DebugSessionSettings> settings)
        {
            _assemblyExporter = exporter;
            _assemblyFactory = assemblyFactory;
            _registrationService = service;
            _assemblyDiffFactory = diffFactory;
            _debugSessionId = settings.Value.DebugSessionId;

        }

        public void UpdateDebugger<TPlugin>(string projectName)
        {
            Console.Write("Fetching Local Assembly...");

            var localAssembly = _assemblyFactory.CreateFromLocalAssemblyContext(typeof(TPlugin));

            localAssembly.Workflows.Clear();
            localAssembly.CustomApis.Clear();

            Console.WriteLine("Fetching Remote Assembly...");

            var registeredAssembly = _assemblyFactory.CreateFromRemoteAssemblyContext(_registrationService, projectName);

            registeredAssembly.Workflows.Clear();
            registeredAssembly.CustomApis.Clear();

            Console.WriteLine("Computing Difference With Local Assembly...");

            var deployPatch = _assemblyDiffFactory.ComputeDiffPatchFromAssemblies(localAssembly, registeredAssembly);

            var patchAsAssembly = _assemblyFactory.CreateFromDeployPatch(deployPatch);

            Console.WriteLine("Fetching Debug Assembly...");

            var debugAssembly = _assemblyFactory.CreateFromDebugAssembly(_registrationService, "XrmFramework.RemoteDebuggerPlugin", out Guid debugPluginId);

            Console.WriteLine("Computing Difference With Debug Assembly...");

            var remoteDebugPatch = _assemblyDiffFactory.ComputeDiffPatchFromAssemblies(patchAsAssembly, debugAssembly);

            _registrationStrategy = WrapRemoteDebugPatch(remoteDebugPatch, debugPluginId, typeof(TPlugin));

            Console.WriteLine("Updating the Remote Debugger Plugin...");
            ExecuteRegistrationStrategy();

            Console.WriteLine("Updating the Debug Session...");

            //RegisterStepsToDebugSession(_flatAssemblyContext.Steps);
        }

        private IDiffPatch WrapRemoteDebugPatch(IDiffPatch remoteDebugPatch, Guid debugPluginId, Type TPlugin)
        {
            /*
                * TODO Now that I know which operations I need to do for each component,
                * I need to remove the unnecessary Components that went through (PluginAssembly and Plugins), essentially keeping only steps and stepimages
                * Put the right Plugin Id to each step
                * Fill the UnsecureConfiguration in each step
                */

            remoteDebugPatch.SetComputedWhere(c => c.Component.Rank < 2);

            var localPlugins = TPlugin.Assembly.GetTypes();

            var stepsToInit = remoteDebugPatch
                .GetComponentsWhere(d => d.Component is Step && d.DiffResult == RegistrationState.ToCreate)
                .Select(s => (Step)s);

            foreach (var step in stepsToInit)
            {
                step.ParentId = debugPluginId;

                var assemblyQualifiedName = localPlugins.First(p => p.FullName == step.PluginTypeFullName).AssemblyQualifiedName;

                var config = string.IsNullOrWhiteSpace(step.UnsecureConfig) ? new StepConfiguration() : JsonConvert.DeserializeObject<StepConfiguration>(step.UnsecureConfig);

                config.PluginName = step.PluginTypeFullName;
                config.AssemblyQualifiedName = assemblyQualifiedName;
                config.DebugSessionId = _debugSessionId;

                step.UnsecureConfig = JsonConvert.SerializeObject(config);
            }

            return remoteDebugPatch;
        }

        private void RegisterStepsToDebugSession(ICollection<Step> steps)
        {
            var debugSession = _registrationService.GetById<DebugSession>(_debugSessionId);
            var stepsHashes = steps
                .Where(s => s.RegistrationState is RegistrationState.ToCreate or RegistrationState.ToUpdate)
                .Select(s => s.Description.GetHashCode());
        }
    }
}
