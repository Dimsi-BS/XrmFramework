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
    public static class SolutioncomponentDefinition
    {
        public const string EntityName = "solutioncomponent";
        public const string EntityCollectionName = "solutioncomponentss";

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Columns
        {
            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "solutioncomponentid";

            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [CrmLookup(SolutionDefinition.EntityName, SolutionDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.solution_solutioncomponent)]
            public const string SolutionId = "solutionid";

        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToOneRelationships
        {
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string lk_solutioncomponentbase_createdonbehalfby = "lk_solutioncomponentbase_createdonbehalfby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_solutioncomponentbase_modifiedonbehalfby = "lk_solutioncomponentbase_modifiedonbehalfby";
            [Relationship(SolutionDefinition.EntityName, EntityRole.Referencing, "solutionid", SolutioncomponentDefinition.Columns.SolutionId)]
            public const string solution_solutioncomponent = "solution_solutioncomponent";
            [Relationship(SolutioncomponentDefinition.EntityName, EntityRole.Referencing, "rootsolutioncomponentid_solutioncomponent", "rootsolutioncomponentid")]
            public const string solutioncomponent_parent_solutioncomponent = "solutioncomponent_parent_solutioncomponent";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class OneToManyRelationships
        {
            [Relationship(SolutioncomponentDefinition.EntityName, EntityRole.Referenced, "solutioncomponent_parent_solutioncomponent", "rootsolutioncomponentid")]
            public const string solutioncomponent_parent_solutioncomponent = "solutioncomponent_parent_solutioncomponent";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_solutioncomponent", "objectid")]
            public const string userentityinstancedata_solutioncomponent = "userentityinstancedata_solutioncomponent";
        }
    }
}
