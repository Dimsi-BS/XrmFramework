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

        /// <summary>
        /// Local marker for the framework's internal .table files: the option set is frozen and must
        /// not be regenerated. Absent from CRM metadata, it is never produced by a generation
        /// command — it is only read back and rewritten as-is so it is not lost.
        /// </summary>
        [JsonProperty("Locked")]
        public bool IsLocked { get; set; }
    }
}