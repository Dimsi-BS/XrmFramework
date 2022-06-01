using AutoMapper;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using XrmFramework.BindingModel;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;
using XrmFramework.RemoteDebugger;
using XrmFramework.RemoteDebugger.Client.Configuration;
using XrmFramework.RemoteDebugger.Model.CrmComponentInfos;

namespace XrmFramework.DeployUtils
{
    public partial class RegistrationHelper
    {
        private readonly DebugSession _debugSession;
        private readonly IMapper _mapper;
        private readonly DebugAssemblySettings _debugSettings;

        public RegistrationHelper(IRegistrationService service,
                                  IAssemblyExporter exporter,
                                  IAssemblyFactory assemblyFactory,
                                  AssemblyDiffFactory diffFactory,
                                  IMapper mapper,
                                  IOptions<DebugSession> settings)
        {
            _assemblyExporter = exporter;
            _assemblyFactory = assemblyFactory;
            _registrationService = service;
            _assemblyDiffFactory = diffFactory;
            _mapper = mapper;
            _debugSession = settings.Value;
            _debugSettings = new DebugAssemblySettings(_debugSession.Id);
        }

        /// <summary>
        /// Compares the following three <c>Assemblies</c> :
        /// <list type="bullet">
        ///     <item>Local</item>
        ///     <item>Remote</item>
        ///     <item>RemoteDebugger</item>
        /// </list>
        /// And determines which operations are needed in order to have the Remote + Remote Debugger have the same behaviour as the Local
        /// </summary>
        /// <remarks>
        /// This will silence the obsolete steps deployed on Remote and add the new/updated ones to the Debugger so it will still trigger and redirect to the Relay
        /// </remarks>
        /// <param name="Assembly">The local Assembly to Debug</param>
        public void UpdateDebugger(Assembly Assembly)
        {
            Console.Write("Fetching Local Assembly...");

            var localAssembly = _assemblyFactory.CreateFromLocalAssemblyContext(Assembly);

            localAssembly.Workflows.Clear();

            Console.WriteLine("Fetching Remote Assembly...");

            var registeredAssembly = _assemblyFactory.CreateFromRemoteAssemblyContext(_registrationService, Assembly.GetName().Name);

            registeredAssembly.Workflows.Clear();

            Console.WriteLine("Computing Difference With Local Assembly...");

            var deployAssemblyDiff = _assemblyDiffFactory.ComputeDiffPatch(localAssembly, registeredAssembly);

            var assemblyToDebug = _assemblyFactory.WrapDiffAssemblyForDebugDiff(deployAssemblyDiff);

            Console.WriteLine("Fetching Debug Assembly...");

            _debugSettings.TargetAssemblyUniqueName = localAssembly.UniqueName;
            var debugAssembly = _assemblyFactory.CreateFromDebugAssembly(_registrationService, _debugSettings);

            Console.WriteLine("Computing Difference With Debug Assembly...");

            var remoteDebugDiff = _assemblyDiffFactory.ComputeDiffPatch(assemblyToDebug, debugAssembly);

            var debugStrategy = _assemblyFactory.WrapDebugDiffForDebugStrategy(remoteDebugDiff, _debugSettings, Assembly);

            Console.WriteLine("Updating the Remote Debugger Plugin...");
            ExecuteStrategy(debugStrategy);

            Console.WriteLine("Updating the Debug Session...");

            RegisterStepsToDebugSession(deployAssemblyDiff);
        }

        /// <summary>
        /// Pushes a patch of the current debug context on the Target Debug Session
        /// The <see cref="AssemblyContextInfo"/> pushed contains metadata on the steps currently being debugged
        /// (with tags <see cref="RegistrationState.ToDelete"/> or <see cref="RegistrationState.ToUpdate"/>)
        /// and will be ignored by the deployed Plugin if the Debugee is online and debugging
        /// </summary>
        /// <param name="deployDiff">The <see cref="IAssemblyContext"/> to convert in a patch and push,
        /// should be the diff between the <c>Local</c> and <c>Remote Assemblies</c></param>
        private void RegisterStepsToDebugSession(IAssemblyContext deployDiff)
        {
            deployDiff.CleanChildrenWithState(RegistrationState.Ignore);

            var patchInfo = _mapper.Map<AssemblyContextInfo>(deployDiff);

            var patches = _debugSession.AssemblyContexts;

            var index = patches.FindIndex(p => p.AssemblyName == patchInfo.AssemblyName);
            if (index == -1)
                patches.Add(patchInfo);
            else
            {
                patches[index] = patchInfo;
            }

            var updatedDebugSession = _debugSession.ToEntity(_registrationService);
            _registrationService.Update(updatedDebugSession);
        }
    }
}
