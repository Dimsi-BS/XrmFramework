// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Plugins;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel;
using Model;
using XrmFramework.Common;
using XrmFramework.Model;
using XrmFramework.RemoteDebugger;

namespace Workflows
{
    public abstract class CustomWorkflowActivity : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Construct the Local plug-in context.
            var localContext = new LocalWorkflowContext(context);

            localContext.Log($"Entered {ChildClassName}.Execute()");

            try
            {
                #region Remote Debugger

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
                        if (debugSession.SessionEnd >= DateTime.Today)
                        {

                            var remoteContext = localContext.RemoteContext;

                            remoteContext.TypeAssemblyQualifiedName = GetType().AssemblyQualifiedName;
                            remoteContext.Id = Guid.NewGuid();

                            SetArgumentsInRemoteContext(context, remoteContext);

                            var uri = new Uri($"{debugSession.RelayUrl}/{debugSession.HybridConnectionName}");

                            try
                            {
                                using (var hybridConnection = new HybridConnection(debugSession.SasKeyName, debugSession.SasConnectionKey, uri.AbsoluteUri))
                                {
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

                                    ExtractArgumentsFromRemoteContext(context, updatedContext, localContext.Logger);
                                }


                                return;
                            }
                            catch (HttpRequestException e)
                            {
                                // Run the plugin as deploy if the remote debugger is not connected
                                localContext.Log($"Error while sending context : {e}");
                            }
                        }
                    }
                }

                #endregion


                if (ActivityAction != null)
                {
                    localContext.Log($"{ChildClassName} is firing for Entity: {localContext.WorkflowContext.PrimaryEntityName}, Message: {localContext.WorkflowContext.MessageName}, Mode: {localContext.WorkflowContext.Mode}");

                    localContext.DumpInputParameters();
                    localContext.DumpSharedVariables();

                    DumpInputArguments(localContext, context);

                    localContext.Log(ActivityAction.Name, "Start");
                    Invoke(ActivityAction, localContext);
                    localContext.Log(ActivityAction.Name, "End");

                    DumpOutputArguments(localContext, context);

                    // now exit - if the derived plug-in has incorrectly registered overlapping event registrations,
                    // guard against multiple executions.
                }
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                localContext.Log($"Exception: {e}");

                // Handle the exception.
                throw;
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                localContext.Log($"Exception: {e.InnerException}");
                throw e.InnerException;
            }
            finally
            {
                localContext.Log($"Exiting {ChildClassName}.Execute()");
            }
        }

        private void ExtractArgumentsFromRemoteContext(CodeActivityContext context, RemoteDebugExecutionContext updatedContext, Logger logger)
        {
            logger.Invoke(nameof(ExtractArgumentsFromRemoteContext), $"{updatedContext.Arguments.Count} Arguments");

            foreach (var argument in updatedContext.Arguments)
            {
                logger.Invoke(nameof(ExtractArgumentsFromRemoteContext), $"argument {argument.Key}");

                var property = GetType().GetProperty(argument.Key);

                if (property == null)
                {
                    break;
                }

                if (typeof(OutArgument).IsAssignableFrom(property.PropertyType) ||
                    typeof(InOutArgument).IsAssignableFrom(property.PropertyType))
                {
                    logger.Invoke(nameof(ExtractArgumentsFromRemoteContext), $"Argument {argument.Key} is InOutArgument or OutArgument");

                    var setMethod = property.PropertyType.GetMethod("Set", new[] { typeof(CodeActivityContext), typeof(object) });

                    logger.Invoke(nameof(ExtractArgumentsFromRemoteContext), $"Setter {setMethod?.Name}");

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

        private void Invoke(MethodInfo entityAction, LocalWorkflowContext localContext)
        {
            var listParamValues = new List<object>();

            foreach (var param in entityAction.GetParameters())
            {
                if (typeof(ICustomWorkflowContext).IsAssignableFrom(param.ParameterType))
                {
                    listParamValues.Add(localContext);
                }
                else if (typeof(IService).IsAssignableFrom(param.ParameterType))
                {
                    var obj = localContext.GetService(param.ParameterType);
                    listParamValues.Add(obj);
                }
            }

            entityAction.Invoke(this, listParamValues.ToArray());
        }

        private void DumpInputArguments(LocalWorkflowContext localContext, CodeActivityContext context)
        {
            localContext.Log("InputArguments :");
            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (typeof(InArgument).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var argument = propertyInfo.GetValue(this, null) as InArgument;

                    localContext.DumpObject(propertyInfo.Name, argument?.Get(context));
                }
            }
        }

        private void DumpOutputArguments(LocalWorkflowContext localContext, CodeActivityContext context)
        {
            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (typeof(OutArgument).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var argument = propertyInfo.GetValue(this, null) as OutArgument;

                    localContext.DumpObject(propertyInfo.Name, argument?.Get(context));
                }
            }
        }

        private string ChildClassName
        {
            get
            {
                return GetType().Name;
            }
        }

        protected void SetAction(string actionName)
        {
            var method = GetType().GetMethod(actionName);
            if (method == null)
            {
                throw new InvalidPluginExecutionException($"The method {ChildClassName}.{actionName} does not exist or is private");
            }
            else if (!method.IsPublic || method.IsStatic)
            {
                throw new InvalidPluginExecutionException($"The method {ChildClassName}.{actionName} should be public and not static");
            }

            foreach (var param in method.GetParameters())
            {
                if (!typeof(ICustomWorkflowContext).IsAssignableFrom(param.ParameterType)
                        && (!param.ParameterType.IsInterface || !typeof(IService).IsAssignableFrom(param.ParameterType)))
                {
                    throw new InvalidPluginExecutionException($"{ChildClassName}.{method.Name} parameter : {param.Name}. Only LocalPluginContext and IService interfaces are allowed as parameters");
                }
            }
            ActivityAction = method;
        }

        protected void SetDisplayName(string displayName)
        {
            DisplayName = displayName;
        }

        protected MethodInfo ActivityAction { get; private set; }

    }
}
