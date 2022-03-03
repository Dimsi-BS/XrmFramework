using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using XrmFramework.Definitions.Internal;

namespace XrmFramework.Definitions
{
    [GeneratedCode("XrmFramework", "2.0")]
    [EntityDefinition]
    [ExcludeFromCodeCoverage]
    [DefinitionManagerIgnore]
    public static class SolutionDefinition
    {
        public const string EntityName = "solution";
        public const string EntityCollectionName = "solutions";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [PrimaryAttribute(PrimaryAttributeType.Name)]
            [StringLength(256)]
            public const string FriendlyName = "friendlyname";

            /// <summary>
            /// 
            /// Type : Boolean
            /// Validity :  Read | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Boolean)]
            public const string IsManaged = "ismanaged";

            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [CrmLookup(PublisherDefinition.EntityName, PublisherDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.publisher_solution)]
            public const string PublisherId = "publisherid";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "solutionid";

            /// <summary>
            /// 
            /// Type : String
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.String)]
            [StringLength(65)]
            public const string UniqueName = "uniquename";

        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToManyRelationships
        {
            [Relationship("package", EntityRole.Referencing, "package_solution", "packageid")]
            public const string package_solution = "package_solution";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToOneRelationships
        {
            [Relationship("fileattachment", EntityRole.Referencing, "fileid", "fileid")]
            public const string fileattachment_solution_fileid = "fileattachment_solution_fileid";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
            public const string lk_solution_createdby = "lk_solution_createdby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
            public const string lk_solution_modifiedby = "lk_solution_modifiedby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string lk_solutionbase_createdonbehalfby = "lk_solutionbase_createdonbehalfby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_solutionbase_modifiedonbehalfby = "lk_solutionbase_modifiedonbehalfby";
            [Relationship("organization", EntityRole.Referencing, "organizationid", "organizationid")]
            public const string organization_solution = "organization_solution";
            [Relationship(PublisherDefinition.EntityName, EntityRole.Referencing, "publisherid", SolutionDefinition.Columns.PublisherId)]
            public const string publisher_solution = "publisher_solution";
            [Relationship(WebResourceDefinition.EntityName, EntityRole.Referencing, "configurationpageid", "configurationpageid")]
            public const string solution_configuration_webresource = "solution_configuration_webresource";
            [Relationship(SolutionDefinition.EntityName, EntityRole.Referencing, "parentsolutionid", "parentsolutionid")]
            public const string solution_parent_solution = "solution_parent_solution";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class OneToManyRelationships
        {
            [Relationship("fileattachment", EntityRole.Referenced, "regardingobjectid_fileattachment_solution", "objectid")]
            public const string FileAttachment_Solution = "FileAttachment_Solution";
            [Relationship("canvasapp", EntityRole.Referenced, "FK_CanvasApp_Solution", "solutionid")]
            public const string FK_CanvasApp_Solution = "FK_CanvasApp_Solution";
            [Relationship("dependencynode", EntityRole.Referenced, "solution_base_dependencynode", "basesolutionid")]
            public const string solution_base_dependencynode = "solution_base_dependencynode";
            [Relationship("fieldpermission", EntityRole.Referenced, "solution_fieldpermission", "solutionid")]
            public const string solution_fieldpermission = "solution_fieldpermission";
            [Relationship("fieldsecurityprofile", EntityRole.Referenced, "solution_fieldsecurityprofile", "solutionid")]
            public const string solution_fieldsecurityprofile = "solution_fieldsecurityprofile";
            [Relationship(SolutionDefinition.EntityName, EntityRole.Referenced, "solution_parent_solution", "parentsolutionid")]
            public const string solution_parent_solution = "solution_parent_solution";
            [Relationship("privilege", EntityRole.Referenced, "solution_privilege", "solutionid")]
            public const string solution_privilege = "solution_privilege";
            [Relationship(RoleDefinition.EntityName, EntityRole.Referenced, "solution_role", "solutionid")]
            public const string solution_role = "solution_role";
            [Relationship("roleprivileges", EntityRole.Referenced, "solution_roleprivileges", "solutionid")]
            public const string solution_roleprivileges = "solution_roleprivileges";
            [Relationship(SolutionComponentDefinition.EntityName, EntityRole.Referenced, "solution_solutioncomponent", SolutionComponentDefinition.Columns.SolutionId)]
            public const string solution_solutioncomponent = "solution_solutioncomponent";
            [Relationship("syncerror", EntityRole.Referenced, "Solution_SyncErrors", "regardingobjectid")]
            public const string Solution_SyncErrors = "Solution_SyncErrors";
            [Relationship("dependencynode", EntityRole.Referenced, "solution_top_dependencynode", "topsolutionid")]
            public const string solution_top_dependencynode = "solution_top_dependencynode";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_solution", "objectid")]
            public const string userentityinstancedata_solution = "userentityinstancedata_solution";
        }
    }
}
