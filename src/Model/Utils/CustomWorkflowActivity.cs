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
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Xml;

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
