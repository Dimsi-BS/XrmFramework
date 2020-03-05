using Microsoft.Xrm.Sdk;
using Model;
using XrmFramework.Common;

namespace XrmFramework.Model
{
    partial class EnvironmentVariable: IEntityModel
    {
        public string Value => Entity.GetAliasedValue<string>($"{EnvironmentVariableValueDefinition.EntityName}.{EnvironmentVariableValueDefinition.Columns.Value}") ?? DefaultValue;

        public Entity Entity { get; set; }
    }
}
