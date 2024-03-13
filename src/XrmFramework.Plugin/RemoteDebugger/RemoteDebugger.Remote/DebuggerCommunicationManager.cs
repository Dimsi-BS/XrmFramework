using System;
using Microsoft.Xrm.Sdk.Query;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using XrmFramework.BindingModel;
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

        protected abstract RemoteDebugExecutionContext InitRemoteContext();

        public void SendLocalContextToDebugSession(DebugSession debugSession)
        {
            using var hybridConnection = InitConnection(debugSession);

            var remoteContext = InitRemoteContext();

            Context.Log("Sending context to local machine : {0}", debugSession.HybridConnectionName);
            RemoteDebuggerMessage? response;
            try
            { 
                response = ExchangeWithRemoteDebugger(hybridConnection, remoteContext);
            }
            catch (Exception e)
            {
                Context.DumpLog();
                Context.Log(e.Message);
                return;
            }
            
            if (response == null)
            {
                Context.Log("No response received");
                return;
            }
            
            if (response.MessageType == RemoteDebuggerMessageType.Exception)
            {
                throw response.GetException();
            }

            var updatedContext = response.GetContext<RemoteDebugExecutionContext>();
            Context.UpdateContext(updatedContext);
        }

        private RemoteDebuggerMessage? ExchangeWithRemoteDebugger(HybridConnection hybridConnection,
            RemoteDebugExecutionContext remoteContext)
        {
            var message = new RemoteDebuggerMessage(RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id);
            RemoteDebuggerMessage? response;
            Context.LogContextEntry();

            while (true)
            {
                response = hybridConnection.SendMessage(message).GetAwaiter().GetResult();

                Context.Log($"Received response : {response?.MessageType}\n");

                if (response == null || response.MessageType is RemoteDebuggerMessageType.Context or RemoteDebuggerMessageType.Exception)
                {
                    break;
                }

                var request = response.GetOrganizationRequest();

                var service = Context.GetOrganizationService(response.UserId);

                Context.Log("Executing local machine request");
                var organizationResponse = service.Execute(request);

                message = new RemoteDebuggerMessage(RemoteDebuggerMessageType.Response, organizationResponse,
                    remoteContext.Id);
                Context.Log("Transferring response to local machine");
            }

            return response;
        }

        private static QueryExpression CreateBaseDebugSessionQuery([Required] params Guid[] initiatingUserIds)
        {
            var queryDebugSessions = BindingModelHelper.GetRetrieveAllQuery<DebugSession>();
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.StateCode, ConditionOperator.Equal,
                DebugSessionState.Active.ToInt());
            queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.DebugeeId, ConditionOperator.In,
                initiatingUserIds.Cast<object>().ToArray());
            return queryDebugSessions;
        }

        private static HybridConnection InitConnection(DebugSession debugSession)
        {
            var uri = new Uri($"{debugSession.RelayUrl.TrimEnd('/')}/{debugSession.HybridConnectionName}");

            return new HybridConnection(debugSession.SasKeyName, debugSession.SasConnectionKey, uri.AbsoluteUri);
        }

        public DebugSession? GetDebugSession()
        {
            var query = CreateBaseDebugSessionQuery(Context.GetInitiatingUserId(), Context.GetRootUserId());
            
            ModifyDebugSessionQuery(query);

            return Context.AdminOrganizationService.RetrieveAll<DebugSession>(query).FirstOrDefault();
        }

        protected virtual void ModifyDebugSessionQuery(QueryExpression query)
        {
        }
    }
}