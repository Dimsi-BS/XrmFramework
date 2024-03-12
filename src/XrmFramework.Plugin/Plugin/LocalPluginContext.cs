// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;

namespace XrmFramework
{
    internal partial class LocalPluginContext : LocalContext, IPluginContext, IServiceProvider
    {
        protected string UnsecuredConfig { get; }
        
        protected string SecuredConfig { get; }

        public LocalPluginContext(IServiceProvider serviceProvider, string unsecuredConfig, string securedConfig)
            : base(serviceProvider)
        {
            UnsecuredConfig = unsecuredConfig;
            SecuredConfig = securedConfig;
            
            if (PluginExecutionContext.ParentContext != null)
            {
                ParentLocalContext = new LocalPluginContext(this, PluginExecutionContext.ParentContext);
            }

            ObjectContainer.RegisterInstanceAs(this, typeof(IPluginContext));
            ObjectContainer.RegisterInstanceAs(this, typeof(ICustomApiContext));
        }

        private LocalPluginContext(LocalPluginContext context, IPluginExecutionContext parentContext)
            : base(context, parentContext)
        {
            if (PluginExecutionContext.ParentContext != null)
            {
                ParentLocalContext = new LocalPluginContext(this, PluginExecutionContext.ParentContext);
            }
        }

        private IPluginExecutionContext PluginExecutionContext => (IPluginExecutionContext)ExecutionContext;

        public bool IsPreOperation()
        {
            return IsStage(Stages.PreOperation);
        }

        public bool IsPreValidation()
        {
            return IsStage(Stages.PreValidation);
        }

        public bool IsPostOperation()
        {
            return IsStage(Stages.PostOperation);
        }

        public bool IsStage(Stages stage)
        {
            return PluginExecutionContext.Stage == (int)stage;
        }

        public string PrimaryEntityName => PluginExecutionContext.PrimaryEntityName;

        public Guid PrimaryEntityId => PluginExecutionContext.PrimaryEntityId;

        public int Stage => PluginExecutionContext.Stage;

        public Guid OrganizationId => PluginExecutionContext.OrganizationId;

        public int Depth => PluginExecutionContext.Depth;

        public IPluginContext ParentContext => (IPluginContext)ParentLocalContext;

        public bool IsMultiplePrePostOperation
        {
            get
            {
                if (ParentContext != null && (IsCreate() || IsUpdate()))
                {
                    var currentTarget = GetInputParameter<Entity>(InputParameters.Target);
                    var parentTarget = ParentContext.GetInputParameter<Entity>(InputParameters.Target);

                    return parentTarget.Attributes.Count != currentTarget.Attributes.Count;
                }
                return false;
            }
        }

        public bool ShouldExecuteStep(Step step)
        {
            var isValid =
                IsStage(step.Stage)
                && Mode == step.Mode
                && IsMessage(step.Message);

            if (isValid && step.EntityName != PrimaryEntityName)
            {
                isValid = false;

                if ((Messages.Associate.Equals(step.Message) || Messages.Disassociate.Equals(step.Message))
                    && !string.IsNullOrWhiteSpace(UnsecuredConfig))
                {
                    try
                    {
                        var stepConfig = JsonConvert.DeserializeObject<StepConfiguration>(UnsecuredConfig);
                        isValid = step.EntityName == stepConfig.RelationshipName;
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            return isValid;
        }

        public void LogStart()
        {
            Log($"Entity: {PrimaryEntityName}, Message: {MessageName}, Stage: {Enum.ToObject(typeof(Stages), Stage)}, Mode: {Mode}");

            Log($"\r\nUserId\t\t\t{UserId}\r\nInitiatingUserId\t{InitiatingUserId}");
            Log($"\r\nStart : {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}");
        }

        public void LogExit()
        {
            Log($"End : {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}\r\n");
        }

        public void LogContextEntry()
        {
            Log("\r\n------------------ Input Variables (before) ------------------");
            DumpInputParameters();
            Log("\r\n------------------ Shared Variables (before) ------------------");
            DumpSharedVariables();
            Log("\r\n---------------------------------------------------------------");
        }
        public void LogContextExit()
        {
            if (IsStage(Stages.PreValidation) || IsStage(Stages.PreOperation))
            {
                Log("\r\n\r\n------------------ Input Variables (after) ------------------");
                DumpInputParameters();
                Log("\r\n------------------ Shared Variables (after) ------------------");
                DumpSharedVariables();
                Log("\r\n---------------------------------------------------------------");
            }
        }

        public void LogNotFiredForFilteringAttributes(string? childClassName, string methodName)
        {
            var stage = Enum.ToObject(typeof(Stages), Stage);

            Log("\r\n{0}.{5} is not fired because filteringAttributes filter is not met.",
                childClassName,
                PrimaryEntityName,
                MessageName,
                stage,
                Mode, methodName);
        }

        public object? GetService(Type serviceType)
            => ObjectContainer.Resolve(serviceType);
    }

}
