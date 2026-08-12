// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Inventory of the tables described by the <c>.table</c> files shipped with the
    /// XrmFramework package (<c>src/XrmFramework/Definitions</c>).
    ///
    /// These files are added as <c>AdditionalFiles</c> to the consumer project: the
    /// Roslyn generator produces <c>SystemUserDefinition</c>, <c>RoleDefinition</c>, ... from them, which
    /// therefore end up in the assembly analyzed by <c>tables sync</c>. Without this filter, the command
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
        /// by <c>tables sync</c> (<c>EnvironmentVariableDefinition.table</c> thus declares
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
    }
}
