using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace XrmFramework.Definitions
{
    [GeneratedCode("XrmFramework", "1.0")]
    [EntityDefinition]

    [ExcludeFromCodeCoverage]
    [DefinitionManagerIgnore]

    public static class WorkflowDefinition
    {
        public const string EntityName = "workflow";
        public const string EntityCollectionName = "workflows";

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
            [StringLength(100)]
            public const string Name = "name";

            /// <summary>
            /// 
            /// Type : Lookup
            /// Validity :  Read 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Lookup)]
            [CrmLookup(PluginTypeDefinition.EntityName, PluginTypeDefinition.Columns.Id, RelationshipName = ManyToOneRelationships.plugintypeid_workflow)]
            public const string PluginTypeId = "plugintypeid";

            /// <summary>
            /// 
            /// Type : Integer
            /// Validity :  Read | Create | Update 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Integer)]
            [Range(-2147483648, 2147483647)]
            public const string Rank = "rank";

            /// <summary>
            /// 
            /// Type : Picklist (ExecutingUser)
            /// Validity :  Read | Create | Update | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Picklist)]
            [OptionSet(typeof(ExecutingUser))]
            public const string RunAs = "runas";

            /// <summary>
            /// 
            /// Type : Uniqueidentifier
            /// Validity :  Read | Create | AdvancedFind 
            /// </summary>
            [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
            [PrimaryAttribute(PrimaryAttributeType.Id)]
            public const string Id = "workflowid";

        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ManyToOneRelationships
        {
            [Relationship("businessunit", EntityRole.Referencing, "owningbusinessunit", "owningbusinessunit")]
            public const string business_unit_workflow = "business_unit_workflow";
            [Relationship("owner", EntityRole.Referencing, "ownerid", "ownerid")]
            public const string owner_workflows = "owner_workflows";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "owninguser", "owninguser")]
            public const string system_user_workflow = "system_user_workflow";
            [Relationship("team", EntityRole.Referencing, "owningteam", "owningteam")]
            public const string team_workflow = "team_workflow";
            [Relationship(WorkflowDefinition.EntityName, EntityRole.Referencing, "activeworkflowid", "activeworkflowid")]
            public const string workflow_active_workflow = "workflow_active_workflow";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdby", "createdby")]
            public const string workflow_createdby = "workflow_createdby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "createdonbehalfby", "createdonbehalfby")]
            public const string workflow_createdonbehalfby = "workflow_createdonbehalfby";
            [Relationship("imagedescriptor", EntityRole.Referencing, "entityimageinstance_workflow", "entityimageid")]
            public const string workflow_entityimage = "workflow_entityimage";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedby", "modifiedby")]
            public const string workflow_modifiedby = "workflow_modifiedby";
            [Relationship(SystemUserDefinition.EntityName, EntityRole.Referencing, "modifiedonbehalfby", "modifiedonbehalfby")]
            public const string workflow_modifiedonbehalfby = "workflow_modifiedonbehalfby";
            [Relationship(PluginTypeDefinition.EntityName, EntityRole.Referencing, "plugintypeid", WorkflowDefinition.Columns.PluginTypeId)]
            public const string plugintypeid_workflow = "plugintypeid_workflow";
            [Relationship(WorkflowDefinition.EntityName, EntityRole.Referencing, "parentworkflowid", "parentworkflowid")]
            public const string workflow_parent_workflow = "workflow_parent_workflow";
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class OneToManyRelationships
        {
            [Relationship("convertruleitem", EntityRole.Referenced, "convertruleitembase_workflowid", "workflowid")]
            public const string convertruleitembase_workflowid = "convertruleitembase_workflowid";
            [Relationship("asyncoperation", EntityRole.Referenced, "lk_asyncoperation_workflowactivationid", "workflowactivationid")]
            public const string lk_asyncoperation_workflowactivationid = "lk_asyncoperation_workflowactivationid";
            [Relationship("expiredprocess", EntityRole.Referenced, "workflow_expiredprocess", "processid")]
            public const string lk_expiredprocess_processid = "lk_expiredprocess_processid";
            [Relationship("leadtoopportunitysalesprocess", EntityRole.Referenced, "workflow_leadtoopportunitysalesprocess", "processid")]
            public const string lk_leadtoopportunitysalesprocess_processid = "lk_leadtoopportunitysalesprocess_processid";
            [Relationship("msdyn_bpf_2c5fe86acc8b414b8322ae571000c799", EntityRole.Referenced, "lk_msdyn_bpf_2c5fe86acc8b414b8322ae571000c799_processid", "processid")]
            public const string lk_msdyn_bpf_2c5fe86acc8b414b8322ae571000c799_processid = "lk_msdyn_bpf_2c5fe86acc8b414b8322ae571000c799_processid";
            [Relationship("msdyn_bpf_665e73aa18c247d886bfc50499c73b82", EntityRole.Referenced, "lk_msdyn_bpf_665e73aa18c247d886bfc50499c73b82_processid", "processid")]
            public const string lk_msdyn_bpf_665e73aa18c247d886bfc50499c73b82_processid = "lk_msdyn_bpf_665e73aa18c247d886bfc50499c73b82_processid";
            [Relationship("msdyn_bpf_989e9b1857e24af18787d5143b67523b", EntityRole.Referenced, "lk_msdyn_bpf_989e9b1857e24af18787d5143b67523b_processid", "processid")]
            public const string lk_msdyn_bpf_989e9b1857e24af18787d5143b67523b_processid = "lk_msdyn_bpf_989e9b1857e24af18787d5143b67523b_processid";
            [Relationship("msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3", EntityRole.Referenced, "lk_msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3_processid", "processid")]
            public const string lk_msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3_processid = "lk_msdyn_bpf_baa0a411a239410cb8bded8b5fdd88e3_processid";
            [Relationship("msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39", EntityRole.Referenced, "lk_msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39_processid", "processid")]
            public const string lk_msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39_processid = "lk_msdyn_bpf_d3d97bac8c294105840e99e37a9d1c39_processid";
            [Relationship("msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d", EntityRole.Referenced, "lk_msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d_processid", "processid")]
            public const string lk_msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d_processid = "lk_msdyn_bpf_d8f9dc7f099f44db9d641dd81fbd470d_processid";
            [Relationship("newprocess", EntityRole.Referenced, "workflow_newprocess", "processid")]
            public const string lk_newprocess_processid = "lk_newprocess_processid";
            [Relationship("opportunitysalesprocess", EntityRole.Referenced, "workflow_opportunitysalesprocess", "processid")]
            public const string lk_opportunitysalesprocess_processid = "lk_opportunitysalesprocess_processid";
            [Relationship("phonetocaseprocess", EntityRole.Referenced, "workflow_phonetocaseprocess", "processid")]
            public const string lk_phonetocaseprocess_processid = "lk_phonetocaseprocess_processid";
            [Relationship("processsession", EntityRole.Referenced, "lk_processsession_processid", "processid")]
            public const string lk_processsession_processid = "lk_processsession_processid";
            [Relationship("translationprocess", EntityRole.Referenced, "workflow_translationprocess", "processid")]
            public const string lk_translationprocess_processid = "lk_translationprocess_processid";
            [Relationship("msdyn_solutionhealthrule", EntityRole.Referenced, "msdyn_workflow_msdyn_solutionhealthrule_resolutionaction", "msdyn_resolutionaction")]
            public const string msdyn_workflow_msdyn_solutionhealthrule_resolutionaction = "msdyn_workflow_msdyn_solutionhealthrule_resolutionaction";
            [Relationship("msdyn_solutionhealthrule", EntityRole.Referenced, "msdyn_workflow_msdyn_solutionhealthrule_Workflow", "msdyn_workflow")]
            public const string msdyn_workflow_msdyn_solutionhealthrule_Workflow = "msdyn_workflow_msdyn_solutionhealthrule_Workflow";
            [Relationship("processstage", EntityRole.Referenced, "process_processstage", "processid")]
            public const string process_processstage = "process_processstage";
            [Relationship("processtrigger", EntityRole.Referenced, "process_processtrigger", "processid")]
            public const string process_processtrigger = "process_processtrigger";
            [Relationship("sla", EntityRole.Referenced, "slabase_workflowid", "workflowid")]
            public const string slabase_workflowid = "slabase_workflowid";
            [Relationship("slaitem", EntityRole.Referenced, "slaitembase_workflowid", "workflowid")]
            public const string slaitembase_workflowid = "slaitembase_workflowid";
            [Relationship("userentityinstancedata", EntityRole.Referenced, "userentityinstancedata_workflow", "objectid")]
            public const string userentityinstancedata_workflow = "userentityinstancedata_workflow";
            [Relationship(WorkflowDefinition.EntityName, EntityRole.Referenced, "workflow_active_workflow", "activeworkflowid")]
            public const string workflow_active_workflow = "workflow_active_workflow";
            [Relationship("annotation", EntityRole.Referenced, "Workflow_Annotation", "objectid")]
            public const string Workflow_Annotation = "Workflow_Annotation";
            [Relationship("workflowdependency", EntityRole.Referenced, "workflow_dependencies", "workflowid")]
            public const string workflow_dependencies = "workflow_dependencies";
            [Relationship(WorkflowDefinition.EntityName, EntityRole.Referenced, "workflow_parent_workflow", "parentworkflowid")]
            public const string workflow_parent_workflow = "workflow_parent_workflow";
            [Relationship("routingrule", EntityRole.Referenced, "Workflow_routingrule", "workflowid")]
            public const string Workflow_routingrule = "Workflow_routingrule";
            [Relationship("syncerror", EntityRole.Referenced, "Workflow_SyncErrors", "regardingobjectid")]
            public const string Workflow_SyncErrors = "Workflow_SyncErrors";
            [Relationship("convertrule", EntityRole.Referenced, "workflowid_convertrule", "workflowid")]
            public const string workflowid_convertrule = "workflowid_convertrule";
            [Relationship("channelaccessprofilerule", EntityRole.Referenced, "workflowid_profilerule", "workflowid")]
            public const string workflowid_profilerule = "workflowid_profilerule";
        }
    }

    [OptionSetDefinition("workflow_runas")]
    public enum ExecutingUser
    {
        [Description("Owner")]
        Owner = 0,
        [Description("Calling User")]
        CallingUser = 1,
    }
}
