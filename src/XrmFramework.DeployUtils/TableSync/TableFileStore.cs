// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using CoreTable = XrmFramework.Core.Table;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Reading and writing of <c>.table</c> files, shared by all commands.
    /// </summary>
    /// <remarks>
    /// The serialization settings exactly reproduce those of the historical DefinitionManager
    /// (<c>Formatting.Indented</c> + <c>DefaultValueHandling.Ignore</c>): any divergence
    /// would produce massive diffs on versioned files.
    /// </remarks>
    public static class TableFileStore
    {
        /// <summary>Extension of table definition files.</summary>
        public const string TableFileExtension = ".table";

        /// <summary>
        /// Name of the file gathering the global option sets. The code generator recognizes it
        /// by this exact name — renaming it would break the generation of shared enumerations.
        /// </summary>
        public const string GlobalOptionSetFileName = "OptionSets";

        /// <summary>Conventional logical name of the global option sets pseudo-table.</summary>
        public const string GlobalOptionSetLogicalName = "globalEnums";

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        public static CoreTable Load(string path)
        {
            var json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<CoreTable>(json)
                   ?? throw new InvalidDataException($"Unable to deserialize {path}");
        }

        public static void Save(string path, CoreTable table)
        {
            var directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(path, JsonConvert.SerializeObject(table, SerializerSettings));
        }

        /// <summary>
        /// Finds the file describing an entity by comparing the <c>LogName</c> of its content,
        /// not its file name.
        /// </summary>
        /// <remarks>
        /// The file name derives from the C# name of the table, which teams freely rename
        /// (<c>Contrat.table</c> becomes <c>ContratLocation.table</c>). Relying on the file name
        /// would lead to creating a duplicate instead of updating the existing file.
        /// </remarks>
        /// <returns>The file path, or <see langword="null" /> if the entity is unknown.</returns>
        public static string FindTableFile(string directory, string entityLogicalName)
        {
            if (!Directory.Exists(directory))
                return null;

            foreach (var path in Directory.GetFiles(directory, "*" + TableFileExtension)
                                          .OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
            {
                CoreTable table;
                try
                {
                    table = Load(path);
                }
                catch (Exception)
                {
                    // An unreadable file must not prevent finding the others;
                    // it will be reported when we actually try to write it.
                    continue;
                }

                if (string.Equals(table.LogicalName, entityLogicalName, StringComparison.OrdinalIgnoreCase))
                    return path;
            }

            return null;
        }

        /// <summary>
        /// File path of a table to create, derived from its C# name.
        /// </summary>
        public static string BuildTableFilePath(string directory, string tableName)
            => Path.Combine(directory, tableName + TableFileExtension);

        /// <summary>
        /// Logical names of the entities already tracked by the project, i.e. described by a
        /// <c>.table</c> file in the directory.
        /// </summary>
        /// <remarks>
        /// The global option sets pseudo-table is excluded: it does not correspond to any entity of
        /// the environment. As with <see cref="FindTableFile" />, an unreadable file is ignored
        /// rather than fatal.
        /// </remarks>
        public static ISet<string> ReadTrackedLogicalNames(string directory)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
                return result;

            foreach (var path in Directory.GetFiles(directory, "*" + TableFileExtension))
            {
                CoreTable table;
                try
                {
                    table = Load(path);
                }
                catch (Exception)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(table.LogicalName)
                    && !string.Equals(table.LogicalName, GlobalOptionSetLogicalName,
                                      StringComparison.OrdinalIgnoreCase))
                    result.Add(table.LogicalName);
            }

            return result;
        }
    }
}
