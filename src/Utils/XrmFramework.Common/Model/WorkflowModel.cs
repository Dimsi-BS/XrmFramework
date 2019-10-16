using System;
using Model;
using XrmFramework.Common;

namespace XrmFramework.Model
{
    [CrmEntity(WorkflowDefinition.EntityName)]
    public class WorkflowModel : IBindingModel
    {
        [CrmMapping(WorkflowDefinition.Columns.PluginTypeId)]
        public PluginType PluginType { get; set; }

        [CrmMapping(WorkflowDefinition.Columns.Name)]
        public string Name { get; set; }

        public Guid Id { get; set; }
    }
}
