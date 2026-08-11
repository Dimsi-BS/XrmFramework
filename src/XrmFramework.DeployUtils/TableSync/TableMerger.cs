// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Core;
using CoreTable = XrmFramework.Core.Table;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Merges metadata freshly read from the CRM with the content of an already versioned
    /// <c>.table</c> file.
    /// </summary>
    /// <remarks>
    /// Guiding principle: <b>everything that becomes a C# identifier belongs to the file, everything
    /// that describes the table belongs to the CRM.</b>
    ///
    /// The names (<c>Name</c>) feed the generated code and are frequently adjusted by hand
    /// (<c>ftp_numeroContrat</c> renamed to <c>NumeroContrat</c>). Overwriting them with the CRM's
    /// <c>SchemaName</c> would break the consumer project's compilation on every
    /// retrieval. Conversely, types, labels, capabilities and bounds are precisely what
    /// we are refreshing.
    ///
    /// This merge deliberately uses neither <see cref="CoreTable.MergeTo" /> nor
    /// <see cref="ColumnCollection" />.Add: their reconciliation rules, designed for
    /// selection in the old graphical tool, overwrite metadata and silently discard
    /// certain columns.
    /// </remarks>
    public static class TableMerger
    {
        /// <summary>
        /// Produces the table to write from the existing content and the CRM metadata.
        /// </summary>
        /// <param name="existing">
        /// Content of the versioned <c>.table</c>, or <see langword="null" /> on the first retrieval.
        /// </param>
        /// <param name="fresh">Table built from the Dataverse metadata.</param>
        public static CoreTable Merge(CoreTable existing, CoreTable fresh)
        {
            if (fresh == null)
                throw new ArgumentNullException(nameof(fresh));

            if (existing == null)
                return fresh;

            var merged = new CoreTable
            {
                LogicalName = fresh.LogicalName,
                CollectionName = fresh.CollectionName,

                // C# type name and local marker: property of the file.
                Name = string.IsNullOrEmpty(existing.Name) ? fresh.Name : existing.Name,
                IsLocked = existing.IsLocked
            };

            MergeColumns(existing, fresh, merged);
            MergeKeys(existing, fresh, merged);
            MergeEnums(existing, fresh, merged);

            // Relationships are identified by their own name: a manual rename would make them
            // impossible to reconcile. They are therefore taken as-is from the CRM.
            merged.OneToManyRelationships.AddRange(fresh.OneToManyRelationships);
            merged.ManyToOneRelationships.AddRange(fresh.ManyToOneRelationships);
            merged.ManyToManyRelationships.AddRange(fresh.ManyToManyRelationships);

            return merged;
        }

        // ──────────────────────────────────────────────────────────────────────
        // Columns
        // ──────────────────────────────────────────────────────────────────────

        private static void MergeColumns(CoreTable existing, CoreTable fresh, CoreTable merged)
        {
            var existingColumns = existing.Columns
                .Where(c => c.LogicalName != null)
                .GroupBy(c => c.LogicalName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var freshLogicalNames = new HashSet<string>(
                fresh.Columns.Select(c => c.LogicalName), StringComparer.OrdinalIgnoreCase);

            foreach (var freshColumn in fresh.Columns)
            {
                if (existingColumns.TryGetValue(freshColumn.LogicalName, out var existingColumn))
                {
                    // C# name, selection and lock come from the file; everything else from the CRM.
                    freshColumn.Name = existingColumn.Name;
                    freshColumn.Selected = existingColumn.Selected;
                    freshColumn.IsLocked = existingColumn.IsLocked;
                }

                merged.Columns.Add(freshColumn);
            }

            // A column deleted in the environment is kept: "pull" refreshes, it does not
            // destroy. It is "tables sync --clean" that handles de-selecting orphans.
            foreach (var orphan in existing.Columns.Where(c => !freshLogicalNames.Contains(c.LogicalName)))
                merged.Columns.Add(orphan);
        }

        /// <summary>
        /// Columns present in the file but absent from the CRM metadata.
        /// </summary>
        public static IReadOnlyList<Column> GetColumnsMissingFromCrm(CoreTable existing, CoreTable fresh)
        {
            if (existing == null || fresh == null)
                return new List<Column>();

            var freshLogicalNames = new HashSet<string>(
                fresh.Columns.Select(c => c.LogicalName), StringComparer.OrdinalIgnoreCase);

            return existing.Columns
                           .Where(c => !freshLogicalNames.Contains(c.LogicalName))
                           .ToList();
        }

        // ──────────────────────────────────────────────────────────────────────
        // Alternate keys
        // ──────────────────────────────────────────────────────────────────────

        /// <remarks>
        /// Keys are reconciled on <see cref="Key.EffectiveLogicalName" />, so that a file written
        /// before <see cref="Key.LogicalName" /> existed — the logical name sat in
        /// <see cref="Key.Name" /> back then — is recognized rather than treated as declaring no key
        /// at all, which would rename every one of its constants on the first pull.
        /// </remarks>
        private static void MergeKeys(CoreTable existing, CoreTable fresh, CoreTable merged)
        {
            var existingKeys = existing.Keys
                .Where(k => !string.IsNullOrEmpty(k.EffectiveLogicalName))
                .GroupBy(k => k.EffectiveLogicalName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            foreach (var freshKey in fresh.Keys)
            {
                if (!string.IsNullOrEmpty(freshKey.EffectiveLogicalName)
                    && existingKeys.TryGetValue(freshKey.EffectiveLogicalName, out var existingKey)
                    && !string.IsNullOrEmpty(existingKey.MemberName))
                {
                    // The key's name becomes a C# constant: it stays with the file.
                    freshKey.Name = existingKey.MemberName;
                }

                merged.Keys.Add(freshKey);
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Option sets
        // ──────────────────────────────────────────────────────────────────────

        private static void MergeEnums(CoreTable existing, CoreTable fresh, CoreTable merged)
        {
            var existingEnums = existing.Enums
                .Where(e => e.LogicalName != null)
                .GroupBy(e => e.LogicalName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var freshLogicalNames = new HashSet<string>(
                fresh.Enums.Select(e => e.LogicalName), StringComparer.OrdinalIgnoreCase);

            foreach (var freshEnum in fresh.Enums)
            {
                if (freshEnum.LogicalName != null
                    && existingEnums.TryGetValue(freshEnum.LogicalName, out var existingEnum))
                {
                    MergeEnum(existingEnum, freshEnum);
                }

                merged.Enums.Add(freshEnum);
            }

            foreach (var orphan in existing.Enums.Where(e => !freshLogicalNames.Contains(e.LogicalName)))
                merged.Enums.Add(orphan);
        }

        /// <summary>
        /// Applies to a freshly read option set the elements belonging to the file: its C# type
        /// name, its lock, and the name of each of its members (matched by numeric value,
        /// the only stable data — the CRM label may change without the code needing to follow).
        /// </summary>
        internal static void MergeEnum(OptionSetEnum existingEnum, OptionSetEnum freshEnum)
        {
            if (!string.IsNullOrEmpty(existingEnum.Name))
                freshEnum.Name = existingEnum.Name;

            freshEnum.IsLocked = existingEnum.IsLocked;

            var existingValues = existingEnum.Values
                .GroupBy(v => v.Value)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var freshValue in freshEnum.Values)
            {
                if (existingValues.TryGetValue(freshValue.Value, out var existingValue)
                    && !string.IsNullOrEmpty(existingValue.Name))
                {
                    freshValue.Name = existingValue.Name;
                }
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Global option sets
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Merges global option sets into the <c>OptionSet.table</c> pseudo-table.
        /// </summary>
        /// <remarks>
        /// Global option sets are shared by all tables: retrieving a single entity
        /// must never remove those referenced by others. The merge is therefore purely
        /// additive.
        /// </remarks>
        public static CoreTable MergeGlobalOptionSets(
            CoreTable existing, IEnumerable<OptionSetEnum> freshEnums)
        {
            var merged = new CoreTable
            {
                LogicalName = TableFileStore.GlobalOptionSetLogicalName,
                Name = TableFileStore.GlobalOptionSetFileName
            };

            if (existing != null)
            {
                merged.LogicalName = existing.LogicalName ?? merged.LogicalName;
                merged.Name = string.IsNullOrEmpty(existing.Name) ? merged.Name : existing.Name;
                merged.IsLocked = existing.IsLocked;
            }

            var existingEnums = (existing?.Enums ?? new List<OptionSetEnum>())
                .Where(e => e.LogicalName != null)
                .GroupBy(e => e.LogicalName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var refreshed = new Dictionary<string, OptionSetEnum>(StringComparer.OrdinalIgnoreCase);

            foreach (var freshEnum in freshEnums ?? Enumerable.Empty<OptionSetEnum>())
            {
                if (freshEnum.LogicalName == null)
                    continue;

                if (refreshed.TryGetValue(freshEnum.LogicalName, out var kept))
                {
                    // The same global option set reached through a second column. Both copies
                    // describe the same CRM choice, so the nullability one of them establishes
                    // holds for the other: keeping only the first would make the flag depend on
                    // which column the retrieval happened to walk first.
                    kept.HasNullValue |= freshEnum.HasNullValue;
                    continue;
                }

                if (existingEnums.TryGetValue(freshEnum.LogicalName, out var existingEnum))
                    MergeEnum(existingEnum, freshEnum);

                refreshed.Add(freshEnum.LogicalName, freshEnum);
                merged.Enums.Add(freshEnum);
            }

            foreach (var untouched in existingEnums.Values.Where(e => !refreshed.ContainsKey(e.LogicalName)))
                merged.Enums.Add(untouched);

            // Stable order: the file is versioned, an order dependent on the retrieved table
            // would produce spurious diffs on every run.
            var ordered = merged.Enums.OrderBy(e => e.LogicalName, StringComparer.OrdinalIgnoreCase).ToList();
            merged.Enums.Clear();
            merged.Enums.AddRange(ordered);

            return merged;
        }
    }
}
