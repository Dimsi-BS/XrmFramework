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
        /// The merge is additive. When both files declare the same option set, its C# name comes from
        /// the copy that selects the column carrying it: only selected columns are generated, so those
        /// are the only option sets whose name the copy in question actually depends on. Everywhere
        /// else the name belongs to the consuming project, and a rename applied to its own
        /// <c>.table</c> takes effect. Should both copies — or neither — select the column, the copy
        /// already in place wins. The members always stay those of that same copy.
        /// </para>
        /// <para>
        /// Relationships are folded in the same way, so that <see cref="RelationSelector" /> decides on
        /// the union of what both copies declare: the project's copy of <c>account</c> knows the 1:N
        /// towards a table the framework's copy never heard of, and the other way around.
        /// </para>
        /// <para>
        /// Alternate keys likewise. Leaving them out gave the copy loaded first the last word on the
        /// whole <c>AlternateKeyNames</c> class: the framework ships <c>systemuser</c> with the one key
        /// it needs, and every key the project tracked on its own copy disappeared from the generated
        /// code.
        /// </para>
        /// </remarks>
        public void MergeTo(Table existingEntity)
        {
            if (existingEntity == null)
            {
                return;
            }

            // Read on both sides before the columns are merged: ColumnCollection.Add propagates a
            // selection onto the copy it keeps, after which both sides look selected.
            var selectedByExisting = SelectedOptionSets(existingEntity);
            var selectedByThis = SelectedOptionSets(this);

            Columns.ToList().ForEach(existingEntity.Columns.Add);

            foreach (var optionSet in Enums)
            {
                if (optionSet?.LogicalName == null)
                {
                    continue;
                }

                var existingOptionSet = existingEntity.Enums.FirstOrDefault(
                    e => string.Equals(e.LogicalName, optionSet.LogicalName, StringComparison.OrdinalIgnoreCase));

                if (existingOptionSet == null)
                {
                    existingEntity.Enums.Add(optionSet);
                }
                else if (!string.IsNullOrEmpty(optionSet.Name)
                         && selectedByThis.Contains(optionSet.LogicalName)
                         && !selectedByExisting.Contains(optionSet.LogicalName))
                {
                    existingOptionSet.Name = optionSet.Name;
                }
            }

            MergeKeys(Keys, existingEntity.Keys);

            MergeRelations(ManyToOneRelationships, existingEntity.ManyToOneRelationships);
            MergeRelations(OneToManyRelationships, existingEntity.OneToManyRelationships);
            MergeRelations(ManyToManyRelationships, existingEntity.ManyToManyRelationships);
        }

        /// <summary>
        /// Adds to <paramref name="existingKeys" /> the alternate keys it does not already know.
        /// </summary>
        /// <remarks>
        /// A key is identified by its logical name, which is the one the CRM is queried on and which
        /// no project renames: two copies declaring it describe the same key. The copy already in
        /// place therefore keeps the C# name it gives it — the code shipped alongside that copy
        /// already refers to it under that name — and only what it leaves empty is taken from the
        /// other copy.
        /// </remarks>
        private static void MergeKeys(ICollection<Key> keys, ICollection<Key> existingKeys)
        {
            foreach (var key in keys)
            {
                if (string.IsNullOrEmpty(key?.EffectiveLogicalName))
                {
                    continue;
                }

                var existingKey = existingKeys.FirstOrDefault(
                    k => string.Equals(k?.EffectiveLogicalName, key.EffectiveLogicalName,
                                       StringComparison.OrdinalIgnoreCase));

                if (existingKey == null)
                {
                    existingKeys.Add(key);
                    continue;
                }

                if (string.IsNullOrEmpty(existingKey.Name))
                {
                    existingKey.Name = key.Name;
                }

                // The columns a key rests on come from the CRM as well. A copy listing none is simply
                // older than the other, and keeping it as is would cost those columns the
                // [AlternateKey] attribute standing for the key.
                if (existingKey.FieldNames.Count == 0)
                {
                    existingKey.FieldNames.AddRange(key.FieldNames);
                }
            }
        }

        /// <summary>
        /// Adds to <paramref name="existingRelations" /> the relationships it does not already know.
        /// </summary>
        /// <remarks>
        /// A relationship is identified by its schema name, which belongs to the CRM and no project
        /// renames — unlike a table, a column or an option set, there is nothing to arbitrate here, so
        /// the copy already in place is simply kept.
        /// </remarks>
        private static void MergeRelations(List<Relation> relations, List<Relation> existingRelations)
        {
            var knownNames = new HashSet<string>(existingRelations.Where(r => !string.IsNullOrEmpty(r?.Name))
                                                                 .Select(r => r.Name),
                                                 StringComparer.OrdinalIgnoreCase);

            foreach (var relation in relations)
            {
                if (string.IsNullOrEmpty(relation?.Name) || !knownNames.Add(relation.Name))
                {
                    continue;
                }

                existingRelations.Add(relation);
            }
        }

        /// <summary>
        /// Logical names of the option sets carried by a selected column of <paramref name="table" />,
        /// the only ones this copy of the table has a say in the naming of.
        /// </summary>
        private static HashSet<string> SelectedOptionSets(Table table)
        => new HashSet<string>(table.Columns.Where(c => c.Selected && !string.IsNullOrEmpty(c.EnumName))
                                            .Select(c => c.EnumName),
                               StringComparer.OrdinalIgnoreCase);

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
