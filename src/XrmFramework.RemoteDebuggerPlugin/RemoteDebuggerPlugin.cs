using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Linq;
using XrmFramework.BindingModel;
using XrmFramework.Definitions;

namespace XrmFramework.RemoteDebugger
{
    /// <summary>
    /// Defines the execution for debugging a Plugin
    /// </summary>
    /// <seealso cref="Microsoft.Xrm.Sdk.IPlugin" />
    public class RemoteDebuggerPlugin : IPlugin
    {
        public RemoteDebuggerPlugin(string unsecuredConfig, string securedConfig)
        {
            UnsecuredConfig = unsecuredConfig;
            SecuredConfig = securedConfig;
            StepConfig = string.IsNullOrEmpty(unsecuredConfig)
            ? new()
            : JsonConvert.DeserializeObject<StepConfiguration>(unsecuredConfig);
        }

        public string UnsecuredConfig { get; }
        public string SecuredConfig { get; }
        public StepConfiguration StepConfig { get; }

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
            localContext.Log($"\r\nIntended Plugin {StepConfig.PluginName}");
            localContext.LogStart();

            if (!GetDebugSession(localContext, out var debugSession))
            {
                LogExit(localContext);
                return;
            }

            localContext.Log($"Debug Session :\n\tDebugeeId : {debugSession.Debugee}\n\tHybridConnectionName  : {debugSession.HybridConnectionName}");

            var remoteContext = InitRemoteContext(localContext, debugSession);

            var uri = new Uri($"{debugSession.RelayUrl}/{debugSession.HybridConnectionName}");

            using var hybridConnection = new HybridConnection(debugSession.SasKeyName, debugSession.SasConnectionKey, uri.AbsoluteUri);
            var message = new RemoteDebuggerMessage(RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id);

            localContext.LogContextEntry();
            try
            {
                RemoteDebuggerMessage response;
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
            }
            catch (Exception e)
            {
                localContext.DumpLog();
                localContext.Log(e.Message);
            }
            LogExit(localContext);
        }

        internal virtual RemoteDebugExecutionContext InitRemoteContext(LocalPluginContext localContext, DebugSession debugSession)
        {
            var remoteContext = localContext.RemoteContext;
            remoteContext.Id = Guid.NewGuid();
            remoteContext.TypeAssemblyQualifiedName = StepConfig.AssemblyQualifiedName;
            remoteContext.UnsecureConfig = UnsecuredConfig;
            remoteContext.SecureConfig = SecuredConfig;
            return remoteContext;
        }

        private void LogExit(LocalPluginContext localContext)
        {
            localContext.LogContextExit();
            localContext.Log($"Exiting {StepConfig.PluginName} Remote Debugging");
            localContext.LogExit();
        }

        internal virtual bool GetDebugSession(LocalPluginContext localContext, out DebugSession debugSession)
        {
            var queryDebugSessions = CreateBaseDebugSessionQuery(localContext);

            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.Id, ConditionOperator.Equal, StepConfig.DebugSessionId);

            debugSession = localContext.AdminOrganizationService.RetrieveAll<DebugSession>(queryDebugSessions).FirstOrDefault();

            return ValidateDebugSession(localContext, debugSession);
        }

        internal static bool ValidateDebugSession(LocalPluginContext localContext, DebugSession debugSession)
        {
            if (debugSession == null)
            {
                localContext.Log("Corresponding DebugSession Not Found");
                return false;
            }

            if (debugSession.SessionEnd <= DateTime.Today)
            {
                localContext.Log("Debug Session expired, please contact your admin");
                return false;
            }

            return true;
        }

        internal static QueryExpression CreateBaseDebugSessionQuery(LocalPluginContext localContext)
        {
            var initiatingUserId = localContext.GetInitiatingUserId().ToString();

            var queryDebugSessions = BindingModelHelper.GetRetrieveAllQuery<DebugSession>();
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.StateCode, ConditionOperator.Equal,
                DebugSessionState.Active.ToInt());
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.Debugee, ConditionOperator.Equal,
                initiatingUserId);
            return queryDebugSessions;
        }
    }
}
