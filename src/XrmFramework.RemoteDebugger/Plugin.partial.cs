using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Xrm.Sdk.Query;
using XrmFramework.BindingModel;
using XrmFramework.RemoteDebugger;

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    partial class Plugin
    {

        private bool SendToRemoteDebugger(LocalPluginContext localContext, string unsecureConfig, string secureConfig)
        {
            if (!localContext.IsDebugContext)
            {
                localContext.Log("The context is genuine");
#if !STANDALONE
                if (!string.IsNullOrEmpty(UnSecuredConfig) && UnSecuredConfig.Contains("debugSessions"))
                {
                    var debuggerUnsecuredConfig = JsonConvert.DeserializeObject<DebuggerUnsecureConfig>(UnSecuredConfig);
#endif

                var initiatingUserId = localContext.GetInitiatingUserId();

                var queryDebugSessions = BindingModelHelper.GetRetrieveAllQuery<DebugSession>();
                queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.DebugeeId, ConditionOperator.Equal, initiatingUserId);
                queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.StateCode, ConditionOperator.Equal, DebugSessionState.Active.ToInt());

#if !STANDALONE
                queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.Id, ConditionOperator.In, debuggerUnsecuredConfig.DebugSessionIds.Cast<object>().ToArray());
#endif

                var debugSession = localContext.AdminOrganizationService.RetrieveAll<DebugSession>(queryDebugSessions).FirstOrDefault();

                localContext.Log($"Debug session : {debugSession}");

                if (debugSession != null)
                {
                    if (debugSession.SessionEnd >= DateTime.Today)
                    {

                        var remoteContext = localContext.RemoteContext;
                        remoteContext.Id = Guid.NewGuid();
                        remoteContext.TypeAssemblyQualifiedName = GetType().AssemblyQualifiedName;
                        remoteContext.UnsecureConfig = unsecureConfig;
                        remoteContext.SecureConfig = secureConfig;

                        var uri = new Uri($"{debugSession.RelayUrl}/{debugSession.HybridConnectionName}");

                        try
                        {
                            using var hybridConnection = new HybridConnection(debugSession.SasKeyName, debugSession.SasConnectionKey, uri.AbsoluteUri);
                            var message = new RemoteDebuggerMessage(RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id);

                            RemoteDebuggerMessage response;
                            while (true)
                            {
                                localContext.Log("Sending context to local machine : {0}", message);

                                response = hybridConnection.SendMessage(message).GetAwaiter().GetResult();

                                localContext.Log("Received response : {0}", response);

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


                            return true;
                        }
                        catch (HttpRequestException)
                        {
                            // Run the plugin as deploy if the remote debugger is not connected
                        }
                    }
                }
#if !STANDALONE
                }
#endif
            }

            return false;
        }
    }
}
