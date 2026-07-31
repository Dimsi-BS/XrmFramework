// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using XrmFramework.Core;
using CoreTable = XrmFramework.Core.Table;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Result of writing a retrieved table.
    /// </summary>
    public sealed class TablePullOutcome
    {
        internal TablePullOutcome(
            string filePath, bool created, CoreTable table, IReadOnlyList<Column> columnsMissingFromCrm)
        {
            FilePath = filePath;
            Created = created;
            Table = table;
            ColumnsMissingFromCrm = columnsMissingFromCrm;
        }

        /// <summary>Path of the written file.</summary>
        public string FilePath { get; }

        /// <summary>True if no file previously described this entity.</summary>
        public bool Created { get; }

        /// <summary>Table as written to disk.</summary>
        public CoreTable Table { get; }

        /// <summary>
        /// Columns present in the file but absent from the environment. They are
        /// kept: a retrieval refreshes, it does not destroy.
        /// </summary>
        public IReadOnlyList<Column> ColumnsMissingFromCrm { get; }
    }

    /// <summary>
    /// Reconciles a table freshly read from the CRM with the versioned <c>.table</c> file,
    /// then writes it.
    /// </summary>
    /// <remarks>
    /// Extracted from the command so that it and the tests exercise exactly the same path:
    /// locating the file, merging, writing. An orchestration duplicated in the tests would be
    /// blind to drifts in the actual command.
    /// </remarks>
    public static class TablePullWriter
    {
        /// <summary>
        /// Writes <paramref name="freshTable" /> into <paramref name="tablesDirectory" />, while
        /// preserving what belongs to the existing file (C# names, selection, locks).
        /// </summary>
        public static TablePullOutcome Write(string tablesDirectory, CoreTable freshTable)
        {
            if (string.IsNullOrWhiteSpace(tablesDirectory))
                throw new ArgumentException("The .table directory is required.", nameof(tablesDirectory));

            if (freshTable == null)
                throw new ArgumentNullException(nameof(freshTable));

            // The file is found by its logical name: its file name follows the C# name of the
            // table, which teams freely rename.
            var path = TableFileStore.FindTableFile(tablesDirectory, freshTable.LogicalName);
            var existing = path == null ? null : TableFileStore.Load(path);

            var merged = TableMerger.Merge(existing, freshTable);
            var missing = TableMerger.GetColumnsMissingFromCrm(existing, freshTable);

            path = path ?? TableFileStore.BuildTableFilePath(tablesDirectory, merged.Name);
            TableFileStore.Save(path, merged);

            return new TablePullOutcome(path, existing == null, merged, missing);
        }
    }
}
