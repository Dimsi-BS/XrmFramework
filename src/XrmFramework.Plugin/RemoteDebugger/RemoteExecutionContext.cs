using Microsoft.Xrm.Sdk;
#if !NET8_0_OR_GREATER
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
#endif
using Newtonsoft.Json;
using System;

namespace XrmFramework.RemoteDebugger;

[JsonObject(MemberSerialization.OptOut)]
public class RemoteDebugExecutionContext : IPluginExecutionContext
#if !NET8_0_OR_GREATER
    , IWorkflowContext
#endif
{
    public RemoteDebugExecutionContext() { }

    public RemoteDebugExecutionContext(IExecutionContext context)
    {
        Mode = context.Mode;
        IsolationMode = context.IsolationMode;
        Depth = context.Depth;
        MessageName = context.MessageName;
        PrimaryEntityName = context.PrimaryEntityName;
        RequestId = context.RequestId;
        SecondaryEntityName = context.SecondaryEntityName;
        InputParameters = context.InputParameters;
        OutputParameters = context.OutputParameters;
        SharedVariables = context.SharedVariables;
        UserId = context.UserId;
        InitiatingUserId = context.InitiatingUserId;
        BusinessUnitId = context.BusinessUnitId;
        OrganizationId = context.OrganizationId;
        OrganizationName = context.OrganizationName;
        PrimaryEntityId = context.PrimaryEntityId;
        PreEntityImages = context.PreEntityImages;
        PostEntityImages = context.PostEntityImages;
        OwningExtension = context.OwningExtension;
        CorrelationId = context.CorrelationId;
        IsExecutingOffline = context.IsExecutingOffline;
        IsOfflinePlayback = context.IsOfflinePlayback;
        IsInTransaction = context.IsInTransaction;
        OperationId = context.OperationId;
        OperationCreatedOn = context.OperationCreatedOn;

        
#if !NET8_0_OR_GREATER
        if (context is IWorkflowContext workflowContext)
        {

            StageName = workflowContext.StageName;
            WorkflowCategory = workflowContext.WorkflowCategory;
            WorkflowMode = workflowContext.WorkflowMode;

            IsWorkflowContext = true;

            if (workflowContext.ParentContext != null)
            {
                RemoteParentContext = new RemoteDebugExecutionContext(workflowContext.ParentContext);
            }
        }
#endif
        if (context is not IPluginExecutionContext pluginContext) return;
        
        Stage = pluginContext.Stage;

        if (pluginContext.ParentContext != null)
        {
            RemoteParentContext = new RemoteDebugExecutionContext(pluginContext.ParentContext);
        }
    }

    public bool IsWorkflowContext { get; set; }

    public int Mode { get; set; }

    public int IsolationMode { get; set; }

    public int Depth { get; set; }

    public string MessageName { get; set; }

    public string PrimaryEntityName { get; set; }

    public Guid? RequestId { get; set; }

    public string SecondaryEntityName { get; set; }

    public ParameterCollection InputParameters { get; set; }

    public ParameterCollection OutputParameters { get; set; }

    public ParameterCollection SharedVariables { get; set; }

    public Guid UserId { get; set; }

    public Guid InitiatingUserId { get; set; }

    public Guid BusinessUnitId { get; set; }

    public Guid OrganizationId { get; set; }

    public string OrganizationName { get; set; }

    public Guid PrimaryEntityId { get; set; }

    public EntityImageCollection PreEntityImages { get; set; }

    public EntityImageCollection PostEntityImages { get; set; }

    public EntityReference OwningExtension { get; set; }

    public Guid CorrelationId { get; set; }

    public bool IsExecutingOffline { get; set; }

    public bool IsOfflinePlayback { get; set; }

    public bool IsInTransaction { get; set; }

    public Guid OperationId { get; set; }

    public DateTime OperationCreatedOn { get; set; }

    public Guid Id { get; set; }

#if !NET8_0_OR_GREATER
    public ArgumentsCollection Arguments { get; set; } = [];
#endif

    public int Stage { get; set; }

    public string StageName { get; }

    public int WorkflowCategory { get; }

    public int WorkflowMode { get; }

    public RemoteDebugExecutionContext RemoteParentContext { get; set; }

    [JsonIgnore]
    IPluginExecutionContext IPluginExecutionContext.ParentContext => RemoteParentContext;

#if !NET8_0_OR_GREATER
    [JsonIgnore]
    IWorkflowContext IWorkflowContext.ParentContext => RemoteParentContext;
#endif

    public string TypeAssemblyQualifiedName { get; set; }

    public string UnsecureConfig { get; set; }

    public string SecureConfig { get; set; }
}
