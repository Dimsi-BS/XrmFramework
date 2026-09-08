using Newtonsoft.Json;
using System.Collections.Generic;

namespace XrmFramework.Core
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Model
    {
        [JsonProperty("tName")]
        public string TableLogicalName { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("ns")]
        public string ModelNamespace { get; set; }

        //Contains the logical name of the columns we want to include in the binding model
        [JsonProperty("Cols")]
        public ICollection<ModelProperty> Properties { get; } = new List<ModelProperty>();

        public MemberSerialization? JsonMemberSerializationStrategy { get; set; }

        /// <summary>
        ///     Extra namespaces to import into the generated file, so a property's
        ///     <see cref="ModelProperty.Attrs" /> entries can name an attribute without qualifying
        ///     it — e.g. add <c>"System.ComponentModel.DataAnnotations"</c> once instead of writing
        ///     <c>"System.ComponentModel.DataAnnotations.StringLength(100)"</c> on every property.
        /// </summary>
        public string[] Usings { get; set; }

        [JsonIgnore]
        public string TypeFullName => $"{ModelNamespace}.{Name}";
    }
}
