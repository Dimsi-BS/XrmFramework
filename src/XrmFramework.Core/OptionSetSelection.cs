using System;
using System.Collections.Generic;
using System.Linq;

namespace XrmFramework.Core
{
    /// <summary>
    /// One option set the table source generator declares, and the file it declares it in.
    /// </summary>
    public sealed class GeneratedOptionSet
    {
        internal GeneratedOptionSet(Table table, OptionSetEnum optionSet, Column column)
        {
            Table = table;
            OptionSet = optionSet;
            Column = column;
        }

        /// <summary>The table whose generated file carries the enum.</summary>
        public Table Table { get; }

        /// <summary>The option set the enum takes its name and its members from.</summary>
        public OptionSetEnum OptionSet { get; }

        /// <summary>
        /// The column the <c>[OptionSetDefinition]</c> attribute names, for an option set local to
        /// <see cref="Table" />. A global option set is attached to its logical name instead and
        /// belongs to no column in particular: <see langword="null" />.
        /// </summary>
        public Column Column { get; }
    }

    /// <summary>
    /// Several option sets the generator would declare under one C# name.
    /// </summary>
    public sealed class OptionSetNameConflict
    {
        internal OptionSetNameConflict(string name, IReadOnlyList<GeneratedOptionSet> claims)
        {
            Name = name;
            Claims = claims;
        }

        /// <summary>The C# name claimed more than once.</summary>
        public string Name { get; }

        /// <summary>The option sets claiming it, the one the generator keeps coming first.</summary>
        public IReadOnlyList<GeneratedOptionSet> Claims { get; }
    }

    /// <summary>
    /// The option sets the table source generator turns into enums, for a given set of tables.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the one place the rule lives. The generator walks <see cref="In" /> to emit the enums;
    /// the 2.* -> 3.1 migration reads <see cref="Names" /> to know which hand-written enums it may
    /// delete from <c>OptionSetDefinitions.cs</c>. The two used to decide on their own, and disagreed:
    /// the migration deleted enums the generator then declined to emit, leaving the project with
    /// columns typed on enums that existed nowhere.
    /// </para>
    /// <para>
    /// An option set becomes an enum when a <b>selected</b> column carries it — global option sets
    /// across every table, local ones within the table declaring them. An unselected column reaches
    /// no generated code, so the option set behind it would be declared for nobody.
    /// </para>
    /// <para>
    /// One C# name yields one enum. When several option sets claim the same name, the first is
    /// declared and the others are reported through <see cref="Conflicts" />, for the generator to
    /// turn into a diagnostic: nothing here picks a different name on the project's behalf, since
    /// that name is what its code already refers to.
    /// </para>
    /// </remarks>
    public sealed class OptionSetSelection
    {
        private static readonly IReadOnlyList<GeneratedOptionSet> None = new GeneratedOptionSet[0];

        private readonly IDictionary<Table, IReadOnlyList<GeneratedOptionSet>> _byTable;

        private OptionSetSelection(IDictionary<Table, IReadOnlyList<GeneratedOptionSet>> byTable,
                                   ISet<string> names,
                                   IReadOnlyList<OptionSetNameConflict> conflicts)
        {
            _byTable = byTable;
            Names = names;
            Conflicts = conflicts;
        }

        /// <summary>
        /// Names of every enum the generator declares — and therefore of every hand-written enum the
        /// migration may delete.
        /// </summary>
        public ISet<string> Names { get; }

        /// <summary>The names claimed by more than one option set, in name order.</summary>
        public IReadOnlyList<OptionSetNameConflict> Conflicts { get; }

        /// <summary>The option sets the generated file of <paramref name="table" /> declares.</summary>
        public IReadOnlyList<GeneratedOptionSet> In(Table table)
        => table != null && _byTable.TryGetValue(table, out var optionSets) ? optionSets : None;

        /// <summary>Works out what the generator declares for <paramref name="tables" />.</summary>
        public static OptionSetSelection Of(IEnumerable<Table> tables)
        {
            var all = tables == null
                          ? new List<Table>()
                          : tables.Where(t => t != null).ToList();

            var byTable = new Dictionary<Table, IReadOnlyList<GeneratedOptionSet>>();

            // Ordinal throughout: two names differing by case alone are two C# identifiers, hence two
            // enums, and neither shadows the other.
            var claims = new Dictionary<string, GeneratedOptionSet>(StringComparer.Ordinal);
            var contested = new Dictionary<string, List<GeneratedOptionSet>>(StringComparer.Ordinal);

            foreach (var table in all)
            {
                List<GeneratedOptionSet> declared = null;

                foreach (var optionSet in table.Enums)
                {
                    // A nameless option set is one no .table ever named: there is no enum to declare,
                    // and no type the columns carrying it could be attributed to.
                    if (optionSet == null || string.IsNullOrEmpty(optionSet.Name))
                    {
                        continue;
                    }

                    var column = SelectedColumnCarrying(optionSet, table, all);

                    if (column == null)
                    {
                        continue;
                    }

                    var generated = new GeneratedOptionSet(table, optionSet,
                                                           optionSet.IsGlobal ? null : column);

                    if (claims.TryGetValue(optionSet.Name, out var owner))
                    {
                        // The same option set declared by two copies of a table is one enum, not a
                        // conflict: that is precisely what this de-duplication is for.
                        if (!IsSameOptionSet(owner.OptionSet, optionSet))
                        {
                            if (!contested.TryGetValue(optionSet.Name, out var claimants))
                            {
                                claimants = new List<GeneratedOptionSet> { owner };
                                contested.Add(optionSet.Name, claimants);
                            }

                            claimants.Add(generated);
                        }

                        continue;
                    }

                    claims.Add(optionSet.Name, generated);

                    (declared ?? (declared = new List<GeneratedOptionSet>())).Add(generated);
                }

                if (declared != null)
                {
                    byTable[table] = declared;
                }
            }

            var conflicts = contested.OrderBy(pair => pair.Key, StringComparer.Ordinal)
                                     .Select(pair => new OptionSetNameConflict(pair.Key, pair.Value))
                                     .ToList();

            return new OptionSetSelection(byTable,
                                          new HashSet<string>(claims.Keys, StringComparer.Ordinal),
                                          conflicts);
        }

        /// <summary>
        /// The selected column that makes the generator declare <paramref name="optionSet" />, or
        /// <see langword="null" /> when none does.
        /// </summary>
        /// <remarks>
        /// A global option set is looked for across every table, a local one only in the table
        /// declaring it — where the column is also what its <c>[OptionSetDefinition]</c> names.
        /// </remarks>
        private static Column SelectedColumnCarrying(OptionSetEnum optionSet, Table table,
                                                     IEnumerable<Table> all)
        {
            if (string.IsNullOrEmpty(optionSet.LogicalName))
            {
                return null;
            }

            var scope = optionSet.IsGlobal ? all : new[] { table };

            return scope.SelectMany(t => t.Columns)
                        .FirstOrDefault(c => c.Selected
                                          && string.Equals(c.EnumName, optionSet.LogicalName,
                                                           StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Whether both declarations describe the one option set of the CRM, which no project renames.
        /// </summary>
        private static bool IsSameOptionSet(OptionSetEnum left, OptionSetEnum right)
        => string.Equals(left.LogicalName, right.LogicalName, StringComparison.OrdinalIgnoreCase);
    }
}
