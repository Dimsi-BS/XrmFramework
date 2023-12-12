using System;
using XrmFramework.BindingModel;

namespace XrmFramework.Model
{
    [CrmEntity(PluginTypeDefinition.EntityName)]
    public class PluginType : IBindingModel
    {
        [CrmMapping(PluginTypeDefinition.Columns.AssemblyName)]
        public string AssemblyName { get; set; }

        [CrmMapping(PluginTypeDefinition.Columns.Culture)]
        public string Culture { get; set; }

        [CrmMapping(PluginTypeDefinition.Columns.Version)]
        public string Version { get; set; }

        [CrmMapping(PluginTypeDefinition.Columns.PublicKeyToken)]
        public string PublicKeyToken { get; set; }

        [CrmMapping(PluginTypeDefinition.Columns.TypeName)]
        public string TypeName { get; set; }

        public string AssemblyQualifiedName => $"{TypeName}, {AssemblyName}, Version={Version}, Culture={Culture}, PublicKeyToken={PublicKeyToken}";

        public Guid Id { get; set; }
    }
}
