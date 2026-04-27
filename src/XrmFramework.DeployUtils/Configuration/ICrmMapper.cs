using Deploy;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using CustomApi = XrmFramework.DeployUtils.Model.CustomApi;
using CustomApiRequestParameter = XrmFramework.DeployUtils.Model.CustomApiRequestParameter;
using CustomApiResponseProperty = XrmFramework.DeployUtils.Model.CustomApiResponseProperty;
using PluginPackage = XrmFramework.DeployUtils.Model.PluginPackage;

namespace XrmFramework.DeployUtils.Configuration;

/// <summary>
///     Replaces AutoMapper for all CRM component mapping operations.
///     Handles local-to-local (deep clone), remote-to-local, and local-to-remote conversions.
/// </summary>
public interface ICrmMapper
{
    // ── Local-to-Local (deep clones) ──────────────────────────────────────────

    AssemblyInfo Clone(AssemblyInfo source);
    PluginPackage Clone(PluginPackage source);
    Plugin Clone(Plugin source);
    Step Clone(Step source);
    StepImage Clone(StepImage source);
    CustomApi Clone(CustomApi source);
    CustomApiRequestParameter Clone(CustomApiRequestParameter source);
    CustomApiResponseProperty Clone(CustomApiResponseProperty source);

    /// <summary>Deep-clones an entire <see cref="IAssemblyContext" /> tree.</summary>
    IAssemblyContext Clone(IAssemblyContext source);

    // ── Remote-to-Local ───────────────────────────────────────────────────────

    AssemblyInfo FromRemote(PluginAssembly source);
    PluginPackage FromRemote(Deploy.PluginPackage source);
    CustomApi FromRemote(Deploy.CustomApi source);
    CustomApiRequestParameter FromRemote(Deploy.CustomApiRequestParameter source);
    CustomApiResponseProperty FromRemote(Deploy.CustomApiResponseProperty source);

    // ── Local-to-Remote ───────────────────────────────────────────────────────

    PluginAssembly ToRemote(AssemblyInfo source);
    Deploy.PluginPackage ToRemote(PluginPackage source);
    Deploy.CustomApi ToRemote(CustomApi source);
    Deploy.CustomApiRequestParameter ToRemote(CustomApiRequestParameter source);
    Deploy.CustomApiResponseProperty ToRemote(CustomApiResponseProperty source);
}
