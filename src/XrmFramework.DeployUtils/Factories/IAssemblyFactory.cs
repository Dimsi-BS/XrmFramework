using System.Reflection;
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
    ///     Imports the <paramref name="assembly" /> Local Assembly and parses it as a <see cref="IAssemblyContext" />
    /// </summary>
    /// <param name="assembly">The local assembly to load</param>
    /// <returns><see cref="IAssemblyContext" /> The parsed AssemblyContext</returns>
    IAssemblyContext CreateFromLocalAssemblyContext(Assembly assembly);

    /// <summary>
    ///     Imports the <paramref name="assemblyName" /> Remote Assembly and parses it as a <see cref="IAssemblyContext" />
    /// </summary>
    /// <param name="service">, a <see cref="IRegistrationService" /> used to retrieve the raw remote assembly data</param>
    /// <param name="assemblyName">, the name of the assembly on the Crm</param>
    /// <returns><see cref="IAssemblyContext" /> The parsed AssemblyContext</returns>
    IAssemblyContext CreateFromRemoteAssemblyContext(IRegistrationService service, string assemblyName);

	AssemblyInfo GetLocalAssemblyInfo(Assembly assembly);
	AssemblyInfo GetRemoteAssemblyInfo(IRegistrationService service, string assemblyName);
}