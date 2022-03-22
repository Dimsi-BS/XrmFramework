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

namespace XrmFramework.RemoteDebugger
{
    public class RemoteDebuggerPlugin2 : IPlugin
    {
        public RemoteDebuggerPlugin2(string unsecuredConfig, string securedConfig)
        {
            UnsecuredConfig = unsecuredConfig;
            SecuredConfig = securedConfig;
        }

        public string UnsecuredConfig { get; }
        public string SecuredConfig { get; }

        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var localContext = new LocalPluginContext(serviceProvider);


            localContext.Log(localContext.RemoteContext.RequestId.ToString());

            if (!localContext.IsDebugContext )
            {
                localContext.Log("The context is genuine");
                localContext.Log($"UnSecuredConfig : {UnsecuredConfig}");

                //if (!string.IsNullOrEmpty(UnSecuredConfig) && UnSecuredConfig.Contains("debugSessions"))
                //{
                //    var debuggerUnsecuredConfig = JsonConvert.DeserializeObject<DebuggerUnsecureConfig>(UnSecuredConfig);

                //    localContext.Log($"Debug session ids : {string.Join(",", debuggerUnsecuredConfig.DebugSessionIds)}");

                var initiatingUserId = localContext.GetInitiatingUserId();

                localContext.Log($"Initiating user Id : {initiatingUserId}");

                var queryDebugSessions = BindingModelHelper.GetRetrieveAllQuery<DebugSession>();
                queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.DebugeeId, ConditionOperator.Equal, initiatingUserId);
                queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.StateCode, ConditionOperator.Equal, DebugSessionState.Active.ToInt());

                //queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.Id, ConditionOperator.In, debuggerUnsecuredConfig.DebugSessionIds.Cast<object>().ToArray());
                
                var debugSession = localContext.AdminOrganizationService.RetrieveAll<DebugSession>(queryDebugSessions).FirstOrDefault();
                if(debugSession == null)
                {
                    localContext.Log("debugSession is null");
                    return;
                }

                if(debugSession.DebugeeId != initiatingUserId)
                {
                    localContext.Log("initiating user id ({0}) does not correspond to debugge id ({1})",initiatingUserId,debugSession.DebugeeId);
                    return;
                }
                localContext.Log($"Debug session : {debugSession}");

                if (debugSession != null)
                {
                    if (debugSession.SessionEnd >= DateTime.Today)
                    {

                        var remoteContext = localContext.RemoteContext;
                        remoteContext.Id = Guid.NewGuid();
                        remoteContext.TypeAssemblyQualifiedName = UnsecuredConfig;
                        remoteContext.UnsecureConfig = UnsecuredConfig;
                        remoteContext.SecureConfig = SecuredConfig;

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


                        }
                        catch (HttpRequestException)
                        {
                            // Run the plugin as deploy if the remote debugger is not connected
                        }
                    }
                }
               
                //}
            }
            else
            {
                localContext.Log("This plugin should not be executed in local context");
                return;
            }



            
            return;

           
            

        }

        private bool SendToRemoteDebugger(LocalPluginContext localContext)
        {
            if (!localContext.IsDebugContext)
            {
                localContext.Log("The context is genuine");
                localContext.Log($"UnSecuredConfig : {UnsecuredConfig}");

                //if (!string.IsNullOrEmpty(UnSecuredConfig) && UnSecuredConfig.Contains("debugSessions"))
                //{
                //    var debuggerUnsecuredConfig = JsonConvert.DeserializeObject<DebuggerUnsecureConfig>(UnSecuredConfig);

                //    localContext.Log($"Debug session ids : {string.Join(",", debuggerUnsecuredConfig.DebugSessionIds)}");

                var initiatingUserId = localContext.GetInitiatingUserId();

                localContext.Log($"Initiating user Id : {initiatingUserId}");

                var queryDebugSessions = BindingModelHelper.GetRetrieveAllQuery<DebugSession>();
                queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.DebugeeId, ConditionOperator.Equal, initiatingUserId);
                queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.StateCode, ConditionOperator.Equal, DebugSessionState.Active.ToInt());

                //queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.Id, ConditionOperator.In, debuggerUnsecuredConfig.DebugSessionIds.Cast<object>().ToArray());

                var debugSession = localContext.AdminOrganizationService.RetrieveAll<DebugSession>(queryDebugSessions).FirstOrDefault();

                localContext.Log($"Debug session : {debugSession}");

                if (debugSession != null)
                {
                    if (debugSession.SessionEnd >= DateTime.Today)
                    {

                        var remoteContext = localContext.RemoteContext;
                        remoteContext.Id = Guid.NewGuid();
                        remoteContext.TypeAssemblyQualifiedName = GetType().AssemblyQualifiedName;
                        remoteContext.UnsecureConfig = UnsecuredConfig;
                        remoteContext.SecureConfig = SecuredConfig;

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
                //}
            }

            return false;
        }


    }
}
