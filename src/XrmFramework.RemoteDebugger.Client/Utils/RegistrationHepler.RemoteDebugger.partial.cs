using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using XrmFramework.BindingModel;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;
using XrmFramework.RemoteDebugger;
using XrmFramework.RemoteDebugger.Client.Configuration;
using XrmFramework.RemoteDebugger.Model.CrmComponentInfos;

namespace XrmFramework.DeployUtils
{
    public partial class RegistrationHelper
    {
        private readonly Guid _debugSessionId;
        private readonly IMapper _mapper;


        public RegistrationHelper(IRegistrationService service,
                                  IAssemblyExporter exporter,
                                  IAssemblyFactory assemblyFactory,
                                  AssemblyDiffFactory diffFactory,
                                  IMapper mapper,
                                  IOptions<DebugSessionSettings> settings)
        {
            _assemblyExporter = exporter;
            _assemblyFactory = assemblyFactory;
            _registrationService = service;
            _assemblyDiffFactory = diffFactory;
            _mapper = mapper;
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

            var deployAssemblyDiff = _assemblyDiffFactory.ComputeDiffPatch(localAssembly, registeredAssembly);

            // We can remove now the diff components that are Ignore and whose children are too (recursively)
            // They would only get in the way otherwise

            deployAssemblyDiff.CleanChildrenWithState(RegistrationState.Ignore);

            var assemblyToDebug = _assemblyFactory.WrapDiffAssemblyForDebugDiff(deployAssemblyDiff);

            Console.WriteLine("Fetching Debug Assembly...");

            var debugAssembly = _assemblyFactory.CreateFromDebugAssembly(_registrationService, "XrmFramework.RemoteDebuggerPlugin", out Guid debugPluginId);

            Console.WriteLine("Computing Difference With Debug Assembly...");

            var remoteDebugDiff = _assemblyDiffFactory.ComputeDiffPatch(assemblyToDebug, debugAssembly);

            var debugStrategy = _assemblyFactory.WrapDebugDiffForDebugDeploy(remoteDebugDiff, debugPluginId, typeof(TPlugin));

            Console.WriteLine("Updating the Remote Debugger Plugin...");
            ExecuteRegistrationStrategy(debugStrategy);

            Console.WriteLine("Updating the Debug Session...");

            RegisterStepsToDebugSession(deployAssemblyDiff);
        }

        private void RegisterStepsToDebugSession(IAssemblyContext deployDiff)
        {
            var debugSession = _registrationService.GetById<DebugSession>(_debugSessionId);
            var patchInfo = _mapper.Map<AssemblyContextInfo>(deployDiff);

            var patches = !string.IsNullOrEmpty(debugSession.AssembliesDebugInfo)
                ? JsonConvert.DeserializeObject<List<AssemblyContextInfo>>(debugSession.AssembliesDebugInfo)
                : new List<AssemblyContextInfo>();

            var index = patches.FindIndex(p => p.AssemblyName == patchInfo.AssemblyName);
            if (index == -1)
                patches.Add(patchInfo);
            else
            {
                patches[index] = patchInfo;
            }

            debugSession.AssembliesDebugInfo = JsonConvert.SerializeObject(patches);

            var updatedDebugSession = debugSession.ToEntity(_registrationService);
            _registrationService.Update(updatedDebugSession);
        }
    }
}
