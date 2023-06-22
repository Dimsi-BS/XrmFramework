using System;
using System.Collections.Generic;
using System.Linq;
using Deploy;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;
using PluginPackage = XrmFramework.DeployUtils.Model.PluginPackage;

namespace XrmFramework.DeployUtils.Utils;

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
		var updatedComponent = _converter.ToRegisterComponent(component);
		_registrationService.Update(updatedComponent);
	}

	public void UpdateAllComponents(IEnumerable<ICrmComponent> componentsToUpdate)
	{
		foreach (var registeringComponent in componentsToUpdate)
			UpdateComponent(registeringComponent);
	}

	public void AddComponentToSolution(ICrmComponent component)
	{
		if (component is PluginPackage package)
		{
			var assembly = _registrationService.GetAssemblyInfoByName(package.AssemblyName);
			AddComponentToSolution(assembly);
		}

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
		return new UpdateRequest()
		{
			Target = _converter.ToRegisterComponent(component)
		};
	}

	public DeleteRequest ToDeleteRequest(ICrmComponent component)
	{
		return new DeleteRequest()
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
		AddSolutionComponentRequest res = null;
		if (_solutionContext.GetComponentByObjectRef(objectRef) != null) return res;
		res = new AddSolutionComponentRequest
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
				PluginAssemblyDefinition.EntityName => (int) componenttype.PluginAssembly,
				PluginTypeDefinition.EntityName => (int) componenttype.PluginType,
				SdkMessageProcessingStepDefinition.EntityName => (int) componenttype.SDKMessageProcessingStep,
				SdkMessageProcessingStepImageDefinition.EntityName => (int) componenttype
					.SDKMessageProcessingStepImage,
				_ => res.ComponentType
			};

		return res;
	}

	public PluginType ToRegisterPluginType(Guid pluginAssemblyId, string pluginFullName)
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