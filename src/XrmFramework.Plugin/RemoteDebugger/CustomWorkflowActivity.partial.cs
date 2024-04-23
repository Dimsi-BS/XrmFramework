using Microsoft.Xrm.Sdk.Query;
using System;
using System.Activities;
using System.Linq;
using System.Net.Http;
using XrmFramework.BindingModel;
using XrmFramework.RemoteDebugger;
using XrmFramework.Definitions;

// ReSharper disable once CheckNamespace
namespace XrmFramework.Workflow
{
    partial class CustomWorkflowActivity
    {
        private bool SendToRemoteDebugger(LocalWorkflowContext localContext, CodeActivityContext context)
        {
            #if !DISABLE_REMOTE_DEBUG
            
            if (!localContext.IsDebugContext)
            {
                localContext.Log("The context is genuine");

                var initiatingUserId = localContext.GetInitiatingUserId();

                var queryDebugSessions = BindingModelHelper.GetRetrieveAllQuery<DebugSession>();
                queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.DebugeeId, ConditionOperator.Equal, initiatingUserId);
                queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.StateCode, ConditionOperator.Equal, DebugSessionState.Active.ToInt());

                var debugSession = localContext.AdminOrganizationService.RetrieveAll<DebugSession>(queryDebugSessions).FirstOrDefault();

                localContext.Log($"Debug session : {debugSession}");

                if (debugSession != null)
                {
                    if (debugSession.SessionEnd < DateTime.Today)
                    {
                        return false;
                    }

                    var remoteContext = localContext.RemoteContext;

                    remoteContext.TypeAssemblyQualifiedName = GetType().AssemblyQualifiedName;
                    remoteContext.Id = Guid.NewGuid();

                    SetArgumentsInRemoteContext(context, remoteContext);

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

                        ExtractArgumentsFromRemoteContext(context, updatedContext, localContext.LogServiceMethod);


                        return true;
                    }
                    catch (HttpRequestException e)
                    {
                        // Run the plugin as deploy if the remote debugger is not connected
                        localContext.Log($"Error while sending context : {e}");
                    }
                }
            }

            #endif
            
            return false;
        }

        private void ExtractArgumentsFromRemoteContext(CodeActivityContext context, RemoteDebugExecutionContext updatedContext, LogServiceMethod logServiceMethod)
        {
            logServiceMethod.Invoke(nameof(ExtractArgumentsFromRemoteContext), $"{updatedContext.Arguments.Count} Arguments");

            foreach (var argument in updatedContext.Arguments)
            {
                logServiceMethod.Invoke(nameof(ExtractArgumentsFromRemoteContext), $"argument {argument.Key}");

                var property = GetType().GetProperty(argument.Key);

                if (property == null)
                {
                    break;
                }

                if (typeof(OutArgument).IsAssignableFrom(property.PropertyType) ||
                    typeof(InOutArgument).IsAssignableFrom(property.PropertyType))
                {
                    logServiceMethod.Invoke(nameof(ExtractArgumentsFromRemoteContext), $"Argument {argument.Key} is InOutArgument or OutArgument");

                    var setMethod = property.PropertyType.GetMethod("Set", new[] { typeof(CodeActivityContext), typeof(object) });

                    logServiceMethod.Invoke(nameof(ExtractArgumentsFromRemoteContext), $"Setter {setMethod?.Name}");

                    if (setMethod != null)
                    {
                        setMethod.Invoke(property.GetValue(this), new[] { context, argument.Value });
                    }
                }
            }
        }

        private void SetArgumentsInRemoteContext(CodeActivityContext context, RemoteDebugExecutionContext remoteContext)
        {
            foreach (var property in GetType().GetProperties())
            {
                if (typeof(InArgument).IsAssignableFrom(property.PropertyType) ||
                    typeof(InOutArgument).IsAssignableFrom(property.PropertyType))
                {
                    var getMethod = property.PropertyType.GetMethod("Get", new[] { typeof(CodeActivityContext) });

                    if (getMethod != null)
                    {
                        remoteContext.Arguments.Add(property.Name, getMethod.Invoke(property.GetValue(this), new object[] { context }));
                    }
                }
            }
        }

    }
}
