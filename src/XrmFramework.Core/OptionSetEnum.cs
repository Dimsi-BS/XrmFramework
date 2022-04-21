using Newtonsoft.Json;
using System.Collections.Generic;

namespace XrmFramework.Core
{
    [JsonObject(MemberSerialization.OptOut)]
    public class OptionSetEnum
    {
        [JsonProperty("LogName")]
        public string LogicalName { get; set; }

        public string Name { get; set; }

        [JsonProperty("Locked")]
        public bool IsLocked { get; set; }

        public ICollection<OptionSetEnumValue> Values { get; } = new List<OptionSetEnumValue>();

        public bool IsGlobal { get; set; }

        public bool HasNullValue { get; set; }
    }
}