using System.Configuration;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Spectre.Console;
#if !NET462_OR_GREATER
using XrmFramework.DeployUtils.Configuration.Federated;
#endif
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

        // Tried rather than demanded: which authentication to use is decided below, and workload
        // identity federation needs no connection string at all.
        var connectionString = ConfigHelper.TryGetSelectedConnectionString();

        var projectConfig = xrmSection.Projects
            .OfType<ProjectElement>()
            .FirstOrDefault(p => p.Name == projectName);

        if (projectConfig == null)
        {
            AnsiConsole.MarkupLine(
                $"[red]Project \"{projectName}\" not found in xrmFramework.config.[/]");
            System.Environment.Exit(1);
        }

        var authentication = ResolveAuthentication(deployOptions, connectionString);

        var deploySettings = new DeploySettings
        {
            ConnectionString        = connectionString,
            PluginSolutionUniqueName = projectConfig!.TargetSolution,
            IsOnPremise             = deployOptions.IsOnPremise,
            Deployment              = projectConfig.Deployment,
            Authentication          = authentication
        };

        // ── DI container configuration ─────────────────────────────────────
        var services = new ServiceCollection()
            .InitServiceCollection()
            .AddScoped<IAssemblyExporter, AssemblyExporter>()
            .AddSingleton(deploySettings)
            .AddSingleton(deployOptions);

#if !NET462_OR_GREATER
        if (authentication == AuthenticationMode.WorkloadIdentity)
            return services.AddFederatedOrganizationService(deploySettings, deployOptions, connectionString);
#endif

        RequireConnectionString(connectionString, xrmSection, deployOptions);

        return services
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
    ///     Settles how the deployment will prove who it is.
    /// </summary>
    /// <remarks>
    ///     <see cref="AuthenticationMode.Auto" /> picks federation only when there is no connection
    ///     string, or when the connection string asks for it. A pipeline that already works by
    ///     feeding a <c>ClientSecret</c> connection string therefore keeps working untouched -
    ///     auto-detection is there to make the no-secret case need no switch, not to take an
    ///     existing one away.
    /// </remarks>
    private static AuthenticationMode ResolveAuthentication(DeployOptions deployOptions, string connectionString)
    {
        if (deployOptions.Authentication != AuthenticationMode.Auto)
            return deployOptions.Authentication;

#if !NET462_OR_GREATER
        if (FederatedConnectionSettings.IsFederatedConnectionString(connectionString))
            return AuthenticationMode.WorkloadIdentity;

        if (string.IsNullOrWhiteSpace(connectionString)
            && AzureDevOpsOidcProvider.IsAvailable(System.Environment.GetEnvironmentVariable))
            return AuthenticationMode.WorkloadIdentity;
#endif

        return AuthenticationMode.ConnectionString;
    }

    /// <summary>
    ///     Fails before the container is built when the connection-string path has nothing to
    ///     connect with, naming the file and the entry rather than letting a null reach the client.
    /// </summary>
    /// <remarks>
    ///     A <see cref="ConfigurationErrorsException" /> rather than a plain
    ///     <see cref="InvalidOperationException" />: the deployment reports these without a stack
    ///     trace, which would hide a real bug if the type it caught were a common one.
    /// </remarks>
    private static void RequireConnectionString(
        string connectionString,
        XrmFrameworkSection xrmSection,
        DeployOptions deployOptions)
    {
        // The missing connection comes first: someone who passed --environment without the job
        // exposing an OIDC endpoint fell back here, and needs to hear about the connection rather
        // than about the option they used correctly.
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            var selected = xrmSection?.SelectedConnection;

            throw new ConfigurationErrorsException(
                string.IsNullOrEmpty(selected)
                    ? "No connection selected: set selectedConnection in Config/xrmFramework.config, " +
                      "or deploy with --auth wif."
                    : $"The connection \"{selected}\" selected in Config/xrmFramework.config is not " +
                      "declared in Config/connectionStrings.config. Add it, or deploy with --auth wif.");
        }

        if (!string.IsNullOrWhiteSpace(deployOptions.EnvironmentUrl))
            throw new ConfigurationErrorsException(
                "--environment only applies to workload identity federation. Add --auth wif, or " +
                "change the Url of the selected connection in Config/connectionStrings.config.");
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
