using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace XrmFramework
{
    [GeneratedCode("XrmFramework", "2.0")]
    [EntityDefinition]
    [ExcludeFromCodeCoverage]
    public static partial class DebugSessionDefinition
    {
        public const string EntityName = "dimsi_xrmframeworkdebugsession";
        public const string EntityCollectionName = "dimsi_xrmframeworkdebugsessions";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : DateTime
            /// Validity :  Read | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.DateTime)]
            [DateTimeBehavior(DateTimeBehavior.UserLocal)]
            public const string CreatedOn = "createdon";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(100)]
            public const string Debugee = "dimsi_debugee";

            /// <summary>
            /// 
            /// Type : Memo
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Memo)]
            [StringLength(2000)]
            public const string DebugInfo = "dimsi_debuginfo";

            /// <summary>
            /// 
            /// Type : Boolean
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Boolean)]
            public const string DebugSYSTEMUSer = "dimsi_debugsystemuser";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(100)]
            public const string HybridConnectionName = "dimsi_hybridconnectionname";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            [StringLength(100)]
            public const string Name = "dimsi_name";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(100)]
            public const string RelayUrl = "dimsi_relayurl";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(100)]
            public const string SasConnectionKey = "dimsi_sasconnectionkey";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(100)]
            public const string SasKeyName = "dimsi_saskeyname";

            /// <summary>
            /// 
            /// Type : DateTime
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.DateTime)]
            [DateTimeBehavior(DateTimeBehavior.UserLocal)]
            public const string SessionEnd = "dimsi_sessionend";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "dimsi_xrmframeworkdebugsessionid";

            /// <summary>
            /// 
            /// Type : State (XrmframeworkdebugsessionState)
            /// Validity :  Read | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.State)]
            [OptionSet(typeof(DebugSessionState))]
            public const string StateCode = "statecode";

        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToOneRelationships
        {
            [Relationship("systemuser", EntityRole.Referencing, "createdby", "createdby")]
            public const string lk_dimsi_xrmframeworkdebugsession_createdby = "lk_dimsi_xrmframeworkdebugsession_createdby";
            [Relationship("systemuser", EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string lk_dimsi_xrmframeworkdebugsession_createdonbehalfby = "lk_dimsi_xrmframeworkdebugsession_createdonbehalfby";
            [Relationship("systemuser", EntityRole.Referencing, "modifiedby", "modifiedby")]
            public const string lk_dimsi_xrmframeworkdebugsession_modifiedby = "lk_dimsi_xrmframeworkdebugsession_modifiedby";
            [Relationship("systemuser", EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_dimsi_xrmframeworkdebugsession_modifiedonbehalfby = "lk_dimsi_xrmframeworkdebugsession_modifiedonbehalfby";
            [Relationship("organization", EntityRole.Referencing, "organizationid", "organizationid")]
            public const string organization_dimsi_xrmframeworkdebugsession = "organization_dimsi_xrmframeworkdebugsession";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class OneToManyRelationships
        {
            [Relationship("asyncoperation", EntityRole.Referenced, "dimsi_xrmframeworkdebugsession_AsyncOperations", "regardingobjectid")]
            public const string dimsi_xrmframeworkdebugsession_AsyncOperations = "dimsi_xrmframeworkdebugsession_AsyncOperations";
            [Relationship("bulkdeletefailure", EntityRole.Referenced, "dimsi_xrmframeworkdebugsession_BulkDeleteFailures", "regardingobjectid")]
            public const string dimsi_xrmframeworkdebugsession_BulkDeleteFailures = "dimsi_xrmframeworkdebugsession_BulkDeleteFailures";
            [Relationship("mailboxtrackingfolder", EntityRole.Referenced, "dimsi_xrmframeworkdebugsession_MailboxTrackingFolders", "regardingobjectid")]
            public const string dimsi_xrmframeworkdebugsession_MailboxTrackingFolders = "dimsi_xrmframeworkdebugsession_MailboxTrackingFolders";
            [Relationship("principalobjectattributeaccess", EntityRole.Referenced, "dimsi_xrmframeworkdebugsession_PrincipalObjectAttributeAccesses", "objectid")]
            public const string dimsi_xrmframeworkdebugsession_PrincipalObjectAttributeAccesses = "dimsi_xrmframeworkdebugsession_PrincipalObjectAttributeAccesses";
            [Relationship("processsession", EntityRole.Referenced, "dimsi_xrmframeworkdebugsession_ProcessSession", "regardingobjectid")]
            public const string dimsi_xrmframeworkdebugsession_ProcessSession = "dimsi_xrmframeworkdebugsession_ProcessSession";
            [Relationship("syncerror", EntityRole.Referenced, "dimsi_xrmframeworkdebugsession_SyncErrors", "regardingobjectid")]
            public const string dimsi_xrmframeworkdebugsession_SyncErrors = "dimsi_xrmframeworkdebugsession_SyncErrors";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "dimsi_xrmframeworkdebugsession_UserEntityInstanceDatas", "objectid")]
            public const string dimsi_xrmframeworkdebugsession_UserEntityInstanceDatas = "dimsi_xrmframeworkdebugsession_UserEntityInstanceDatas";
        }
    }
}
