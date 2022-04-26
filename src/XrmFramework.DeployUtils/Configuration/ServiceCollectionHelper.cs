using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.DeployUtils.Configuration
{
    internal partial class ServiceCollectionHelper
    {
        /// <summary>
        /// Configure the required objects used during Deploy, such as :
        /// <list type="bullet">
        ///     <item><cref>IService</cref>, the service used for communicating with the CRM</item>
        ///     <item><cref>IServiceProvider</cref></item>
        /// </list>
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static IServiceProvider ConfigureForDeploy(string projectName)
        {
            var serviceCollection = InitServiceCollection();

            ParseSolutionSettings(projectName, out string pluginSolutionUniqueName, out string connectionString);

            serviceCollection.Configure<SolutionSettings>((settings) =>
            {
                settings.ConnectionString = connectionString;
                settings.PluginSolutionUniqueName = pluginSolutionUniqueName;
            });

            return serviceCollection.BuildServiceProvider();
        }

        private static ServiceCollection InitServiceCollection()
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

            serviceCollection.AddAutoMapper(Assembly.GetExecutingAssembly());
            return serviceCollection;
        }

        private static void ParseSolutionSettings(string projectName, out string pluginSolutionUniqueName, out string connectionString)
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

            pluginSolutionUniqueName = projectConfig.TargetSolution;

            connectionString = ConfigurationManager.ConnectionStrings[xrmFrameworkConfigSection.SelectedConnection].ConnectionString;
        }

    }
}
