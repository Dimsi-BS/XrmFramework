using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.BindingModel;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model;

/// <summary>
///     Metadata of a Nuget Package
/// </summary>
[CrmEntity(PluginpackageDefinition.EntityName)]
public class PluginPackage : BaseCrmComponent, IBindingModel
{
	[CrmMapping(PluginpackageDefinition.Columns.Name)]
	public string Name { get; set; }

	[CrmMapping(PluginpackageDefinition.Columns.Version)]
	public string Version { get; set; }

	public byte[] Content { get; set; }

	public override string UniqueName
	{
		get => Name;
		set => Name = value;
	}

	#region CrmComponents Implementation

	public override int Rank => -1;
	public override bool DoAddToSolution => true;
	public override bool DoFetchTypeCode => true;
	public override string EntityTypeName => Deploy.PluginPackage.EntityLogicalName;

	#endregion

	#region Children Dummy Implementations

	public override IEnumerable<ICrmComponent> Children => Enumerable.Empty<ICrmComponent>();

	public override void AddChild(ICrmComponent child)
	{
		throw new ArgumentException("PluginPackage doesn't take children");
	}

	protected override void RemoveChild(ICrmComponent child)
	{
	}

	#endregion
}