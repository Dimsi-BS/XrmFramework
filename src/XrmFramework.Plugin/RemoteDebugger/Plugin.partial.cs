using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using XrmFramework.BindingModel;
using XrmFramework.RemoteDebugger;
using XrmFramework.Definitions;

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    partial class Plugin
    {
        private bool SendToRemoteDebugger(LocalPluginContext localContext)
        {
            #if !DISABLE_REMOTE_DEBUG
            
            if (!localContext.IsDebugContext)
            {
                localContext.Log("The context is genuine");
                localContext.Log($"UnSecuredConfig : {UnSecuredConfigFull}");

                var initiatingUserId = localContext.GetInitiatingUserId();
                var rootUserId = localContext.GetRootUserId();

                localContext.Log($"Initiating user Id : {initiatingUserId}, root user Id : {rootUserId}");

                var queryDebugSessions = BindingModelHelper.GetRetrieveAllQuery<DebugSession>();
                queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.DebugeeId, ConditionOperator.In,
                    initiatingUserId, rootUserId);
                queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.StateCode,
                    ConditionOperator.Equal, DebugSessionState.Active.ToInt());

                var debugSession = localContext.AdminOrganizationService.RetrieveAll<DebugSession>(queryDebugSessions)
                    .FirstOrDefault();

                localContext.Log($"Debug session : {debugSession}");

                if (debugSession == null || debugSession.SessionEnd < DateTime.Today)
                {
                    return false;
                }

                var remoteContext = localContext.RemoteContext;
                remoteContext.Id = Guid.NewGuid();
                remoteContext.TypeAssemblyQualifiedName = GetType().AssemblyQualifiedName;
                remoteContext.UnsecureConfig = UnSecuredConfigFull;
                remoteContext.SecureConfig = SecuredConfig;

                var uri = new Uri($"{debugSession.RelayUrl.TrimEnd('/')}/{debugSession.HybridConnectionName}");

                try
                {
                    using var hybridConnection = new HybridConnection(debugSession.SasKeyName,
                        debugSession.SasConnectionKey, uri.AbsoluteUri);
                    var message = new RemoteDebuggerMessage(RemoteDebuggerMessageType.Context, remoteContext,
                        remoteContext.Id);

                    RemoteDebuggerMessage response;
                    while (true)
                    {
                        localContext.Log("Sending context to local machine : {0}", message);

                        response = hybridConnection.SendMessage(message).GetAwaiter().GetResult();

                        localContext.Log("Received response : {0}", response);

                        if (response.MessageType == RemoteDebuggerMessageType.Context ||
                            response.MessageType == RemoteDebuggerMessageType.Exception)
                        {
                            break;
                        }

                        var request = response.GetOrganizationRequest();

                        var service = response.UserId.HasValue
                            ? localContext.GetService(response.UserId.Value)
                            : localContext.AdminOrganizationService;

                        var organizationResponse = service.Execute(request);

                        message = new RemoteDebuggerMessage(RemoteDebuggerMessageType.Response,
                            organizationResponse, remoteContext.Id);
                    }

                    if (response.MessageType == RemoteDebuggerMessageType.Exception)
                    {
                        throw response.GetException();
                    }

                    var updatedContext = response.GetContext<RemoteDebugExecutionContext>();

                    localContext.UpdateContext(updatedContext);


                    return true;
                }
                catch (HttpRequestException)
                {
                    // Run the plugin as deploy if the remote debugger is not connected
                }
            }

            #endif
            
            return false;
        }
    }
}