//HintName: Particulier.table.cs
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
    public static partial class ParticulierDefinition
    {
        public const string EntityName = "ftp_particulier";
        public const string EntityCollectionName = "ftp_particuliers";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            [StringLength(100)]
            public const string Name = "ftp_name";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "ftp_particulierid";

        }
        public static class OneToManyRelationships
        {
            [Relationship(ContratdelocationDefinition.EntityName, EntityRole.Referenced, "ftp_ContratdeLocation_Locataire_ftp_Parti", ContratdelocationDefinition.Columns.Locataire)]
            public const string ftp_ContratdeLocation_Locataire_ftp_Parti = "ftp_ContratdeLocation_Locataire_ftp_Parti";
        }
    }


}
