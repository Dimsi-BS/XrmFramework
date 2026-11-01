using System;
using System.Collections.Generic;

namespace XrmFramework.Core
{
    /// <summary>
    /// The alternate keys the generated code declares, for one table.
    /// </summary>
    /// <remarks>
    /// Each key becomes one constant of the generated <c>AlternateKeyNames</c> class: a key the CRM
    /// knows under no name declares nothing, and two keys claiming the same constant declare it
    /// once. The rule lives here rather than in either generator, alongside
    /// <see cref="OptionSetSelection" /> and <see cref="RelationSelector" />: the C# and the
    /// TypeScript have to declare the same keys, and a copy of the rule in each is a copy that
    /// drifts.
    /// </remarks>
    public static class KeySelector
    {
        private static readonly IReadOnlyList<Key> None = new Key[0];

        /// <summary>The keys <paramref name="table" />'s generated code declares.</summary>
        public static IReadOnlyList<Key> Declared(Table table)
        {
            if (table?.Keys == null)
            {
                return None;
            }

            var declared = new List<Key>();

            // Ordinal: two names differing by case alone are two C# identifiers, hence two
            // constants, and neither shadows the other.
            var claimedNames = new HashSet<string>(StringComparer.Ordinal);

            foreach (var key in table.Keys)
            {
                if (string.IsNullOrEmpty(key?.EffectiveLogicalName) || !claimedNames.Add(key.MemberName))
                {
                    continue;
                }

                declared.Add(key);
            }

            return declared;
        }
    }
}
