using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.BindingModel;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Model;

/// <summary>
///     Metadata of a Nuget Package
/// </summary>
[CrmEntity(PluginPackageDefinition.EntityName)]
public class PluginPackage : BaseCrmComponent, IBindingModel, IAssemblyComponent
{
	[CrmMapping(PluginPackageDefinition.Columns.Name)]
	public string Name { get; set; }

	[CrmMapping(PluginPackageDefinition.Columns.Version)]
	public string Version { get; set; }

	public byte[] Content { get; set; }

	public override string UniqueName
	{
		get => Name;
		set => Name = value;
	}

	public string HumanName => "Package";

	/// <summary>
	///     Name of the assembly the package wraps, i.e. the unique name stripped of the publisher's
	///     customization prefix. Split on the FIRST separator only: assembly names routinely contain
	///     underscores, and splitting on all of them used to truncate the name to its second segment.
	/// </summary>
	public string AssemblyName
	{
		get
		{
			var separatorIndex = Name?.IndexOf('_') ?? -1;
			return separatorIndex < 0 ? Name : Name.Substring(separatorIndex + 1);
		}
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
