//HintName: Contratdelocation.table.cs

using System;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using XrmFramework;

namespace XrmFramework.Definitions
{
    [GeneratedCode("XrmFramework", "2.0")]
    [EntityDefinition]
    [ExcludeFromCodeCoverage]
    public static partial class ContratdelocationDefinition
    {
        public const string EntityName = "ftp_contratdelocation";
        public const string EntityCollectionName = "ftp_contratdelocations";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [CrmLookup("ftp_agentimmobilier","ftp_agent",RelationshipName = ManyToOneRelationships.ftp_ContratdeLocation_Agent_ftp_AgentImmo)]
            public const string Agent = "ftp_agent";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "ftp_contratdelocationid";

            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [CrmLookup(ParticulierDefinition.EntityName,ParticulierDefinition.Columns.Id,RelationshipName = ManyToOneRelationships.ftp_ContratdeLocation_Locataire_ftp_Parti)]
            public const string Locataire = "ftp_locataire";

            /// <summary>
            /// 
            /// Type : Integer
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Integer)]
            [Range(0, 2000)]
            public const string Loyer = "ftp_loyer";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            [StringLength(100)]
            public const string Name = "ftp_name";

        }
        public static class ManyToOneRelationships
        {
            [Relationship("systemuser", EntityRole.Referencing, "createdby", "createdby")]
            public const string lk_ftp_contratdelocation_createdby = "lk_ftp_contratdelocation_createdby";
            [Relationship("systemuser", EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string lk_ftp_contratdelocation_createdonbehalfby = "lk_ftp_contratdelocation_createdonbehalfby";
            [Relationship("systemuser", EntityRole.Referencing, "modifiedby", "modifiedby")]
            public const string lk_ftp_contratdelocation_modifiedby = "lk_ftp_contratdelocation_modifiedby";
            [Relationship("systemuser", EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_ftp_contratdelocation_modifiedonbehalfby = "lk_ftp_contratdelocation_modifiedonbehalfby";
            [Relationship("systemuser", EntityRole.Referencing, "owninguser", "owninguser")]
            public const string user_ftp_contratdelocation = "user_ftp_contratdelocation";
            [Relationship("team", EntityRole.Referencing, "owningteam", "owningteam")]
            public const string team_ftp_contratdelocation = "team_ftp_contratdelocation";
            [Relationship("owner", EntityRole.Referencing, "ownerid", "ownerid")]
            public const string owner_ftp_contratdelocation = "owner_ftp_contratdelocation";
            [Relationship("businessunit", EntityRole.Referencing, "owningbusinessunit", "owningbusinessunit")]
            public const string business_unit_ftp_contratdelocation = "business_unit_ftp_contratdelocation";
            [Relationship("ftp_agentimmobilier", EntityRole.Referencing, "ftp_Agent", ContratdelocationDefinition.Columns.Agent)]
            public const string ftp_ContratdeLocation_Agent_ftp_AgentImmo = "ftp_ContratdeLocation_Agent_ftp_AgentImmo";
            [Relationship(ParticulierDefinition.EntityName, EntityRole.Referencing, "ftp_Locataire", ContratdelocationDefinition.Columns.Locataire)]
            public const string ftp_ContratdeLocation_Locataire_ftp_Parti = "ftp_ContratdeLocation_Locataire_ftp_Parti";
            [Relationship(ParticulierDefinition.EntityName, EntityRole.Referencing, "ftp_Proprietaire", "ftp_proprietaire")]
            public const string ftp_ContratdeLocation_Proprietaire_ftp_Pa = "ftp_ContratdeLocation_Proprietaire_ftp_Pa";
        }
        public static class OneToManyRelationships
        {
            [Relationship("syncerror", EntityRole.Referenced, "ftp_contratdelocation_SyncErrors", "regardingobjectid")]
            public const string ftp_contratdelocation_SyncErrors = "ftp_contratdelocation_SyncErrors";
            [Relationship("asyncoperation", EntityRole.Referenced, "ftp_contratdelocation_AsyncOperations", "regardingobjectid")]
            public const string ftp_contratdelocation_AsyncOperations = "ftp_contratdelocation_AsyncOperations";
            [Relationship("mailboxtrackingfolder", EntityRole.Referenced, "ftp_contratdelocation_MailboxTrackingFolders", "regardingobjectid")]
            public const string ftp_contratdelocation_MailboxTrackingFolders = "ftp_contratdelocation_MailboxTrackingFolders";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "ftp_contratdelocation_UserEntityInstanceDatas", "objectid")]
            public const string ftp_contratdelocation_UserEntityInstanceDatas = "ftp_contratdelocation_UserEntityInstanceDatas";
            [Relationship("processsession", EntityRole.Referenced, "ftp_contratdelocation_ProcessSession", "regardingobjectid")]
            public const string ftp_contratdelocation_ProcessSession = "ftp_contratdelocation_ProcessSession";
            [Relationship("bulkdeletefailure", EntityRole.Referenced, "ftp_contratdelocation_BulkDeleteFailures", "regardingobjectid")]
            public const string ftp_contratdelocation_BulkDeleteFailures = "ftp_contratdelocation_BulkDeleteFailures";
            [Relationship("principalobjectattributeaccess", EntityRole.Referenced, "ftp_contratdelocation_PrincipalObjectAttributeAccesses", "objectid")]
            public const string ftp_contratdelocation_PrincipalObjectAttributeAccesses = "ftp_contratdelocation_PrincipalObjectAttributeAccesses";
            [Relationship("duplicaterecord", EntityRole.Referenced, "ftp_contratdelocation_DuplicateMatchingRecord", "duplicaterecordid")]
            public const string ftp_contratdelocation_DuplicateMatchingRecord = "ftp_contratdelocation_DuplicateMatchingRecord";
            [Relationship("duplicaterecord", EntityRole.Referenced, "ftp_contratdelocation_DuplicateBaseRecord", "baserecordid")]
            public const string ftp_contratdelocation_DuplicateBaseRecord = "ftp_contratdelocation_DuplicateBaseRecord";
        }
    }


}
