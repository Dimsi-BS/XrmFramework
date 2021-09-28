using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XrmFramework.DefinitionManager.Model
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Relation
    {
        public string Name { get; set; }

        [JsonProperty("Etn")]
        public string EntityName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EntityRole Role { get; set; }

        [JsonProperty("NavPropName")]
        public string NavigationPropertyName { get; set; }

        [JsonProperty("LookName")]
        public string LookupFieldName { get; set; }
    }
}