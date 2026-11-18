// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using XrmFramework.Core;
using CoreTable = XrmFramework.Core.Table;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Inventory of the tables described by the <c>.table</c> files shipped with the
    /// XrmFramework package (<c>src/XrmFramework/Definitions</c>).
    ///
    /// These files are added as <c>AdditionalFiles</c> to the consumer project: the
    /// Roslyn generator produces <c>SystemUserDefinition</c>, <c>RoleDefinition</c>, ... from them, which
    /// therefore end up in the assembly analyzed by <c>migrate sync-tables</c>. Without this filter, the command
    /// would drop duplicates in the project's <c>Definitions</c> directory
    /// (<c>SystemUser.table</c>, <c>Role.table</c>, ...) of files already provided by the framework.
    ///
    /// The filter only applies to <b>creation</b>: a project may legitimately track its own
    /// copy of a framework table in order to declare additional columns there — the
    /// framework's columns being marked <c>"Locked": true</c>. As soon as the file exists,
    /// <see cref="TableFileSyncer"/> updates it like any other.
    ///
    /// The list is hard-coded here rather than deduced by reflection over the current assembly:
    /// XrmFramework.DeployUtils also compiles its own <c>.table</c> files (Publisher,
    /// SolutionComponent, WebResource, Pluginpackage) which are not shipped to consumer
    /// projects and must therefore remain synchronizable.
    /// <c>FrameworkTableCatalogTests</c> verifies that this inventory exactly matches the
    /// files present in the repository.
    /// </summary>
    public static class FrameworkTableCatalog
    {
        /// <summary>
        /// Pair (table name, logical name) of a <c>.table</c> shipped by the framework.
        /// The table name is the one from the JSON (<c>Name</c>), not the file name: it is the one that
        /// gives its name to the generated class, and therefore to the <see cref="DefinitionInfo.TableName"/> seen
        /// by <c>migrate sync-tables</c> (<c>EnvironmentVariableDefinition.table</c> thus declares
        /// <c>EnvironmentVariable</c>).
        /// </summary>
        private sealed class FrameworkTable
        {
            public FrameworkTable(string name, string logicalName)
            {
                Name = name;
                LogicalName = logicalName;
            }

            public string Name { get; }

            public string LogicalName { get; }
        }

        private static readonly FrameworkTable[] ShippedTables =
        {
            new FrameworkTable("BusinessUnit",                        "businessunit"),
            new FrameworkTable("CustomApi",                           "customapi"),
            new FrameworkTable("CustomApiRequestParameter",           "customapirequestparameter"),
            new FrameworkTable("CustomApiResponseProperty",           "customapiresponseproperty"),
            new FrameworkTable("DebugSession",                        "dimsi_debugsession"),
            new FrameworkTable("EnvironmentVariable",                 "environmentvariabledefinition"),
            new FrameworkTable("EnvironmentVariableValue",            "environmentvariablevalue"),
            new FrameworkTable("OptionSets",                          "globalEnums"),
            new FrameworkTable("PluginAssembly",                      "pluginassembly"),
            new FrameworkTable("PluginType",                          "plugintype"),
            new FrameworkTable("Role",                                "role"),
            new FrameworkTable("SdkMessage",                          "sdkmessage"),
            new FrameworkTable("SdkMessageFilter",                    "sdkmessagefilter"),
            new FrameworkTable("SdkMessageProcessingStep",            "sdkmessageprocessingstep"),
            new FrameworkTable("SdkMessageProcessingStepImage",       "sdkmessageprocessingstepimage"),
            new FrameworkTable("SdkMessageProcessingStepSecureConfig", "sdkmessageprocessingstepsecureconfig"),
            new FrameworkTable("Solution",                            "solution"),
            new FrameworkTable("SystemUser",                          "systemuser"),
            new FrameworkTable("SystemUserRoles",                     "systemuserroles"),
            new FrameworkTable("Team",                                "team"),
            new FrameworkTable("Workflow",                            "workflow")
        };

        private static readonly HashSet<string> Names =
            new HashSet<string>(ShippedTables.Select(t => t.Name), StringComparer.OrdinalIgnoreCase);

        private static readonly HashSet<string> LogicalNamesSet =
            new HashSet<string>(ShippedTables.Select(t => t.LogicalName), StringComparer.OrdinalIgnoreCase);

        /// <summary>Table names (<c>Name</c>) of the <c>.table</c> files shipped by the framework.</summary>
        public static IReadOnlyCollection<string> TableNames => Names;

        /// <summary>Logical names (<c>LogName</c>) of the <c>.table</c> files shipped by the framework.</summary>
        public static IReadOnlyCollection<string> LogicalNames => LogicalNamesSet;

        /// <summary>
        /// True if the Definition describes a table shipped by the framework.
        /// </summary>
        public static bool IsFrameworkTable(DefinitionInfo definition)
            => definition != null
            && IsFrameworkTable(definition.TableName, definition.EntityName);

        /// <summary>
        /// True if either of the provided identifiers designates a table shipped by the framework.
        /// Both are tested independently: a <c>.table</c> renamed on the project side remains
        /// recognizable by its logical name, and a file whose content is unreadable remains
        /// recognizable by its name. <c>null</c> is accepted for the unknown identifier.
        /// </summary>
        public static bool IsFrameworkTable(string tableName, string logicalName)
            => (!string.IsNullOrEmpty(tableName) && Names.Contains(tableName))
            || (!string.IsNullOrEmpty(logicalName) && LogicalNamesSet.Contains(logicalName));

        // ──────────────────────────────────────────────────────────────────────
        // Seeding the locks
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Prefix of the embedded resources holding the shipped <c>.table</c> files; see the
        /// <c>EmbeddedResource</c> item in the project file.
        /// </summary>
        private const string ShippedTableResourcePrefix = "XrmFramework.ShippedTables.";

        private static readonly Lazy<IDictionary<string, CoreTable>> ShippedByLogicalName =
            new Lazy<IDictionary<string, CoreTable>>(LoadShippedTables);

        private static IDictionary<string, CoreTable> LoadShippedTables()
        {
            var assembly = typeof(FrameworkTableCatalog).GetTypeInfo().Assembly;
            var tables = new Dictionary<string, CoreTable>(StringComparer.OrdinalIgnoreCase);

            foreach (var resource in assembly.GetManifestResourceNames())
            {
                if (!resource.StartsWith(ShippedTableResourcePrefix, StringComparison.Ordinal))
                    continue;

                CoreTable table;

                try
                {
                    using (var stream = assembly.GetManifestResourceStream(resource))
                    using (var reader = new StreamReader(stream))
                        table = JsonConvert.DeserializeObject<CoreTable>(reader.ReadToEnd());
                }
                catch (Exception)
                {
                    // A shipped file this build cannot read seeds nothing, rather than breaking
                    // every command that touches a .table.
                    continue;
                }

                if (!string.IsNullOrEmpty(table?.LogicalName))
                    tables[table.LogicalName] = table;
            }

            return tables;
        }

        /// <summary>
        /// Carries onto <paramref name="table" /> the locks the framework ships for it, and the
        /// names those locks stand for.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A project may track its own copy of a framework table in order to declare further
        /// columns there, and that copy is pulled from the environment like any other: the CRM
        /// knows nothing of <c>Locked</c>, and <see cref="TableMerger" /> keeps the flag the file
        /// already carries — which, for a copy a pull created, is none. The lock therefore never
        /// reached the project's copy, and nothing said so: the <c>Locked</c> column of
        /// <see cref="ColumnHelper" />'s listing simply showed blank, and the generators were left
        /// arbitrating a locked name against a renamed one at every build.
        /// </para>
        /// <para>
        /// Seeding them here, on the paths that write a <c>.table</c>, makes the project's copy
        /// agree with the framework's instead. The name travels with the lock: marking a column
        /// locked is precisely saying that the framework's own code refers to the constant
        /// generated from its name.
        /// </para>
        /// <para>
        /// Only what the copy already tracks is annotated. A locked column the project does not
        /// declare is left out — the framework's own copy declares it, and adding it here would
        /// widen the project's generated surface on its behalf. The table's own <c>Name</c> is left
        /// alone as well: renaming a table renames its file and its generated class, which
        /// <see cref="TableFileSyncer" /> arbitrates in a pass of its own.
        /// </para>
        /// </remarks>
        /// <returns>The number of declarations seeded, for the caller to report.</returns>
        public static int SeedLocks(CoreTable table)
        {
            if (table == null
                || string.IsNullOrEmpty(table.LogicalName)
                || !ShippedByLogicalName.Value.TryGetValue(table.LogicalName, out var shipped))
            {
                return 0;
            }

            var seeded = 0;

            if (shipped.IsLocked && !table.IsLocked)
            {
                table.IsLocked = true;
                seeded++;
            }

            foreach (var shippedColumn in shipped.Columns.Where(IsLockedAndNamed))
            {
                var column = table.Columns.FirstOrDefault(
                    c => string.Equals(c.LogicalName, shippedColumn.LogicalName,
                                       StringComparison.OrdinalIgnoreCase));

                if (column != null && Seed(column, shippedColumn))
                    seeded++;
            }

            foreach (var shippedEnum in shipped.Enums.Where(
                         e => e != null && e.IsLocked && !string.IsNullOrEmpty(e.LogicalName)))
            {
                var optionSet = table.Enums.FirstOrDefault(
                    e => string.Equals(e?.LogicalName, shippedEnum.LogicalName,
                                       StringComparison.OrdinalIgnoreCase));

                if (optionSet != null && Seed(optionSet, shippedEnum))
                    seeded++;
            }

            return seeded;
        }

        private static bool IsLockedAndNamed(Column column)
        => column != null && column.IsLocked && !string.IsNullOrEmpty(column.LogicalName);

        private static bool Seed(Column column, Column shipped)
        {
            var seeded = false;

            if (!column.IsLocked)
            {
                column.IsLocked = true;
                seeded = true;
            }

            if (!string.IsNullOrEmpty(shipped.Name)
                && !string.Equals(column.Name, shipped.Name, StringComparison.Ordinal))
            {
                column.Name = shipped.Name;
                seeded = true;
            }

            return seeded;
        }

        /// <remarks>
        /// The members travel with the name, as they do when the two copies are folded at build
        /// time: a name and its members describe one generated enum, and the code the framework
        /// ships writes <c>TeamType.Owner</c>. Seeding the locked name over the project's own
        /// members would leave the file describing an enum nothing compiles against.
        /// </remarks>
        private static bool Seed(OptionSetEnum optionSet, OptionSetEnum shipped)
        {
            var seeded = false;

            if (!optionSet.IsLocked)
            {
                optionSet.IsLocked = true;
                seeded = true;
            }

            if (string.IsNullOrEmpty(shipped.Name))
            {
                return seeded;
            }

            if (!string.Equals(optionSet.Name, shipped.Name, StringComparison.Ordinal))
            {
                optionSet.Name = shipped.Name;
                seeded = true;
            }

            if (!MembersMatch(optionSet, shipped))
            {
                optionSet.Values.Clear();

                foreach (var value in shipped.Values)
                    optionSet.Values.Add(value);

                seeded = true;
            }

            return seeded;
        }

        /// <summary>
        /// Whether the two copies already declare the same members, by value and by name — so that
        /// seeding twice reports nothing the second time.
        /// </summary>
        private static bool MembersMatch(OptionSetEnum optionSet, OptionSetEnum shipped)
        => optionSet.Values.Count == shipped.Values.Count
        && optionSet.Values.OrderBy(v => v.Value)
                    .Zip(shipped.Values.OrderBy(v => v.Value),
                         (mine, theirs) => mine.Value == theirs.Value
                                        && string.Equals(mine.Name, theirs.Name, StringComparison.Ordinal))
                    .All(same => same);
    }
}
