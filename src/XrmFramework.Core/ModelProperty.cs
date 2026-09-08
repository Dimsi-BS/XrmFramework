using System.ComponentModel;
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

        /// <summary>
        ///     Which table a polymorphic lookup — <c>Customer</c>, <c>Owner</c>, a Regarding
        ///     column — points at for this property. Such a column declares several many-to-one
        ///     relationships, and nothing else says which one the property maps.
        /// </summary>
        public string LookupTargetTableLogicalName { get; set; }

        /// <summary>
        ///     Whether this property embeds another binding model behind the lookup, filled from
        ///     the record it points at rather than carrying the lookup's own value.
        ///     <see cref="TypeFullName" /> names that model's class — the same field an
        ///     <see cref="ExtendBindingModel" /> property already uses to name what it nests, so a
        ///     model's target is always spelled in the one place. Mutually exclusive with
        ///     <see cref="LookupTargetColumnLogicalName" />: the two describe different things to
        ///     read through the same lookup.
        /// </summary>
        public bool LookupTargetModel { get; set; }

        /// <summary>
        ///     A single column of the targeted record to project onto this property, reached
        ///     through a <c>LinkEntity</c> aliased on the lookup column's logical name.
        /// </summary>
        public string LookupTargetColumnLogicalName { get; set; }

        /// <summary>
        ///     Carries another binding model over the <em>same</em> record. <see cref="TypeFullName" />
        ///     names it, and that model must be declared by a <c>.model</c> file targeting the same
        ///     table — nothing is read through a lookup here, both halves describe one row.
        /// </summary>
        /// <remarks>
        ///     What keeps a payload's shape when part of it is nested — <c>"prospect": { … }</c> —
        ///     rather than flattened onto the parent. The property carries no
        ///     <see cref="LogicalName" />: it maps no column of its own, its model does.
        /// </remarks>
        public bool ExtendBindingModel { get; set; }

        /// <summary>
        ///     Allows the link to be followed beyond the first level. Below that depth the query
        ///     builder stops unless the property asks for it, which is what keeps a model from
        ///     dragging in the whole graph.
        /// </summary>
        public bool FollowLink { get; set; }

        /// <summary>
        ///     Tolerates a targeted record that does not exist, instead of failing the mapping.
        ///     Third argument of <c>[CrmLookup]</c>.
        /// </summary>
        public bool AllowNotExisting { get; set; }

        /// <remarks>
        ///     <c>[DefaultValue(true)]</c> tells <c>DefaultValueHandling.Ignore</c> serialization
        ///     — used when writing a <c>.model</c> file back out, e.g. by <c>migrate sync-models</c>
        ///     — that <see langword="true" /> is the value to omit, not the CLR default
        ///     <see langword="false" />; without it a property meaning "read-only" would silently
        ///     round-trip as the default "read-write".
        /// </remarks>
        [JsonProperty("UsePropCh")]
        [DefaultValue(true)]
        public bool IsValidForUpdate { get; set; } = true;

        public string JsonPropertyName { get; set; }

        public string JsonConverterType { get; set; }

        /// <summary>Emits <c>[JsonIgnore]</c> on the generated property.</summary>
        public bool JsonIgnore { get; set; }

        public string[] JsonConverterConstructorArguments { get; set; }

        public string ModelConverterType { get; set; }

        public string ModelConverterConstructorArguments { get; set; }

        /// <summary>
        ///     Extra attributes to emit verbatim on the generated property — e.g.
        ///     <c>"StringLength(100)"</c> or <c>"DataMember(Name = \"Foo\")"</c>. Each entry is
        ///     wrapped in <c>[...]</c> exactly as written, so its type must already be in scope;
        ///     see <see cref="Model.Usings" /> to add a namespace once for the whole file instead
        ///     of qualifying every entry.
        /// </summary>
        public string[] Attrs { get; set; }
    }
}
