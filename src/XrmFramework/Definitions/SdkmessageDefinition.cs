using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using XrmFramework.Definitions.Internal;

namespace XrmFramework.Definitions
{
    [GeneratedCode("XrmFramework", "2.0")]
    [EntityDefinition]
    [ExcludeFromCodeCoverage]
    [DefinitionManagerIgnore]
    public static class SdkmessageDefinition
    {
        public const string EntityName = "sdkmessage";
        public const string EntityCollectionName = "sdkmessages";

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
            public const string Name = "name";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "sdkmessageid";

        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToOneRelationships
        {
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
            public const string createdby_sdkmessage = "createdby_sdkmessage";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string lk_sdkmessage_createdonbehalfby = "lk_sdkmessage_createdonbehalfby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string lk_sdkmessage_modifiedonbehalfby = "lk_sdkmessage_modifiedonbehalfby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
            public const string modifiedby_sdkmessage = "modifiedby_sdkmessage";
            [Relationship("organization", EntityRole.Referencing, "organizationid", "organizationid")]
            public const string organization_sdkmessage = "organization_sdkmessage";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class OneToManyRelationships
        {
            [Relationship("sdkmessagepair", EntityRole.Referenced, "message_sdkmessagepair", "sdkmessageid")]
            public const string message_sdkmessagepair = "message_sdkmessagepair";
            [Relationship(CustomapiDefinition.EntityName, EntityRole.Referenced, "CustomAPIId", CustomapiDefinition.Columns.SdkMessageId)]
            public const string sdkmessage_customapi = "sdkmessage_customapi";
            [Relationship("serviceplanmapping", EntityRole.Referenced, "sdkmessage_serviceplanmapping", "sdkmessage")]
            public const string sdkmessage_serviceplanmapping = "sdkmessage_serviceplanmapping";
            [Relationship(SdkmessagefilterDefinition.EntityName, EntityRole.Referenced, "sdkmessageid_sdkmessagefilter", SdkmessagefilterDefinition.Columns.SdkMessageId)]
            public const string sdkmessageid_sdkmessagefilter = "sdkmessageid_sdkmessagefilter";
            [Relationship(SdkMessageProcessingStepDefinition.EntityName, EntityRole.Referenced, "sdkmessageid_sdkmessageprocessingstep", SdkMessageProcessingStepDefinition.Columns.SdkMessageId)]
            public const string sdkmessageid_sdkmessageprocessingstep = "sdkmessageid_sdkmessageprocessingstep";
            [Relationship("workflowdependency", EntityRole.Referenced, "sdkmessageid_workflow_dependency", "sdkmessageid")]
            public const string sdkmessageid_workflow_dependency = "sdkmessageid_workflow_dependency";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_sdkmessage", "objectid")]
            public const string userentityinstancedata_sdkmessage = "userentityinstancedata_sdkmessage";
        }
    }
}
