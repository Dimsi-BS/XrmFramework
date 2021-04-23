using Microsoft.Xrm.Sdk.Workflow;
using XrmFramework.RemoteDebugger;

// ReSharper disable once CheckNamespace
namespace XrmFramework.Workflow
{
    partial class LocalWorkflowContext : LocalContext
    {
#if !REMOTE_DEBUGGER
        public IWorkflowContext WorkflowContext { get; set; }
#endif


        public RemoteDebugExecutionContext RemoteContext => new RemoteDebugExecutionContext(WorkflowContext);
    }
}
