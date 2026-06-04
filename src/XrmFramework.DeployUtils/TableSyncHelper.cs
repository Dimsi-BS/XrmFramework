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
        /// <param name="args">Arguments de ligne de commande.</param>
        public static void SyncTables(params string[] args)
        {
            Parser.Default
                  .ParseArguments<TableSyncCommandOptions>(args)
                  .WithParsed(Run)
                  .WithNotParsed(errors =>
                  {
                      foreach (var error in errors)
                          AnsiConsole.MarkupLine($"[red]Erreur de paramètre :[/] {error}");

                      Environment.Exit(1);
                  });
        }

        // ──────────────────────────────────────────────────────────────────────────

        private static void Run(TableSyncCommandOptions options)
        {
            AnsiConsole.MarkupLine("[bold]XrmFramework · Synchronisation des fichiers .table[/]");
            AnsiConsole.MarkupLine($"  DLL       : [cyan]{options.DllPath}[/]");
            AnsiConsole.MarkupLine($"  Répertoire: [cyan]{options.TablesDirectory}[/]");
            AnsiConsole.MarkupLine($"  Mode clean: [cyan]{(options.Clean ? "oui" : "non")}[/]");
            AnsiConsole.WriteLine();

            try
            {
                // 1. Analyser le DLL
                AnsiConsole.MarkupLine("Analyse du DLL...");
                var definitions = DefinitionAnalyzer.ExtractDefinitions(options.DllPath);

                if (definitions.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]Aucune classe [EntityDefinition] trouvée dans le DLL.[/]");
                    return;
                }

                // 2. Synchroniser les .table
                var syncer = new TableFileSyncer(options.TablesDirectory);
                syncer.Sync(definitions, options.Clean);

                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[green]Synchronisation terminée.[/]");
            }
            catch (FileNotFoundException ex)
            {
                AnsiConsole.MarkupLine($"[red]Fichier introuvable :[/] {ex.FileName}");
                Environment.Exit(2);
            }
            catch (DirectoryNotFoundException ex)
            {
                AnsiConsole.MarkupLine($"[red]Répertoire introuvable :[/] {ex.Message}");
                Environment.Exit(2);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                Environment.Exit(3);
            }
        }
    }
}
