// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using Plugins;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Xml;
using Model;
using XrmFramework.Common;
using XrmFramework.Debugger;
using XrmFramework.Model;

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

            localContext.Log(string.Format("Entered {0}.Execute()", ChildClassName));

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
                        if (debugSession.SessionEnd < DateTime.Today)
                        {
                            throw new InvalidPluginExecutionException($"The debug session for user {localContext.InitiatingUserId} has ended, update the session to use it");
                        }

                        var remoteContext = localContext.RemoteContext;

                        remoteContext.ActivityAssemblyQualifiedName = GetType().AssemblyQualifiedName;
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

                                var updatedContext = response.GetContext<RemoteDebugWorkflowExecutionContext>();

                                localContext.UpdateContext(updatedContext);

                                ExtractArgumentsFromRemoteContext(context, updatedContext);
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

                #endregion


                if (ActivityAction != null)
                {
                    localContext.Log(string.Format(
                        "{0} is firing for Entity: {1}, Message: {2}, Mode: {3}",
                        ChildClassName,
                        localContext.WorkflowContext.PrimaryEntityName,
                        localContext.WorkflowContext.MessageName,
                        localContext.WorkflowContext.Mode));

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
                localContext.Log(string.Format("Exception: {0}", e));

                // Handle the exception.
                throw;
            }
            catch (TargetInvocationException e)
            {
                localContext.Log(string.Format("Exception: {0}", e.InnerException));
                throw e.InnerException;
            }
            finally
            {
                localContext.Log(string.Format("Exiting {0}.Execute()", ChildClassName));
            }
        }

        private void ExtractArgumentsFromRemoteContext(CodeActivityContext context, RemoteDebugWorkflowExecutionContext updatedContext)
        {
            foreach (var argument in updatedContext.Arguments)
            {
                var property = GetType().GetProperty(argument.Key);

                if (property == null)
                {
                    break;
                }

                if (typeof(OutArgument).IsAssignableFrom(property.PropertyType) ||
                    typeof(InOutArgument).IsAssignableFrom(property.PropertyType))
                {
                    var setMethod = property.PropertyType.GetMethod("Set", new[] {typeof(CodeActivityContext)});

                    if (setMethod != null)
                    {
                        setMethod.Invoke(property.GetValue(this), new object[] {context, argument.Value});
                    }
                }
            }
        }

        private void SetArgumentsInRemoteContext(CodeActivityContext context, RemoteDebugWorkflowExecutionContext remoteContext)
        {
            foreach (var property in GetType().GetProperties())
            {
                if (typeof(InArgument).IsAssignableFrom(property.PropertyType) ||
                    typeof(InOutArgument).IsAssignableFrom(property.PropertyType))
                {
                    var getMethod = property.PropertyType.GetMethod("Get", new[] {typeof(CodeActivityContext)});

                    if (getMethod != null)
                    {
                        remoteContext.Arguments.Add(property.Name, getMethod.Invoke(property.GetValue(this), new object[] {context}));
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
            foreach (var propertyInfo in this.GetType().GetProperties())
            {
                if (typeof(InArgument).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var argument = propertyInfo.GetValue(this, null) as InArgument;

                    localContext.DumpObject(propertyInfo.Name, argument.Get(context));
                }
            }
        }

        private void DumpOutputArguments(LocalWorkflowContext localContext, CodeActivityContext context)
        {
            foreach (var propertyInfo in this.GetType().GetProperties())
            {
                if (typeof(OutArgument).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var argument = propertyInfo.GetValue(this, null) as OutArgument;

                    localContext.DumpObject(propertyInfo.Name, argument.Get(context));
                }
            }
        }

        private string ChildClassName
        {
            get
            {
                return this.GetType().Name;
            }
        }

        protected void SetAction(string actionName)
        {
            var method = this.GetType().GetMethod(actionName);
            if (method == null)
            {
                throw new InvalidPluginExecutionException(string.Format("The method {0}.{1} does not exist or is private", ChildClassName, actionName));
            }
            else if (!method.IsPublic || method.IsStatic)
            {
                throw new InvalidPluginExecutionException(string.Format("The method {0}.{1} should be public and not static", ChildClassName, actionName));
            }

            foreach (var param in method.GetParameters())
            {
                if (!typeof(ICustomWorkflowContext).IsAssignableFrom(param.ParameterType)
                        && (!param.ParameterType.IsInterface || !typeof(IService).IsAssignableFrom(param.ParameterType)))
                {
                    throw new InvalidPluginExecutionException(string.Format("{0}.{1} parameter : {2}. Only LocalPluginContext and IService interfaces are allowed as parameters", ChildClassName, method.Name, param.Name));
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
