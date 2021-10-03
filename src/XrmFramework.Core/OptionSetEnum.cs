using System.Collections.Generic;
using Newtonsoft.Json;

namespace XrmFramework.Core
{
    [JsonObject(MemberSerialization.OptOut)]
    public class OptionSetEnum
    {
        [JsonProperty("LogName")]
        public string LogicalName { get; set; }

        public string Name { get; set; }

        public ICollection<OptionSetEnumValue> Values { get; } = new List<OptionSetEnumValue>();

        public bool IsGlobal { get; set; }

        public bool HasNullValue { get; set; }
    }
}