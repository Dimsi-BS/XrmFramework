// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace XrmFramework.Workflows.Tests.Fakes
{
    /// <summary>
    /// The members <see cref="IExecutionContext" /> declares, which both kinds of context carry.
    /// </summary>
    /// <remarks>
    /// Hand-written rather than mocked: what is under test is a type test against the interface a
    /// context implements, and a mocking framework's proxy is a type of its own making — exactly
    /// the thing that must not be what the answer depends on.
    /// </remarks>
    public abstract class FakeExecutionContext : IExecutionContext
    {
        public int Mode { get; set; }
        public int IsolationMode { get; set; }
        public int Depth { get; set; }
        public string MessageName { get; set; } = "Create";
        public string PrimaryEntityName { get; set; } = "account";
        public Guid? RequestId { get; set; }
        public string SecondaryEntityName { get; set; } = string.Empty;
        public ParameterCollection InputParameters { get; set; } = new();
        public ParameterCollection OutputParameters { get; set; } = new();
        public ParameterCollection SharedVariables { get; set; } = new();
        public Guid UserId { get; set; }
        public Guid InitiatingUserId { get; set; }
        public Guid BusinessUnitId { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = "contoso";
        public Guid PrimaryEntityId { get; set; }
        public EntityImageCollection PreEntityImages { get; set; } = new();
        public EntityImageCollection PostEntityImages { get; set; } = new();
        public EntityReference OwningExtension { get; set; } = new("sdkmessageprocessingstep", Guid.NewGuid());
        public Guid CorrelationId { get; set; }
        public bool IsExecutingOffline { get; set; }
        public bool IsOfflinePlayback { get; set; }
        public bool IsInTransaction { get; set; }
        public Guid OperationId { get; set; }
        public DateTime OperationCreatedOn { get; set; }
    }

    /// <summary>
    /// A workflow context, as the CRM hands one to a custom workflow activity.
    /// </summary>
    /// <remarks>
    /// The class name is deliberately not <c>IWorkflowContext</c>: the constructor under test used
    /// to recognise a workflow context by <c>GetType().Name == "IWorkflowContext"</c>, and the
    /// CRM's own implementation is a concrete class whose name is nothing of the sort. A fake named
    /// to satisfy that comparison would have passed against the bug.
    /// </remarks>
    public class FakeWorkflowContext : FakeExecutionContext, IWorkflowContext
    {
        public string StageName { get; set; } = "Both";
        public int WorkflowCategory { get; set; }
        public int WorkflowMode { get; set; }
        public IWorkflowContext? ParentContext { get; set; }
    }

    /// <summary>A plugin context, which must keep taking the plugin branch.</summary>
    public class FakePluginContext : FakeExecutionContext, IPluginExecutionContext
    {
        public int Stage { get; set; } = 40;
        public IPluginExecutionContext? ParentContext { get; set; }
    }
}
