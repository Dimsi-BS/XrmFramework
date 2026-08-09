// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Spectre.Console;
using CoreTable = XrmFramework.Core.Table;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Second half of the 2.* -> 3.1 migration: once the <c>.table</c> files are up to date, cleans up the
    /// <c>*Definition.cs</c> files sitting next to them.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Under 2.*, the DefinitionManager wrote both a <c>.table</c> and its <c>*Definition.cs</c>. From 3.1
    /// on, <c>TableSourceFileGenerator</c> emits that class at compile time from the <c>.table</c> alone,
    /// so the checked-in file is no longer a source — it is a duplicate that breaks the build.
    /// </para>
    /// <para>
    /// For each file, the generated members are stripped (see <see cref="DefinitionSourceRewriter"/>).
    /// If nothing survives, the file is deleted; otherwise it is rewritten as
    /// <c>*Definition.partial.cs</c>, holding only what the project added by hand.
    /// </para>
    /// <para>
    /// A file is only touched when a <c>.table</c> in the directory declares the matching table — i.e.
    /// when the generator really will produce a replacement. A <c>*Definition.cs</c> with no <c>.table</c>
    /// behind it is reported and left alone: deleting it would drop the definition altogether.
    /// </para>
    /// <para>
    /// <c>OptionSetDefinitions.cs</c>, the separate file 2.* wrote for the option set enums, goes the
    /// same way: the enums the generator re-emits are stripped, and the file is deleted once nothing
    /// is left in it.
    /// </para>
    /// </remarks>
    public sealed class DefinitionFileMigrator
    {
        private const string DefinitionSuffix = "Definition";
        private const string CSharpExtension = ".cs";
        private const string PartialExtension = ".partial.cs";

        /// <summary>
        /// The file the 2.* DefinitionManager wrote all the option set enums into, next to the
        /// <c>*Definition.cs</c> files.
        /// </summary>
        private const string OptionSetFileName = "OptionSetDefinitions" + CSharpExtension;

        private readonly string _definitionsDirectory;

        public DefinitionFileMigrator(string definitionsDirectory)
        {
            if (!Directory.Exists(definitionsDirectory))
                throw new DirectoryNotFoundException(
                    $"Definition directory not found: {definitionsDirectory}");

            _definitionsDirectory = definitionsDirectory;
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Main entry point
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Migrates every <c>*Definition.cs</c> of the directory, plus its <c>OptionSetDefinitions.cs</c>.
        /// Returns the number of files skipped, which is what the caller needs to warn about a partial
        /// migration.
        /// </summary>
        public int Migrate()
        {
            var csFiles = Directory.GetFiles(_definitionsDirectory, "*" + CSharpExtension);

            var files = csFiles
                .Where(IsDefinitionFile)
                .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var optionSetFile = csFiles.FirstOrDefault(
                path => string.Equals(Path.GetFileName(path), OptionSetFileName, StringComparison.OrdinalIgnoreCase));

            if (files.Count == 0 && optionSetFile == null)
            {
                AnsiConsole.MarkupLine(
                    "[grey]No *Definition.cs to migrate: the directory already holds only .table files.[/]");
                return 0;
            }

            if (files.Count > 0)
                AnsiConsole.MarkupLine($"[bold]{files.Count}[/] *Definition.cs file(s) inherited from version 2.*.");

            var tables = LoadTables();
            var generatedEnums = CollectGeneratedEnumNames(tables);

            var deleted = 0;
            var rewritten = 0;
            var trimmed = 0;
            var skipped = 0;

            foreach (var file in files)
            {
                if (MigrateFile(file, tables, generatedEnums, ref deleted, ref rewritten))
                    continue;

                skipped++;
            }

            if (optionSetFile != null && !MigrateOptionSetFile(optionSetFile, generatedEnums, ref deleted, ref trimmed))
                skipped++;

            AnsiConsole.MarkupLine(
                $"  [bold]{deleted}[/] file(s) deleted, [bold]{rewritten}[/] converted to .partial.cs" +
                (trimmed > 0 ? $", [bold]{trimmed}[/] trimmed in place" : string.Empty) +
                (skipped > 0 ? $", [yellow]{skipped}[/] left untouched" : string.Empty) + ".");

            return skipped;
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Migrating a single file
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>Returns false when the file was left untouched.</summary>
        private bool MigrateFile(string file, IDictionary<string, CoreTable> tables,
                                 ICollection<string> generatedEnums, ref int deleted, ref int rewritten)
        {
            var fileName = Path.GetFileName(file);
            var className = Path.GetFileNameWithoutExtension(file);
            var tableName = className.Substring(0, className.Length - DefinitionSuffix.Length);

            if (!tables.ContainsKey(tableName))
            {
                Skipped(fileName, $"no .table declares the {tableName} table");
                return false;
            }

            string source;
            try
            {
                source = File.ReadAllText(file);
            }
            catch (IOException ex)
            {
                Skipped(fileName, ex.Message);
                return false;
            }

            var result = DefinitionSourceRewriter.Rewrite(source, className, generatedEnums);

            if (result.Outcome == DefinitionRewriteOutcome.Skipped)
            {
                Skipped(fileName, result.Reason);
                return false;
            }

            if (result.Outcome == DefinitionRewriteOutcome.Delete)
            {
                File.Delete(file);
                deleted++;
                AnsiConsole.MarkupLine(
                    $"[red]Deleted[/] {Markup.Escape(fileName)} " +
                    "(entirely regenerated from the .table)");
                return true;
            }

            var partialPath = Path.Combine(_definitionsDirectory, className + PartialExtension);

            if (File.Exists(partialPath))
            {
                Skipped(fileName,
                    $"{className + PartialExtension} already exists — merge the two files by hand");
                return false;
            }

            File.WriteAllText(partialPath, result.NewText, DetectEncoding(file));
            File.Delete(file);
            rewritten++;

            AnsiConsole.MarkupLine(
                $"[blue]Converted[/] {Markup.Escape(fileName)} -> {Markup.Escape(className + PartialExtension)} " +
                $"([bold]{result.KeptMembers.Count}[/] member(s) kept: " +
                $"{Markup.Escape(string.Join(", ", result.KeptMembers.ToArray()))})");

            return true;
        }

        /// <summary>
        /// Migrates <c>OptionSetDefinitions.cs</c>: the enums the generator re-emits are dropped, and the
        /// file with them once nothing is left. Returns false when it was left untouched.
        /// </summary>
        /// <remarks>
        /// Unlike a <c>*Definition.cs</c>, this file is backed by no single <c>.table</c> — it gathers the
        /// option sets of the whole directory — so what vouches for it is the set of enums the generator
        /// will emit. When that set is empty, nothing here is regenerated and the file is left alone
        /// rather than deleted: this is the signature of a wrong directory, or of <c>.table</c> files that
        /// declare no selected option set column.
        /// </remarks>
        private static bool MigrateOptionSetFile(string file, ICollection<string> generatedEnums,
                                                 ref int deleted, ref int trimmed)
        {
            var fileName = Path.GetFileName(file);

            string source;
            try
            {
                source = File.ReadAllText(file);
            }
            catch (IOException ex)
            {
                Skipped(fileName, ex.Message);
                return false;
            }

            var result = DefinitionSourceRewriter.RewriteOptionSets(source, generatedEnums);

            if (result.Outcome == DefinitionRewriteOutcome.Skipped)
            {
                Skipped(fileName, result.Reason);
                return false;
            }

            if (result.Outcome == DefinitionRewriteOutcome.Delete)
            {
                File.Delete(file);
                deleted++;
                AnsiConsole.MarkupLine(
                    $"[red]Deleted[/] {Markup.Escape(fileName)} " +
                    "(option set enums entirely regenerated from the .table files)");
                return true;
            }

            if (result.RemovedEnums.Count == 0)
            {
                Skipped(fileName, "no enum of this file is regenerated from the .table files");
                return false;
            }

            File.WriteAllText(file, result.NewText, DetectEncoding(file));
            trimmed++;

            AnsiConsole.MarkupLine(
                $"[blue]Trimmed[/] {Markup.Escape(fileName)} " +
                $"([bold]{result.RemovedEnums.Count}[/] enum(s) removed, " +
                $"[bold]{result.KeptMembers.Count}[/] kept: " +
                $"{Markup.Escape(string.Join(", ", result.KeptMembers.ToArray()))})");

            return true;
        }

        private static void Skipped(string fileName, string reason)
            => AnsiConsole.MarkupLine(
                $"[yellow]Left as-is[/] {Markup.Escape(fileName)} ({Markup.Escape(reason ?? "unknown reason")})");

        // ──────────────────────────────────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// A <c>*Definition.cs</c> that is not already a <c>*Definition.partial.cs</c> — the latter is what
        /// this migration produces, and re-running it must not process its own output.
        /// </summary>
        private static bool IsDefinitionFile(string path)
        {
            var name = Path.GetFileName(path);

            return name.EndsWith(DefinitionSuffix + CSharpExtension, StringComparison.OrdinalIgnoreCase)
                && name.Length > (DefinitionSuffix + CSharpExtension).Length;
        }

        /// <summary>
        /// Indexes the directory's <c>.table</c> files by their declared <c>Name</c> — the name the
        /// generator derives the class from, which does not always match the file name
        /// (<c>Systemuser.table</c> declares <c>SystemUser</c>).
        /// </summary>
        private IDictionary<string, CoreTable> LoadTables()
        {
            var tables = new Dictionary<string, CoreTable>(StringComparer.OrdinalIgnoreCase);

            foreach (var path in Directory.GetFiles(_definitionsDirectory, "*.table"))
            {
                CoreTable table;
                try
                {
                    table = JsonConvert.DeserializeObject<CoreTable>(File.ReadAllText(path));
                }
                catch (JsonException)
                {
                    // An unreadable .table is the .table synchronization's problem, not this step's:
                    // it simply does not vouch for any *Definition.cs.
                    continue;
                }

                if (table != null && !string.IsNullOrEmpty(table.Name))
                    tables[table.Name] = table;
            }

            return tables;
        }

        /// <summary>
        /// Names of the option set enums <c>TableSourceFileGenerator</c> will emit for these tables.
        /// </summary>
        /// <remarks>
        /// The generator only emits an enum when a <b>selected</b> column references it — global option
        /// sets across all tables, local ones within their own table. An enum no column uses is therefore
        /// not regenerated, and the corresponding C# enum must be kept.
        /// </remarks>
        private static ICollection<string> CollectGeneratedEnumNames(IDictionary<string, CoreTable> tables)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            var all = tables.Values.ToList();

            foreach (var table in all)
                foreach (var optionSet in table.Enums)
                {
                    if (string.IsNullOrEmpty(optionSet.Name))
                        continue;

                    var scope = optionSet.IsGlobal ? all : new List<CoreTable> { table };

                    if (scope.Any(t => t.Columns.Any(
                            c => c.Selected
                              && string.Equals(c.EnumName, optionSet.LogicalName, StringComparison.OrdinalIgnoreCase))))
                    {
                        names.Add(optionSet.Name);
                    }
                }

            return names;
        }

        /// <summary>
        /// Reuses the source file's encoding so that migration alone does not add or drop a BOM.
        /// </summary>
        private static Encoding DetectEncoding(string path)
        {
            var preamble = new byte[3];

            using (var stream = File.OpenRead(path))
            {
                var read = stream.Read(preamble, 0, preamble.Length);
                var hasBom = read == 3 && preamble[0] == 0xEF && preamble[1] == 0xBB && preamble[2] == 0xBF;

                return new UTF8Encoding(hasBom);
            }
        }
    }
}
