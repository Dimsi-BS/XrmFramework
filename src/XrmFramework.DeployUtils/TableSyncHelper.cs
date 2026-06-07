// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using CommandLine;
using Spectre.Console;
using XrmFramework.DeployUtils.CommandOptions;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.DeployUtils
{
    /// <summary>
    /// Point d'entrée pour la synchronisation des fichiers .table depuis un assembly.
    /// </summary>
    /// <example>
    /// <code>
    /// // Dans le Program.cs du projet Deploy :
    /// TableSyncHelper.SyncTables(args);
    /// </code>
    ///
    /// Arguments attendus :
    /// <code>
    ///   --dll        &lt;chemin.dll&gt;   (obligatoire)
    ///   --tables-dir &lt;répertoire&gt;   (obligatoire)
    ///   --clean                       (optionnel)
    /// </code>
    /// </example>
    public static class TableSyncHelper
    {
        /// <summary>
        /// Synchronise les fichiers .table selon les classes <c>[EntityDefinition]</c>
        /// trouvées dans le DLL indiqué par <paramref name="args"/>.
        /// </summary>
        /// <remarks>
        /// Point d'entrée hérité conservé pour compatibilité : il effectue le parsing
        /// CommandLineParser puis délègue à <see cref="Sync(string, string, bool)"/>.
        /// En cas d'échec, le processus est terminé via <see cref="Environment.Exit(int)"/>.
        /// Les nouveaux appelants (CLI Spectre) doivent préférer <see cref="Sync(string, string, bool)"/>.
        /// </remarks>
        /// <param name="args">Arguments de ligne de commande.</param>
        public static void SyncTables(params string[] args)
        {
            Parser.Default
                  .ParseArguments<TableSyncCommandOptions>(args)
                  .WithParsed(options =>
                  {
                      var exitCode = Sync(options.DllPath, options.TablesDirectory, options.Clean);
                      if (exitCode != 0)
                          Environment.Exit(exitCode);
                  })
                  .WithNotParsed(errors =>
                  {
                      foreach (var error in errors)
                          AnsiConsole.MarkupLine($"[red]Erreur de paramètre :[/] {error}");

                      Environment.Exit(1);
                  });
        }

        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Synchronise les fichiers <c>.table</c> du répertoire indiqué à partir des classes
        /// <c>[EntityDefinition]</c> trouvées dans le DLL fourni.
        /// </summary>
        /// <param name="dllPath">Chemin vers le DLL à analyser.</param>
        /// <param name="tablesDirectory">Répertoire contenant les fichiers .table.</param>
        /// <param name="clean">
        /// Met <c>Select=false</c> sur les colonnes orphelines et supprime les .table
        /// entièrement générés par l'outil sans donnée CRM.
        /// </param>
        /// <returns>
        /// Code de sortie : <c>0</c> succès, <c>2</c> fichier/répertoire introuvable,
        /// <c>3</c> erreur inattendue.
        /// </returns>
        public static int Sync(string dllPath, string tablesDirectory, bool clean)
        {
            AnsiConsole.MarkupLine("[bold]XrmFramework · Synchronisation des fichiers .table[/]");
            AnsiConsole.MarkupLine($"  DLL       : [cyan]{dllPath}[/]");
            AnsiConsole.MarkupLine($"  Répertoire: [cyan]{tablesDirectory}[/]");
            AnsiConsole.MarkupLine($"  Mode clean: [cyan]{(clean ? "oui" : "non")}[/]");
            AnsiConsole.WriteLine();

            try
            {
                // 1. Analyser le DLL
                AnsiConsole.MarkupLine("Analyse du DLL...");
                var definitions = DefinitionAnalyzer.ExtractDefinitions(dllPath);

                if (definitions.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]Aucune classe [EntityDefinition] trouvée dans le DLL.[/]");
                    return 0;
                }

                // 2. Synchroniser les .table
                var syncer = new TableFileSyncer(tablesDirectory);
                syncer.Sync(definitions, clean);

                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[green]Synchronisation terminée.[/]");
                return 0;
            }
            catch (FileNotFoundException ex)
            {
                AnsiConsole.MarkupLine($"[red]Fichier introuvable :[/] {ex.FileName}");
                return 2;
            }
            catch (DirectoryNotFoundException ex)
            {
                AnsiConsole.MarkupLine($"[red]Répertoire introuvable :[/] {ex.Message}");
                return 2;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                return 3;
            }
        }
    }
}
