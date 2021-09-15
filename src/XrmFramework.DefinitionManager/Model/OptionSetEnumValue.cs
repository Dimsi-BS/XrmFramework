using System.Collections.Generic;
using Newtonsoft.Json;

namespace XrmFramework.DefinitionManager.Model
{
    [JsonObject(MemberSerialization.OptOut)]
    public class OptionSetEnumValue
    {
        public int Value { get; set; }

        public string Name { get; set; }

        public ICollection<LocalizedLabel> Labels { get; } = new List<LocalizedLabel>();

        [JsonProperty("ExtVal")]
        public string ExternalValue { get; set; }
    }
}