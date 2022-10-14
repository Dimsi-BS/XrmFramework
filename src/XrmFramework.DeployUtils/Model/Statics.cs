// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework.DeployUtils.Model
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

    public enum InputParameters
    {
        Assignee,
        EntityMoniker,
        Query,
        RelatedEntities,
        Relationship,
        State,
        Status,
        Target
    }

    public enum OutputParameters
    {
        BusinessEntityCollection
    }


    // ReSharper disable once PartialTypeWithSinglePart
    public partial class Messages
    {
        public static Messages AddItem { get; } = new("AddItem");
        public static Messages AddListMembers { get; } = new("AddListMembers");
        public static Messages AddMember { get; } = new("AddMember");
        public static Messages AddMembers { get; } = new("AddMembers");
        public static Messages AddPrivileges { get; } = new("AddPrivileges");
        public static Messages AddProductToKit { get; } = new("AddProductToKit");
        public static Messages AddRecurrence { get; } = new("AddRecurrence");
        public static Messages AddToQueue { get; } = new("AddToQueue");
        public static Messages AddUserToRecordTeam { get; } = new("AddUserToRecordTeam");
        public static Messages Assign { get; } = new("Assign");
        public static Messages AssignUserRoles { get; } = new("AssignUserRoles");
        public static Messages Associate { get; } = new("Associate");
        public static Messages BackgroundSend { get; } = new("BackgroundSend");
        public static Messages Book { get; } = new("Book");
        public static Messages Cancel { get; } = new("Cancel");
        public static Messages CheckIncoming { get; } = new("CheckIncoming");
        public static Messages CheckPromote { get; } = new("CheckPromote");
        public static Messages Clone { get; } = new("Clone");
        public static Messages Close { get; } = new("Close");
        public static Messages CopyDynamicListToStatic { get; } = new("CopyDynamicListToStatic");
        public static Messages CopySystemForm { get; } = new("CopySystemForm");
        public static Messages Create { get; } = new("Create");
        public static Messages CreateException { get; } = new("CreateException");
        public static Messages CreateInstance { get; } = new("CreateInstance");
        public static Messages Delete { get; } = new("Delete");
        public static Messages DeleteOpenInstances { get; } = new("DeleteOpenInstances");
        public static Messages DeliverIncoming { get; } = new("DeliverIncoming");
        public static Messages DeliverPromote { get; } = new("DeliverPromote");
        public static Messages DetachFromQueue { get; } = new("DetachFromQueue");
        public static Messages Disassociate { get; } = new("Disassociate");
        public static Messages Execute { get; } = new("Execute");
        public static Messages ExecuteById { get; } = new("ExecuteById");
        public static Messages Export { get; } = new("Export");
        public static Messages ExportAll { get; } = new("ExportAll");
        public static Messages ExportCompressed { get; } = new("ExportCompressed");
        public static Messages ExportCompressedAll { get; } = new("ExportCompressedAll");
        public static Messages GrantAccess { get; } = new("GrantAccess");
        public static Messages Handle { get; } = new("Handle");
        public static Messages Import { get; } = new("Import");
        public static Messages ImportAll { get; } = new("ImportAll");
        public static Messages ImportCompressedAll { get; } = new("ImportCompressedAll");
        public static Messages ImportCompressedWithProgress { get; } = new("ImportCompressedWithProgress");
        public static Messages ImportWithProgress { get; } = new("ImportWithProgress");
        public static Messages LockInvoicePricing { get; } = new("LockInvoicePricing");
        public static Messages LockSalesOrderPricing { get; } = new("LockSalesOrderPricing");
        public static Messages Lose { get; } = new("Lose");
        public static Messages Merge { get; } = new("Merge");
        public static Messages ModifyAccess { get; } = new("ModifyAccess");
        public static Messages Publish { get; } = new("Publish");
        public static Messages PublishAll { get; } = new("PublishAll");
        public static Messages QualifyLead { get; } = new("QualifyLead");
        public static Messages Recalculate { get; } = new("Recalculate");
        public static Messages RemoveItem { get; } = new("RemoveItem");
        public static Messages RemoveMember { get; } = new("RemoveMember");
        public static Messages RemoveMembers { get; } = new("RemoveMembers");
        public static Messages RemovePrivilege { get; } = new("RemovePrivilege");
        public static Messages RemoveProductFromKit { get; } = new("RemoveProductFromKit");
        public static Messages RemoveRelated { get; } = new("RemoveRelated");
        public static Messages RemoveUserFromRecordTeam { get; } = new("RemoveUserFromRecordTeam");
        public static Messages RemoveUserRoles { get; } = new("RemoveUserRoles");
        public static Messages ReplacePrivileges { get; } = new("ReplacePrivileges");
        public static Messages Reschedule { get; } = new("Reschedule");
        public static Messages Retrieve { get; } = new("Retrieve");
        public static Messages RetrieveExchangeRate { get; } = new("RetrieveExchangeRate");
        public static Messages RetrieveFilteredForms { get; } = new("RetrieveFilteredForms");
        public static Messages RetrieveMultiple { get; } = new("RetrieveMultiple");
        public static Messages RetrievePersonalWall { get; } = new("RetrievePersonalWall");
        public static Messages RetrievePrincipalAccess { get; } = new("RetrievePrincipalAccess");
        public static Messages RetrieveRecordWall { get; } = new("RetrieveRecordWall");
        public static Messages RetrieveSharedPrincipalsAndAccess { get; } = new("RetrieveSharedPrincipalsAndAccess");
        public static Messages RetrieveUnpublished { get; } = new("RetrieveUnpublished");
        public static Messages RetrieveUnpublishedMultiple { get; } = new("RetrieveUnpublishedMultiple");
        public static Messages RevokeAccess { get; } = new("RevokeAccess");
        public static Messages Route { get; } = new("Route");
        public static Messages Send { get; } = new("Send");
        public static Messages SendFromTemplate { get; } = new("SendFromTemplate");
        public static Messages SetRelated { get; } = new("SetRelated");
        [Obsolete("Use Update message with state filtering attribute instead")]
        public static Messages SetState { get; } = new("SetState");
        [Obsolete("Use Update message with state filtering attribute instead")]
        public static Messages SetStateDynamicEntity { get; } = new("SetStateDynamicEntity");
        public static Messages TriggerServiceEndpointCheck { get; } = new("TriggerServiceEndpointCheck");
        public static Messages UnlockInvoicePricing { get; } = new("UnlockInvoicePricing");
        public static Messages UnlockSalesOrderPricing { get; } = new("UnlockSalesOrderPricing");
        public static Messages Update { get; } = new("Update");
        public static Messages ValidateRecurrenceRule { get; } = new("ValidateRecurrenceRule");
        public static Messages Win { get; } = new("Win");
        public static Messages ExecuteWorkflow { get; } = new("ExecuteWorkflow");
        public static Messages Default { get; } = new("Default");

        protected string MessageName { get; }

        protected Messages(string messageName)
        {
            MessageName = messageName;
        }

        public static bool operator ==(Messages x, Messages y) => x?.MessageName == y?.MessageName;

        public static bool operator !=(Messages x, Messages y) => !(x == y);

        public override string ToString() => MessageName;

        public override int GetHashCode() => MessageName.GetHashCode();

        public override bool Equals(object obj) => obj is Messages message && message.MessageName == MessageName;

        public static Messages GetMessage(string messageName)
        {
            if (string.IsNullOrEmpty(messageName))
            {
                return Default;
            }

            return new Messages(messageName);
        }
    }
}