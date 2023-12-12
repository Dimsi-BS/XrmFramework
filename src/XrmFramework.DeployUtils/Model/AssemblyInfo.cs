using XrmFramework.BindingModel;

namespace XrmFramework.DeployUtils.Model;

/// <summary>
///     Metadata of the Assembly
/// </summary>
[CrmEntity(PluginAssemblyDefinition.EntityName)]
public class AssemblyInfo : BaseCrmComponent, IBindingModel, IAssemblyComponent
{
	/// <summary>Common Name of the Assembly</summary>
	[CrmMapping(PluginAssemblyDefinition.Columns.Name)]
	public string Name { get; set; }

	[CrmMapping(PluginAssemblyDefinition.Columns.SourceType)]
	public TypeDeSource SourceType { get; set; }

	[CrmMapping(PluginAssemblyDefinition.Columns.IsolationMode)]
	public ModeDIsolation IsolationMode { get; set; }

	[CrmMapping(PluginAssemblyDefinition.Columns.Culture)]
	public string Culture { get; set; }

	[CrmMapping(PluginAssemblyDefinition.Columns.PublicKeyToken)]
	public string PublicKeyToken { get; set; }

	[CrmMapping(PluginAssemblyDefinition.Columns.Version)]
	public string Version { get; set; }

	[CrmMapping(PluginAssemblyDefinition.Columns.Description)]
	public string Description { get; set; }

	// [CrmMapping(PluginAssemblyDefinition.Columns.Content)]
	public byte[] Content { get; set; }

	[CrmMapping(PluginAssemblyDefinition.Columns.PackageId)]
	public PluginPackage Package { get; set; }

	[CrmMapping(PluginAssemblyDefinition.Columns.Id)]
	public new Guid Id
	{
		get => base.Id;
		set => base.Id = value;
	}

	#region ICrmComponents

	public override string EntityTypeName => PluginAssemblyDefinition.EntityName;

	public override string UniqueName
	{
		get => Name;
		set => Name = value;
	}

	public new RegistrationState RegistrationState
	{
		// If the package exists, it will be manipulated and I shouldn't touch the assembly
		// get => Package == null
		// 	? base.RegistrationState
		// 	: RegistrationState.Computed;
		get => base.RegistrationState;
		set => base.RegistrationState = value;
	}

	public override IEnumerable<ICrmComponent> Children =>
		Package == null
			? Enumerable.Empty<ICrmComponent>()
			: new[] {Package};

	public override void AddChild(ICrmComponent child)
	{
		if (child is null)
			return;
		if (child is not PluginPackage package)
			throw new ArgumentException("AssemblyInfo doesn't take this type of component");
		base.AddChild(package);
		Package = package;
	}

	protected override void RemoveChild(ICrmComponent child)
	{
		if (child is not PluginPackage)
			throw new ArgumentException("AssemblyInfo doesn't have this type of component");
		Package = null;
	}

	public string HumanName => "Assembly";
	public string AssemblyName => Name;
	public override int Rank => 0;
	public override bool DoAddToSolution => true;
	public override bool DoFetchTypeCode => false;

	#endregion
}