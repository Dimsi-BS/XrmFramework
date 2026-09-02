// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Spectre.Console;
using XrmFramework.Core;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Service;
using CoreTable = XrmFramework.Core.Table;
using DataverseMetadata = Microsoft.Xrm.Sdk.Metadata;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Commands connected to the environment: list tables and retrieve their metadata
    /// as <c>.table</c> files.
    /// </summary>
    /// <remarks>
    /// In accordance with the CLI contract, these entry points return an exit code and
    /// never call <c>Environment.Exit</c>.
    /// </remarks>
    public static class CrmTableHelper
    {
        /// <summary>Success.</summary>
        public const int ExitSuccess = 0;

        /// <summary>No table matches the requested criteria.</summary>
        public const int ExitNoMatch = 1;

        /// <summary>Configuration or directory not found.</summary>
        public const int ExitNotFound = 2;

        /// <summary>Unexpected error.</summary>
        public const int ExitError = 3;

        // ══════════════════════════════════════════════════════════════════════
        // tables list
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Displays the tables of the selected environment, filterable by prefix.
        /// </summary>
        /// <param name="projectRoot">Explicit root, or <see langword="null" /> to discover it.</param>
        /// <param name="prefix">Only keep logical names starting with this prefix.</param>
        /// <param name="filter">Substring searched for in the logical name or the label.</param>
        /// <param name="customOnly">Only keep custom tables.</param>
        public static int List(string projectRoot, string prefix, string filter, bool customOnly)
        {
            try
            {
                var location = ResolveLocation(projectRoot);
                if (location == null)
                    return ExitNotFound;

                var connectionString = ResolveConnectionString(location.ProjectRoot, out var url);
                AnsiConsole.MarkupLine($"Environment: [cyan]{Markup.Escape(url ?? "unknown")}[/]");
                AnsiConsole.WriteLine();

                var service = Connect(connectionString);

                var entities = RetrieveEntityList(service)
                    .Where(e => Matches(e, prefix, filter, customOnly))
                    .OrderBy(e => e.LogicalName, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (entities.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No table matches the criteria.[/]");
                    return ExitNoMatch;
                }

                RenderEntityTable(entities, location.TablesDirectory);

                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"[bold]{entities.Count}[/] table(s) found.");
                return ExitSuccess;
            }
            catch (Exception ex)
            {
                return ReportUnexpected(ex);
            }
        }

        private static void RenderEntityTable(
            IList<DataverseMetadata.EntityMetadata> entities, string tablesDirectory)
        {
            // Flags what is already tracked in the project: it's the information most needed
            // when deciding what to retrieve.
            var trackedLogicalNames = TableFileStore.ReadTrackedLogicalNames(tablesDirectory);

            var grid = new Spectre.Console.Table().Border(TableBorder.Rounded);
            grid.AddColumn("Logical name");
            grid.AddColumn("Label");
            grid.AddColumn("Custom");
            grid.AddColumn(".table");

            foreach (var entity in entities)
            {
                var isTracked = trackedLogicalNames.Contains(entity.LogicalName);

                grid.AddRow(
                    Markup.Escape(entity.LogicalName ?? string.Empty),
                    Markup.Escape(entity.DisplayName?.UserLocalizedLabel?.Label ?? string.Empty),
                    entity.IsCustomEntity.GetValueOrDefault() ? "yes" : "",
                    isTracked ? "[green]yes[/]" : "");
            }

            AnsiConsole.Write(grid);
        }

        // ══════════════════════════════════════════════════════════════════════
        // tables pull
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Generates or updates the <c>.table</c> files for the requested entities, or for all
        /// entities already tracked by the project if no criteria are given.
        /// </summary>
        /// <param name="projectRoot">Explicit root, or <see langword="null" /> to discover it.</param>
        /// <param name="tablesDirectory">Target directory, or <see langword="null" /> to infer it.</param>
        /// <param name="tableNames">Explicitly requested logical names.</param>
        /// <param name="prefix">Also retrieves all tables starting with this prefix.</param>
        /// <param name="noPrompt">Skips the interactive confirmation.</param>
        public static int Pull(
            string projectRoot,
            string tablesDirectory,
            IEnumerable<string> tableNames,
            string prefix,
            bool noPrompt)
        {
            try
            {
                var location = ResolveLocation(projectRoot);
                if (location == null)
                    return ExitNotFound;

                var targetDirectory = tablesDirectory ?? location.TablesDirectory;
                if (string.IsNullOrWhiteSpace(targetDirectory))
                {
                    AnsiConsole.MarkupLine(
                        "[red]Unable to infer the .table directory.[/] " +
                        "Declare [cyan]XrmFrameworkCoreProjectName[/] in the root's " +
                        "Directory.Build.props, or pass [cyan]--tables-dir[/].");
                    return ExitNotFound;
                }

                var connectionString = ResolveConnectionString(location.ProjectRoot, out var url);

                AnsiConsole.MarkupLine($"Environment: [cyan]{Markup.Escape(url ?? "unknown")}[/]");
                AnsiConsole.MarkupLine($"Directory     : [cyan]{Markup.Escape(targetDirectory)}[/]");

                var selection = ResolveSelection(targetDirectory, tableNames, prefix);
                if (selection == null)
                    return ExitNoMatch;

                var service = Connect(connectionString);

                var requested = ResolveRequestedEntities(service, selection, prefix, out var unknown);

                foreach (var name in unknown)
                    AnsiConsole.MarkupLine($"[yellow]Table not found in the environment:[/] {Markup.Escape(name)}");

                if (requested.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No table to retrieve.[/]");
                    return ExitNoMatch;
                }

                AnsiConsole.MarkupLine($"Tables        : [cyan]{requested.Count}[/]");
                AnsiConsole.WriteLine();

                if (!noPrompt && !AnsiConsole.Confirm("Retrieve the metadata for these tables?"))
                    return ExitSuccess;

                Directory.CreateDirectory(targetDirectory);

                var publisherPrefixes = RetrievePublisherPrefixes(service);
                var globalEnums = new List<OptionSetEnum>();
                var failures = 0;

                foreach (var logicalName in requested)
                {
                    if (!PullSingleTable(service, logicalName, targetDirectory, publisherPrefixes, globalEnums))
                        failures++;
                }

                SaveGlobalOptionSets(targetDirectory, globalEnums);

                AnsiConsole.WriteLine();

                if (failures > 0)
                {
                    AnsiConsole.MarkupLine($"[red]{failures}[/] table(s) failed.");
                    return ExitError;
                }

                AnsiConsole.MarkupLine("[green]Retrieval complete.[/]");
                return ExitSuccess;
            }
            catch (Exception ex)
            {
                return ReportUnexpected(ex);
            }
        }

        /// <summary>
        /// Determines the logical names to request from the environment. Without criteria, the selection
        /// defaults to the project itself: all the tables already described by a <c>.table</c>.
        /// </summary>
        /// <remarks>
        /// Resolved <b>before</b> the connection: an empty directory is a command-line
        /// error, no need to authenticate the user just to report it.
        /// </remarks>
        /// <returns>
        /// The names to request, or <see langword="null" /> if nothing is selectable — the corresponding
        /// message has then already been displayed.
        /// </returns>
        private static IList<string> ResolveSelection(
            string targetDirectory, IEnumerable<string> tableNames, string prefix)
        {
            var names = SplitNames(tableNames).ToList();

            if (names.Count > 0 || !string.IsNullOrWhiteSpace(prefix))
                return names;

            var tracked = TableFileStore.ReadTrackedLogicalNames(targetDirectory)
                                        .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                                        .ToList();

            if (tracked.Count == 0)
            {
                AnsiConsole.MarkupLine(
                    "[yellow]No .table file in this directory.[/] Specify the tables to " +
                    "retrieve via [cyan]--table[/] or [cyan]--prefix[/].");
                return null;
            }

            AnsiConsole.MarkupLine("Selection     : [cyan]tables already tracked by the project[/]");

            return tracked;
        }

        private static bool PullSingleTable(
            IOrganizationService service,
            string logicalName,
            string targetDirectory,
            IList<string> publisherPrefixes,
            List<OptionSetEnum> globalEnums)
        {
            try
            {
                var metadata = RetrieveEntity(service, logicalName);
                var conversion = MetadataTableFactory.Convert(metadata, publisherPrefixes);

                globalEnums.AddRange(conversion.GlobalEnums);

                var outcome = TablePullWriter.Write(targetDirectory, conversion.Table);

                var fileName = Path.GetFileName(outcome.FilePath);
                if (outcome.Created)
                    AnsiConsole.MarkupLine(
                        $"[green]Created[/]    {Markup.Escape(fileName)} " +
                        $"([bold]{outcome.Table.Columns.Count}[/] column(s))");
                else
                    AnsiConsole.MarkupLine(
                        $"[blue]Updated[/] {Markup.Escape(fileName)} " +
                        $"([bold]{outcome.Table.Columns.Count}[/] column(s))");

                if (outcome.ColumnsMissingFromCrm.Count > 0)
                    AnsiConsole.MarkupLine(
                        $"           [yellow]{outcome.ColumnsMissingFromCrm.Count} column(s) missing " +
                        "from the environment, kept:[/] " +
                        Markup.Escape(string.Join(", ",
                            outcome.ColumnsMissingFromCrm.Select(c => c.LogicalName))));

                return true;
            }
            catch (Exception ex)
            {
                // A failing table must not stop the others.
                AnsiConsole.MarkupLine(
                    $"[red]Failed[/]     {Markup.Escape(logicalName)}: {Markup.Escape(ex.Message)}");
                return false;
            }
        }

        private static void SaveGlobalOptionSets(string targetDirectory, List<OptionSetEnum> globalEnums)
        {
            if (globalEnums.Count == 0)
                return;

            var path = TableFileStore.FindTableFile(targetDirectory, TableFileStore.GlobalOptionSetLogicalName)
                       ?? TableFileStore.BuildTableFilePath(targetDirectory, TableFileStore.GlobalOptionSetFileName);

            var existing = File.Exists(path) ? TableFileStore.Load(path) : null;
            var merged = TableMerger.MergeGlobalOptionSets(existing, globalEnums);

            TableFileStore.Save(path, merged);

            AnsiConsole.MarkupLine(
                $"[blue]Updated[/] {Markup.Escape(Path.GetFileName(path))} " +
                $"([bold]{merged.Enums.Count}[/] global option set(s))");
        }

        // ══════════════════════════════════════════════════════════════════════
        // Metadata access
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Lists entities without their attributes: much faster than a full retrieval,
        /// and sufficient for listing or resolving a prefix.
        /// </summary>
        private static IList<DataverseMetadata.EntityMetadata> RetrieveEntityList(IOrganizationService service)
        {
            var response = (RetrieveAllEntitiesResponse)service.Execute(new RetrieveAllEntitiesRequest
            {
                EntityFilters = DataverseMetadata.EntityFilters.Entity,
                RetrieveAsIfPublished = true
            });

            return response.EntityMetadata;
        }

        private static DataverseMetadata.EntityMetadata RetrieveEntity(
            IOrganizationService service, string logicalName)
        {
            var response = (RetrieveEntityResponse)service.Execute(new RetrieveEntityRequest
            {
                LogicalName = logicalName,
                EntityFilters = DataverseMetadata.EntityFilters.Entity
                                | DataverseMetadata.EntityFilters.Attributes
                                | DataverseMetadata.EntityFilters.Relationships,
                RetrieveAsIfPublished = true
            });

            return response.EntityMetadata;
        }

        /// <summary>
        /// Customization prefixes declared by the environment's publishers, used to
        /// derive C# names from schema names.
        /// </summary>
        private static IList<string> RetrievePublisherPrefixes(IOrganizationService service)
        {
            var query = new QueryExpression("publisher") { ColumnSet = new ColumnSet("customizationprefix") };

            return service.RetrieveMultiple(query)
                          .Entities
                          .Select(e => e.GetAttributeValue<string>("customizationprefix"))
                          .Where(p => !string.IsNullOrWhiteSpace(p))
                          .Distinct(StringComparer.OrdinalIgnoreCase)
                          // Longest prefix first: "ftpx_" must be tested before "ftp_",
                          // otherwise a name would be truncated by the wrong prefix.
                          .OrderByDescending(p => p.Length)
                          .ThenBy(p => p, StringComparer.OrdinalIgnoreCase)
                          .ToList();
        }

        /// <summary>
        /// Builds the list of logical names to retrieve, validating the explicit names against
        /// the environment in order to flag typos rather than failing table by table.
        /// </summary>
        private static IList<string> ResolveRequestedEntities(
            IOrganizationService service,
            IEnumerable<string> tableNames,
            string prefix,
            out IList<string> unknown)
        {
            var available = new HashSet<string>(
                RetrieveEntityList(service).Select(e => e.LogicalName), StringComparer.OrdinalIgnoreCase);

            var requested = new List<string>();
            unknown = new List<string>();

            foreach (var name in SplitNames(tableNames))
            {
                if (available.Contains(name))
                    requested.Add(name);
                else
                    unknown.Add(name);
            }

            if (!string.IsNullOrWhiteSpace(prefix))
                requested.AddRange(available.Where(
                    n => n.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)));

            return requested.Distinct(StringComparer.OrdinalIgnoreCase)
                            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                            .ToList();
        }

        /// <summary>
        /// Accepts equally the repeated option and comma-separated lists.
        /// </summary>
        internal static IEnumerable<string> SplitNames(IEnumerable<string> values)
            => (values ?? Enumerable.Empty<string>())
               .Where(v => !string.IsNullOrWhiteSpace(v))
               .SelectMany(v => v.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
               .Select(v => v.Trim())
               .Where(v => v.Length > 0);

        internal static bool Matches(
            DataverseMetadata.EntityMetadata entity, string prefix, string filter, bool customOnly)
        {
            if (customOnly && !entity.IsCustomEntity.GetValueOrDefault())
                return false;

            if (!string.IsNullOrWhiteSpace(prefix)
                && !(entity.LogicalName ?? string.Empty).StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return false;

            if (string.IsNullOrWhiteSpace(filter))
                return true;

            var label = entity.DisplayName?.UserLocalizedLabel?.Label ?? string.Empty;

            return (entity.LogicalName ?? string.Empty).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                   || label.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // ══════════════════════════════════════════════════════════════════════
        // Configuration and connection
        // ══════════════════════════════════════════════════════════════════════

        /// <remarks>Internal: also reused by <see cref="ColumnHelper" />, which resolves the same
        /// project root but never connects to the environment.</remarks>
        internal static ProjectConfigLocation ResolveLocation(string projectRoot)
        {
            if (!string.IsNullOrWhiteSpace(projectRoot))
            {
                var explicitRoot = Path.GetFullPath(projectRoot);
                var located = ProjectConfigLocator.Locate(explicitRoot);

                // With an explicit root, we do not allow walking up: the user has designated
                // a specific location and must be warned if it does not contain the configuration.
                if (located == null || !string.Equals(located.ProjectRoot.TrimEnd(Path.DirectorySeparatorChar),
                        explicitRoot.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine(
                        $"[red]No Config/xrmFramework.config in[/] {Markup.Escape(explicitRoot)}");
                    return null;
                }

                return located;
            }

            var current = Directory.GetCurrentDirectory();
            var discovered = ProjectConfigLocator.Locate(current);

            if (discovered == null)
            {
                AnsiConsole.MarkupLine(
                    "[red]XrmFramework configuration not found.[/] No " +
                    "[cyan]Config/xrmFramework.config[/] found while walking up from " +
                    $"{Markup.Escape(current)}. Use [cyan]--project-root[/] to designate it.");
            }

            return discovered;
        }

        /// <summary>
        /// Loads the project configuration and extracts the selected connection string from it.
        /// </summary>
        /// <remarks>
        /// Deliberately separated from <see cref="Connect" />: the Dataverse client connects in
        /// its constructor, so the target URL must be displayed <b>before</b>. Otherwise an
        /// authentication failure leaves the user unaware of which environment was targeted.
        /// </remarks>
        private static string ResolveConnectionString(string projectRoot, out string url)
        {
            ConfigHelper.UseProjectConfig(projectRoot);

            var connectionString = ConfigHelper.GetSelectedConnectionString();

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "The connection selected in xrmFramework.config has no matching " +
                    "string in connectionStrings.config.");

            // The full string is never exposed: it contains the client secret.
            url = new DeploySettings { ConnectionString = connectionString }.Url;

            return connectionString;
        }

        private static IOrganizationService Connect(string connectionString)
            => new RegistrationService(connectionString);

        /// <remarks>Internal: also reused by <see cref="ColumnHelper" /> to report the same
        /// categories of failure with the same messages.</remarks>
        internal static int ReportUnexpected(Exception ex)
        {
            switch (ex)
            {
                case FileNotFoundException fileNotFound:
                    AnsiConsole.MarkupLine(
                        $"[red]File not found:[/] {Markup.Escape(fileNotFound.FileName ?? fileNotFound.Message)}");
                    return ExitNotFound;

                case DirectoryNotFoundException directoryNotFound:
                    AnsiConsole.MarkupLine(
                        $"[red]Directory not found:[/] {Markup.Escape(directoryNotFound.Message)}");
                    return ExitNotFound;

                default:
                    AnsiConsole.WriteException(ex);
                    return ExitError;
            }
        }

    }
}
