//HintName: Particulier.table.cs

using System;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using XrmFramework;

namespace FrameworkTests
{
    [GeneratedCode("XrmFramework", "2.0")]
    [EntityDefinition]
    [ExcludeFromCodeCoverage]
    public static partial class ParticulierDefinition
    {
        public const string EntityName = "ftp_particulier";
        public const string EntityCollectionName = "ftp_particuliers";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : Integer
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Integer)]
            [Range(100, 2000)]
            [DateTimeBehavior(DateTimeBehavior.UserLocal)]
            public const string Budgetparmois = "ftp_budgetparmois";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            [StringLength(100)]
            [DateTimeBehavior(DateTimeBehavior.UserLocal)]
            public const string Name = "ftp_name";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            [DateTimeBehavior(DateTimeBehavior.UserLocal)]
            public const string Id = "ftp_particulierid";

        }
        public static class ManyToOneRelationships
        {
            [Relationship("systemuser", EntityRole.Referencing, "createdby", "createdby")]
            public const string lk_ftp_particulier_createdby = "lk_ftp_particulier_createdby";
            [Relationship("systemuser", EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string lk_ftp_particulier_createdonbehalfby = "lk_ftp_particulier_createdonbehalfby";
            [Relationship("systemuser", EntityRole.Referencing, "modifiedby", "modifiedby")]
            public const string lk_ftp_particulier_modifiedby = "lk_ftp_particulier_modifiedby";
            [Relationship("systemuser", EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_ftp_particulier_modifiedonbehalfby = "lk_ftp_particulier_modifiedonbehalfby";
            [Relationship("systemuser", EntityRole.Referencing, "owninguser", "owninguser")]
            public const string user_ftp_particulier = "user_ftp_particulier";
            [Relationship("team", EntityRole.Referencing, "owningteam", "owningteam")]
            public const string team_ftp_particulier = "team_ftp_particulier";
            [Relationship("owner", EntityRole.Referencing, "ownerid", "ownerid")]
            public const string owner_ftp_particulier = "owner_ftp_particulier";
            [Relationship("businessunit", EntityRole.Referencing, "owningbusinessunit", "owningbusinessunit")]
            public const string business_unit_ftp_particulier = "business_unit_ftp_particulier";
        }
        public static class OneToManyRelationships
        {
            [Relationship("syncerror", EntityRole.Referenced, "ftp_particulier_SyncErrors", "regardingobjectid")]
            public const string ftp_particulier_SyncErrors = "ftp_particulier_SyncErrors";
            [Relationship("asyncoperation", EntityRole.Referenced, "ftp_particulier_AsyncOperations", "regardingobjectid")]
            public const string ftp_particulier_AsyncOperations = "ftp_particulier_AsyncOperations";
            [Relationship("mailboxtrackingfolder", EntityRole.Referenced, "ftp_particulier_MailboxTrackingFolders", "regardingobjectid")]
            public const string ftp_particulier_MailboxTrackingFolders = "ftp_particulier_MailboxTrackingFolders";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "ftp_particulier_UserEntityInstanceDatas", "objectid")]
            public const string ftp_particulier_UserEntityInstanceDatas = "ftp_particulier_UserEntityInstanceDatas";
            [Relationship("processsession", EntityRole.Referenced, "ftp_particulier_ProcessSession", "regardingobjectid")]
            public const string ftp_particulier_ProcessSession = "ftp_particulier_ProcessSession";
            [Relationship("bulkdeletefailure", EntityRole.Referenced, "ftp_particulier_BulkDeleteFailures", "regardingobjectid")]
            public const string ftp_particulier_BulkDeleteFailures = "ftp_particulier_BulkDeleteFailures";
            [Relationship("principalobjectattributeaccess", EntityRole.Referenced, "ftp_particulier_PrincipalObjectAttributeAccesses", "objectid")]
            public const string ftp_particulier_PrincipalObjectAttributeAccesses = "ftp_particulier_PrincipalObjectAttributeAccesses";
            [Relationship("duplicaterecord", EntityRole.Referenced, "ftp_particulier_DuplicateMatchingRecord", "duplicaterecordid")]
            public const string ftp_particulier_DuplicateMatchingRecord = "ftp_particulier_DuplicateMatchingRecord";
            [Relationship("duplicaterecord", EntityRole.Referenced, "ftp_particulier_DuplicateBaseRecord", "baserecordid")]
            public const string ftp_particulier_DuplicateBaseRecord = "ftp_particulier_DuplicateBaseRecord";
            [Relationship("ftp_contrat", EntityRole.Referenced, "ftp_Contrat_Proprietaire_ftp_Particulier", "ftp_proprietaire")]
            public const string ftp_Contrat_Proprietaire_ftp_Particulier = "ftp_Contrat_Proprietaire_ftp_Particulier";
            [Relationship("ftp_contrat", EntityRole.Referenced, "ftp_Contrat_Locataire_ftp_Particulier", "ftp_locataire")]
            public const string ftp_Contrat_Locataire_ftp_Particulier = "ftp_Contrat_Locataire_ftp_Particulier";
            [Relationship(ContratdelocationDefinition.EntityName, EntityRole.Referenced, "ftp_ContratdeLocation_Locataire_ftp_Parti", ContratdelocationDefinition.Columns.Locataire)]
            public const string ftp_ContratdeLocation_Locataire_ftp_Parti = "ftp_ContratdeLocation_Locataire_ftp_Parti";
            [Relationship(ContratdelocationDefinition.EntityName, EntityRole.Referenced, "ftp_ContratdeLocation_Proprietaire_ftp_Pa", "ftp_proprietaire")]
            public const string ftp_ContratdeLocation_Proprietaire_ftp_Pa = "ftp_ContratdeLocation_Proprietaire_ftp_Pa";
        }


    }
}
