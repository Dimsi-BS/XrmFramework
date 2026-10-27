using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.BindingModel;
using XrmFramework.DeployUtils.Model.Interfaces;

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
	public SourceType SourceType { get; set; }

	[CrmMapping(PluginAssemblyDefinition.Columns.IsolationMode)]
	public IsolationMode IsolationMode { get; set; }

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

	#region ICrmComponents

	public override string EntityTypeName => PluginAssemblyDefinition.EntityName;

	public override string UniqueName
	{
		get => Name;
		set => Name = value;
	}

	/// <summary>
	///     In package mode the <c>pluginassembly</c> row is created and maintained by Dataverse out of
	///     the pushed package, so the framework must not write it: pushing the raw DLL as assembly
	///     content would undo what the package put there. The state is therefore forced to
	///     <see cref="Interfaces.RegistrationState.Computed" /> as soon as a package is attached, which
	///     keeps the assembly out of the create/update/delete batches without touching the diff.
	///     <para>
	///         Overridden rather than shadowed: the pipeline walks components as
	///         <see cref="ICrmComponent" />, so a <c>new</c> member would never be reached.
	///     </para>
	/// </summary>
	public override RegistrationState RegistrationState
	{
		get => Package == null
			? base.RegistrationState
			: Interfaces.RegistrationState.Computed;
		set => base.RegistrationState = value;
	}

	/// <summary>
	///     Always empty, the package included. A plugin package is not registered through the
	///     create/update/delete batches the diff produces: it has to be pushed on its own, before
	///     them, because Dataverse answers that push by creating the very plugin types the steps in
	///     those batches are registered against. Keeping it out of the assembly context's ordered
	///     component pool is what lets the deployment own that ordering - <see cref="Package" />
	///     stays reachable for it.
	/// </summary>
	public override IEnumerable<ICrmComponent> Children => Enumerable.Empty<ICrmComponent>();

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
