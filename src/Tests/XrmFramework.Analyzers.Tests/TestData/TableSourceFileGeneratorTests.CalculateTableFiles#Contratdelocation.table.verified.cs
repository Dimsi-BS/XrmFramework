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
            [CrmLookup(ParticulierDefinition.EntityName, ParticulierDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.ftp_ContratdeLocation_Locataire_ftp_Parti)]
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
            [Relationship(ParticulierDefinition.EntityName, EntityRole.Referencing, "ftp_Locataire", ContratdelocationDefinition.Columns.Locataire)]
            public const string ftp_ContratdeLocation_Locataire_ftp_Parti = "ftp_ContratdeLocation_Locataire_ftp_Parti";
        }
    }


}
