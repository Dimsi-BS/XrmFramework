using Microsoft.Xrm.Sdk.Query;
using System;
using XrmFramework.BindingModel;
using XrmFramework.Definitions;
using XrmFramework.RemoteDebugger;

namespace XrmFramework.Remote
{
    internal abstract class DebuggerCommunicationManager : IDebuggerCommunicationManager
    {
        public abstract DebugSession GetDebugSession(LocalPluginContext localContext);

        protected abstract RemoteDebugExecutionContext InitRemoteContext(LocalPluginContext localContext);

        public void SendLocalContextToDebugSession(DebugSession debugSession, LocalPluginContext localContext)
        {
            using var hybridConnection = InitConnection(debugSession);

            var remoteContext = InitRemoteContext(localContext);

            localContext.Log("Sending context to local machine : {0}", debugSession.HybridConnectionName);
            try
            {
                var response = SendToRemoteDebugger(hybridConnection, localContext, remoteContext);
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
        }


        protected static QueryExpression CreateBaseDebugSessionQuery(string initiatingUserId)
        {
            var queryDebugSessions = BindingModelHelper.GetRetrieveAllQuery<DebugSession>();
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.StateCode, ConditionOperator.Equal,
                DebugSessionState.Active.ToInt());
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.Debugee, ConditionOperator.Equal,
                initiatingUserId);
            return queryDebugSessions;
        }

        private RemoteDebuggerMessage SendToRemoteDebugger(HybridConnection hybridConnection, LocalPluginContext localContext, RemoteDebugExecutionContext remoteContext)
        {
            var message = new RemoteDebuggerMessage(RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id);
            RemoteDebuggerMessage response;
            localContext.LogContextEntry();

            while (true)
            {
                response = hybridConnection.SendMessage(message).GetAwaiter().GetResult();

                localContext.Log($"Received response : {response.MessageType}\n");

                if (response.MessageType is RemoteDebuggerMessageType.Context or RemoteDebuggerMessageType.Exception)
                {
                    break;
                }

                var request = response.GetOrganizationRequest();

                var service = response.UserId.HasValue ? localContext.GetService(response.UserId.Value) : localContext.AdminOrganizationService;

                localContext.Log("Executing local machine request");
                var organizationResponse = service.Execute(request);

                message = new RemoteDebuggerMessage(RemoteDebuggerMessageType.Response, organizationResponse, remoteContext.Id);
                localContext.Log("Transferring response to local machine");
            }

            return response;
        }

        private static HybridConnection InitConnection(DebugSession debugSession)
        {
            var uri = new Uri($"{debugSession.RelayUrl}/{debugSession.HybridConnectionName}");

            return new HybridConnection(debugSession.SasKeyName, debugSession.SasConnectionKey, uri.AbsoluteUri);
        }

        DebugSession IDebuggerCommunicationManager.GetDebugSession(LocalPluginContext localContext)
        {
            return GetDebugSession(localContext);
        }

        void IDebuggerCommunicationManager.SendLocalContextToDebugSession(DebugSession debugSession,
            LocalPluginContext localContext)
        {
            SendLocalContextToDebugSession(debugSession, localContext);
        }
    }
}
