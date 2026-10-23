using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using XrmFramework.DeployUtils.Configuration;
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
	private readonly DeploySettings _deploySettings;
	private readonly DeployOptions _deployOptions;

	public AssemblyFactory(IAssemblyImporter importer,
		ISolutionContext solutionContext,
		DeploySettings deploySettings,
		DeployOptions deployOptions)
	{
		_importer = importer;
		_solutionContext = solutionContext;
		_deploySettings = deploySettings;
		_deployOptions = deployOptions;
	}

	public IAssemblyContext CreateFromLocalAssemblyContext(string dllPath)
	{
		// Inventory by EXECUTING the registration code (constructors / AddSteps), via the
		// shared XrmFramework.PluginInventory engine: in-process on net462, out-of-process
		// (net462 exe) from net8/net10. The result is the same JSON schema the reader consumes.
		var json = GetManifestJson(dllPath);
		if (string.IsNullOrEmpty(json))
			throw new InvalidOperationException(
				$"The inventory of assembly '{dllPath}' is empty. Verify that the DLL is indeed an XrmFramework plugin assembly.");

		var localAssembly = new AssemblyContext
		{
			AssemblyInfo = GetLocalAssemblyInfo(dllPath)
		};

		if (_deploySettings.UsePluginPackage)
			AttachLocalPackage(localAssembly.AssemblyInfo, dllPath);

		foreach (var plugin in PluginInventoryReader.ReadPlugins(json))
			localAssembly.AddChild(plugin);

		foreach (var workflow in PluginInventoryReader.ReadWorkflows(json))
			localAssembly.AddChild(workflow);

		// The publisher's prefix (connected environment) is used in the UniqueName of custom APIs.
		var prefix = _solutionContext.Publisher.CustomizationPrefix;
		foreach (var customApi in PluginInventoryReader.ReadCustomApis(json, prefix))
			localAssembly.AddChild(customApi);

		return localAssembly;
	}

	/// <summary>
	///     Reads the <c>.nupkg</c> built next to <paramref name="dllPath" /> and hangs it under the
	///     assembly, which is what switches the rest of the pipeline into package mode.
	/// </summary>
	private void AttachLocalPackage(AssemblyInfo assemblyInfo, string dllPath)
	{
		var packagePath = string.IsNullOrWhiteSpace(_deployOptions.PackagePath)
			? AssemblyImporter.ResolvePackagePath(dllPath)
			: _deployOptions.PackagePath;

		if (string.IsNullOrWhiteSpace(packagePath))
			throw new InvalidOperationException(
				$"No plugin package found for '{Path.GetFileName(dllPath)}'. The project is configured " +
				"for package deployment, so build it with <XrmFrameworkPluginPackage>true</XrmFrameworkPluginPackage>, " +
				"or point at the .nupkg with --package.");

		assemblyInfo.AddChild(_importer.CreatePackageFromLocal(assemblyInfo, packagePath));
	}

	private static string GetManifestJson(string dllPath)
	{
#if NET462_OR_GREATER
		// Already on .NET Framework: plugins are instantiated in-process.
		return PluginInventory.PluginInventoryEngine.BuildManifestJson(dllPath);
#else
		// net8/net10: delegated to the net462 exe (instantiating a net462 plugin here is impossible).
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
