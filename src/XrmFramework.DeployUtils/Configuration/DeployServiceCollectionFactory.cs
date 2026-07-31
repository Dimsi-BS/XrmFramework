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
    ///     Builds a <see cref="DeploySettings" /> from the configuration and deployment
    ///     options, then configures the DI container.
    /// </summary>
    /// <param name="projectName">
    ///     Project name as declared in <c>xrmFramework.config</c>.
    ///     Used to resolve the name of the target CRM solution.
    /// </param>
    /// <param name="deployOptions">
    ///     Deployment options (OnPremise, silent mode, etc.) passed by
    ///     <see cref="RegistrationHelper.RegisterPluginsAndWorkflows{TPlugin}" />.
    /// </param>
    public static IServiceCollection CreateServiceCollection(string projectName, DeployOptions deployOptions)
    {
        // ── Reading the configuration ────────────────────────────────────────
        // Goes through ConfigHelper to honor any project configuration loaded
        // explicitly via ConfigHelper.UseProjectConfig (standalone CLI case).
        var xrmSection       = ConfigHelper.GetSection();
        var connectionString = ConfigHelper.GetSelectedConnectionString();

        var projectConfig = xrmSection.Projects
            .OfType<ProjectElement>()
            .FirstOrDefault(p => p.Name == projectName);

        if (projectConfig == null)
        {
            AnsiConsole.MarkupLine(
                $"[red]Project \"{projectName}\" not found in xrmFramework.config.[/]");
            System.Environment.Exit(1);
        }

        var deploySettings = new DeploySettings
        {
            ConnectionString        = connectionString,
            PluginSolutionUniqueName = projectConfig!.TargetSolution,
            IsOnPremise             = deployOptions.IsOnPremise
        };

        // ── DI container configuration ─────────────────────────────────────
        return new ServiceCollection()
            .InitServiceCollection()
            .AddScoped<IAssemblyExporter, AssemblyExporter>()
            .AddSingleton(deploySettings)
            .AddScoped<IOrganizationService>(sp =>
            {
                var settings = sp.GetRequiredService<DeploySettings>();
#if NET462_OR_GREATER
                // net462: CrmServiceClient supports Online and On-Premises (NTLM/AD/OAuth).
                return new Microsoft.Xrm.Tooling.Connector.CrmServiceClient(settings.ConnectionString);
#else
                // net8+: ServiceClient (Dataverse) supports Online and On-Premises via OAuth.
                return new Microsoft.PowerPlatform.Dataverse.Client.ServiceClient(settings.ConnectionString);
#endif
            });
    }

    /// <summary>
    ///     Registers the base services common to all deployment configurations.
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
