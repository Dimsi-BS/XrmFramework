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
    internal partial class ServiceCollectionHelper
    {
        public static IServiceProvider ConfigureForRemoteDebug(string projectName)
        {
            if (ConfigurationManager.ConnectionStrings["DebugConnectionString"] == null)
            {
                throw new Exception("The connectionString \"DebugConnectionString\" is not defined.");
            }

            var serviceCollection = InitServiceCollection();

            ChooseSolutionSettings(projectName, out string pluginSolutionUniqueName, out string connectionString);

            var debugSessionId = GetDebugSessionId(connectionString);

            serviceCollection.Configure<SolutionSettings>((settings) =>
            {
                settings.ConnectionString = connectionString;
                settings.PluginSolutionUniqueName = pluginSolutionUniqueName;
            });


            serviceCollection.Configure<DebugSessionSettings>((settings) =>
            {
                settings.DebugSessionId = debugSessionId;
            });

            return serviceCollection.BuildServiceProvider();
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

            Console.WriteLine("Which connection would you like to use ? Type Number or Name (leave blank to cancel)");

            int i = 0;
            foreach (ConnectionStringSettings connection in connectionStrings.ConnectionStrings)
            {
                if (connection.Name.StartsWith("Xrm"))
                {
                    connectionStringIntDic.Add(i, connection);
                    connectionStringNameDic.Add(connection.Name, connection);
                    Console.WriteLine($"\t{i} : {connection.Name}");
                    i++;
                }
            }

            Console.WriteLine();
            var response = Console.ReadLine();

            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("Debug Canceled");
                System.Environment.Exit(0);
            }

            if (int.TryParse(response, out int value))
            {
                connectionString = connectionStringIntDic[value].ConnectionString;
            }
            else
            {
                connectionString = connectionStringNameDic[response].ConnectionString;
            }
        }

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

                if (key.Equals("SharedAccessKeyName")) SasKeyName = value;
                else if (key.Equals("EntityPath")) entityPath = value;
            }
        }
    }
}
