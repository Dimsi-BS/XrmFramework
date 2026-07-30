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
    /// Commandes connectées à l'environnement : lister les tables et récupérer leurs métadonnées
    /// sous forme de fichiers <c>.table</c>.
    /// </summary>
    /// <remarks>
    /// Conformément au contrat du CLI, ces points d'entrée retournent un code de sortie et
    /// n'appellent jamais <c>Environment.Exit</c>.
    /// </remarks>
    public static class CrmTableHelper
    {
        /// <summary>Succès.</summary>
        public const int ExitSuccess = 0;

        /// <summary>Aucune table ne correspond aux critères demandés.</summary>
        public const int ExitNoMatch = 1;

        /// <summary>Configuration ou répertoire introuvable.</summary>
        public const int ExitNotFound = 2;

        /// <summary>Erreur inattendue.</summary>
        public const int ExitError = 3;

        // ══════════════════════════════════════════════════════════════════════
        // tables list
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Affiche les tables de l'environnement sélectionné, filtrables par préfixe.
        /// </summary>
        /// <param name="projectRoot">Racine explicite, ou <see langword="null" /> pour la découvrir.</param>
        /// <param name="prefix">Ne retenir que les noms logiques commençant par ce préfixe.</param>
        /// <param name="filter">Sous-chaîne recherchée dans le nom logique ou le libellé.</param>
        /// <param name="customOnly">Ne retenir que les tables personnalisées.</param>
        public static int List(string projectRoot, string prefix, string filter, bool customOnly)
        {
            try
            {
                var location = ResolveLocation(projectRoot);
                if (location == null)
                    return ExitNotFound;

                var connectionString = ResolveConnectionString(location.ProjectRoot, out var url);
                AnsiConsole.MarkupLine($"Environnement : [cyan]{Markup.Escape(url ?? "inconnu")}[/]");
                AnsiConsole.WriteLine();

                var service = Connect(connectionString);

                var entities = RetrieveEntityList(service)
                    .Where(e => Matches(e, prefix, filter, customOnly))
                    .OrderBy(e => e.LogicalName, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (entities.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]Aucune table ne correspond aux critères.[/]");
                    return ExitNoMatch;
                }

                RenderEntityTable(entities, location.TablesDirectory);

                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"[bold]{entities.Count}[/] table(s) trouvée(s).");
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
            // Signale ce qui est déjà suivi dans le projet : c'est l'information qui manque le plus
            // au moment de choisir quoi récupérer.
            var trackedLogicalNames = ReadTrackedLogicalNames(tablesDirectory);

            var grid = new Spectre.Console.Table().Border(TableBorder.Rounded);
            grid.AddColumn("Nom logique");
            grid.AddColumn("Libellé");
            grid.AddColumn("Perso.");
            grid.AddColumn(".table");

            foreach (var entity in entities)
            {
                var isTracked = trackedLogicalNames.Contains(entity.LogicalName);

                grid.AddRow(
                    Markup.Escape(entity.LogicalName ?? string.Empty),
                    Markup.Escape(entity.DisplayName?.UserLocalizedLabel?.Label ?? string.Empty),
                    entity.IsCustomEntity.GetValueOrDefault() ? "oui" : "",
                    isTracked ? "[green]oui[/]" : "");
            }

            AnsiConsole.Write(grid);
        }

        // ══════════════════════════════════════════════════════════════════════
        // tables pull
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Génère ou met à jour les fichiers <c>.table</c> des entités demandées.
        /// </summary>
        /// <param name="projectRoot">Racine explicite, ou <see langword="null" /> pour la découvrir.</param>
        /// <param name="tablesDirectory">Répertoire cible, ou <see langword="null" /> pour le déduire.</param>
        /// <param name="tableNames">Noms logiques explicitement demandés.</param>
        /// <param name="prefix">Récupère en outre toutes les tables commençant par ce préfixe.</param>
        /// <param name="noPrompt">Ignore la confirmation interactive.</param>
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
                        "[red]Impossible de déduire le répertoire des .table.[/] " +
                        "Déclarez [cyan]XrmFrameworkCoreProjectName[/] dans le Directory.Build.props " +
                        "de la racine, ou passez [cyan]--tables-dir[/].");
                    return ExitNotFound;
                }

                var connectionString = ResolveConnectionString(location.ProjectRoot, out var url);

                AnsiConsole.MarkupLine($"Environnement : [cyan]{Markup.Escape(url ?? "inconnu")}[/]");
                AnsiConsole.MarkupLine($"Répertoire    : [cyan]{Markup.Escape(targetDirectory)}[/]");

                var service = Connect(connectionString);

                var requested = ResolveRequestedEntities(service, tableNames, prefix, out var unknown);

                foreach (var name in unknown)
                    AnsiConsole.MarkupLine($"[yellow]Table introuvable dans l'environnement :[/] {Markup.Escape(name)}");

                if (requested.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]Aucune table à récupérer.[/]");
                    return ExitNoMatch;
                }

                AnsiConsole.MarkupLine($"Tables        : [cyan]{requested.Count}[/]");
                AnsiConsole.WriteLine();

                if (!noPrompt && !AnsiConsole.Confirm("Récupérer les métadonnées de ces tables ?"))
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
                    AnsiConsole.MarkupLine($"[red]{failures}[/] table(s) en échec.");
                    return ExitError;
                }

                AnsiConsole.MarkupLine("[green]Récupération terminée.[/]");
                return ExitSuccess;
            }
            catch (Exception ex)
            {
                return ReportUnexpected(ex);
            }
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

                // Le fichier est retrouvé par son nom logique : son nom de fichier suit le nom C#
                // de la table, que les équipes renomment librement.
                var path = TableFileStore.FindTableFile(targetDirectory, logicalName);
                var existing = path == null ? null : TableFileStore.Load(path);

                var merged = TableMerger.Merge(existing, conversion.Table);
                var missing = TableMerger.GetColumnsMissingFromCrm(existing, conversion.Table);

                path = path ?? TableFileStore.BuildTableFilePath(targetDirectory, merged.Name);
                TableFileStore.Save(path, merged);

                var fileName = Path.GetFileName(path);
                if (existing == null)
                    AnsiConsole.MarkupLine(
                        $"[green]Créé[/]       {Markup.Escape(fileName)} " +
                        $"([bold]{merged.Columns.Count}[/] colonne(s))");
                else
                    AnsiConsole.MarkupLine(
                        $"[blue]Mis à jour[/] {Markup.Escape(fileName)} " +
                        $"([bold]{merged.Columns.Count}[/] colonne(s))");

                if (missing.Count > 0)
                    AnsiConsole.MarkupLine(
                        $"           [yellow]{missing.Count} colonne(s) absente(s) de l'environnement, " +
                        "conservée(s) :[/] " +
                        Markup.Escape(string.Join(", ", missing.Select(c => c.LogicalName))));

                return true;
            }
            catch (Exception ex)
            {
                // Une table en échec ne doit pas interrompre les autres.
                AnsiConsole.MarkupLine(
                    $"[red]Échec[/]      {Markup.Escape(logicalName)} : {Markup.Escape(ex.Message)}");
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
                $"[blue]Mis à jour[/] {Markup.Escape(Path.GetFileName(path))} " +
                $"([bold]{merged.Enums.Count}[/] option set(s) global(aux))");
        }

        // ══════════════════════════════════════════════════════════════════════
        // Accès aux métadonnées
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Liste les entités sans leurs attributs : bien plus rapide que la récupération complète,
        /// et suffisant pour lister ou résoudre un préfixe.
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
        /// Préfixes de personnalisation déclarés par les éditeurs de l'environnement, utilisés pour
        /// dériver les noms C# depuis les noms de schéma.
        /// </summary>
        private static IList<string> RetrievePublisherPrefixes(IOrganizationService service)
        {
            var query = new QueryExpression("publisher") { ColumnSet = new ColumnSet("customizationprefix") };

            return service.RetrieveMultiple(query)
                          .Entities
                          .Select(e => e.GetAttributeValue<string>("customizationprefix"))
                          .Where(p => !string.IsNullOrWhiteSpace(p))
                          .Distinct(StringComparer.OrdinalIgnoreCase)
                          // Le préfixe le plus long d'abord : « ftpx_ » doit être testé avant « ftp_ »,
                          // sinon un nom serait tronqué par le mauvais préfixe.
                          .OrderByDescending(p => p.Length)
                          .ThenBy(p => p, StringComparer.OrdinalIgnoreCase)
                          .ToList();
        }

        /// <summary>
        /// Construit la liste des noms logiques à récupérer, en validant les noms explicites contre
        /// l'environnement afin de signaler les fautes de frappe plutôt que d'échouer table par table.
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
        /// Accepte indifféremment l'option répétée et les listes séparées par des virgules.
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
        // Configuration et connexion
        // ══════════════════════════════════════════════════════════════════════

        private static ProjectConfigLocation ResolveLocation(string projectRoot)
        {
            if (!string.IsNullOrWhiteSpace(projectRoot))
            {
                var explicitRoot = Path.GetFullPath(projectRoot);
                var located = ProjectConfigLocator.Locate(explicitRoot);

                // Avec une racine explicite, on n'accepte pas de remonter : l'utilisateur a désigné
                // un emplacement précis et doit être averti s'il ne contient pas la configuration.
                if (located == null || !string.Equals(located.ProjectRoot.TrimEnd(Path.DirectorySeparatorChar),
                        explicitRoot.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine(
                        $"[red]Aucun Config/xrmFramework.config dans[/] {Markup.Escape(explicitRoot)}");
                    return null;
                }

                return located;
            }

            var current = Directory.GetCurrentDirectory();
            var discovered = ProjectConfigLocator.Locate(current);

            if (discovered == null)
            {
                AnsiConsole.MarkupLine(
                    "[red]Configuration XrmFramework introuvable.[/] Aucun " +
                    "[cyan]Config/xrmFramework.config[/] trouvé en remontant depuis " +
                    $"{Markup.Escape(current)}. Utilisez [cyan]--project-root[/] pour la désigner.");
            }

            return discovered;
        }

        /// <summary>
        /// Charge la configuration du projet et en extrait la chaîne de connexion sélectionnée.
        /// </summary>
        /// <remarks>
        /// Volontairement séparé de <see cref="Connect" /> : le client Dataverse se connecte dans
        /// son constructeur, donc l'URL cible doit être affichée <b>avant</b>. Sinon un échec
        /// d'authentification laisse l'utilisateur sans savoir quel environnement était visé.
        /// </remarks>
        private static string ResolveConnectionString(string projectRoot, out string url)
        {
            ConfigHelper.UseProjectConfig(projectRoot);

            var connectionString = ConfigHelper.GetSelectedConnectionString();

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "La connexion sélectionnée dans xrmFramework.config n'a pas de chaîne " +
                    "correspondante dans connectionStrings.config.");

            // On n'expose jamais la chaîne complète : elle contient le secret client.
            url = new DeploySettings { ConnectionString = connectionString }.Url;

            return connectionString;
        }

        private static IOrganizationService Connect(string connectionString)
            => new RegistrationService(connectionString);

        private static int ReportUnexpected(Exception ex)
        {
            switch (ex)
            {
                case FileNotFoundException fileNotFound:
                    AnsiConsole.MarkupLine(
                        $"[red]Fichier introuvable :[/] {Markup.Escape(fileNotFound.FileName ?? fileNotFound.Message)}");
                    return ExitNotFound;

                case DirectoryNotFoundException directoryNotFound:
                    AnsiConsole.MarkupLine(
                        $"[red]Répertoire introuvable :[/] {Markup.Escape(directoryNotFound.Message)}");
                    return ExitNotFound;

                default:
                    AnsiConsole.WriteException(ex);
                    return ExitError;
            }
        }

        /// <summary>
        /// Noms logiques déjà présents dans le répertoire des <c>.table</c>.
        /// </summary>
        private static HashSet<string> ReadTrackedLogicalNames(string tablesDirectory)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(tablesDirectory) || !Directory.Exists(tablesDirectory))
                return result;

            foreach (var path in Directory.GetFiles(tablesDirectory, "*" + TableFileStore.TableFileExtension))
            {
                try
                {
                    var table = TableFileStore.Load(path);
                    if (!string.IsNullOrEmpty(table.LogicalName))
                        result.Add(table.LogicalName);
                }
                catch (Exception)
                {
                    // Un .table illisible n'empêche pas de lister l'environnement.
                }
            }

            return result;
        }
    }
}
