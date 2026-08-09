using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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

        [JsonProperty("Cols", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public ColumnCollection Columns { get; } = new ColumnCollection();

        [JsonProperty("NtoN", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public List<Relation> ManyToManyRelationships { get; } = new();

        [JsonProperty("OneToN", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public List<Relation> OneToManyRelationships { get; } = new();

        [JsonProperty("NToOne", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public List<Relation> ManyToOneRelationships { get; } = new();

        [JsonProperty("Locked")]
        public bool IsLocked { get; set; } = false;

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public ICollection<Key> Keys { get; } = new List<Key>();

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public List<OptionSetEnum> Enums { get; } = new();

        [JsonIgnore]
        public bool Selected { get; set; }

        /// <summary>
        /// Folds this table into <paramref name="existingEntity" />, the copy the
        /// <see cref="TableCollection" /> already holds for the same logical name.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A table shipped by the framework and tracked again by the project is declared twice: the
        /// package's <c>.table</c> and the project's own copy. Only the first one loaded survives in the
        /// collection, so whatever the other one brings has to be folded into it — its columns, and the
        /// option sets those columns reference.
        /// </para>
        /// <para>
        /// Merging the columns alone left a column selected only in the project's copy pointing at an
        /// option set nobody declared any more: the generator emitted it without its <c>[OptionSet]</c>
        /// attribute, and emitted no <c>enum</c> for it.
        /// </para>
        /// <para>
        /// The merge is additive, and on a conflict the copy already in place wins: an option set both
        /// files declare keeps the name and the members of the one loaded first. A rename applied to a
        /// single copy therefore stays without effect — rename it in both.
        /// </para>
        /// </remarks>
        public void MergeTo(Table existingEntity)
        {
            if (existingEntity == null)
            {
                return;
            }

            Columns.ToList().ForEach(existingEntity.Columns.Add);

            foreach (var optionSet in Enums)
            {
                if (optionSet?.LogicalName == null
                    || existingEntity.Enums.Any(e => string.Equals(e.LogicalName, optionSet.LogicalName,
                                                                   StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                existingEntity.Enums.Add(optionSet);
            }
        }

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
