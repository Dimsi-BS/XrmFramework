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

        public string ModelNamespace { get; set; }

        //Contains the logical name of the columns we want to include in the binding model
        [JsonProperty("Cols")]
        public ICollection<ModelProperty> Properties { get; } = new List<ModelProperty>();

        public MemberSerialization? JsonMemberSerializationStrategy { get; set; }
    }
}
