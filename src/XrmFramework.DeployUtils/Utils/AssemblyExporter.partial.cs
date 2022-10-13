using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils;

public partial class AssemblyExporter
{
	/// <summary>
	///     Deletes a <see cref="ICrmComponent" /> on the Crm
	/// </summary>
	/// This method is in a partial file because it is implemented differently in the RemoteDebugger.Client project
	public void DeleteComponent(ICrmComponent component)
	{
		_registrationService.Delete(component.EntityTypeName, component.Id);

		if (component is CustomApi customApi)
			_registrationService.Delete(PluginTypeDefinition.EntityName, customApi.ParentId);
	}

	public IEnumerable<OrganizationRequest> ToDeleteRequestCollection(IEnumerable<ICrmComponent> componentsToDelete)
	{
		var sortedList = componentsToDelete.ToList();

		var customApiPluginTypes = sortedList.OfType<CustomApi>()
			.Select(c => new Plugin(c.UniqueName) {Id = c.ParentId})
			.ToList();
		sortedList.AddRange(customApiPluginTypes);

		sortedList.Sort((x, y) => -x.Rank.CompareTo(y.Rank));

		return sortedList.Select(ToDeleteRequest);
	}

	/// <summary>
	///     Creates a <see cref="ICrmComponent" /> on the Crm
	/// </summary>
	/// This method is in a partial file because it is implemented differently in the RemoteDebugger.Client project
	public Guid CreateComponent(ICrmComponent component)
	{
		if (component is CustomApi customApi) CreateCustomApiPluginType(customApi);

		component.Id = component is IAssemblyContext or PluginPackage
			? Guid.NewGuid()
			: Guid.Empty;

		var registeringComponent = _converter.ToRegisterComponent(component);
		var result = _registrationService.Create(registeringComponent);
		component.Id = result;
		registeringComponent.Id = component.Id;

		AddComponentToSolution(component);

		return result;
	}


	private void CreateCustomApiPluginType(CustomApi customApi)
	{
		var customApiPluginType = ToRegisterPluginType(customApi.AssemblyId, customApi.FullName);
		var id = _registrationService.Create(customApiPluginType);
		customApi.ParentId = id;
	}
}