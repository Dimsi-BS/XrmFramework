using System;

namespace Plugins
{
    public delegate void Logger(string methodName, string message, params object[] args);

    public delegate void TraceLogger(string message, params object[] args);

    public enum Stages
    {
        PreValidation = 10,
        PreOperation = 20,
        PostOperation = 40
    }

    public enum Modes
    {
        Synchronous = 0,
        Asynchronous = 1
    }

    public enum WorkflowModes
    {
        Asynchronous = 0,
        RealTime = 1
    }

    public enum InputParameters
    {
        Assignee,
        EntityMoniker,
        OpportunityClose,
        Query,
        Record,
        RelatedEntities,
        Relationship,
        State,
        Status,
        SystemUserId,
        Target,
        TeamTemplateId,
        /// <summary>
        /// Used by AddRecurrence Message
        /// </summary>
        AppointmentId,
        FetchXml
    }

    public enum OutputParameters
    {
        BusinessEntityCollection,
        ValidationResult
    }

    public enum Messages
    {
        AddItem,
        AddListMembers,
        AddMember,
        AddMembers,
        AddPrivileges,
        AddProductToKit,
        AddRecurrence,
        AddToQueue,
        AddUserToRecordTeam,
        Assign,
        AssignUserRoles,
        Associate,
        BackgroundSend,
        Book,
        Cancel,
        CheckIncoming,
        CheckPromote,
        Clone,
        Close,
        CopyDynamicListToStatic,
        CopySystemForm,
        Create,
        CreateException,
        CreateInstance,
        Delete,
        DeleteOpenInstances,
        DeliverIncoming,
        DeliverPromote,
        DetachFromQueue,
        Disassociate,
        Execute,
        ExecuteById,
        Export,
        ExportAll,
        ExportCompressed,
        ExportCompressedAll,
        GrantAccess,
        Handle,
        Import,
        ImportAll,
        ImportCompressedAll,
        ImportCompressedWithProgress,
        ImportWithProgress,
        LockInvoicePricing,
        LockSalesOrderPricing,
        Lose,
        Merge,
        ModifyAccess,
        Publish,
        PublishAll,
        QualifyLead,
        Recalculate,
        RemoveItem,
        RemoveMember,
        RemoveMembers,
        RemovePrivilege,
        RemoveProductFromKit,
        RemoveRelated,
        RemoveUserFromRecordTeam,
        RemoveUserRoles,
        ReplacePrivileges,
        Reschedule,
        Retrieve,
        RetrieveExchangeRate,
        RetrieveFilteredForms,
        RetrieveMultiple,
        RetrievePersonalWall,
        RetrievePrincipalAccess,
        RetrieveRecordWall,
        RetrieveSharedPrincipalsAndAccess,
        RetrieveUnpublished,
        RetrieveUnpublishedMultiple,
        RevokeAccess,
        Route,
        Send,
        SendFromTemplate,
        SetRelated,
        [Obsolete("Use Update message with state filtering attribute instead")]
        SetState,
        [Obsolete("Use Update message with state filtering attribute instead")]
        SetStateDynamicEntity,
        TriggerServiceEndpointCheck,
        UnlockInvoicePricing,
        UnlockSalesOrderPricing,
        Update,
        ValidateRecurrenceRule,
        Win,
        ExecuteWorkflow,
        Default
        , pchmcs_ExecuteFetchXml
    }
}