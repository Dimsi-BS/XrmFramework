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
        public DateTimeBehavior DateTimeBehavior { get; set; }

        public bool IsMultiSelect { get; set; }

        public string EnumName { get; set; }

        [JsonIgnore]
        public bool Selected { get; set; }
    }
}
