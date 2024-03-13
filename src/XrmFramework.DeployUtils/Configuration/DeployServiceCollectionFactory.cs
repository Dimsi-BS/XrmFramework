using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
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
    ///         <item>
    ///             <see cref="AutoMapper.IMapper" />, used for conversion between <see cref="Deploy" /> and
    ///             <see cref="Model" /> objects
    ///             as well as cloning
    ///         </item>
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

                return new CrmServiceClient(deploySettings.ConnectionString);
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
        .AddScoped<AssemblyDiffFactory>()
        .AddSingleton<IAssemblyFactory, AssemblyFactory>()
        .AddSingleton<RegistrationHelper>()

        // Searches every AutoMapper Profiles declared in this Assembly and configures a mapper according to them
        .AddAutoMapper(Assembly.GetExecutingAssembly());

}