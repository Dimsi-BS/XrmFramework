using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Factories;

/// <summary>
///     Factory in charge of creating the <see cref="IAssemblyContext" /> from various scenarios
/// </summary>
public partial interface IAssemblyFactory
{
    /// <summary>
    ///     Builds the local <see cref="IAssemblyContext" /> by INVENTORYING the assembly located at
    ///     <paramref name="dllPath" />: actual execution of the registration code (constructors /
    ///     AddSteps) via XrmFramework.PluginInventory (in-process on .NET Framework, or out of
    ///     process through the inventory host otherwise).
    /// </summary>
    /// <param name="dllPath">Path to the local plugin assembly to inventory.</param>
    /// <returns><see cref="IAssemblyContext" /> rebuilt from the inventory.</returns>
    IAssemblyContext CreateFromLocalAssemblyContext(string dllPath);

    /// <summary>
    ///     Imports the <paramref name="assemblyName" /> Remote Assembly and parses it as a <see cref="IAssemblyContext" />
    /// </summary>
    /// <param name="service">, a <see cref="IRegistrationService" /> used to retrieve the raw remote assembly data</param>
    /// <param name="assemblyName">, the name of the assembly on the Crm</param>
    /// <returns><see cref="IAssemblyContext" /> The parsed AssemblyContext</returns>
    IAssemblyContext CreateFromRemoteAssemblyContext(IRegistrationService service, string assemblyName);

	AssemblyInfo GetLocalAssemblyInfo(string dllPath);
	AssemblyInfo GetRemoteAssemblyInfo(IRegistrationService service, string assemblyName);
}