using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins
{

    public sealed class LocalWorkflowContext : LocalContext, ICustomWorkflowContext
    {
        private CodeActivityContext _context;

        public IWorkflowContext WorkflowContext => (IWorkflowContext)ExecutionContext;

        public LocalWorkflowContext(CodeActivityContext context)
            : base(context)
        {
            _context = context;
        }

        T ICustomWorkflowContext.GetArgumentValue<T>(InArgument<T> argument)
        {
            return argument.Get(_context);
        }

        void ICustomWorkflowContext.SetArgumentValue<T>(OutArgument<T> argument, T value)
        {
            argument.Set(_context, value);
        }

        T ICustomWorkflowContext.GetArgumentValue<T>(InOutArgument<T> argument)
        {
            return argument.Get(_context);
        }

        void ICustomWorkflowContext.SetArgumentValue<T>(InOutArgument<T> argument, T value)
        {
            argument.Set(_context, value);
        }

        public EntityReference ObjectRef => new EntityReference(WorkflowContext.PrimaryEntityName, WorkflowContext.PrimaryEntityId);

        public WorkflowModes WorkflowMode
        {
            get { return (WorkflowModes)Enum.ToObject(typeof(WorkflowModes), WorkflowContext.WorkflowMode); }
        }

        public void DumpObject(string parameterName, object parameter)
        {
            LogHelper.DumpObject(parameterName, parameter);
        }
    }
}
