using System;
using Microsoft.Xrm.Sdk;
using XrmFramework.BindingModel;

namespace XrmFramework.Model
{
    [CrmEntity(EnvironmentVariableDefinition.EntityName)]
    public partial class EnvironmentVariable: IBindingModel, IEntityModel
    {
        [CrmMapping(EnvironmentVariableDefinition.Columns.SchemaName)]
        public string SchemaName { get; set; }

        [CrmMapping(EnvironmentVariableDefinition.Columns.DisplayName)]
        public string DisplayName { get; set; }

        [CrmMapping(EnvironmentVariableDefinition.Columns.Type)]
        public EnvironmentVariableType Type { get; set; }

        [CrmMapping(EnvironmentVariableDefinition.Columns.DefaultValue)]
        public string DefaultValue { get; set; }

        public string Value => Entity.GetAliasedValue<string>($"{EnvironmentVariableValueDefinition.EntityName}.{EnvironmentVariableValueDefinition.Columns.Value}") ?? DefaultValue;

        public Entity Entity { get; set; }

        public Guid Id { get; set; }
    }
}
