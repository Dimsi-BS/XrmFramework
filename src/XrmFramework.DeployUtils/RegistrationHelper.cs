﻿// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Spectre.Console;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Exporters;
using XrmFramework.DeployUtils.Factories;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;
using XrmFramework.DeployUtils.Service;
using XrmFramework.RemoteDebugger.Client.Recorder;
using IAssemblyFactory = XrmFramework.DeployUtils.Factories.IAssemblyFactory;

namespace XrmFramework.DeployUtils;

public partial class RegistrationHelper
{
    private readonly AssemblyDiffFactory _assemblyDiffFactory;
    private readonly IAssemblyExporter _assemblyExporter;
    private readonly IAssemblyFactory _assemblyFactory;
    private readonly IConsoleService _consoleService;
    private readonly IRegistrationService _registrationService;

    public RegistrationHelper(IRegistrationService registrationService,
      IAssemblyExporter assemblyExporter,
      IAssemblyFactory assemblyFactory,
      IConsoleService consoleService,
      AssemblyDiffFactory assemblyDiffFactory)
    {
        _registrationService = registrationService;
        _assemblyExporter = assemblyExporter;
        _assemblyFactory = assemblyFactory;
        _consoleService = consoleService;
        _assemblyDiffFactory = assemblyDiffFactory;
    }

    /// <summary>
    ///     Entrypoint for registering the <typeparamref name="TPlugin" /> in the solution <paramref name="projectName" />
    /// </summary>
    /// <typeparam name="TPlugin">Root type of all components to deploy, should be <c>XrmFramework.Plugin</c></typeparam>
    public static void RegisterPluginsAndWorkflows<TPlugin>()
    {
        var localDll = typeof(TPlugin).Assembly;
        var serviceCollection = DeployServiceCollectionFactory.CreateServiceCollection(localDll.GetName().Name);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var solutionSettingsOptions = serviceProvider.GetRequiredService<IOptions<DeploySettings>>();

        AnsiConsole.WriteLine($@"Assembly {localDll.GetName().Name}");
        AnsiConsole.WriteLine(@"You are about to deploy on organization:");
        AnsiConsole.WriteLine(@$"Url : {solutionSettingsOptions.Value.Url}");
        AnsiConsole.WriteLine(@$"ClientId : {solutionSettingsOptions.Value.ClientId}");
        AnsiConsole.Ask("If ok press any key.", true);
        AnsiConsole.WriteLine(@"Connecting to CRM...");

        var solutionContext = serviceProvider.GetRequiredService<ISolutionContext>();
        solutionContext.InitSolutionContext();

        var registrationHelper = serviceProvider.GetRequiredService<RegistrationHelper>();

        registrationHelper.Register(localDll);
    }

    /// <summary>
    ///     Main algorithm for deploying the <see cref="Assembly" /> assembly
    /// </summary>
    /// <param name="localDll">The local Assembly, should appear in <c>xrmFramework.config</c></param>
    private void Register(Assembly localDll)
    {
        _consoleService.SetStatus("Fetching Local Assembly...");

        var localAssembly = _assemblyFactory.CreateFromLocalAssemblyContext(localDll);

        _consoleService.SetStatus("Fetching Remote Assembly...");

        var registeredAssembly = _assemblyFactory.CreateFromRemoteAssemblyContext(_registrationService, localDll.GetName().Name);

        _consoleService.SetStatus("Computing Difference...");

        var registrationStrategy = _assemblyDiffFactory.ComputeDiffPatch(localAssembly, registeredAssembly);

        _consoleService.SetStatus(@"Executing Registration Strategy...");

        ExecuteStrategy(registrationStrategy);

    }

    /// <summary>
    ///     Deploy the <paramref name="strategy" />'s components according to their <see cref="RegistrationState" />
    /// </summary>
    /// <param name="strategy"></param>
    private void ExecuteStrategy(IAssemblyContext strategy)
    {
        var strategyPool = strategy.ComponentsOrderedPool;

        var stepsForMetadata = strategyPool.OfType<Step>()
          .Where(s => s.RegistrationState is RegistrationState.ToCreate or RegistrationState.ToUpdate);

        _assemblyExporter.InitExportMetadata(stepsForMetadata);

		var componentsToDelete = strategyPool.Where(c =>
			                                            c.RegistrationState == RegistrationState.ToDelete);
		_assemblyExporter.DeleteAllComponents(componentsToDelete);
		
		var componentsToUpdate = strategyPool.Where(c =>
			                                            c.RegistrationState == RegistrationState.ToUpdate);
		_assemblyExporter.UpdateAllComponents(componentsToUpdate);

		var componentsToCreate = strategyPool.Where(c =>
			c.RegistrationState == RegistrationState.ToCreate);
		_assemblyExporter.CreateAllComponents(componentsToCreate);
	}

	private void ExecuteAllRequests(List<OrganizationRequest> allRequests)
	{
		foreach (var organizationRequest in allRequests)
		{
			_registrationService.Execute(organizationRequest);
		}
	}

    /// <summary>
    ///     Creates the Assembly, deleting all obsolete components in the process by calling
    ///     <see cref="CreateDeleteRequests" />
    /// </summary>
    private void RegisterAssembly(Assembly localAssembly)
    {
        var localInfo = _assemblyFactory.GetLocalAssemblyInfo(localAssembly);
        var remoteInfo = _assemblyFactory.GetRemoteAssemblyInfo(_registrationService, localInfo.Name);

        var operation = _assemblyDiffFactory.ComputeAssemblyOperation(localInfo, remoteInfo);

        if (operation.RegistrationState == RegistrationState.ToCreate)
        {
            AnsiConsole.WriteLine($"\tCreating {operation.HumanName}...");
            _assemblyExporter.CreateComponent(operation);
        }

        else if (operation.RegistrationState == RegistrationState.ToUpdate)
        {
            AnsiConsole.WriteLine($"\tUpdating {operation.HumanName}");
            _assemblyExporter.UpdateComponent(operation);
        }
    }

    private IEnumerable<OrganizationRequest> CreateDeleteRequests(IReadOnlyCollection<ICrmComponent> strategyPool)
    {
        var componentsToDelete = strategyPool
          .Where(d => d.RegistrationState == RegistrationState.ToDelete);

        return _assemblyExporter.ToDeleteRequestCollection(componentsToDelete);
    }

    private IEnumerable<OrganizationRequest> CreateUpdateRequests(IReadOnlyCollection<ICrmComponent> strategyPool)
    {
        var componentsToUpdate = strategyPool
          .Where(d => d.RegistrationState == RegistrationState.ToUpdate);

		return _assemblyExporter.ToUpdateRequestCollection(componentsToUpdate);
	}
}