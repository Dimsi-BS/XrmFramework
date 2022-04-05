using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.BindingModel;
using XrmFramework.Definitions;
using XrmFramework.RemoteDebugger;
using XrmFramework.Utils;

namespace XrmFramework.RemoteDebugger
{
    public class RemoteDebuggerPlugin : IPlugin
    {
        public RemoteDebuggerPlugin(string unsecuredConfig, string securedConfig)
        {
            UnsecuredConfig = unsecuredConfig;
            SecuredConfig = securedConfig;
            DebuggerUnsecureConfig = JsonSerializer.Deserialize<DebuggerUnsecureConfig>(unsecuredConfig);
        }

        public string UnsecuredConfig { get; }
        public string SecuredConfig { get; }
        public DebuggerUnsecureConfig DebuggerUnsecureConfig { get; }

        public void Execute(IServiceProvider serviceProvider)
        {
            #region null check and localContext get
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var localContext = new LocalPluginContext(serviceProvider);

            if (localContext.IsDebugContext)
            {
                localContext.Log("This plugin should not be executed in local context");
                return;
            }
            #endregion

            localContext.Log("The context is genuine");
            localContext.Log($"\r\nIntended Plugin {DebuggerUnsecureConfig.PluginName}");
            localContext.LogStart();

            var debugSession = GetDebugSession(localContext, DebuggerUnsecureConfig.DebugSessionId);

            localContext.Log($"Debug Session :\n{debugSession}");

            var remoteContext = localContext.RemoteContext;
            remoteContext.Id = Guid.NewGuid();
            remoteContext.TypeAssemblyQualifiedName = DebuggerUnsecureConfig.AssemblyQualifiedName;
            remoteContext.UnsecureConfig = UnsecuredConfig;
            remoteContext.SecureConfig = SecuredConfig;

            var uri = new Uri($"{debugSession.RelayUrl}/{debugSession.HybridConnectionName}");

            using var hybridConnection = new HybridConnection(debugSession.SasKeyName, debugSession.SasConnectionKey, uri.AbsoluteUri);
            var message = new RemoteDebuggerMessage(RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id);

            RemoteDebuggerMessage response;

            localContext.LogContextEntry();

            while (true)
            {
                localContext.Log("Sending context to local machine : {0}", debugSession.HybridConnectionName);

                response = hybridConnection.SendMessage(message).GetAwaiter().GetResult();

                localContext.Log($"Received response : {response.MessageType}\n");

                if (response.MessageType == RemoteDebuggerMessageType.Context || response.MessageType == RemoteDebuggerMessageType.Exception)
                {
                    break;
                }

                var request = response.GetOrganizationRequest();

                var service = response.UserId.HasValue ? localContext.GetService(response.UserId.Value) : localContext.AdminOrganizationService;

                var organizationResponse = service.Execute(request);

                message = new RemoteDebuggerMessage(RemoteDebuggerMessageType.Response, organizationResponse, remoteContext.Id);
            }

            if (response.MessageType == RemoteDebuggerMessageType.Exception)
            {
                throw response.GetException();
            }

            var updatedContext = response.GetContext<RemoteDebugExecutionContext>();

            localContext.UpdateContext(updatedContext);

            localContext.LogContextExit();
            localContext.Log($"Exiting {DebuggerUnsecureConfig.PluginName} Remote Debugging");
            localContext.LogExit();
        }

        private DebugSession GetDebugSession(LocalPluginContext localContext, Guid sessionId)
        {
            var initiatingUserId = localContext.GetInitiatingUserId();
            var intendedSession = sessionId;

            var queryDebugSessions = BindingModelHelper.GetRetrieveAllQuery<DebugSession>();
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.Id, ConditionOperator.Equal, intendedSession);
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.StateCode, ConditionOperator.Equal, DebugSessionState.Active.ToInt());

            var debugSession = localContext.AdminOrganizationService.RetrieveAll<DebugSession>(queryDebugSessions).FirstOrDefault();

            #region checkers, if wrong does System.Envionment.Exit(1)
            bool goodSession = true;
            if (debugSession == null)
            {
                localContext.Log("debugSession is null");
                goodSession = false;
            }

            if(debugSession.StateCode == DebugSessionState.Inactive)
            {
                localContext.Log("Debug Session is inactive");
                goodSession = false;
            }

            if (initiatingUserId != debugSession.DebugeeId)
            {
                localContext.Log($"Debug Session Id : {debugSession.Id}");
                localContext.Log($"\nThis user was not meant to debug this step on this debug session");
                goodSession = false;
            }
            if (debugSession.SessionEnd <= DateTime.Today)
            {
                localContext.Log("Debug Session expired, please contact your admin");
                goodSession = false;
            }
            if(!goodSession) System.Environment.Exit(1);
            #endregion
            return debugSession;
        }
    }
}
