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
        public static Messages AddItem = new Messages("additem");
        public static Messages AddListMembers = new Messages("addlistmembers");
        public static Messages AddMember = new Messages("addmember");
        public static Messages AddMembers = new Messages("addmembers");
        public static Messages AddPrivileges = new Messages("addprivileges");
        public static Messages AddProductToKit = new Messages("addproducttokit");
        public static Messages AddRecurrence = new Messages("addrecurrence");
        public static Messages AddToQueue = new Messages("addtoqueue");
        public static Messages AddUserToRecordTeam = new Messages("addusertorecordteam");
        public static Messages Assign = new Messages("assign");
        public static Messages AssignUserRoles = new Messages("assignuserroles");
        public static Messages Associate = new Messages("associate");
        public static Messages BackgroundSend = new Messages("backgroundsend");
        public static Messages Book = new Messages("book");
        public static Messages Cancel = new Messages("cancel");
        public static Messages CheckIncoming = new Messages("checkincoming");
        public static Messages CheckPromote = new Messages("checkpromote");
        public static Messages Clone = new Messages("clone");
        public static Messages Close = new Messages("close");
        public static Messages CopyDynamicListToStatic = new Messages("copydynamiclisttostatic");
        public static Messages CopySystemForm = new Messages("copysystemform");
        public static Messages Create = new Messages("create");
        public static Messages CreateException = new Messages("createexception");
        public static Messages CreateInstance = new Messages("createinstance");
        public static Messages Delete = new Messages("delete");
        public static Messages DeleteOpenInstances = new Messages("deleteopeninstances");
        public static Messages DeliverIncoming = new Messages("deliverincoming");
        public static Messages DeliverPromote = new Messages("deliverpromote");
        public static Messages DetachFromQueue = new Messages("detachfromqueue");
        public static Messages Disassociate = new Messages("disassociate");
        public static Messages Execute = new Messages("execute");
        public static Messages ExecuteById = new Messages("executebyid");
        public static Messages Export = new Messages("export");
        public static Messages ExportAll = new Messages("exportall");
        public static Messages ExportCompressed = new Messages("exportcompressed");
        public static Messages ExportCompressedAll = new Messages("exportcompressedall");
        public static Messages GrantAccess = new Messages("grantaccess");
        public static Messages Handle = new Messages("handle");
        public static Messages Import = new Messages("import");
        public static Messages ImportAll = new Messages("importall");
        public static Messages ImportCompressedAll = new Messages("importcompressedall");
        public static Messages ImportCompressedWithProgress = new Messages("importcompressedwithprogress");
        public static Messages ImportWithProgress = new Messages("importwithprogress");
        public static Messages LockInvoicePricing = new Messages("lockinvoicepricing");
        public static Messages LockSalesOrderPricing = new Messages("locksalesorderpricing");
        public static Messages Lose = new Messages("lose");
        public static Messages Merge = new Messages("merge");
        public static Messages ModifyAccess = new Messages("modifyaccess");
        public static Messages Publish = new Messages("publish");
        public static Messages PublishAll = new Messages("publishall");
        public static Messages QualifyLead = new Messages("qualifylead");
        public static Messages Recalculate = new Messages("recalculate");
        public static Messages RemoveItem = new Messages("removeitem");
        public static Messages RemoveMember = new Messages("removemember");
        public static Messages RemoveMembers = new Messages("removemembers");
        public static Messages RemovePrivilege = new Messages("removeprivilege");
        public static Messages RemoveProductFromKit = new Messages("removeproductfromkit");
        public static Messages RemoveRelated = new Messages("removerelated");
        public static Messages RemoveUserFromRecordTeam = new Messages("removeuserfromrecordteam");
        public static Messages RemoveUserRoles = new Messages("removeuserroles");
        public static Messages ReplacePrivileges = new Messages("replaceprivileges");
        public static Messages Reschedule = new Messages("reschedule");
        public static Messages Retrieve = new Messages("retrieve");
        public static Messages RetrieveExchangeRate = new Messages("retrieveexchangerate");
        public static Messages RetrieveFilteredForms = new Messages("retrievefilteredforms");
        public static Messages RetrieveMultiple = new Messages("retrievemultiple");
        public static Messages RetrievePersonalWall = new Messages("retrievepersonalwall");
        public static Messages RetrievePrincipalAccess = new Messages("retrieveprincipalaccess");
        public static Messages RetrieveRecordWall = new Messages("retrieverecordwall");
        public static Messages RetrieveSharedPrincipalsAndAccess = new Messages("retrievesharedprincipalsandaccess");
        public static Messages RetrieveUnpublished = new Messages("retrieveunpublished");
        public static Messages RetrieveUnpublishedMultiple = new Messages("retrieveunpublishedmultiple");
        public static Messages RevokeAccess = new Messages("revokeaccess");
        public static Messages Route = new Messages("route");
        public static Messages Send = new Messages("send");
        public static Messages SendFromTemplate = new Messages("sendfromtemplate");
        public static Messages SetRelated = new Messages("setrelated");
        [Obsolete("Use Update message with state filtering attribute instead")]
        public static Messages SetState = new Messages("setstate");
        [Obsolete("Use Update message with state filtering attribute instead")]
        public static Messages SetStateDynamicEntity = new Messages("setstatedynamicentity");
        public static Messages TriggerServiceEndpointCheck = new Messages("triggerserviceendpointcheck");
        public static Messages UnlockInvoicePricing = new Messages("unlockinvoicepricing");
        public static Messages UnlockSalesOrderPricing = new Messages("unlocksalesorderpricing");
        public static Messages Update = new Messages("update");
        public static Messages ValidateRecurrenceRule = new Messages("validaterecurrencerule");
        public static Messages Win = new Messages("win");
        public static Messages ExecuteWorkflow = new Messages("executeworkflow");
        public static Messages Default = new Messages("default");

        protected string MessageName { get; }

        protected Messages(string messageName)
        {
            MessageName = messageName.ToLowerInvariant();
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