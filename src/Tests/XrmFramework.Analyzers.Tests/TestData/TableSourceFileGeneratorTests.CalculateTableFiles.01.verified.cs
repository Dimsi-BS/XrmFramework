//HintName: Contratdelocation.table.cs
using System;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;

namespace XrmFramework
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
            [Relationship("ftp_agentimmobilier", EntityRole.Referencing, "ftp_Agent", "ftp_agent")]
            public const string ftp_ContratdeLocation_Agent_ftp_AgentImmo = "ftp_ContratdeLocation_Agent_ftp_AgentImmo";
            [Relationship(ParticulierDefinition.EntityName, EntityRole.Referencing, "ftp_Locataire", "ftp_locataire")]
            public const string ftp_ContratdeLocation_Locataire_ftp_Parti = "ftp_ContratdeLocation_Locataire_ftp_Parti";
            [Relationship(ParticulierDefinition.EntityName, EntityRole.Referencing, "ftp_Proprietaire", "ftp_proprietaire")]
            public const string ftp_ContratdeLocation_Proprietaire_ftp_Pa = "ftp_ContratdeLocation_Proprietaire_ftp_Pa";
        }
        public static class OneToManyRelationships
        {
        }
    }


}
