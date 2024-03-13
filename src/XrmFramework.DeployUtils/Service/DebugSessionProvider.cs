
using Microsoft.Xrm.Sdk.Query;
using Spectre.Console;
using System.Configuration;
using XrmFramework.BindingModel;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.RemoteDebugger;

namespace XrmFramework.DeployUtils.Service
{
    public class DebugSessionProvider : IDebugSessionProvider
    {
        private readonly IDeploySettingsProvider _deploySettingsProvider;

        private readonly Lazy<DebugSession> _debugSession;

        public DebugSessionProvider(IDeploySettingsProvider deploySettingsProvider)
        {
            _deploySettingsProvider = deploySettingsProvider;

            _debugSession = new Lazy<DebugSession>(() =>
            {
                var deploySettings = _deploySettingsProvider.GetSelectedDeploySettings();

                return AnsiConsole.Status()
                .Start("Connecting to CRM", ctx => {

                    var client = new RegistrationService(deploySettings.ConnectionString);
                    var debugSessionString = ConfigurationManager.ConnectionStrings["DebugConnectionString"].ConnectionString;

                    var keyName = ConnectionStringParser.GetConnectionStringField(debugSessionString, "SharedAccessKeyName");
                    var entityPath = ConnectionStringParser.GetConnectionStringField(debugSessionString, "EntityPath");

                    var queryDebugSessions = new QueryExpression(DebugSessionDefinition.EntityName);
                    queryDebugSessions.ColumnSet.AllColumns = true;
                    queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.SasKeyName, ConditionOperator.Equal, keyName);
                    queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.HybridConnectionName, ConditionOperator.Equal, entityPath);

                    var debugSession = client.RetrieveAll<DebugSession>(queryDebugSessions).FirstOrDefault();

                    if (debugSession == null) throw new ArgumentException("Debug Session not Found on the Crm");

                    return debugSession;
                });
            });
        }

        public DebugSession DebugSession => _debugSession.Value;
    }
}
