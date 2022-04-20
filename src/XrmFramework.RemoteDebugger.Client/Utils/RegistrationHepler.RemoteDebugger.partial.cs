using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.BindingModel;
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

            var deployDiffAssembly = _assemblyDiffFactory.ComputeDiffPatchFromAssemblies(localAssembly, registeredAssembly);

            //var patchAsAssembly = _assemblyFactory.CreateFromDeployPatch(deployPatch);

            Console.WriteLine("Fetching Debug Assembly...");

            var debugAssembly = _assemblyFactory.CreateFromDebugAssembly(_registrationService, "XrmFramework.RemoteDebuggerPlugin", out Guid debugPluginId);

            Console.WriteLine("Computing Difference With Debug Assembly...");

            //var remoteDebugPatch = _assemblyDiffFactory.ComputeDiffPatchFromAssemblies(patchAsAssembly, debugAssembly);

            //_registrationStrategy = WrapRemoteDebugPatch(remoteDebugPatch, debugPluginId, typeof(TPlugin));

            Console.WriteLine("Updating the Remote Debugger Plugin...");
            //ExecuteRegistrationStrategy();

            Console.WriteLine("Updating the Debug Session...");

            //RegisterStepsToDebugSession(_flatAssemblyContext.Steps);
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
