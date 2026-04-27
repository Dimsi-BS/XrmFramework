using Microsoft.Extensions.DependencyInjection;


#if NET462_OR_GREATER

#else
using Microsoft.PowerPlatform.Dataverse.Client;
#endif
using Microsoft.Xrm.Sdk;
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
    ///     Configures the required objects used during Deploy, such as :
    ///     <list type="bullet">
    ///         <item><see cref="IRegistrationService" />, the service used for communicating with the CRM</item>
    ///         <item><see cref="ICrmMapper" />, used for conversion between <see cref="Deploy" /> and
    ///             <see cref="Model" /> objects as well as cloning</item>
    ///         <item><see cref="DeploySettings" />, an object that contains information on the target <c>Solution</c></item>
    ///         <item>The configuration of all other implemented interfaces</item>
    ///     </list>
    /// </summary>
    /// <param name="projectName">Name of the target solution</param>
    /// <returns><see cref="IServiceProvider" /> the service provider used to instantiate every object needed</returns>
    public static IServiceCollection CreateServiceCollection(string projectName)
    {
        var serviceCollection =
            new ServiceCollection()
            .InitServiceCollection()
            .AddScoped<IAssemblyExporter, AssemblyExporter>()
            .AddSingleton<IDeploySettingsProvider, DeploySettingsProvider>()
            .AddSingleton<ITargetSolutionProvider>(_ => new TargetSolutionProvider(projectName))
            .AddScoped<IOrganizationService>(sp =>
            {
                var deploySettingsProvider = sp.GetRequiredService<IDeploySettingsProvider>();
                var deploySettings = deploySettingsProvider.GetSelectedDeploySettings();

#if NET462_OR_GREATER
                return new Microsoft.Xrm.Tooling.Connector.CrmServiceClient(deploySettings.ConnectionString);
#else
                return new ServiceClient(deploySettings.ConnectionString);
#endif
            });

        return serviceCollection;
    }

    /// <summary>
    ///     Configures the base <see cref="IServiceCollection" /> required for deploy,
    ///     for more functionalities you can add them in the returned <see cref="IServiceCollection" />
    /// </summary>
    /// <returns>
    ///     <see cref="IServiceCollection" />
    /// </returns>
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
