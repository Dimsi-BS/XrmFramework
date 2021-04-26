using Microsoft.Xrm.Sdk.Workflow;
using XrmFramework.RemoteDebugger;

// ReSharper disable once CheckNamespace
namespace XrmFramework.Workflow
{
    partial class LocalWorkflowContext : LocalContext
    {
        public RemoteDebugExecutionContext RemoteContext => new RemoteDebugExecutionContext(WorkflowContext);
    }
}
