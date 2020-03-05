// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
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

    public partial class Messages
    {
        public static readonly Messages AddItem = new Messages("AddItem");
        public static readonly Messages AddListMembers = new Messages("AddListMembers");
        public static readonly Messages AddMember = new Messages("AddMember");
        public static readonly Messages AddMembers = new Messages("AddMembers");
        public static readonly Messages AddPrivileges = new Messages("AddPrivileges");
        public static readonly Messages AddProductToKit = new Messages("AddProductToKit");
        public static readonly Messages AddRecurrence = new Messages("AddRecurrence");
        public static readonly Messages AddToQueue = new Messages("AddToQueue");
        public static readonly Messages AddUserToRecordTeam = new Messages("AddUserToRecordTeam");
        public static readonly Messages Assign = new Messages("Assign");
        public static readonly Messages AssignUserRoles = new Messages("AssignUserRoles");
        public static readonly Messages Associate = new Messages("Associate");
        public static readonly Messages BackgroundSend = new Messages("BackgroundSend");
        public static readonly Messages Book = new Messages("Book");
        public static readonly Messages Cancel = new Messages("Cancel");
        public static readonly Messages CheckIncoming = new Messages("CheckIncoming");
        public static readonly Messages CheckPromote = new Messages("CheckPromote");
        public static readonly Messages Clone = new Messages("Clone");
        public static readonly Messages Close = new Messages("Close");
        public static readonly Messages CopyDynamicListToStatic = new Messages("CopyDynamicListToStatic");
        public static readonly Messages CopySystemForm = new Messages("CopySystemForm");
        public static readonly Messages Create = new Messages("Create");
        public static readonly Messages CreateException = new Messages("CreateException");
        public static readonly Messages CreateInstance = new Messages("CreateInstance");
        public static readonly Messages Delete = new Messages("Delete");
        public static readonly Messages DeleteOpenInstances = new Messages("DeleteOpenInstances");
        public static readonly Messages DeliverIncoming = new Messages("DeliverIncoming");
        public static readonly Messages DeliverPromote = new Messages("DeliverPromote");
        public static readonly Messages DetachFromQueue = new Messages("DetachFromQueue");
        public static readonly Messages Disassociate = new Messages("Disassociate");
        public static readonly Messages Execute = new Messages("Execute");
        public static readonly Messages ExecuteById = new Messages("ExecuteById");
        public static readonly Messages Export = new Messages("Export");
        public static readonly Messages ExportAll = new Messages("ExportAll");
        public static readonly Messages ExportCompressed = new Messages("ExportCompressed");
        public static readonly Messages ExportCompressedAll = new Messages("ExportCompressedAll");
        public static readonly Messages GrantAccess = new Messages("GrantAccess");
        public static readonly Messages Handle = new Messages("Handle");
        public static readonly Messages Import = new Messages("Import");
        public static readonly Messages ImportAll = new Messages("ImportAll");
        public static readonly Messages ImportCompressedAll = new Messages("ImportCompressedAll");
        public static readonly Messages ImportCompressedWithProgress = new Messages("ImportCompressedWithProgress");
        public static readonly Messages ImportWithProgress = new Messages("ImportWithProgress");
        public static readonly Messages LockInvoicePricing = new Messages("LockInvoicePricing");
        public static readonly Messages LockSalesOrderPricing = new Messages("LockSalesOrderPricing");
        public static readonly Messages Lose = new Messages("Lose");
        public static readonly Messages Merge = new Messages("Merge");
        public static readonly Messages ModifyAccess = new Messages("ModifyAccess");
        public static readonly Messages Publish = new Messages("Publish");
        public static readonly Messages PublishAll = new Messages("PublishAll");
        public static readonly Messages QualifyLead = new Messages("QualifyLead");
        public static readonly Messages Recalculate = new Messages("Recalculate");
        public static readonly Messages RemoveItem = new Messages("RemoveItem");
        public static readonly Messages RemoveMember = new Messages("RemoveMember");
        public static readonly Messages RemoveMembers = new Messages("RemoveMembers");
        public static readonly Messages RemovePrivilege = new Messages("RemovePrivilege");
        public static readonly Messages RemoveProductFromKit = new Messages("RemoveProductFromKit");
        public static readonly Messages RemoveRelated = new Messages("RemoveRelated");
        public static readonly Messages RemoveUserFromRecordTeam = new Messages("RemoveUserFromRecordTeam");
        public static readonly Messages RemoveUserRoles = new Messages("RemoveUserRoles");
        public static readonly Messages ReplacePrivileges = new Messages("ReplacePrivileges");
        public static readonly Messages Reschedule = new Messages("Reschedule");
        public static readonly Messages Retrieve = new Messages("Retrieve");
        public static readonly Messages RetrieveExchangeRate = new Messages("RetrieveExchangeRate");
        public static readonly Messages RetrieveFilteredForms = new Messages("RetrieveFilteredForms");
        public static readonly Messages RetrieveMultiple = new Messages("RetrieveMultiple");
        public static readonly Messages RetrievePersonalWall = new Messages("RetrievePersonalWall");
        public static readonly Messages RetrievePrincipalAccess = new Messages("RetrievePrincipalAccess");
        public static readonly Messages RetrieveRecordWall = new Messages("RetrieveRecordWall");
        public static readonly Messages RetrieveSharedPrincipalsAndAccess = new Messages("RetrieveSharedPrincipalsAndAccess");
        public static readonly Messages RetrieveUnpublished = new Messages("RetrieveUnpublished");
        public static readonly Messages RetrieveUnpublishedMultiple = new Messages("RetrieveUnpublishedMultiple");
        public static readonly Messages RevokeAccess = new Messages("RevokeAccess");
        public static readonly Messages Route = new Messages("Route");
        public static readonly Messages Send = new Messages("Send");
        public static readonly Messages SendFromTemplate = new Messages("SendFromTemplate");
        public static readonly Messages SetRelated = new Messages("SetRelated");
        [Obsolete("Use Update message with state filtering attribute instead")]
        public static readonly Messages SetState = new Messages("SetState");
        [Obsolete("Use Update message with state filtering attribute instead")]
        public static readonly Messages SetStateDynamicEntity = new Messages("SetStateDynamicEntity");
        public static readonly Messages TriggerServiceEndpointCheck = new Messages("TriggerServiceEndpointCheck");
        public static readonly Messages UnlockInvoicePricing = new Messages("UnlockInvoicePricing");
        public static readonly Messages UnlockSalesOrderPricing = new Messages("UnlockSalesOrderPricing");
        public static readonly Messages Update = new Messages("Update");
        public static readonly Messages ValidateRecurrenceRule = new Messages("ValidateRecurrenceRule");
        public static readonly Messages Win = new Messages("Win");
        public static readonly Messages ExecuteWorkflow = new Messages("ExecuteWorkflow");
        public static readonly Messages Default = new Messages("Default");

        protected string MessageName { get; }

        protected Messages(string messageName)
        {
            MessageName = messageName;
        }

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