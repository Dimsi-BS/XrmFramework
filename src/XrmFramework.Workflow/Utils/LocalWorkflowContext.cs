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

        public LocalWorkflowContext(CodeActivityContext context)

#if !STANDALONE
            : base(context)
#endif

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


        public void LogCollection(Dictionary<string, object> list)
        {
            Logger.LogCollection(list);
        }

#if STANDALONE


        internal void DumpSharedVariables()
        {
            throw new NotImplementedException();
        }

        internal void DumpInputParameters()
        {
            throw new NotImplementedException();
        }

        public Guid UserId => throw new NotImplementedException();

        public EntityReference UserRef => throw new NotImplementedException();

        public Guid CorrelationId => throw new NotImplementedException();

        public void Log(string message, params object[] paramsObject)
        {
            throw new NotImplementedException();
        }

        internal void LogError(Exception innerException)
        {
            throw new NotImplementedException();
        }

        public T GetService<T>(Type parameterType) where T : IService
        {
            throw new NotImplementedException();
        }

        internal void DumpLog()
        {
            throw new NotImplementedException();
        }

        public T GetService<T>() where T : IService
        {
            throw new NotImplementedException();
        }

        public object GetService(Type type)
        {
            throw new NotImplementedException();
        }
#endif
    }
}