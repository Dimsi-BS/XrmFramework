using Newtonsoft.Json;

namespace XrmFramework.Core
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ModelProperty
    {
        public string Name { get; set; }

        [JsonProperty("Type")]
        public string TypeFullName { get; set; }

        [JsonProperty("LogN")]
        public string LogicalName;

        public string LookupTargetTableLogicalName { get; set; }

        public string LookupTargetModel { get; set; }

        public string LookupTargetColumnLogicalName { get; set; }

        [JsonProperty("UsePropCh")]
        public bool IsValidForUpdate { get; set; } = true;

        public string JsonPropertyName { get; set; }

        public string JsonConverterType { get; set; }

        public string JsonIgnore { get; set; }

        public string[] JsonConverterConstructorArguments { get; set; }

        public string ModelConverterType { get; set; }

        public string ModelConverterConstructorArguments { get; set; }
    }
}
