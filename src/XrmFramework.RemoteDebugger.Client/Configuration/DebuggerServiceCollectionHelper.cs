using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using XrmFramework.BindingModel;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Exporters;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;
using XrmFramework.RemoteDebugger.Client.Recorder;

namespace XrmFramework.RemoteDebugger.Client.Configuration
{
    /// <summary>
    /// Configures the necessary services and parameters of the project
    /// </summary>
    public static class DebuggerServiceCollectionHelper
    {
        /// <summary>
        /// Configures the required objects used during RemoteDebug, such as :
        /// <list type="bullet">
        ///     <item><see cref="IRegistrationService"/>, the service used for communicating with the CRM</item>
        ///     <item><see cref="AutoMapper.IMapper"/>, used for conversion between <see cref="Deploy"/> and <see cref="Model"/> objects
        ///         as well as cloning</item>
        ///     <item><see cref="DeploySettings"/>, an object that contains information on the target <c>Solution</c></item>
        ///     <item>The configuration of all other implemented interfaces used by <c>Dependency Injection</c></item>
        /// </list>
        /// </summary>
        /// <returns><see cref="IServiceProvider"/> the service provider used to instantiate every object needed</returns>
        public static IServiceProvider ConfigureForRemoteDebug<T>() where T: class, IRemoteDebuggerMessageManager
        {
            if (ConfigurationManager.ConnectionStrings["DebugConnectionString"] == null)
            {
                throw new Exception("The connectionString \"DebugConnectionString\" is not defined.");
            }

            var serviceCollection = new ServiceCollection()
                .InitServiceCollection()
                .AddScoped<IAssemblyExporter, DebuggerAssemblyExporter>()
                .AddScoped<ISessionRecorder, SessionRecorder>()
                .AddScoped<IRemoteDebuggerMessageManager, T>();

            var connectionString = ChooseConnectionString();

            var debugSession = GetDebugSession(connectionString);

            serviceCollection.Configure<DeploySettings>((settings) =>
            {
                settings.ConnectionString = connectionString;
            });

            serviceCollection.AddScoped<IOrganizationService, CrmServiceClient>(_ => new CrmServiceClient(connectionString));

            serviceCollection.Configure<DebugSession>(ds =>
            {
                debugSession.CopyTo(ds);
            });

            return serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Allows for the user to choose the <c>Target Environment</c> on console
        /// </summary>
        /// <returns>The chosen Connection String</returns>
        private static string ChooseConnectionString()
        {
            var connectionStringNameDic = new Dictionary<string, ConnectionStringSettings>();
            var connectionStringIntDic = new Dictionary<int, ConnectionStringSettings>();

            var connectionStrings = (ConnectionStringsSection)ConfigurationManager.GetSection("connectionStrings");

            Console.WriteLine(@"Which connection would you like to use ? Type Number or Name (leave blank to cancel)");

            var i = 0;
            foreach (ConnectionStringSettings connection in connectionStrings.ConnectionStrings)
            {
                if (!connection.Name.StartsWith("Xrm"))
                {
                    continue;
                }

                connectionStringIntDic.Add(i, connection);
                connectionStringNameDic.Add(connection.Name, connection);
                Console.WriteLine(@$"    {i} : {connection.Name}");
                i++;
            }

            Console.WriteLine();
            var response = Console.ReadLine();

            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine(@"Debug Canceled");
                Environment.Exit(0);
            }

            var connectionString = int.TryParse(response, out int value)
                ? connectionStringIntDic[value].ConnectionString
                : connectionStringNameDic[response].ConnectionString;

            return connectionString;
        }

        /// <summary>
        /// Retrieves the user's debug Session with a given <paramref name="connectionString"/>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>The Id of the <c>Debug Session</c></returns>
        /// <exception cref="ArgumentException"> if the Debug Session doesn't exist on the CRM</exception>
        private static DebugSession GetDebugSession(string connectionString)
        {
            var client = new RegistrationService(connectionString);
            var debugSessionString = ConfigurationManager.ConnectionStrings["DebugConnectionString"].ConnectionString;

            var keyName = ConnectionStringParser.GetConnectionStringField(debugSessionString, "SharedAccessKeyName");
            var entityPath = ConnectionStringParser.GetConnectionStringField(debugSessionString, "EntityPath");

            var queryDebugSessions = new QueryExpression(DebugSessionDefinition.EntityName);
            queryDebugSessions.ColumnSet.AllColumns = true;
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.SasKeyName, ConditionOperator.Equal, keyName);
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.HybridConnectionName, ConditionOperator.Equal, entityPath);

            var debugSession = client.RetrieveAll<DebugSession>(queryDebugSessions).FirstOrDefault();

            if (debugSession == null)
            {
                debugSession = new DebugSession
                {
                    
                };
            }

            return debugSession;
        }
    }
}
