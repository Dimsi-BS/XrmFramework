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
        /// already in place wins. The members follow the name: see <see cref="TakeNameAndMembers" />.
        /// </para>
        /// <para>
        /// Unless the option set is locked, in which case <see cref="LockArbitration.LockedName" />
        /// settles it ahead of the selection: a locked name belongs to the framework shipping it,
        /// not to the project tracking a copy of the table.
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
                    continue;
                }

                // Read before the lock is carried over: once both sides look locked there is no
                // lock left to arbitrate on, and the name would fall back to the selection rule.
                var naming = NamingCopy(existingOptionSet, optionSet, selectedByThis, selectedByExisting);

                // The lock describes the option set, not the copy that happened to be loaded
                // first: it survives the fold either way round.
                existingOptionSet.IsLocked |= optionSet.IsLocked;

                if (naming != null)
                {
                    TakeNameAndMembers(existingOptionSet, naming);
                }
            }

            MergeKeys(Keys, existingEntity.Keys);

            MergeRelations(ManyToOneRelationships, existingEntity.ManyToOneRelationships);
            MergeRelations(OneToManyRelationships, existingEntity.OneToManyRelationships);
            MergeRelations(ManyToManyRelationships, existingEntity.ManyToManyRelationships);
        }

        /// <summary>
        /// The copy whose name — and therefore whose members — the folded option set takes, or
        /// <see langword="null" /> when the one already in place keeps both.
        /// </summary>
        private static OptionSetEnum NamingCopy(OptionSetEnum existingOptionSet, OptionSetEnum optionSet,
                                                ISet<string> selectedByThis, ISet<string> selectedByExisting)
        {
            // A locked name belongs to the framework shipping it, whichever copy the fold keeps.
            // Exactly one of the two is locked whenever this returns a name, so the locked copy is
            // the one that names it.
            if (LockArbitration.LockedName(existingOptionSet.IsLocked, existingOptionSet.Name,
                                           optionSet.IsLocked, optionSet.Name) != null)
            {
                return existingOptionSet.IsLocked ? existingOptionSet : optionSet;
            }

            // Otherwise the copy that selects the column carrying it names it.
            return !string.IsNullOrEmpty(optionSet.Name)
                && selectedByThis.Contains(optionSet.LogicalName)
                && !selectedByExisting.Contains(optionSet.LogicalName)
                       ? optionSet
                       : null;
        }

        /// <summary>
        /// Takes onto <paramref name="existingOptionSet" /> the name and the members of the copy
        /// that names it.
        /// </summary>
        /// <remarks>
        /// A name and its members describe one generated enum and cannot come from different copies.
        /// The framework ships <c>team|teamtype</c> as <c>TeamType</c> with <c>Owner</c>,
        /// <c>Access</c>, ... and its own code writes <c>TeamType.Owner</c>; keeping the locked name
        /// over a project copy's members emitted <c>enum TeamType { Proprietaire, Acces, ... }</c>,
        /// which that code does not compile against — and which of the two won depended on the order
        /// the copies reached the fold in.
        /// </remarks>
        private static void TakeNameAndMembers(OptionSetEnum existingOptionSet, OptionSetEnum naming)
        {
            existingOptionSet.Name = naming.Name;

            if (ReferenceEquals(existingOptionSet, naming))
            {
                return;
            }

            existingOptionSet.Values.Clear();

            foreach (var value in naming.Values)
            {
                existingOptionSet.Values.Add(value);
            }
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
