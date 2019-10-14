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
                throw new ArgumentNullException("serviceProvider");
            }

            //IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            //workflowContext.BusinessUnitId 
            //workflowContext.InitiatingUserId

            // Construct the Local plug-in context.
            var localContext = new LocalWorkflowContext(context);

            localContext.Log(string.Format("Entered {0}.Execute()", ChildClassName));

            try
            {
                #region Remote Debugger

                if (!localContext.IsDebugContext)
                {
                    localContext.Log("The context is genuine");
#if !DEBUG
                if (!string.IsNullOrEmpty(UnSecuredConfig) && UnSecuredConfig.Contains("debugSessions"))
                {
                    var debuggerUnsecuredConfig = JsonConvert.DeserializeObject<DebuggerUnsecureConfig>(UnSecuredConfig);
#endif

                    var queryDebugSessions = BindingModelHelper.GetRetrieveAllQuery<DebugSession>();
                    queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.DebugeeId, ConditionOperator.Equal, localContext.InitiatingUserId);
                    queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.StateCode, ConditionOperator.Equal, DebugSessionState.Active.ToInt());

#if !DEBUG
                queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.Id, ConditionOperator.In, debuggerUnsecuredConfig.DebugSessionIds.ToArray());
#endif

                    var debugSession = localContext.AdminOrganizationService.RetrieveAll<DebugSession>(queryDebugSessions).FirstOrDefault();

                    localContext.Log($"Debug session : {debugSession}");

                    if (debugSession != null)
                    {
                        if (debugSession.SessionEnd < DateTime.Today)
                        {
                            throw new InvalidPluginExecutionException($"The debug session for user {localContext.InitiatingUserId} has ended, update the session to use it");
                        }

                        var remoteContext = localContext.RemoteContext;
                        remoteContext.Id = Guid.NewGuid();

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
                            }


                            return;
                        }
                        catch (HttpRequestException)
                        {
                            // Run the plugin as deploy if the remote debugger is not connected
                        }
                    }
#if !DEBUG
                }
#endif
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
