using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace XrmFramework.Core
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Table : IComparable<Table>
    {
        [JsonProperty("LogName")]
        public string LogicalName { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("CollName")]
        public string CollectionName { get; set; }

        [JsonProperty("Cols")]
        public ColumnCollection Columns { get; } = new();
        
        [JsonProperty("NtoN")]
        public List<Relation> ManyToManyRelationships { get; } = new();

        [JsonProperty("OneToN")]
        public List<Relation> OneToManyRelationships { get; } = new();

        [JsonProperty("NToOne")]
        public List<Relation> ManyToOneRelationships { get; } = new();

        public ICollection<Key> Keys { get; set; }

        public List<OptionSetEnum> Enums { get; } = new();

        [JsonIgnore]
        public bool Selected { get; set; }

        public void MergeTo(Table existingEntity) 
            => existingEntity?.Columns.MergeColumns(Columns);

        public int CompareTo(Table other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var nameComparison = string.Compare(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
            if (nameComparison != 0) return nameComparison;
            return string.Compare(LogicalName, other.LogicalName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
