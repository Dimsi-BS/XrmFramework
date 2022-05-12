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

namespace XrmFramework
{
    /// <summary>
    ///     Base class for all Plugins.
    /// </summary>
    //[DebuggerNonUserCode]
    public abstract partial class Plugin : IPlugin
    {
        //Steps at which this plugin will invoke a method
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
            // Create a new step with the input parameters 
            var step = new Step(this, messageName, stage, mode, entityName, actionName, columns);
            // Invoked method must exist, be public and non static
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
                //Parameters for the step method must be an interface like IPluginContext and IService
                if (!param.ParameterType.IsInterface || (!typeof(IPluginContext).IsAssignableFrom(param.ParameterType)
                                                          && !typeof(IService).IsAssignableFrom(param.ParameterType)))
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
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            // Construct the Local plug-in context.
            var localContext = new LocalPluginContext(serviceProvider);

            localContext.Log($"\r\nClass {ChildClassName}");
            localContext.LogStart();
            localContext.Log("The context is genuine");

            // If currently remote debugging, no need to go on
            if (SendToRemoteDebugger(localContext)) return;

            localContext.LogContextEntry();

            //This is a virtual method, it is overriden in the CustomApi derived class
            var steps = InitStepsToExecute(localContext);
            try
            {
                InvokeSteps(localContext, steps);
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                localContext.Log($"Exception: {e}");
                throw;
            }
            catch (Exception e)
            {
                PluginExceptionHandler(localContext, e);
            }

            localContext.LogContextExit();
            localContext.Log($"Exiting {ChildClassName}.Execute()");
            localContext.LogExit();
        }


        private void InvokeSteps(LocalPluginContext localContext, IEnumerable<Step> steps)
        {
            var methodsToAvoid = string.IsNullOrEmpty(UnSecuredConfig)
                ? new List<string>()
                : JsonConvert.DeserializeObject<StepConfiguration>(UnSecuredConfig).BannedMethods;

            // Create stage enum value with local context

            var sw = new Stopwatch();

            // Execute the action corresponding to each step
            foreach (var step in steps)
            {
                //Proceed to filtering
                // If the filtering condition are not met, proceed to the next step without executing the action corresponding to this one

                if (!ShouldExecuteForFilteringAttributes(localContext, step))
                {
                    localContext.LogNotFiredForFilteringAttributes(ChildClassName, step.Method.Name);
                    continue;
                }

                // Get the info regarding the method corresponding to the step
                var entityAction = step.Method;

                if (methodsToAvoid.Contains(entityAction.Name))
                {
                    localContext.Log(
                        $"\r\n{ChildClassName}.{step.Method.Name} is not fired because it has been manually disabled in the Unsecured Configuration.");
                    continue;
                }

                localContext.Log($"\r\n\r\n{ChildClassName}.{step.Method.Name} is firing");

                sw.Restart();

                localContext.Log($"{ChildClassName}.{step.Method.Name} Start");
                //Call the method corresponding to the step
                localContext.InvokeMethod(this, entityAction);

                localContext.Log($"{ChildClassName}.{step.Method.Name} End, duration : {sw.Elapsed}");
            }
        }

        internal virtual IEnumerable<Step> InitStepsToExecute(LocalPluginContext localContext)
        {
            return from a in _newRegisteredEvents
                   where
                       localContext.ShouldExecuteStep(a)
                   select a;
        }

        private static bool ShouldExecuteForFilteringAttributes(LocalPluginContext localContext, Step step)
        {
            if (!(step.Message == Messages.Update && step.FilteringAttributes.Any()))
            {
                return true;
            }

            var target = localContext.GetInputParameter<Entity>(InputParameters.Target);
            //Create a variable of which the value signals one or more of the filtered attributes is present
            var useStep = false;
            //Test the target to determine the value of useStep
            foreach (var attributeName in target.Attributes.Select(a => a.Key))
            {
                useStep |= step.FilteringAttributes.Contains(attributeName);
            }

            return useStep;
        }

        private void PluginExceptionHandler(LocalPluginContext localContext, Exception exception)
        {

            switch (exception)
            {
                case TargetInvocationException e:

                    localContext.Log($"Exception : {e.InnerException}");
                    if (e.InnerException != null)
                    {
                        if (e.InnerException is InvalidPluginExecutionException invalidPluginExecutionException)
                        {
                            throw invalidPluginExecutionException;
                        }

                        throw new InvalidPluginExecutionException(e.InnerException.Message);
                    }
                    throw e;

                case JsonException e:
                    {
                        throw new InvalidPluginExecutionException(e.ToString());
                    }
                default: throw exception;
            }
        }

    }
}