using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Service;
using XrmFramework.RemoteDebugger.Client.Configuration;

namespace XrmFramework.DeployUtils.Configuration
{
    /// <summary>
    /// Configures the necessary services and parameters of the project
    /// </summary>
    internal partial class ServiceCollectionHelper
    {
        /// <summary>
        /// Configures the required objects used during RemoteDebug, such as :
        /// <list type="bullet">
        ///     <item><see cref="IRegistrationService"/>, the service used for communicating with the CRM</item>
        ///     <item><see cref="AutoMapper.IMapper"/>, used for conversion between <see cref="Deploy"/> and <see cref="Model"/> objects
        ///         as well as cloning</item>
        ///     <item><see cref="SolutionSettings"/>, an object that contains information on the target <c>Solution</c></item>
        ///     <item><see cref="DebugSessionSettings"/>, an object that contains information on the target <c>Debug Session</c></item>
        ///     <item>The configuration of all other implemented interfaces used by <c>Dependency Injection</c></item>
        /// </list>
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns><see cref="IServiceProvider"/> the service provider used to instantiate every object needed</returns>

        public static IServiceProvider ConfigureForRemoteDebug(string projectName)
        {
            if (ConfigurationManager.ConnectionStrings["DebugConnectionString"] == null)
            {
                throw new Exception("The connectionString \"DebugConnectionString\" is not defined.");
            }

            var serviceCollection = InitServiceCollection();

            var solutionSettings = ChooseSolutionSettings(projectName);

            var debugSessionId = GetDebugSessionId(solutionSettings.ConnectionString);

            serviceCollection.Configure<SolutionSettings>((settings) =>
            {
                settings.ConnectionString = solutionSettings.ConnectionString;
                settings.PluginSolutionUniqueName = solutionSettings.PluginSolutionUniqueName;
            });


            serviceCollection.Configure<DebugSessionSettings>((settings) =>
            {
                settings.DebugSessionId = debugSessionId;
            });

            return serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Allows for the user to choose the <c>Target Environment</c> on console
        /// </summary>
        /// <param name="projectName">Name of the target solution</param>
        /// <returns>The Connection String and Target Plugin Name wrapped in a <see cref="SolutionSettings"/></returns>
        private static SolutionSettings ChooseSolutionSettings(string projectName)
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


            var connectionStringNameDic = new Dictionary<string, ConnectionStringSettings>();
            var connectionStringIntDic = new Dictionary<int, ConnectionStringSettings>();

            var connectionStrings = (ConnectionStringsSection)ConfigurationManager.GetSection("connectionStrings");

            Console.WriteLine("Which connection would you like to use ? Type Number or Name (leave blank to cancel)");

            int i = 0;
            foreach (ConnectionStringSettings connection in connectionStrings.ConnectionStrings)
            {
                if (!connection.Name.StartsWith("Xrm")) continue;
                connectionStringIntDic.Add(i, connection);
                connectionStringNameDic.Add(connection.Name, connection);
                Console.WriteLine($"\t{i} : {connection.Name}");
                i++;
            }

            Console.WriteLine();
            var response = Console.ReadLine();

            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("Debug Canceled");
                System.Environment.Exit(0);
            }

            var pluginSolutionUniqueName = projectConfig.TargetSolution;
            var connectionString = int.TryParse(response, out int value)
                ? connectionStringIntDic[value].ConnectionString
                : connectionStringNameDic[response].ConnectionString;

            return new SolutionSettings()
            {
                PluginSolutionUniqueName = pluginSolutionUniqueName,
                ConnectionString = connectionString,
            };
        }

        /// <summary>
        /// Retrieves the user's debug Session with a given <paramref name="connectionString"/>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>The Id of the <c>Debug Session</c></returns>
        /// <exception cref="ArgumentException"></exception>
        private static Guid GetDebugSessionId(string connectionString)
        {
            var client = new RegistrationService(connectionString);
            var debugSessionString = ConfigurationManager.ConnectionStrings["DebugConnectionString"].ConnectionString;

            ParseDebugConnectionString(debugSessionString, out string key, out string path);

            var queryDebugSessions = new QueryExpression(DebugSessionDefinition.EntityName);
            queryDebugSessions.ColumnSet.AllColumns = true;
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.SasKeyName, ConditionOperator.Equal, key);
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.HybridConnectionName, ConditionOperator.Equal, path);

            var debugSession = client.RetrieveAll(queryDebugSessions).FirstOrDefault();

            if (debugSession == null) throw new ArgumentException("Debug Session not Found on the Crm");

            return debugSession.Id;
        }

        private static void ParseDebugConnectionString(string raw, out string SasKeyName, out string entityPath)
        {
            var columns = raw.Split(';');
            SasKeyName = "";
            entityPath = "";
            foreach (var column in columns)
            {
                var key = column.Split('=')[0].Trim();
                if (string.IsNullOrEmpty(key)) continue;
                var value = column.Split('=')[1].Trim();

                switch (key)
                {
                    case "SharedAccessKeyName":
                        SasKeyName = value;
                        break;
                    case "EntityPath":
                        entityPath = value;
                        break;
                }
            }
        }
    }
}
