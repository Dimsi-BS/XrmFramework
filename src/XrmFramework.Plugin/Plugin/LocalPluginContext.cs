// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Xrm.Sdk;
using System;

namespace XrmFramework
{
    internal partial class LocalPluginContext : LocalContext, IPluginContext
    {
        public LocalPluginContext(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
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
            Log("------------------ Input Variables (before) ------------------");
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

    }

}
