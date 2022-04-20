using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using XrmFramework.Definitions.Internal;

namespace XrmFramework.Definitions
{
    [GeneratedCode("XrmFramework", "2.0")]
    [EntityDefinition]
    [ExcludeFromCodeCoverage]
    [DefinitionManagerIgnore]
    public static class SystemUserRolesDefinition
    {
        public const string EntityName = "systemuserroles";
        public const string EntityCollectionName = "";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : 
            /// Validity :  
            /// </summary>
            public const string RoleId = "roleid";

            /// <summary>
            /// 
            /// Type : 
            /// Validity :  
            /// </summary>
            public const string SystemUserId = "systemuserid";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "systemuserroleid";

        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToManyRelationships
        {
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "systemuserroles", SystemUserRolesDefinition.Columns.SystemUserId)]
            public const string systemuserroles_association = "systemuserroles_association";
        }
    }
}
