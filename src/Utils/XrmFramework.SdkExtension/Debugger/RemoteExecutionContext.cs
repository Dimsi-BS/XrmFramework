using System;
using Microsoft.Xrm.Sdk;
#if INTERNAL_NEWTONSOFT
using Newtonsoft.Json.Xrm;
#else 
using Newtonsoft.Json;
#endif

namespace XrmFramework.Debugger
{
    [Serializable]
    public class RemoteDebugExecutionContext : IExecutionContext
    {
        protected RemoteDebugExecutionContext() { }

        protected RemoteDebugExecutionContext(IExecutionContext context)
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
        }

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
    }

    [Serializable]
    public class RemoteDebugPluginExecutionContext : RemoteDebugExecutionContext, IPluginExecutionContext
    {
        public RemoteDebugPluginExecutionContext()
        {
        }

        public RemoteDebugPluginExecutionContext(IPluginExecutionContext context) : base(context)
        {
            Stage = context.Stage;

            if (context.ParentContext != null)
            {
                RemoteParentContext = new RemoteDebugPluginExecutionContext(context.ParentContext);
            }
        }

        public int Stage { get; set; }

        public RemoteDebugPluginExecutionContext RemoteParentContext { get; set; }

        [JsonIgnore]
        public IPluginExecutionContext ParentContext => RemoteParentContext;

        public Guid Id { get; set; }
    }

}
