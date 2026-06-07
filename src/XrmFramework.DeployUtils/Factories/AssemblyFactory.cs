using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

	public IAssemblyContext CreateFromManifestAssemblyContext(Assembly assembly)
	{
		var json = PluginManifestReader.ReadManifestJson(assembly);
		if (string.IsNullOrEmpty(json))
			throw new InvalidOperationException(
				$"Aucun manifeste ({PluginManifestReader.ManifestTypeName}) trouvé dans l'assembly '{assembly.GetName().Name}'. " +
				"Le package XrmFramework.PluginManifest.Generator est-il bien référencé par le projet plugin ?");

		var localAssembly = new AssemblyContext
		{
			AssemblyInfo = GetLocalAssemblyInfo(assembly)
		};

		foreach (var plugin in PluginManifestReader.ReadPlugins(json))
			localAssembly.AddChild(plugin);

		foreach (var workflow in PluginManifestReader.ReadWorkflows(json))
			localAssembly.AddChild(workflow);

		// Le préfixe du publisher (environnement connecté) entre dans le UniqueName des custom APIs.
		var prefix = _solutionContext.Publisher.CustomizationPrefix;
		foreach (var customApi in PluginManifestReader.ReadCustomApis(json, prefix))
			localAssembly.AddChild(customApi);

		return localAssembly;
	}

	public IAssemblyContext CreateFromRemoteAssemblyContext(IRegistrationService service, string assemblyName)
	{
		var assembly = service.GetAssemblyInfoByName(assemblyName);

		var registeredAssembly = _importer.CreateAssemblyFromRemote(assembly);

		if (assembly == null) return registeredAssembly;

		FillRemoteAssemblyContext(service, registeredAssembly);

		return registeredAssembly;
	}

	public AssemblyInfo GetLocalAssemblyInfo(Assembly assembly)
	{
		var result = _importer.CreateAssemblyFromLocal(assembly);
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
