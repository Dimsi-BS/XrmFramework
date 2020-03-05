// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using System.Reflection;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.Xrm.Sdk.Query;
using Model;
using XrmFramework.Common;
#if INTERNAL_NEWTONSOFT
using Newtonsoft.Json.Xrm;
#else 
using Newtonsoft.Json;
#endif
using XrmFramework.RemoteDebugger;
using XrmFramework.Model;

namespace Plugins
{
    /// <summary>
    ///     Base class for all Plugins.
    /// </summary>
    //[DebuggerNonUserCode]
    public abstract class Plugin : IPlugin
    {
        private readonly IList<Step> _newRegisteredEvents = new Collection<Step>();

        protected const string PreImageName = "PreImage";

        protected const string PostImageName = "PostImage";

        private string SecuredConfig { get; set; }

        private string UnSecuredConfig { get; set; }

        public bool StepsInitialized { get; private set; }

        #region .ctor

        /// <summary>
        ///     Initializes a new instance of the <see cref="Plugin" /> class.
        /// </summary>
        protected Plugin(string unsecuredConfig, string securedConfig) : this(unsecuredConfig, securedConfig, false)
        {
        }

        protected Plugin(string unsecuredConfig, string securedConfig, bool delayStepRegistration)
        {
            SecuredConfig = securedConfig;
            UnSecuredConfig = unsecuredConfig;
            InitializeSteps(delayStepRegistration);
        }
        #endregion

        protected void InitializeSteps(bool delayStepRegistration = false)
        {
            if (delayStepRegistration)
            {
                return;
            }
            AddSteps();
            StepsInitialized = true;
        }

        protected abstract void AddSteps();

        public ReadOnlyCollection<Step> Steps => new ReadOnlyCollection<Step>(_newRegisteredEvents);

        protected void AddStep(Stages stage, Messages messageName, Modes mode, string entityName, string actionName, params string[] columns)
        {
            var list = _newRegisteredEvents;

            var step = new Step(this, messageName, stage, mode, entityName, actionName, columns);

            if (step.Method == null)
            {
                throw new InvalidPluginExecutionException($"The method {ChildClassName}.{actionName} used during {messageName} message does not exist or is private");
            }
            else if (!step.Method.IsPublic || step.Method.IsStatic)
            {
                throw new InvalidPluginExecutionException($"The method {ChildClassName}.{actionName} used during {messageName} message should be public and not static");
            }

            foreach (var param in step.Method.GetParameters())
            {
                if (!param.ParameterType.IsInterface || (!typeof(IPluginContext).IsAssignableFrom(param.ParameterType)
                        && (!typeof(IService).IsAssignableFrom(param.ParameterType))))
                {
                    throw new InvalidPluginExecutionException($"{ChildClassName}.{actionName} parameter : {param.Name}. Only IPluginContext and IService interfaces are allowed as parameters");
                }
            }

            list.Add(step);
        }

        /// <summary>
        ///     Gets or sets the name of the child class.
        /// </summary>
        /// <value>The name of the child class.</value>
        protected string ChildClassName => GetType().Name;

        /// <summary>
        ///     Executes the plug-in.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        ///     For improved performance, Microsoft Dynamics CRM caches plug-in instances.
        ///     The plug-in's Execute method should be written to be stateless as the constructor
        ///     is not called for every invocation of the plug-in. Also, multiple system threads
        ///     could execute the plug-in at the same time. All per invocation state information
        ///     is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        public void Execute(IServiceProvider serviceProvider)
        {
            var sw = new Stopwatch();
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            // Construct the Local plug-in context.
            var localContext = new LocalPluginContext(serviceProvider);

            localContext.Log($"Entity: {localContext.PrimaryEntityName}, Message: {localContext.MessageName}, Stage: {Enum.ToObject(typeof(Stages), localContext.Stage)}, Mode: {localContext.Mode}");

            localContext.Log($"\r\nClass {ChildClassName}");
            localContext.Log($"\r\nUserId\t\t\t\t{localContext.UserId}\r\nInitiatingUserId\t{localContext.InitiatingUserId}");
            localContext.Log($"\r\nStart : {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}");
            sw.Restart();


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

                    var initiatingUserId = localContext.GetInitiatingUserId();

                    var queryDebugSessions = BindingModelHelper.GetRetrieveAllQuery<DebugSession>();
                    queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.DebugeeId, ConditionOperator.Equal, initiatingUserId);
                    queryDebugSessions.Criteria.AddCondition(DebugSessionDefinition.Columns.StateCode, ConditionOperator.Equal, DebugSessionState.Active.ToInt());

#if !DEBUG
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
                                }


