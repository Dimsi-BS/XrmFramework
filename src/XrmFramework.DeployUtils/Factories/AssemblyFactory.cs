using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Importers;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Factories;

/// <summary>
///     Base implementation of <see cref="IAssemblyFactory" />
/// </summary>
internal partial class AssemblyFactory : IAssemblyFactory
{
	private readonly IAssemblyImporter _importer;
	private readonly ISolutionContext _solutionContext;

	public AssemblyFactory(IAssemblyImporter importer, ISolutionContext solutionContext)
	{
		_importer = importer;
		_solutionContext = solutionContext;
	}

	public IAssemblyContext CreateFromLocalAssemblyContext(string dllPath)
	{
		// Inventaire par EXÉCUTION du code d'enregistrement (constructeurs / AddSteps), via le
		// moteur partagé XrmFramework.PluginInventory : in-process sur net462, hors-process
		// (exe net462) depuis net8/net10. Le résultat est le même schéma JSON que le reader consomme.
		var json = GetManifestJson(dllPath);
		if (string.IsNullOrEmpty(json))
			throw new InvalidOperationException(
				$"L'inventaire de l'assembly '{dllPath}' est vide. Vérifiez que la DLL est bien une assembly plugin XrmFramework.");

		var localAssembly = new AssemblyContext
		{
			AssemblyInfo = GetLocalAssemblyInfo(dllPath)
		};

		foreach (var plugin in PluginInventoryReader.ReadPlugins(json))
			localAssembly.AddChild(plugin);

		foreach (var workflow in PluginInventoryReader.ReadWorkflows(json))
			localAssembly.AddChild(workflow);

		// Le préfixe du publisher (environnement connecté) entre dans le UniqueName des custom APIs.
		var prefix = _solutionContext.Publisher.CustomizationPrefix;
		foreach (var customApi in PluginInventoryReader.ReadCustomApis(json, prefix))
			localAssembly.AddChild(customApi);

		return localAssembly;
	}

	private static string GetManifestJson(string dllPath)
	{
#if NET462_OR_GREATER
		// Déjà sous .NET Framework : on instancie les plugins in-process.
		return PluginInventory.PluginInventoryEngine.BuildManifestJson(dllPath);
#else
		// net8/net10 : délégué à l'exe net462 (impossible d'instancier un plugin net462 ici).
		return PluginInventoryProcessRunner.Run(dllPath);
#endif
	}

	public IAssemblyContext CreateFromRemoteAssemblyContext(IRegistrationService service, string assemblyName)
	{
		var assembly = service.GetAssemblyInfoByName(assemblyName);

		var registeredAssembly = _importer.CreateAssemblyFromRemote(assembly);

		if (assembly == null) return registeredAssembly;

		FillRemoteAssemblyContext(service, registeredAssembly);

		return registeredAssembly;
	}

	public AssemblyInfo GetLocalAssemblyInfo(string dllPath)
	{
		var result = _importer.CreateAssemblyFromLocal(dllPath);
		return result;
	}

	public AssemblyInfo GetRemoteAssemblyInfo(IRegistrationService service, string assemblyName)
	{
		return service.GetAssemblyInfoByName(assemblyName);
	}

	/// <summary>
	///     Fills an <see cref="IAssemblyContext" /> which only contains an <see cref="Model.AssemblyInfo" /> from the Crm
	/// </summary>
	/// <param name="service">The Client used for communicating with the Crm</param>
	/// <param name="registeredAssembly">The Assembly to fill</param>
	private void FillRemoteAssemblyContext(IRegistrationService service,
		IAssemblyContext registeredAssembly)
	{
		var customApis = GetParsedCustomApis(service, registeredAssembly.AssemblyInfo.Id);

		var registeredPluginTypes = service.GetRegisteredPluginTypes(registeredAssembly.AssemblyInfo.Id);

		// This filters PluginTypes that are not CustomApis
		registeredPluginTypes = registeredPluginTypes.Where(p => customApis.TrueForAll(c => c.ParentId != p.Id))
			.ToList();

		var steps = GetParsedSteps(service, registeredAssembly.AssemblyInfo.Id);

		var pluginsAndWorkflows = registeredPluginTypes
			.Select(p => _importer.CreatePluginFromRemote(p, steps))
			.ToList();

		var plugins = pluginsAndWorkflows.Where(p => !p.IsWorkflow).ToList();
		var workflows = pluginsAndWorkflows.Where(p => p.IsWorkflow).ToList();

		plugins.ForEach(registeredAssembly.AddChild);
		workflows.ForEach(registeredAssembly.AddChild);
		customApis.ForEach(registeredAssembly.AddChild);
	}


	private List<CustomApi> GetParsedCustomApis(IRegistrationService service, Guid targetId)
	{
		var registeredCustomApis = service.GetRegisteredCustomApis(targetId);
		var registeredRequestParameters = service.GetRegisteredCustomApiRequestParameters(targetId);
		var registeredResponseProperties = service.GetRegisteredCustomApiResponseProperties(targetId);

		var customApis = registeredCustomApis
			.Select(c =>
				_importer.CreateCustomApiFromRemote(c, registeredRequestParameters, registeredResponseProperties))
			.ToList();

		return customApis;
	}

	private List<Step> GetParsedSteps(IRegistrationService service, Guid targetId)
	{
		var registeredSteps = service.GetRegisteredSteps(targetId);
		var registeredStepImages = service.GetRegisteredImages(targetId);

		var steps = new List<Step>();

		foreach (var s in registeredSteps)
		{
			if (!_importer.TryCreateStepFromRemote(s, registeredStepImages, out var step))
			{
				continue;
			}
			
			steps.Add(step);
		}
		
		return steps.ToList();
	}
}
