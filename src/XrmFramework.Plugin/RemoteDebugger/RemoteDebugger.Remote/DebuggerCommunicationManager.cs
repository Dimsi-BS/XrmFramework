using Microsoft.Xrm.Sdk.Query;
using System;
using XrmFramework.BindingModel;
using XrmFramework.Definitions;
using XrmFramework.RemoteDebugger;

namespace XrmFramework.Remote
{
    internal abstract class DebuggerCommunicationManager : IDebuggerCommunicationManager
    {
        protected LocalPluginContext Context { get; }

        protected DebuggerCommunicationManager(LocalPluginContext context)
        {
            Context = context;
        }

        public abstract DebugSession GetDebugSession();

        protected abstract RemoteDebugExecutionContext InitRemoteContext();

        public void SendLocalContextToDebugSession(DebugSession debugSession)
        {
            using var hybridConnection = InitConnection(debugSession);

            var remoteContext = InitRemoteContext();

            Context.Log("Sending context to local machine : {0}", debugSession.HybridConnectionName);
            try
            {
                var response = ExchangeWithRemoteDebugger(hybridConnection, remoteContext);
                if (response.MessageType == RemoteDebuggerMessageType.Exception)
                {
                    throw response.GetException();
                }
                var updatedContext = response.GetContext<RemoteDebugExecutionContext>();
                Context.UpdateContext(updatedContext);
            }
            catch (Exception e)
            {
                Context.DumpLog();
                Context.Log(e.Message);
            }
        }




        private RemoteDebuggerMessage ExchangeWithRemoteDebugger(HybridConnection hybridConnection, RemoteDebugExecutionContext remoteContext)
        {
            var message = new RemoteDebuggerMessage(RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id);
            RemoteDebuggerMessage response;
            Context.LogContextEntry();

            while (true)
            {
                response = hybridConnection.SendMessage(message).GetAwaiter().GetResult();

                Context.Log($"Received response : {response.MessageType}\n");

                if (response.MessageType is RemoteDebuggerMessageType.Context or RemoteDebuggerMessageType.Exception)
                {
                    break;
                }

                var request = response.GetOrganizationRequest();

                var service = response.UserId.HasValue ? Context.GetService(response.UserId.Value) : Context.AdminOrganizationService;

                Context.Log("Executing local machine request");
                var organizationResponse = service.Execute(request);

                message = new RemoteDebuggerMessage(RemoteDebuggerMessageType.Response, organizationResponse, remoteContext.Id);
                Context.Log("Transferring response to local machine");
            }

            return response;
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

        private static HybridConnection InitConnection(DebugSession debugSession)
        {
            var uri = new Uri($"{debugSession.RelayUrl}/{debugSession.HybridConnectionName}");

            return new HybridConnection(debugSession.SasKeyName, debugSession.SasConnectionKey, uri.AbsoluteUri);
        }

        DebugSession IDebuggerCommunicationManager.GetDebugSession()
        {
            return GetDebugSession();
        }

        void IDebuggerCommunicationManager.SendLocalContextToDebugSession(DebugSession debugSession)
        {
            SendLocalContextToDebugSession(debugSession);
        }
    }
}
