using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Newtonsoft.Json;

namespace XrmFramework.RemoteDebugger;

partial class RemoteDebugExecutionContext : IWorkflowContext
{
    public ArgumentsCollection Arguments { get; set; } = new();

    [JsonIgnore]
    IWorkflowContext IWorkflowContext.ParentContext => RemoteParentContext;

    partial void InitRemoteWorkflowContext(IExecutionContext context)
    {
        if (context is IWorkflowContext workflowContext){
            StageName = workflowContext.StageName;
            WorkflowCategory = workflowContext.WorkflowCategory;
            WorkflowMode = workflowContext.WorkflowMode;

            IsWorkflowContext = true;

            if (workflowContext.ParentContext != null)
            {
                RemoteParentContext = new RemoteDebugExecutionContext(workflowContext.ParentContext);
            }
        }
    }
}