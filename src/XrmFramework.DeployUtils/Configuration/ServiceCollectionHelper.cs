using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.DeployUtils.Configuration
{
    /// <summary>
    /// Configures the necessary services and parameters of the project
    /// </summary>
    internal partial class ServiceCollectionHelper
    {
        /// <summary>
        /// Configures the required objects used during Deploy, such as :
        /// <list type="bullet">
        ///     <item><see cref="IRegistrationService"/>, the service used for communicating with the CRM</item>
        ///     <item><see cref="AutoMapper.IMapper"/>, used for conversion between <see cref="Deploy"/> and <see cref="Model"/> objects
        ///         as well as cloning</item>
        ///     <item><see cref="DeploySettings"/>, an object that contains information on the target <c>Solution</c></item>
        ///     <item>The configuration of all other implemented interfaces</item>
        /// </list>
        /// </summary>
        /// <param name="projectName">Name of the target solution</param>
        /// <returns><see cref="IServiceProvider"/> the service provider used to instantiate every object needed</returns>
        public static IServiceProvider ConfigureForDeploy(string projectName)
        {
            var serviceCollection = InitServiceCollection();

            var settings = ParseSolutionSettings(projectName);

            serviceCollection.Configure<DeploySettings>((s) =>
            {
                s.ConnectionString = settings.ConnectionString;
                s.PluginSolutionUniqueName = settings.PluginSolutionUniqueName;
            });

            serviceCollection.AddScoped<IOrganizationService, CrmServiceClient>(_ => new CrmServiceClient(settings.ConnectionString));

            return serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Configures the base <see cref="IServiceCollection"/> required for deploy,
        /// for more functionalities you can add them in the returned <see cref="IServiceCollection"/>
        /// </summary>
        /// <returns><see cref="IServiceCollection"/></returns>
        private static IServiceCollection InitServiceCollection()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped<IRegistrationService, RegistrationService>();
            serviceCollection.AddScoped<ISolutionContext, SolutionContext>();
            serviceCollection.AddScoped<IAssemblyExporter, AssemblyExporter>();
            serviceCollection.AddScoped<IAssemblyImporter, AssemblyImporter>();
            serviceCollection.AddScoped<ICrmComponentComparer, CrmComponentComparer>();
            serviceCollection.AddScoped<ICrmComponentConverter, CrmComponentConverter>();
            serviceCollection.AddScoped<AssemblyDiffFactory>();
            serviceCollection.AddSingleton<IAssemblyFactory, AssemblyFactory>();
            serviceCollection.AddSingleton<RegistrationHelper>();

            // Searches every AutoMapper Profiles declared in this Assembly and configures a mapper according to them
            serviceCollection.AddAutoMapper(Assembly.GetExecutingAssembly());
            return serviceCollection;
        }

        /// <summary>
        /// Retrieves the config files to get the solution settings
        /// </summary>
        /// <param name="projectName">, The name of the target solution</param>
        /// <returns><see cref="DeploySettings"/> that stores the selected ConnectionString and the target solution name</returns>
        private static DeploySettings ParseSolutionSettings(string projectName)
        {
            var xrmFrameworkConfigSection = ConfigHelper.GetSection();

            var projectConfig = xrmFrameworkConfigSection.Projects.OfType<ProjectElement>()
                .FirstOrDefault(p => p.Name == projectName);

            if (projectConfig == null)
            {
                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"No reference to the project {projectName} has been found in the xrmFramework.config file.");
                Console.ForegroundColor = defaultColor;
                System.Environment.Exit(1);
            }

            return new DeploySettings()
            {
                PluginSolutionUniqueName = projectConfig.TargetSolution,
                ConnectionString = ConfigurationManager.ConnectionStrings[xrmFrameworkConfigSection.SelectedConnection].ConnectionString
            };
        }

    }
}
