// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using XrmFramework.Workflow;

namespace XrmFramework
{
    /// <summary>
    ///     Base class for all Plugins.
    /// </summary>
    //[DebuggerNonUserCode]
    public abstract partial class Plugin : IPlugin
    {
        private readonly IList<Step> _newRegisteredEvents = new Collection<Step>();

        protected const string PreImageName = "PreImage";

        protected const string PostImageName = "PostImage";

        private string SecuredConfig { get; }

        private string UnSecuredConfigFull { get; }

        protected string UnsecuredConfig { get; }

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
            UnSecuredConfigFull = unsecuredConfig;

            try 
            {
                var stepConfiguration = JsonConvert.DeserializeObject<StepConfiguration>(unsecuredConfig);

                UnsecuredConfig = stepConfiguration.Configuration;
            }
            catch (Exception) 
            {
                // Problème de désérialization de la config
                UnsecuredConfig = unsecuredConfig;
            }

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

        protected void SetCustomApiInfos(string methodName)
        {
            _isCustomApi = true;
            _customApiMethodName = methodName;

            var customApiAttribute = GetType().GetCustomAttribute<CustomApiAttribute>();

            _customApiEntityName = string.IsNullOrWhiteSpace(customApiAttribute.BoundEntityLogicalName) ? string.Empty : customApiAttribute.BoundEntityLogicalName;
        }

        private bool _isCustomApi;
        private string _customApiMethodName;
        private string _customApiEntityName;

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
            var localContext = new LocalPluginContext(serviceProvider, UnSecuredConfigFull, SecuredConfig);

            localContext.Log($"Entity: {localContext.PrimaryEntityName}, Message: {localContext.MessageName}, Stage: {Enum.ToObject(typeof(Stages), localContext.Stage)}, Mode: {localContext.Mode}");

            localContext.Log($"\r\nClass {ChildClassName}");
            localContext.Log($"\r\nUserId\t\t\t\t{localContext.UserId}\r\nInitiatingUserId\t{localContext.InitiatingUserId}");
            localContext.Log($"\r\nStart : {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}");
            sw.Restart();


            try
            {
                if (SendToRemoteDebugger(localContext))
                {
                    return;
                }

                IEnumerable<Step> steps;

                if (_isCustomApi)
                {
                    steps = new List<Step>
                    {
                        new Step(this, localContext.MessageName, Stages.PostOperation, Modes.Synchronous, _customApiEntityName, _customApiMethodName)
                    };
                }
                else
                {
                    steps =
                        from a in _newRegisteredEvents
                        where
                            localContext.ShouldExecuteStep(a)
                        select a;
                }

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

                    localContext.InvokeMethod(this, entityAction);

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

                if (e.InnerException != null)
                {
                    if (e.InnerException is InvalidPluginExecutionException invalidPluginExecutionException)
                    {
                        throw invalidPluginExecutionException;
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException(e.InnerException.Message);
                    }
                }
            }
            catch (JsonException e)
            {
                throw new InvalidPluginExecutionException(e.ToString());
            }
            finally
            {
                localContext.Log($"Exiting {ChildClassName}.Execute()");

                localContext.Log($"End : {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}\r\n");

                localContext.FlushLogs();
            }
        }
    }
}