using System;
using XrmFramework.BindingModel;

namespace XrmFramework.Model
{
    [CrmEntity(SdkMessageProcessingStepDefinition.EntityName)]
    public class SdkMessageProcessingStep : IBindingModel
    {
        [CrmMapping(SdkMessageProcessingStepDefinition.Columns.PluginTypeId)]
        public PluginType PluginType { get; set; }

        [CrmMapping(SdkMessageProcessingStepDefinition.Columns.Name)]
        public string Name { get; set; }

        public Guid Id { get; set; }
    }
}
