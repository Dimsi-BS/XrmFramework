﻿// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Xrm.Sdk;

namespace XrmFramework
{
    internal partial class LocalPluginContext : LocalContext, IPluginContext
    {
        public LocalPluginContext(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            if (PluginExecutionContext.ParentContext != null)
            {
                ParentPluginContext = new LocalPluginContext(this, PluginExecutionContext.ParentContext);
            }

            ObjectContainer.RegisterInstanceAs(this, typeof(IPluginContext));
        }

        private LocalPluginContext(LocalPluginContext context, IPluginExecutionContext parentContext)
            : base(context, parentContext)
        {
            if (PluginExecutionContext.ParentContext != null)
            {
                ParentPluginContext = new LocalPluginContext(this, PluginExecutionContext.ParentContext);
            }

            ObjectContainer.RegisterInstanceAs(this, typeof(IPluginContext));
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

        public ParameterCollection SharedVariables => PluginExecutionContext.SharedVariables;

        public IPluginContext ParentContext => ParentPluginContext;

        public LocalPluginContext ParentPluginContext { get; }

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

        public EntityReference ObjectRef => throw new NotImplementedException();
        

    }

}
