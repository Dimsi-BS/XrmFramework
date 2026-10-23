using System;
using System.Collections.Generic;
using System.Linq;
using Deploy;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Converters;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;
using XrmFramework.DeployUtils.Service;
using PluginPackage = XrmFramework.DeployUtils.Model.PluginPackage;

namespace XrmFramework.DeployUtils.Exporters;

/// <summary>
///     Base implementation of <see cref="IAssemblyExporter" />
/// </summary>
public partial class AssemblyExporter : IAssemblyExporter
{
	private readonly ICrmComponentConverter _converter;
	private readonly IRegistrationService _registrationService;
	private readonly ISolutionContext _solutionContext;

	public AssemblyExporter(ISolutionContext solutionContext, IRegistrationService service,
		ICrmComponentConverter converter)
	{
		_solutionContext = solutionContext;
		_registrationService = service;
		_converter = converter;
	}

	public void CreateAllComponents(IEnumerable<ICrmComponent> componentsToCreate)
	{
		var sortedList = componentsToCreate.ToList();
		sortedList.Sort((x, y) => x.Rank.CompareTo(y.Rank));

		foreach (var component in sortedList)
		{
			component.Id = Guid.Empty;
			CreateComponent(component);
		}
	}

	public IEnumerable<OrganizationRequest> ToUpdateRequestCollection(IEnumerable<ICrmComponent> componentsToUpdate)
	{
		var sortedList = componentsToUpdate.ToList();

		sortedList.Sort((x, y) => x.Rank.CompareTo(y.Rank));

		return sortedList.Select(ToUpdateRequest);
	}

	public void DeleteAllComponents(IEnumerable<ICrmComponent> componentsToDelete)
	{
		var sortedList = componentsToDelete.ToList();
		sortedList.Sort((x, y) => -x.Rank.CompareTo(y.Rank));

		foreach (var component in sortedList) DeleteComponent(component);
	}

	public void UpdateComponent(ICrmComponent component)
	{
		var updatedComponent = _converter.ToUpdateComponent(component);
		_registrationService.Update(updatedComponent);
	}

	public void PushPackage(PluginPackage localPackage, PluginPackage registeredPackage)
	{
		if (localPackage == null) throw new ArgumentNullException(nameof(localPackage));

		if (registeredPackage == null)
		{
			// First push: name, unique name, version and content all travel. They are the only
			// chance to set the name and the version, which the platform then freezes.
			localPackage.Id = Guid.Empty;
			CreateComponent(localPackage);
			return;
		}

		localPackage.Id = registeredPackage.Id;
		UpdateComponent(localPackage);
		AddComponentToSolution(localPackage);
	}

	public void UpdateAllComponents(IEnumerable<ICrmComponent> componentsToUpdate)
	{
		foreach (var registeringComponent in componentsToUpdate)
			UpdateComponent(registeringComponent);
	}

	public void AddComponentToSolution(ICrmComponent component)
	{
		// A package used to also add its assembly here, by name. That cannot work on a first push:
		// Dataverse creates the pluginassembly row from the package AFTER this call, so the lookup
		// came back null and the recursive call threw. The assembly is added by the deployment once
		// the package has been unpacked and the row actually exists.
		if (!component.DoAddToSolution) return;

		int? entityTypeCode = component.DoFetchTypeCode
			? _registrationService.GetIntEntityTypeCode(component.EntityTypeName)
			: null;

		var addSolutionComponentRequest =
			CreateAddSolutionComponentRequest(new EntityReference(component.EntityTypeName, component.Id),
				entityTypeCode);

		if (addSolutionComponentRequest != null) _registrationService.Execute(addSolutionComponentRequest);
	}

	public void InitExportMetadata(IEnumerable<Step> steps)
	{
		_solutionContext.InitExportMetadata(steps);
	}

	private UpdateRequest ToUpdateRequest(ICrmComponent component)
	{
		return new UpdateRequest
		{
			Target = _converter.ToRegisterComponent(component)
		};
	}

	internal DeleteRequest ToDeleteRequest(ICrmComponent component)
	{
		return new DeleteRequest
		{
			Target = new EntityReference(component.EntityTypeName, component.Id)
		};
	}

	/// <summary>
	///     Creates a request to add a component in the form of an <see cref="EntityReference" />
	///     to the current Solution
	/// </summary>
	/// <param name="objectRef"></param>
	/// <param name="objectTypeCode"></param>
	/// <returns></returns>
	public AddSolutionComponentRequest CreateAddSolutionComponentRequest(EntityReference objectRef,
		int? objectTypeCode = null)
	{
		if (_solutionContext.GetComponentByObjectRef(objectRef) != null)
		{
			return null;
		}

		var res = new AddSolutionComponentRequest
		{
			AddRequiredComponents = false,
			ComponentId = objectRef.Id,
			SolutionUniqueName = _solutionContext.SolutionName
		};

		if (objectTypeCode.HasValue)
			res.ComponentType = objectTypeCode.Value;
		else
			res.ComponentType = objectRef.LogicalName switch
			{
				PluginAssemblyDefinition.EntityName => (int) ComponentType.PluginAssembly,
				PluginTypeDefinition.EntityName => (int) ComponentType.PluginType,
				SdkMessageProcessingStepDefinition.EntityName => (int) ComponentType.SDKMessageProcessingStep,
				SdkMessageProcessingStepImageDefinition.EntityName => (int) ComponentType.SDKMessageProcessingStepImage,
				_ => res.ComponentType
			};

		return res;
	}

	private PluginType ToRegisterPluginType(Guid pluginAssemblyId, string pluginFullName)
	{
		var t = new PluginType
		{
			PluginAssemblyId = new EntityReference
			{
				LogicalName = PluginAssemblyDefinition.EntityName,
				Id = pluginAssemblyId
			},
			TypeName = pluginFullName,
			FriendlyName = pluginFullName,
			Name = pluginFullName,
			Description = pluginFullName
		};

		return t;
	}
}
