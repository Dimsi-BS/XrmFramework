using System.Runtime.Serialization;

namespace Model.Sdk.Metadata
{
    [DataContract]
    public class ModelMetadata
    {
        [DataMember(Name = "entities")]
        public IList<EntityMetadata> Entities { get; set; } = new List<EntityMetadata>();

        [DataMember(Name = "enums")]
        public IList<EnumMetadata> Enums { get; set; } = new List<EnumMetadata>();
    }
}
