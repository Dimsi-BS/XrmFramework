// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    public delegate void LogServiceMethod(string methodName, string message, params object[] args);

    public delegate void LogMethod(string message, params object[] args);

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

    [EnumGeneration]
    public partial class InputParameters
    {
        protected string ParameterName { get; }

        protected InputParameters(string inputParameterName)
        {
            ParameterName = inputParameterName;
        }

        public override string ToString() => ParameterName;

        public override int GetHashCode() => ParameterName.GetHashCode();

        public override bool Equals(object obj) => obj is InputParameters message && message.ParameterName == ParameterName;

        public static InputParameters Assignee = new InputParameters("Assignee");
        public static InputParameters EntityMoniker = new InputParameters("EntityMoniker");
        public static InputParameters OpportunityClose = new InputParameters("OpportunityClose");
        public static InputParameters Query = new InputParameters("Query");
        public static InputParameters Record = new InputParameters("Record");
        public static InputParameters RelatedEntities = new InputParameters("RelatedEntities");
        public static InputParameters Relationship = new InputParameters("Relationship");
        public static InputParameters State = new InputParameters("State");
        public static InputParameters Status = new InputParameters("Status");
        public static InputParameters SystemUserId = new InputParameters("SystemUserId");
        public static InputParameters Target = new InputParameters("Target");
        public static InputParameters TeamTemplateId = new InputParameters("TeamTemplateId");
        public static InputParameters AppointmentId = new InputParameters("AppointmentId");
        public static InputParameters FetchXml = new InputParameters("FetchXml");
        public static InputParameters SubordinateId = new InputParameters("SubordinateId");
        public static InputParameters UpdateContent = new InputParameters("UpdateContent");
    }

    [EnumGeneration]
    public partial class OutputParameters
    {
        protected string ParameterName { get; }

        protected OutputParameters(string outputParameterName)
        {
            ParameterName = outputParameterName;
        }

        public override string ToString() => ParameterName;

        public override int GetHashCode() => ParameterName.GetHashCode();

        public override bool Equals(object obj) => obj is OutputParameters message && message.ParameterName == ParameterName;

        public static OutputParameters BusinessEntityCollection = new OutputParameters("BusinessEntityCollection");
        public static OutputParameters ValidationResult = new OutputParameters("ValidationResult");
    }

    [EnumGeneration]
    public partial class Messages
    {
        public static Messages AddItem = new Messages("AddItem");
        public static Messages AddListMembers = new Messages("AddListMembers");
        public static Messages AddMember = new Messages("AddMember");
        public static Messages AddMembers = new Messages("AddMembers");
        public static Messages AddPrivileges = new Messages("AddPrivileges");
        public static Messages AddProductToKit = new Messages("AddProductToKit");
        public static Messages AddRecurrence = new Messages("AddRecurrence");
        public static Messages AddToQueue = new Messages("AddToQueue");
        public static Messages AddUserToRecordTeam = new Messages("AddUserToRecordTeam");
        public static Messages Assign = new Messages("Assign");
        public static Messages AssignUserRoles = new Messages("AssignUserRoles");
        public static Messages Associate = new Messages("Associate");
        public static Messages BackgroundSend = new Messages("BackgroundSend");
        public static Messages Book = new Messages("Book");
        public static Messages Cancel = new Messages("Cancel");
        public static Messages CheckIncoming = new Messages("CheckIncoming");
        public static Messages CheckPromote = new Messages("CheckPromote");
        public static Messages Clone = new Messages("Clone");
        public static Messages Close = new Messages("Close");
        public static Messages CopyDynamicListToStatic = new Messages("CopyDynamicListToStatic");
        public static Messages CopySystemForm = new Messages("CopySystemForm");
        public static Messages Create = new Messages("Create");
        public static Messages CreateException = new Messages("CreateException");
        public static Messages CreateInstance = new Messages("CreateInstance");
        public static Messages Delete = new Messages("Delete");
        public static Messages DeleteOpenInstances = new Messages("DeleteOpenInstances");
        public static Messages DeliverIncoming = new Messages("DeliverIncoming");
        public static Messages DeliverPromote = new Messages("DeliverPromote");
        public static Messages DetachFromQueue = new Messages("DetachFromQueue");
        public static Messages Disassociate = new Messages("Disassociate");
        public static Messages Execute = new Messages("Execute");
        public static Messages ExecuteById = new Messages("ExecuteById");
        public static Messages Export = new Messages("Export");
        public static Messages ExportAll = new Messages("ExportAll");
        public static Messages ExportCompressed = new Messages("ExportCompressed");
        public static Messages ExportCompressedAll = new Messages("ExportCompressedAll");
        public static Messages GrantAccess = new Messages("GrantAccess");
        public static Messages Handle = new Messages("Handle");
        public static Messages Import = new Messages("Import");
        public static Messages ImportAll = new Messages("ImportAll");
        public static Messages ImportCompressedAll = new Messages("ImportCompressedAll");
        public static Messages ImportCompressedWithProgress = new Messages("ImportCompressedWithProgress");
        public static Messages ImportWithProgress = new Messages("ImportWithProgress");
        public static Messages LockInvoicePricing = new Messages("LockInvoicePricing");
        public static Messages LockSalesOrderPricing = new Messages("LockSalesOrderPricing");
        public static Messages Lose = new Messages("Lose");
        public static Messages Merge = new Messages("Merge");
        public static Messages ModifyAccess = new Messages("ModifyAccess");
        public static Messages Publish = new Messages("Publish");
        public static Messages PublishAll = new Messages("PublishAll");
        public static Messages QualifyLead = new Messages("QualifyLead");
        public static Messages Recalculate = new Messages("Recalculate");
        public static Messages RemoveItem = new Messages("RemoveItem");
        public static Messages RemoveMember = new Messages("RemoveMember");
        public static Messages RemoveMembers = new Messages("RemoveMembers");
        public static Messages RemovePrivilege = new Messages("RemovePrivilege");
        public static Messages RemoveProductFromKit = new Messages("RemoveProductFromKit");
        public static Messages RemoveRelated = new Messages("RemoveRelated");
        public static Messages RemoveUserFromRecordTeam = new Messages("RemoveUserFromRecordTeam");
        public static Messages RemoveUserRoles = new Messages("RemoveUserRoles");
        public static Messages ReplacePrivileges = new Messages("ReplacePrivileges");
        public static Messages Reschedule = new Messages("Reschedule");
        public static Messages Retrieve = new Messages("Retrieve");
        public static Messages RetrieveExchangeRate = new Messages("RetrieveExchangeRate");
        public static Messages RetrieveFilteredForms = new Messages("RetrieveFilteredForms");
        public static Messages RetrieveMultiple = new Messages("RetrieveMultiple");
        public static Messages RetrievePersonalWall = new Messages("RetrievePersonalWall");
        public static Messages RetrievePrincipalAccess = new Messages("RetrievePrincipalAccess");
        public static Messages RetrieveRecordWall = new Messages("RetrieveRecordWall");
        public static Messages RetrieveSharedPrincipalsAndAccess = new Messages("RetrieveSharedPrincipalsAndAccess");
        public static Messages RetrieveUnpublished = new Messages("RetrieveUnpublished");
        public static Messages RetrieveUnpublishedMultiple = new Messages("RetrieveUnpublishedMultiple");
        public static Messages RevokeAccess = new Messages("RevokeAccess");
        public static Messages Route = new Messages("Route");
        public static Messages Send = new Messages("Send");
        public static Messages SendFromTemplate = new Messages("SendFromTemplate");
        public static Messages SetRelated = new Messages("SetRelated");
        [Obsolete("Use Update message with state filtering attribute instead")]
        public static Messages SetState = new Messages("SetState");
        [Obsolete("Use Update message with state filtering attribute instead")]
        public static Messages SetStateDynamicEntity = new Messages("SetStateDynamicEntity");
        public static Messages TriggerServiceEndpointCheck = new Messages("TriggerServiceEndpointCheck");
        public static Messages UnlockInvoicePricing = new Messages("UnlockInvoicePricing");
        public static Messages UnlockSalesOrderPricing = new Messages("UnlockSalesOrderPricing");
        public static Messages Update = new Messages("Update");
        public static Messages ValidateRecurrenceRule = new Messages("ValidateRecurrenceRule");
        public static Messages Win = new Messages("Win");
        public static Messages ExecuteWorkflow = new Messages("ExecuteWorkflow");
        public static Messages Default = new Messages("Default");

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