using System.Collections.Generic;
using Newtonsoft.Json;

namespace XrmFramework.DefinitionManager.Model
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Entity
    {
        [JsonProperty("LogName")]
        public string LogicalName { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("CollName")]
        public string CollectionName { get; set; }

        [JsonProperty("Attrs")]
        public List<Attribute> Attributes { get; } = new();
        
        public List<Relation> ManyToManyRelationships { get; } = new();

        public List<Relation> OneToManyRelationships { get; } = new();

        public List<Relation> ManyToOneRelationships { get; } = new();

        public ICollection<Key> Keys { get; set; }

        public List<OptionSetEnum> Enums { get; } = new();

        [JsonIgnore]
        public bool Selected { get; set; }
    }
}
