﻿using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.Options;
using Spectre.Console;
using XrmFramework.BindingModel;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Exporters;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;
using XrmFramework.RemoteDebugger;
using XrmFramework.DeployUtils.Factories;
using XrmFramework.DeployUtils.Model.Interfaces;
using XrmFramework.RemoteDebugger.Client.Configuration;
using XrmFramework.RemoteDebugger.Model.CrmComponentInfos;
using IAssemblyFactory = XrmFramework.DeployUtils.Factories.IAssemblyFactory;
using XrmFramework.RemoteDebugger.Client.Recorder;

namespace XrmFramework.DeployUtils;

public partial class RegistrationHelper
{
    private readonly DebugSession _debugSession;
    private readonly DebugAssemblySettings _debugSettings;
    private readonly IMapper _mapper;

    public RegistrationHelper(IRegistrationService service,
      IAssemblyExporter exporter,
      IAssemblyFactory assemblyFactory,
      AssemblyDiffFactory diffFactory,
      IConsoleService consoleService,
      IMapper mapper,
      IDebugSessionProvider debugSessionProvider)
    {
        _assemblyExporter = exporter;
        _assemblyFactory = assemblyFactory;
        _registrationService = service;
        _assemblyDiffFactory = diffFactory;
        _mapper = mapper;
        _debugSession = debugSessionProvider.DebugSession;
        _debugSettings = new DebugAssemblySettings(_debugSession.Id);

        _consoleService = consoleService;
    }

    /// <summary>
    ///     Compares the following three <c>Assemblies</c> :
    ///     <list type="bullet">
    ///         <item>Local</item>
    ///         <item>Remote</item>
    ///         <item>RemoteDebugger</item>
    ///     </list>
    ///     And determines which operations are needed in order to have the Remote + Remote Debugger have the same behaviour as
    ///     the Local
    /// </summary>
    /// <remarks>
    ///     This will silence the obsolete steps deployed on Remote and add the new/updated ones to the Debugger so it will
    ///     still trigger and redirect to the Relay
    /// </remarks>
    /// <param name="assembly">The local Assembly to Debug</param>
    public void UpdateDebugger(Assembly assembly)
    {
        _consoleService.SetStatus(@$"Assembly {assembly.GetName().Name} (Fetching Local Assembly...)");

        var localAssembly = _assemblyFactory.CreateFromLocalAssemblyContext(assembly);

        localAssembly.Workflows.Clear();

        _consoleService.SetStatus(@$"Assembly {assembly.GetName().Name} (Fetching Remote Assembly...)");

        var registeredAssembly =
          _assemblyFactory.CreateFromRemoteAssemblyContext(_registrationService, assembly.GetName().Name);

        registeredAssembly?.Workflows.Clear();

        _consoleService.SetStatus(@$"Assembly {assembly.GetName().Name} (Computing Difference With Local Assembly...)");

        var deployAssemblyDiff = _assemblyDiffFactory.ComputeDiffPatch(localAssembly, registeredAssembly);

        var assemblyToDebug = _assemblyFactory.WrapDiffAssemblyForDebugDiff(deployAssemblyDiff);

        _consoleService.SetStatus(@$"Assembly {assembly.GetName().Name} (Fetching Debug Assembly...)");

        _debugSettings.TargetAssemblyUniqueName = localAssembly.AssemblyInfo.Name;
        var debugAssembly = _assemblyFactory.CreateFromDebugAssembly(_registrationService, _debugSettings);

        _consoleService.SetStatus(@$"Assembly {assembly.GetName().Name} (Computing Difference With Debug Assembly...)");

        var remoteDebugDiff = _assemblyDiffFactory.ComputeDiffPatch(assemblyToDebug, debugAssembly);

        var debugStrategy = _assemblyFactory.WrapDebugDiffForDebugStrategy(remoteDebugDiff, _debugSettings, assembly);

        _consoleService.SetStatus(@$"Assembly {assembly.GetName().Name} (Executing Registration Strategy...)");

        ExecuteStrategy(debugStrategy);

        _consoleService.SetStatus(@$"Assembly {assembly.GetName().Name} (Updating the Debug Session...)");

        RegisterStepsToDebugSession(deployAssemblyDiff);
    }

    /// <summary>
    ///     Pushes a patch of the current debug context on the Target Debug Session
    ///     The <see cref="AssemblyContextInfo" /> pushed contains metadata on the steps currently being debugged
    ///     (with tags <see cref="RegistrationState.ToDelete" /> or <see cref="RegistrationState.ToUpdate" />)
    ///     and will be ignored by the deployed Plugin if the Debugee is online and debugging
    /// </summary>
    /// <param name="deployDiff">
    ///     The <see cref="IAssemblyContext" /> to convert in a patch and push,
    ///     should be the diff between the <c>Local</c> and <c>Remote Assemblies</c>
    /// </param>
    private void RegisterStepsToDebugSession(IAssemblyContext deployDiff)
    {
        deployDiff.CleanChildrenWithState(RegistrationState.Ignore);

        var patchInfo = _mapper.Map<AssemblyContextInfo>(deployDiff);

        var patches = _debugSession.AssemblyContexts;

        var index = patches.FindIndex(p => p.AssemblyName == patchInfo.AssemblyName);
        if (index == -1)
            patches.Add(patchInfo);
        else
            patches[index] = patchInfo;

        var updatedDebugSession = _debugSession.ToEntity(_registrationService);
        _registrationService.Update(updatedDebugSession);
    }
}