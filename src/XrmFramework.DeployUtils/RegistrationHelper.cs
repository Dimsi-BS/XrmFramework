// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.DeployUtils;

public partial class RegistrationHelper
{
	private readonly AssemblyDiffFactory _assemblyDiffFactory;
	private readonly IAssemblyExporter _assemblyExporter;
	private readonly IAssemblyFactory _assemblyFactory;
	private readonly IRegistrationService _registrationService;

	public RegistrationHelper(IRegistrationService registrationService,
		IAssemblyExporter assemblyExporter,
		IAssemblyFactory assemblyFactory,
		AssemblyDiffFactory assemblyDiffFactory)
	{
		_registrationService = registrationService;
		_assemblyExporter = assemblyExporter;
		_assemblyFactory = assemblyFactory;
		_assemblyDiffFactory = assemblyDiffFactory;
	}

	/// <summary>
	///     Entrypoint for registering the <typeparamref name="TPlugin" /> in the solution <paramref name="projectName" />
	/// </summary>
	/// <typeparam name="TPlugin">Root type of all components to deploy, should be <c>XrmFramework.Plugin</c></typeparam>
	/// <param name="projectName">Name of the local project as named in <c>xrmFramework.config</c></param>
	public static void RegisterPluginsAndWorkflows<TPlugin>(string projectName)
	{
		var localDll = typeof(TPlugin).Assembly;
		var serviceProvider = ServiceCollectionHelper.ConfigureForDeploy(localDll.GetName().Name);

		var solutionSettings = serviceProvider.GetRequiredService<IOptions<DeploySettings>>();

		Console.WriteLine($"Assembly {localDll.GetName().Name}\n");
		Console.WriteLine(
			$"You are about to deploy on organization:\nUrl : {solutionSettings.Value.Url}\nClientId : {solutionSettings.Value.ClientId}\nIf ok press any key.");
		Console.ReadKey();
		Console.WriteLine("Connecting to CRM...");

		var solutionContext = serviceProvider.GetRequiredService<ISolutionContext>();
		solutionContext.InitSolutionContext();

		var registrationHelper = serviceProvider.GetRequiredService<RegistrationHelper>();

		registrationHelper.Register(localDll);
	}

	/// <summary>
	///     Main algorithm for deploying the <see cref="Assembly" /> assembly
	/// </summary>
	/// <param name="localDll">The local Assembly, should appear in <c>xrmFramework.config</c></param>
	protected void Register(Assembly localDll)
	{
		RegisterAssembly(localDll);

		Console.WriteLine("\tFetching Local Assembly...");

		var localAssembly = _assemblyFactory.CreateFromLocalAssemblyContext(localDll);

		Console.WriteLine("\tFetching Remote Assembly...");

		var registeredAssembly =
			_assemblyFactory.CreateFromRemoteAssemblyContext(_registrationService, localDll.GetName().Name);

		Console.WriteLine("\tComputing Difference...");

		var registrationStrategy = _assemblyDiffFactory.ComputeDiffPatch(localAssembly, registeredAssembly);

		Console.WriteLine($"\tExecuting Registration Strategy...");

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

		var allRequests = CreateDeleteRequests(strategyPool).ToList();

		allRequests.AddRange(CreateUpdateRequests(strategyPool));

		ExecuteAllRequests(allRequests);

		var componentsToCreate = strategyPool.Where(c =>
			c.RegistrationState == RegistrationState.ToCreate);
		_assemblyExporter.CreateAllComponents(componentsToCreate);
	}

	private void ExecuteAllRequests(List<OrganizationRequest> allRequests)
	{
		var crmRequests = InitCrmRequest();
		while (allRequests.Any())
		{
			crmRequests.Requests.AddRange(allRequests.Take(1000));
			allRequests.RemoveRange(0, Math.Min(allRequests.Count, 1000));
			var results = (ExecuteMultipleResponse) _registrationService.Execute(crmRequests);
			if (results.IsFaulted) throw new Exception(results.Responses.Last().Fault.Message);
			crmRequests.Requests.Clear();
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
			Console.WriteLine($"\tCreating {operation.HumanName}...");
			_assemblyExporter.CreateComponent(operation);
		}

		else if (operation.RegistrationState == RegistrationState.ToUpdate)
		{
			Console.WriteLine($"\tUpdating {operation.HumanName}");
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

	private static ExecuteMultipleRequest InitCrmRequest()
	{
		return new ExecuteMultipleRequest
		{
			// Assign settings that define execution behavior: continue on error, return responses.
			Settings = new ExecuteMultipleSettings()
			{
				ContinueOnError = false,
				ReturnResponses = true
			},
			// Create an empty organization request collection.
			Requests = new OrganizationRequestCollection()
		};
	}
}