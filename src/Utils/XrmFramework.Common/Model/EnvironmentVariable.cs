using System;
using Model;
using XrmFramework.Common;

namespace XrmFramework.Model
{
    [CrmEntity(EnvironmentVariableDefinition.EntityName)]
    public partial class EnvironmentVariable : IBindingModel
    {
        [CrmMapping(EnvironmentVariableDefinition.Columns.SchemaName)]
        public string SchemaName { get; set; }

        [CrmMapping(EnvironmentVariableDefinition.Columns.DisplayName)]
        public string DisplayName { get; set; }

        [CrmMapping(EnvironmentVariableDefinition.Columns.Type)]
        public EnvironmentVariableType Type { get; set; }

        [CrmMapping(EnvironmentVariableDefinition.Columns.DefaultValue)]
        public string DefaultValue { get; set; }

        public Guid Id { get; set; }
    }
}
