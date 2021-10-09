using System;
using XrmFramework.BindingModel;
using XrmFramework.Definitions.Internal;

namespace XrmFramework.Model
{
    [CrmEntity(WorkflowDefinition.EntityName)]
    public class WorkflowModel : IBindingModel
    {
        [CrmMapping(WorkflowDefinition.Columns.PluginTypeId)]
        public Guid PluginType { get; set; }

        [CrmMapping(WorkflowDefinition.Columns.Name)]
        public string Name { get; set; }

        public Guid Id { get; set; }
    }
}
