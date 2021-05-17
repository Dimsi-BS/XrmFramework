// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Activities;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.Workflow
{
    public abstract partial class CustomWorkflowActivity : CodeActivity
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
                if (SendToRemoteDebugger(localContext, context))
                {
                    return;
                }

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
                localContext.LogError(e);

                // Handle the exception.
                throw;
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                localContext.LogError(e.InnerException);
                throw e.InnerException;
            }
            finally
            {
                localContext.Log($"Exiting {ChildClassName}.Execute()");

                localContext.DumpLog();
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
                    var obj = ServiceFactory.GetService(param.ParameterType, localContext);
                    listParamValues.Add(obj);
                }
            }

            entityAction.Invoke(this, listParamValues.ToArray());
        }

        private void DumpInputArguments(LocalWorkflowContext localContext, CodeActivityContext context)
        {
            localContext.Log("InputArguments :");

            var list = new Dictionary<string, object>();
            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (typeof(InArgument).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var argument = propertyInfo.GetValue(this, null) as InArgument;

                    list.Add(propertyInfo.Name, argument?.Get(context));
                }
            }

            localContext.LogCollection(list);
        }

        private void DumpOutputArguments(LocalWorkflowContext localContext, CodeActivityContext context)
        {
            localContext.Log("OutputArguments :");

            var list = new Dictionary<string, object>();
            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (typeof(OutArgument).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var argument = propertyInfo.GetValue(this, null) as OutArgument;

                    list.Add(propertyInfo.Name, argument?.Get(context));
                }
            }

            localContext.LogCollection(list);
        }

        private string ChildClassName => GetType().Name;

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
