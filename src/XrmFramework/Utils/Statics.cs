// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
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

    // ReSharper disable once PartialTypeWithSinglePart
    public partial class InputParameters : IEquatable<InputParameters>
    {
        protected string ParameterName { get; }

        protected InputParameters(string inputParameterName)
        {
            ParameterName = inputParameterName;
        }

        public override string ToString() => ParameterName;

        public static InputParameters Assignee { get; } = new InputParameters("Assignee");
        public static InputParameters EntityMoniker { get; } = new InputParameters("EntityMoniker");
        public static InputParameters OpportunityClose { get; } = new InputParameters("OpportunityClose");
        public static InputParameters Query { get; } = new InputParameters("Query");
        public static InputParameters Record { get; } = new InputParameters("Record");
        public static InputParameters RelatedEntities { get; } = new InputParameters("RelatedEntities");
        public static InputParameters Relationship { get; } = new InputParameters("Relationship");
        public static InputParameters State { get; } = new InputParameters("State");
        public static InputParameters Status { get; } = new InputParameters("Status");
        public static InputParameters SystemUserId { get; } = new InputParameters("SystemUserId");
        public static InputParameters Target { get; } = new InputParameters("Target");
        public static InputParameters TeamTemplateId { get; } = new InputParameters("TeamTemplateId");
        public static InputParameters AppointmentId { get; } = new InputParameters("AppointmentId");
        public static InputParameters FetchXml { get; } = new InputParameters("FetchXml");
        public static InputParameters SubordinateId { get; } = new InputParameters("SubordinateId");
        public static InputParameters UpdateContent { get; } = new InputParameters("UpdateContent");

        /// <inheritdoc />
        public bool Equals(InputParameters other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(ParameterName, other.ParameterName, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InputParameters)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (ParameterName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(ParameterName) : 0);
        }
    }

    // ReSharper disable once PartialTypeWithSinglePart
    public partial class OutputParameters : IEquatable<OutputParameters>
    {
        protected string ParameterName { get; }

        protected OutputParameters(string outputParameterName)
        {
            ParameterName = outputParameterName;
        }

        public override string ToString() => ParameterName;

        public override int GetHashCode()
        {
            return (ParameterName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(ParameterName) : 0);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OutputParameters)obj);
        }

        public static OutputParameters BusinessEntityCollection { get; } = new OutputParameters("BusinessEntityCollection");
        public static OutputParameters ValidationResult { get; } = new OutputParameters("ValidationResult");

        /// <inheritdoc />
        public bool Equals(OutputParameters other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(ParameterName, other.ParameterName, StringComparison.OrdinalIgnoreCase);
        }
    }

    // ReSharper disable once PartialTypeWithSinglePart
    public partial class Messages : IEquatable<Messages>
    {
        public static Messages AddItem { get; } = new Messages("AddItem");
        public static Messages AddListMembers { get; } = new Messages("AddListMembers");
        public static Messages AddMember { get; } = new Messages("AddMember");
        public static Messages AddMembers { get; } = new Messages("AddMembers");
        public static Messages AddPrivileges { get; } = new Messages("AddPrivileges");
        public static Messages AddProductToKit { get; } = new Messages("AddProductToKit");
        public static Messages AddRecurrence { get; } = new Messages("AddRecurrence");
        public static Messages AddToQueue { get; } = new Messages("AddToQueue");
        public static Messages AddUserToRecordTeam { get; } = new Messages("AddUserToRecordTeam");
        public static Messages Assign { get; } = new Messages("Assign");
        public static Messages AssignUserRoles { get; } = new Messages("AssignUserRoles");
        public static Messages Associate { get; } = new Messages("Associate");
        public static Messages BackgroundSend { get; } = new Messages("BackgroundSend");
        public static Messages Book { get; } = new Messages("Book");
        public static Messages Cancel { get; } = new Messages("Cancel");
        public static Messages CheckIncoming { get; } = new Messages("CheckIncoming");
        public static Messages CheckPromote { get; } = new Messages("CheckPromote");
        public static Messages Clone { get; } = new Messages("Clone");
        public static Messages Close { get; } = new Messages("Close");
        public static Messages CopyDynamicListToStatic { get; } = new Messages("CopyDynamicListToStatic");
        public static Messages CopySystemForm { get; } = new Messages("CopySystemForm");
        public static Messages Create { get; } = new Messages("Create");
        public static Messages CreateException { get; } = new Messages("CreateException");
        public static Messages CreateInstance { get; } = new Messages("CreateInstance");
        public static Messages Delete { get; } = new Messages("Delete");
        public static Messages DeleteOpenInstances { get; } = new Messages("DeleteOpenInstances");
        public static Messages DeliverIncoming { get; } = new Messages("DeliverIncoming");
        public static Messages DeliverPromote { get; } = new Messages("DeliverPromote");
        public static Messages DetachFromQueue { get; } = new Messages("DetachFromQueue");
        public static Messages Disassociate { get; } = new Messages("Disassociate");
        public static Messages Execute { get; } = new Messages("Execute");
        public static Messages ExecuteById { get; } = new Messages("ExecuteById");
        public static Messages Export { get; } = new Messages("Export");
        public static Messages ExportAll { get; } = new Messages("ExportAll");
        public static Messages ExportCompressed { get; } = new Messages("ExportCompressed");
        public static Messages ExportCompressedAll { get; } = new Messages("ExportCompressedAll");
        public static Messages GrantAccess { get; } = new Messages("GrantAccess");
        public static Messages Handle { get; } = new Messages("Handle");
        public static Messages Import { get; } = new Messages("Import");
        public static Messages ImportAll { get; } = new Messages("ImportAll");
        public static Messages ImportCompressedAll { get; } = new Messages("ImportCompressedAll");
        public static Messages ImportCompressedWithProgress { get; } = new Messages("ImportCompressedWithProgress");
        public static Messages ImportWithProgress { get; } = new Messages("ImportWithProgress");
        public static Messages LockInvoicePricing { get; } = new Messages("LockInvoicePricing");
        public static Messages LockSalesOrderPricing { get; } = new Messages("LockSalesOrderPricing");
        public static Messages Lose { get; } = new Messages("Lose");
        public static Messages Merge { get; } = new Messages("Merge");
        public static Messages ModifyAccess { get; } = new Messages("ModifyAccess");
        public static Messages Publish { get; } = new Messages("Publish");
        public static Messages PublishAll { get; } = new Messages("PublishAll");
        public static Messages QualifyLead { get; } = new Messages("QualifyLead");
        public static Messages Recalculate { get; } = new Messages("Recalculate");
        public static Messages RemoveItem { get; } = new Messages("RemoveItem");
        public static Messages RemoveMember { get; } = new Messages("RemoveMember");
        public static Messages RemoveMembers { get; } = new Messages("RemoveMembers");
        public static Messages RemovePrivilege { get; } = new Messages("RemovePrivilege");
        public static Messages RemoveProductFromKit { get; } = new Messages("RemoveProductFromKit");
        public static Messages RemoveRelated { get; } = new Messages("RemoveRelated");
        public static Messages RemoveUserFromRecordTeam { get; } = new Messages("RemoveUserFromRecordTeam");
        public static Messages RemoveUserRoles { get; } = new Messages("RemoveUserRoles");
        public static Messages ReplacePrivileges { get; } = new Messages("ReplacePrivileges");
        public static Messages Reschedule { get; } = new Messages("Reschedule");
        public static Messages Retrieve { get; } = new Messages("Retrieve");
        public static Messages RetrieveExchangeRate { get; } = new Messages("RetrieveExchangeRate");
        public static Messages RetrieveFilteredForms { get; } = new Messages("RetrieveFilteredForms");
        public static Messages RetrieveMultiple { get; } = new Messages("RetrieveMultiple");
        public static Messages RetrievePersonalWall { get; } = new Messages("RetrievePersonalWall");
        public static Messages RetrievePrincipalAccess { get; } = new Messages("RetrievePrincipalAccess");
        public static Messages RetrieveRecordWall { get; } = new Messages("RetrieveRecordWall");
        public static Messages RetrieveSharedPrincipalsAndAccess { get; } = new Messages("RetrieveSharedPrincipalsAndAccess");
        public static Messages RetrieveUnpublished { get; } = new Messages("RetrieveUnpublished");
        public static Messages RetrieveUnpublishedMultiple { get; } = new Messages("RetrieveUnpublishedMultiple");
        public static Messages RevokeAccess { get; } = new Messages("RevokeAccess");
        public static Messages Route { get; } = new Messages("Route");
        public static Messages Send { get; } = new Messages("Send");
        public static Messages SendFromTemplate { get; } = new Messages("SendFromTemplate");
        public static Messages SetRelated { get; } = new Messages("SetRelated");
        [Obsolete("Use Update message with state filtering attribute instead")]
        public static Messages SetState { get; } = new Messages("SetState");
        [Obsolete("Use Update message with state filtering attribute instead")]
        public static Messages SetStateDynamicEntity { get; } = new Messages("SetStateDynamicEntity");
        public static Messages TriggerServiceEndpointCheck { get; } = new Messages("TriggerServiceEndpointCheck");
        public static Messages UnlockInvoicePricing { get; } = new Messages("UnlockInvoicePricing");
        public static Messages UnlockSalesOrderPricing { get; } = new Messages("UnlockSalesOrderPricing");
        public static Messages Update { get; } = new Messages("Update");
        public static Messages ValidateRecurrenceRule { get; } = new Messages("ValidateRecurrenceRule");
        public static Messages Win { get; } = new Messages("Win");
        public static Messages ExecuteWorkflow { get; } = new Messages("ExecuteWorkflow");
        public static Messages Default { get; } = new Messages("Default");

        protected string MessageName { get; }

        protected Messages(string messageName)
        {
            MessageName = messageName;
        }

        public override string ToString() => MessageName;

        public static Messages GetMessage(string messageName)
        {
            if (string.IsNullOrEmpty(messageName))
            {
                return Default;
            }

            return new Messages(messageName);
        }

        /// <inheritdoc />
        public bool Equals(Messages other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(MessageName, other.MessageName, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Messages)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (MessageName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(MessageName) : 0);
        }
    }
}