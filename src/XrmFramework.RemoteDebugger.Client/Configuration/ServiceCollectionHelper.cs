using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;
using XrmFramework.RemoteDebugger.Client.Utils;

namespace XrmFramework.RemoteDebugger.Client.Configuration
{
    internal class ServiceCollectionHelper
    {
        public static IServiceProvider ConfigureServiceProvider(string projectName)
        {
            if (ConfigurationManager.ConnectionStrings["DebugConnectionString"] == null)
            {
                throw new Exception("The connectionString \"DebugConnectionString\" is not defined.");
            }

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped<IRegistrationService, RegistrationService>();
            serviceCollection.AddScoped<ISolutionContext, SolutionContext>();
            serviceCollection.AddScoped<IAssemblyExporter, AssemblyExporter>();
            serviceCollection.AddScoped<IAssemblyImporter, AssemblyImporter>();
            serviceCollection.AddSingleton<IAssemblyFactory, AssemblyFactory>();
            serviceCollection.AddSingleton<IRemoteDebuggerAssemblyHandler, RemoteDebuggerAssemblyHandler>();
            serviceCollection.AddSingleton<RemoteDebuggerPluginHandler>();


            ChooseSolutionSettings(projectName, out string pluginSolutionUniqueName, out string connectionString);

            serviceCollection.Configure<SolutionSettings>((settings) =>
            {
                settings.ConnectionString = connectionString;
                settings.PluginSolutionUniqueName = pluginSolutionUniqueName;
            });

            var debugUserUri = ConfigurationManager.ConnectionStrings["DebugConnectionString"].ConnectionString;

            serviceCollection.Configure<DebugSessionSettings>((settings) =>
            {
                settings.ParseAndSetUri(debugUserUri);
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }

        public static void ChooseSolutionSettings(string projectName, out string pluginSolutionUniqueName, out string connectionString)
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

            var connectionStringNameDic = new Dictionary<string, ConnectionStringSettings>();
            var connectionStringIntDic = new Dictionary<int, ConnectionStringSettings>();

            var connectionStrings = (ConnectionStringsSection)ConfigurationManager.GetSection("connectionStrings");

            Console.WriteLine("Which connection would you like to use ? Type Number or Name");

            int i = 0;
            foreach (ConnectionStringSettings connection in connectionStrings.ConnectionStrings)
            {
                connectionStringIntDic.Add(i, connection);
                connectionStringNameDic.Add(connection.Name, connection);
                Console.WriteLine($"\t{i} : {connection.Name}");
                i++;
            }

            Console.WriteLine();
            var response = Console.ReadLine();

            if (int.TryParse(response, out int value))
            {
                connectionString = connectionStringIntDic[value].ConnectionString;
            }
            else
            {
                connectionString = connectionStringNameDic[response].ConnectionString;
            }
        }
    }
}
