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
    /// Lecture et écriture des fichiers <c>.table</c>, partagées par toutes les commandes.
    /// </summary>
    /// <remarks>
    /// Les réglages de sérialisation reproduisent exactement ceux du DefinitionManager historique
    /// (<c>Formatting.Indented</c> + <c>DefaultValueHandling.Ignore</c>) : toute divergence
    /// produirait des diffs massifs sur des fichiers versionnés.
    /// </remarks>
    public static class TableFileStore
    {
        /// <summary>Extension des fichiers de définition de table.</summary>
        public const string TableFileExtension = ".table";

        /// <summary>
        /// Nom du fichier rassemblant les option sets globaux. Le générateur de code le reconnaît
        /// par ce nom exact — le renommer romprait la génération des énumérations partagées.
        /// </summary>
        public const string GlobalOptionSetFileName = "OptionSet";

        /// <summary>Nom logique conventionnel du pseudo-table des option sets globaux.</summary>
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
                   ?? throw new InvalidDataException($"Impossible de désérialiser {path}");
        }

        public static void Save(string path, CoreTable table)
        {
            var directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(path, JsonConvert.SerializeObject(table, SerializerSettings));
        }

        /// <summary>
        /// Recherche le fichier décrivant une entité en comparant le <c>LogName</c> de son contenu,
        /// et non son nom de fichier.
        /// </summary>
        /// <remarks>
        /// Le nom de fichier dérive du nom C# de la table, que les équipes renomment librement
        /// (<c>Contrat.table</c> devenu <c>ContratLocation.table</c>). Se fier au nom de fichier
        /// conduirait à créer un doublon au lieu de mettre à jour le fichier existant.
        /// </remarks>
        /// <returns>Le chemin du fichier, ou <see langword="null" /> si l'entité est inconnue.</returns>
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
                    // Un fichier illisible ne doit pas empêcher de retrouver les autres ;
                    // il sera signalé au moment où l'on tentera réellement de l'écrire.
                    continue;
                }

                if (string.Equals(table.LogicalName, entityLogicalName, StringComparison.OrdinalIgnoreCase))
                    return path;
            }

            return null;
        }

        /// <summary>
        /// Chemin du fichier d'une table à créer, dérivé de son nom C#.
        /// </summary>
        public static string BuildTableFilePath(string directory, string tableName)
            => Path.Combine(directory, tableName + TableFileExtension);

        /// <summary>
        /// Noms logiques des entités déjà suivies par le projet, c'est-à-dire décrites par un
        /// fichier <c>.table</c> du répertoire.
        /// </summary>
        /// <remarks>
        /// Le pseudo-table des option sets globaux est exclu : il ne correspond à aucune entité de
        /// l'environnement. Comme pour <see cref="FindTableFile" />, un fichier illisible est ignoré
        /// plutôt que fatal.
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
