// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using XrmFramework.RemoteDebugger;

namespace Plugins
{

    public sealed class LocalWorkflowContext : LocalContext, ICustomWorkflowContext
    {
        private readonly CodeActivityContext _context;

        public IWorkflowContext WorkflowContext => (IWorkflowContext)ExecutionContext;

        public LocalWorkflowContext(CodeActivityContext context)
            : base(context)
        {
            _context = context;
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

        public EntityReference ObjectRef => new EntityReference(WorkflowContext.PrimaryEntityName, WorkflowContext.PrimaryEntityId);

        public WorkflowModes WorkflowMode
            => (WorkflowModes)Enum.ToObject(typeof(WorkflowModes), WorkflowContext.WorkflowMode);

        public void DumpObject(string parameterName, object parameter)
        {
            LogHelper.DumpObject(parameterName, parameter);
        }

        public RemoteDebugExecutionContext RemoteContext => new RemoteDebugExecutionContext(WorkflowContext);
    }
}
