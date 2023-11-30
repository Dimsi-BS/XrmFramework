// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Activities;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Workflow;
using EntityReference = Microsoft.Xrm.Sdk.EntityReference;

namespace XrmFramework.Workflow
{

    public partial class LocalWorkflowContext : LocalContext, ICustomWorkflowContext
    {
        private readonly CodeActivityContext _context;

        public IWorkflowContext WorkflowContext => (IWorkflowContext)ExecutionContext;

        public LocalWorkflowContext(CodeActivityContext context) : base(context)
        {
            _context = context;

            ObjectContainer.RegisterInstanceAs(this, typeof(ICustomWorkflowContext));
        }

        T ICustomWorkflowContext.GetArgumentValue<T>(InArgument<T> argument)
            => argument.Get(_context);

        void ICustomWorkflowContext.SetArgumentValue<T>(OutArgument<T> argument, T value)
        {
            argument.Set(_context, value);
        }

        T ICustomWorkflowContext.GetArgumentValue<T>(InOutArgument<T> argument)
            => argument.Get(_context);

        void ICustomWorkflowContext.SetArgumentValue<T>(InOutArgument<T> argument, T value)
        {
            argument.Set(_context, value);
        }

        public EntityReference ObjectRef => new(WorkflowContext.PrimaryEntityName, WorkflowContext.PrimaryEntityId);

        public WorkflowModes WorkflowMode
            => (WorkflowModes)Enum.ToObject(typeof(WorkflowModes), WorkflowContext.WorkflowMode);


        public void LogCollection(Dictionary<string, object> list)
        {
            Logger.LogCollection(list);
        }


        public object? GetService(Type serviceType)
            => ObjectContainer.Resolve(serviceType);

        public TService GetService<TService>() where TService : IService
            => ObjectContainer.Resolve<TService>();
    }
}