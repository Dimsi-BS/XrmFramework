// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace XrmFramework.Core
{
    /// <summary>
    /// Picks, among the relationships a table declares, those the generated definition turns into
    /// constants.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <c>.table</c> file carries every relationship the CRM reports for the table — <c>systemuser</c>
    /// alone comes with about four thousand of them. Generating a constant for each buries the handful
    /// a project names under thousands it will never name, and makes the generated code grow with the
    /// environment instead of with the project.
    /// </para>
    /// <para>
    /// A relationship earns its constant when a <b>selected lookup column</b> stands behind it,
    /// whichever side of the relationship carries that column. Selecting <c>contact.accountid</c> is
    /// therefore enough to produce both ends of the pair: the N:1 by which a contact reaches its
    /// account, and the 1:N by which an account reaches its contacts.
    /// </para>
    /// <para>
    /// N:N relationships have no lookup column to select — the two they rest on belong to an intersect
    /// table no project declares. They keep the only rule that ever applied to them: the table at the
    /// other end has to be part of the compilation.
    /// </para>
    /// <para>
    /// The relationships examined are those of the merged table, so both what the framework's
    /// <c>.table</c> and the project's own copy declare — see <see cref="Table.MergeTo" />.
    /// </para>
    /// </remarks>
    public static class RelationSelector
    {
        private static readonly IReadOnlyList<Relation> None = new Relation[0];

        /// <summary>
        /// N:1 relationships of <paramref name="table" /> reached through one of its selected lookup
        /// columns.
        /// </summary>
        /// <remarks>
        /// The rule is the one column generation already follows: a selected lookup column is emitted
        /// with a <c>[CrmLookup(..., RelationshipName = ManyToOneRelationships.X)]</c> attribute, so
        /// the constant it names must exist — and no other one has to.
        /// <para>
        /// The table at the other end may well be absent from the compilation: the relationship then
        /// names it as a literal, exactly as before. Dropping it would leave the attribute above
        /// pointing at nothing.
        /// </para>
        /// </remarks>
        public static IReadOnlyList<Relation> ManyToOne(Table table)
        => table == null
               ? None
               : table.ManyToOneRelationships
                      .Where(relation => SelectsLookup(table, relation?.LookupFieldName))
                      .ToList();

        /// <summary>
        /// 1:N relationships of <paramref name="table" /> whose lookup column, carried by the table at
        /// the other end, is selected there.
        /// </summary>
        /// <remarks>
        /// This is the mirror image of <see cref="ManyToOne" />: the same selected lookup answers for
        /// both, so the two ends of a relationship are always generated together — or not at all. A
        /// referencing table left out of the compilation answers for nothing and takes its 1:N with it.
        /// </remarks>
        public static IReadOnlyList<Relation> OneToMany(TableCollection tables, Table table)
        => tables == null || table == null
               ? None
               : table.OneToManyRelationships
                      .Where(relation => SelectsLookup(tables.Get(relation?.EntityName),
                                                       relation?.LookupFieldName))
                      .ToList();

        /// <summary>
        /// N:N relationships of <paramref name="table" /> whose other end is part of the compilation.
        /// </summary>
        public static IReadOnlyList<Relation> ManyToMany(TableCollection tables, Table table)
        => tables == null || table == null
               ? None
               : table.ManyToManyRelationships
                      .Where(relation => tables.Get(relation?.EntityName) != null)
                      .ToList();

        /// <summary>
        /// Tells whether <paramref name="table" /> selects the column named
        /// <paramref name="lookupFieldName" />, the lookup a relationship rests on.
        /// </summary>
        private static bool SelectsLookup(Table table, string lookupFieldName)
        => table != null
           && !string.IsNullOrEmpty(lookupFieldName)
           && table.Columns.Any(column => column.Selected
                                          && string.Equals(column.LogicalName, lookupFieldName,
                                                           StringComparison.OrdinalIgnoreCase));
    }
}
