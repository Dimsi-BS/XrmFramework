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
    ///     Construit l'<see cref="IAssemblyContext" /> local en INVENTORIANT l'assembly située à
    ///     <paramref name="dllPath" /> : exécution réelle du code d'enregistrement (constructeurs /
    ///     AddSteps) via XrmFramework.PluginInventory (in-process net462, ou exe net462 hors-process
    ///     depuis net8/net10).
    /// </summary>
    /// <param name="dllPath">Chemin de l'assembly plugin locale à inventorier.</param>
    /// <returns><see cref="IAssemblyContext" /> reconstruit depuis l'inventaire.</returns>
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