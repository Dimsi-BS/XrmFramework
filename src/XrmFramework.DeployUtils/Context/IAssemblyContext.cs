using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context;

public interface IAssemblyContext : ICrmComponent
{
	/// <summary>
	///     <inheritdoc cref="Model.AssemblyInfo" />
	/// </summary>
	AssemblyInfo AssemblyInfo { get; }

	/// <summary>Plugins of the Assembly</summary>
	ICollection<Plugin> Plugins { get; }

	/// <summary>Workflows of the Assembly</summary>
	ICollection<Plugin> Workflows { get; }

	/// <summary>CustomApis of the Assembly</summary>
	ICollection<CustomApi> CustomApis { get; }

	/// <summary>
	///     Computes a Collection of all the components contained in the Assembly
	/// </summary>
	/// <remarks>
	///     The Collection is sorted by ascending <see cref="ICrmComponent.Rank" /><br />
	///     This way each component is guaranteed to appear before its children an so forth
	/// </remarks>
	IReadOnlyCollection<ICrmComponent> ComponentsOrderedPool { get; }

	/// <summary>
	///     Indicates if the assembly needs to be registered on the CRM first, through the package or the assembly
	/// </summary>
	/// <returns></returns>
	bool NeedsRegister();
}