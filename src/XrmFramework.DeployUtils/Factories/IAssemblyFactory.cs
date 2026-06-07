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
    ///     Construit l'<see cref="IAssemblyContext" /> local à partir du manifeste de steps embarqué
    ///     dans l'<paramref name="assembly" /> (constante générée par XrmFramework.PluginManifest.Generator),
    ///     <b>sans instancier</b> aucun type. Unique source de l'assembly locale (tous TFM).
    /// </summary>
    /// <param name="assembly">The local plugin assembly carrying the manifest.</param>
    /// <returns><see cref="IAssemblyContext" /> rebuilt from the manifest.</returns>
    IAssemblyContext CreateFromManifestAssemblyContext(Assembly assembly);

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