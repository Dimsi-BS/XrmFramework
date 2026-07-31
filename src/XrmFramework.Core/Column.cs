using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XrmFramework.Core
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Column
    {
        [JsonProperty("LogName")]
        public string LogicalName { get; set; }
        
        public string Name { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        public AttributeTypeCode Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PrimaryType PrimaryType { get; set; }

        [JsonProperty("Capa")]
        public AttributeCapabilities Capabilities { get; set; }

        public ICollection<LocalizedLabel> Labels { get; } = new List<LocalizedLabel>();

        [JsonProperty("StrLen")]
        public int? StringLength { get; set; }

        public double? MinRange { get; set; }

        public double? MaxRange { get; set; }

        [JsonProperty("DatBehav")]
        public DateTimeBehavior? DateTimeBehavior { get; set; }

        public bool IsMultiSelect { get; set; }

        public string EnumName { get; set; }

        [JsonProperty("Select")]
        public bool Selected { get; set; }

        /// <summary>
        /// Local marker for the framework's internal .table files: the column is frozen and must
        /// not be regenerated. Absent from CRM metadata, it is never produced by a generation
        /// command — it is only read back and rewritten as-is so it is not lost.
        /// </summary>
        [JsonProperty("Locked")]
        public bool IsLocked { get; set; }
    }
}
