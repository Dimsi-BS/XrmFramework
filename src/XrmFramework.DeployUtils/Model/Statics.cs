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


    public partial class Messages
    {
        public static Messages AddItem = new("AddItem");
        public static Messages AddListMembers = new("AddListMembers");
        public static Messages AddMember = new("AddMember");
        public static Messages AddMembers = new("AddMembers");
        public static Messages AddPrivileges = new("AddPrivileges");
        public static Messages AddProductToKit = new("AddProductToKit");
        public static Messages AddRecurrence = new("AddRecurrence");
        public static Messages AddToQueue = new("AddToQueue");
        public static Messages AddUserToRecordTeam = new("AddUserToRecordTeam");
        public static Messages Assign = new("Assign");
        public static Messages AssignUserRoles = new("AssignUserRoles");
        public static Messages Associate = new("Associate");
        public static Messages BackgroundSend = new("BackgroundSend");
        public static Messages Book = new("Book");
        public static Messages Cancel = new("Cancel");
        public static Messages CheckIncoming = new("CheckIncoming");
        public static Messages CheckPromote = new("CheckPromote");
        public static Messages Clone = new("Clone");
        public static Messages Close = new("Close");
        public static Messages CopyDynamicListToStatic = new("CopyDynamicListToStatic");
        public static Messages CopySystemForm = new("CopySystemForm");
        public static Messages Create = new("Create");
        public static Messages CreateException = new("CreateException");
        public static Messages CreateInstance = new("CreateInstance");
        public static Messages Delete = new("Delete");
        public static Messages DeleteOpenInstances = new("DeleteOpenInstances");
        public static Messages DeliverIncoming = new("DeliverIncoming");
        public static Messages DeliverPromote = new("DeliverPromote");
        public static Messages DetachFromQueue = new("DetachFromQueue");
        public static Messages Disassociate = new("Disassociate");
        public static Messages Execute = new("Execute");
        public static Messages ExecuteById = new("ExecuteById");
        public static Messages Export = new("Export");
        public static Messages ExportAll = new("ExportAll");
        public static Messages ExportCompressed = new("ExportCompressed");
        public static Messages ExportCompressedAll = new("ExportCompressedAll");
        public static Messages GrantAccess = new("GrantAccess");
        public static Messages Handle = new("Handle");
        public static Messages Import = new("Import");
        public static Messages ImportAll = new("ImportAll");
        public static Messages ImportCompressedAll = new("ImportCompressedAll");
        public static Messages ImportCompressedWithProgress = new("ImportCompressedWithProgress");
        public static Messages ImportWithProgress = new("ImportWithProgress");
        public static Messages LockInvoicePricing = new("LockInvoicePricing");
        public static Messages LockSalesOrderPricing = new("LockSalesOrderPricing");
        public static Messages Lose = new("Lose");
        public static Messages Merge = new("Merge");
        public static Messages ModifyAccess = new("ModifyAccess");
        public static Messages Publish = new("Publish");
        public static Messages PublishAll = new("PublishAll");
        public static Messages QualifyLead = new("QualifyLead");
        public static Messages Recalculate = new("Recalculate");
        public static Messages RemoveItem = new("RemoveItem");
        public static Messages RemoveMember = new("RemoveMember");
        public static Messages RemoveMembers = new("RemoveMembers");
        public static Messages RemovePrivilege = new("RemovePrivilege");
        public static Messages RemoveProductFromKit = new("RemoveProductFromKit");
        public static Messages RemoveRelated = new("RemoveRelated");
        public static Messages RemoveUserFromRecordTeam = new("RemoveUserFromRecordTeam");
        public static Messages RemoveUserRoles = new("RemoveUserRoles");
        public static Messages ReplacePrivileges = new("ReplacePrivileges");
        public static Messages Reschedule = new("Reschedule");
        public static Messages Retrieve = new("Retrieve");
        public static Messages RetrieveExchangeRate = new("RetrieveExchangeRate");
        public static Messages RetrieveFilteredForms = new("RetrieveFilteredForms");
        public static Messages RetrieveMultiple = new("RetrieveMultiple");
        public static Messages RetrievePersonalWall = new("RetrievePersonalWall");
        public static Messages RetrievePrincipalAccess = new("RetrievePrincipalAccess");
        public static Messages RetrieveRecordWall = new("RetrieveRecordWall");
        public static Messages RetrieveSharedPrincipalsAndAccess = new("RetrieveSharedPrincipalsAndAccess");
        public static Messages RetrieveUnpublished = new("RetrieveUnpublished");
        public static Messages RetrieveUnpublishedMultiple = new("RetrieveUnpublishedMultiple");
        public static Messages RevokeAccess = new("RevokeAccess");
        public static Messages Route = new("Route");
        public static Messages Send = new("Send");
        public static Messages SendFromTemplate = new("SendFromTemplate");
        public static Messages SetRelated = new("SetRelated");
        [Obsolete("Use Update message with state filtering attribute instead")]
        public static Messages SetState = new("SetState");
        [Obsolete("Use Update message with state filtering attribute instead")]
        public static Messages SetStateDynamicEntity = new("SetStateDynamicEntity");
        public static Messages TriggerServiceEndpointCheck = new("TriggerServiceEndpointCheck");
        public static Messages UnlockInvoicePricing = new("UnlockInvoicePricing");
        public static Messages UnlockSalesOrderPricing = new("UnlockSalesOrderPricing");
        public static Messages Update = new("Update");
        public static Messages ValidateRecurrenceRule = new("ValidateRecurrenceRule");
        public static Messages Win = new("Win");
        public static Messages ExecuteWorkflow = new("ExecuteWorkflow");
        public static Messages Default = new("Default");

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