                                return;
                            }
                            catch (HttpRequestException)
                            {
                                // Run the plugin as deploy if the remote debugger is not connected
                            }
                        }
                    }
#if !DEBUG
                }
#endif
                }

                #endregion




                // Iterate over all of the expected registered events to ensure that the plugin
                // has been invoked by an expected event
                // For any given plug-in event at an instance in time, we would expect at most 1 result to match.
                var steps =
                    (from a in _newRegisteredEvents
                     where (
                               localContext.IsStage(a.Stage) &&
                               localContext.IsMessage(a.Message) &&
                               localContext.Mode == a.Mode &&
                               (string.IsNullOrWhiteSpace(a.EntityName) ||
                                a.EntityName == localContext.PrimaryEntityName)
                           )
                     select a);

                var stage = Enum.ToObject(typeof(Stages), localContext.Stage);
                sw.Restart();
                localContext.Log("------------------ Input Variables (before) ------------------");
                localContext.DumpInputParameters();
                localContext.Log("\r\n------------------ Shared Variables (before) ------------------");
                localContext.DumpSharedVariables();
                localContext.Log("\r\n---------------------------------------------------------------");

                foreach (var step in steps)
                {
                    sw.Restart();
                    if (step.Message == Messages.Update && step.FilteringAttributes.Any())
                    {

                        var target = localContext.GetInputParameter<Entity>(InputParameters.Target);

                        var useStep = false;

                        foreach (var attributeName in target.Attributes.Select(a => a.Key))
                        {
                            useStep |= step.FilteringAttributes.Contains(attributeName);
                        }

                        if (!useStep)
                        {
                            localContext.Log(
                                "\r\n{0}.{5} is not fired because filteringAttributes filter is not met.",
                                ChildClassName,
                                localContext.PrimaryEntityName,
                                localContext.MessageName,
                                stage,
                                localContext.Mode, step.Method.Name);
                            continue;
                        }

                    }

                    var entityAction = step.Method;

                    localContext.Log($"\r\n\r\n{ChildClassName}.{step.Method.Name} is firing");

                    sw.Restart();

                    localContext.Log($"{ChildClassName}.{step.Method.Name} Start");

                    Invoke(entityAction, localContext);

                    localContext.Log($"{ChildClassName}.{step.Method.Name} End, duration : {sw.Elapsed}");
                }

                if (localContext.IsStage(Stages.PreValidation) || localContext.IsStage(Stages.PreOperation))
                {
                    localContext.Log("\r\n\r\n------------------ Input Variables (after) ------------------");
                    localContext.DumpInputParameters();
                    localContext.Log("\r\n------------------ Shared Variables (after) ------------------");
                    localContext.DumpSharedVariables();
                    localContext.Log("\r\n---------------------------------------------------------------");
                }
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                localContext.Log($"Exception: {e}");

                // Handle the exception.
                throw;
            }
            catch (TargetInvocationException e)
            {
                localContext.Log($"Exception : {e.InnerException}");

                if (e.InnerException != null) throw e.InnerException;
            }
            catch (JsonException e)
            {
                throw new InvalidPluginExecutionException(e.ToString());
            }
            finally
            {
                localContext.Log($"Exiting {ChildClassName}.Execute()");

                localContext.Log($"End : {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}\r\n");
            }
        }

        private void Invoke(MethodInfo entityAction, LocalPluginContext localContext)
        {
            var listParamValues = new List<object>();

            foreach (var param in entityAction.GetParameters())
            {
                if (typeof(IPluginContext).IsAssignableFrom(param.ParameterType))
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
    }
}