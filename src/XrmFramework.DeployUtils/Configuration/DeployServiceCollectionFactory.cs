using System.Configuration;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Spectre.Console;
using XrmFramework.DeployUtils.Comparers;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Converters;
using XrmFramework.DeployUtils.Exporters;
using XrmFramework.DeployUtils.Factories;
using XrmFramework.DeployUtils.Importers;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Configuration;

/// <summary>
///     Configures the necessary services and parameters of the project
/// </summary>
internal static class DeployServiceCollectionFactory
{
    /// <summary>
    ///     Construit un <see cref="DeploySettings" /> à partir de la configuration et des options
    ///     de déploiement, puis configure le conteneur DI.
    /// </summary>
    /// <param name="projectName">
    ///     Nom du projet tel que déclaré dans <c>xrmFramework.config</c>.
    ///     Utilisé pour résoudre le nom de la solution CRM cible.
    /// </param>
    /// <param name="deployOptions">
    ///     Options de déploiement (OnPremise, mode silencieux, etc.) transmises par
    ///     <see cref="RegistrationHelper.RegisterPluginsAndWorkflows{TPlugin}" />.
    /// </param>
    public static IServiceCollection CreateServiceCollection(string projectName, DeployOptions deployOptions)
    {
        // ── Lecture de la configuration ────────────────────────────────────────
        var xrmSection     = ConfigHelper.GetSection();
        var connectionString = ConfigurationManager
            .ConnectionStrings[xrmSection.SelectedConnection]
            .ConnectionString;

        var projectConfig = xrmSection.Projects
            .OfType<ProjectElement>()
            .FirstOrDefault(p => p.Name == projectName);

        if (projectConfig == null)
        {
            AnsiConsole.MarkupLine(
                $"[red]Le projet «{projectName}» est introuvable dans xrmFramework.config.[/]");
            System.Environment.Exit(1);
        }

        var deploySettings = new DeploySettings
        {
            ConnectionString        = connectionString,
            PluginSolutionUniqueName = projectConfig!.TargetSolution,
            IsOnPremise             = deployOptions.IsOnPremise
        };

        // ── Configuration du conteneur DI ─────────────────────────────────────
        return new ServiceCollection()
            .InitServiceCollection()
            .AddScoped<IAssemblyExporter, AssemblyExporter>()
            .AddSingleton(deploySettings)
            .AddScoped<IOrganizationService>(sp =>
            {
                var settings = sp.GetRequiredService<DeploySettings>();
#if NET462_OR_GREATER
                // net462 : CrmServiceClient supporte Online et On-Premises (NTLM/AD/OAuth).
                return new Microsoft.Xrm.Tooling.Connector.CrmServiceClient(settings.ConnectionString);
#else
                // net8+ : ServiceClient (Dataverse) supporte Online et On-Premises via OAuth.
                return new Microsoft.PowerPlatform.Dataverse.Client.ServiceClient(settings.ConnectionString);
#endif
            });
    }

    /// <summary>
    ///     Enregistre les services de base communs à toutes les configurations de déploiement.
    /// </summary>
    public static IServiceCollection InitServiceCollection(this IServiceCollection serviceCollection)
      => serviceCollection
        .AddScoped<IRegistrationService, RegistrationService>()
        .AddScoped<ISolutionContext, SolutionContext>()
        .AddScoped<IAssemblyImporter, AssemblyImporter>()
        .AddScoped<ICrmComponentComparer, CrmComponentComparer>()
        .AddScoped<ICrmComponentConverter, CrmComponentConverter>()
        .AddScoped<IConsoleService, ConsoleService>()
        .AddScoped<AssemblyDiffFactory>()
        .AddSingleton<IAssemblyFactory, AssemblyFactory>()
        .AddSingleton<RegistrationHelper>()
        .AddSingleton<ICrmMapper, CrmMapper>();
}
