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
        [JsonConverter(typeof(LegacyDateTimeBehaviorConverter))]
        public DateTimeBehavior? DateTimeBehavior { get; set; }

        public bool IsMultiSelect { get; set; }

        /// <summary>
        /// Whether the column holds several options at once — an <c>OptionSetValueCollection</c>
        /// rather than an <c>OptionSetValue</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Three shapes say so, and all three are in the wild. The CRM reports a multi-select choice
        /// as a <see cref="AttributeTypeCode.Virtual" /> attribute, so an older pull wrote
        /// <c>"Type": "Virtual"</c>; the current one writes <c>"Type": "Picklist"</c>; and a file
        /// edited by hand to work around the generator missing the flag altogether carries
        /// <c>"Type": "MultiSelectPicklist"</c>. The flag is what the three agree on — except the
        /// last, which says it through the type alone.
        /// </para>
        /// <para>
        /// Asking this rather than <see cref="IsMultiSelect" /> is what keeps every consumer of a
        /// <c>.table</c> — the C# generator, the mapping checks, the TypeScript definitions — reading
        /// the same answer out of whichever shape the file happens to carry.
        /// </para>
        /// </remarks>
        [JsonIgnore]
        public bool IsMultiSelectChoice
            => IsMultiSelect || Type == AttributeTypeCode.MultiSelectPicklist;

        /// <summary>
        /// The attribute type the generated code must declare — what the runtime mappers branch on.
        /// </summary>
        /// <remarks>
        /// <see cref="Type" /> is the type as the <c>.table</c> carries it, which for a multi-select
        /// choice is the type of nothing the SDK returns: a column written <c>Picklist</c> or
        /// <c>Virtual</c> alongside <see cref="IsMultiSelect" /> holds an
        /// <c>OptionSetValueCollection</c>, and declaring it <c>Picklist</c> is what made
        /// <c>Entity.GetAttributeValue&lt;OptionSetValue&gt;</c> throw on reading it.
        /// </remarks>
        [JsonIgnore]
        public AttributeTypeCode EffectiveType
            => IsMultiSelectChoice ? AttributeTypeCode.MultiSelectPicklist : Type;

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
