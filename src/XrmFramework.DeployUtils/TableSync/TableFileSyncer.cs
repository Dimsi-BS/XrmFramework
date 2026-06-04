// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Spectre.Console;
using CoreTable = XrmFramework.Core.Table;
using XrmFramework.Core;

namespace XrmFramework.DeployUtils.TableSync
{

/// <summary>
/// Synchronise les fichiers .table d'un répertoire avec les informations
/// extraites des classes *Definition d'un assembly.
/// </summary>
public sealed class TableFileSyncer
{
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        Formatting = Formatting.Indented,
        DefaultValueHandling = DefaultValueHandling.Ignore
    };

    private readonly string _tablesDirectory;

    public TableFileSyncer(string tablesDirectory)
    {
        if (!Directory.Exists(tablesDirectory))
            throw new DirectoryNotFoundException(
                $"Répertoire .table introuvable : {tablesDirectory}");

        _tablesDirectory = tablesDirectory;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Point d'entrée principal
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Synchronise les fichiers .table selon les <paramref name="definitions"/> fournies.
    /// </summary>
    /// <param name="definitions">Infos extraites du DLL par <see cref="DefinitionAnalyzer"/>.</param>
    /// <param name="clean">
    ///   Si true, met Select=false sur les colonnes absentes de toute Definition
    ///   et supprime les .table entièrement créés par l'outil (aucune donnée CRM réelle).
    /// </param>
    public void Sync(IReadOnlyList<DefinitionInfo> definitions, bool clean = false)
    {
        // Index des logical names sélectionnés par entité, utilisé en mode --clean
        // pour identifier les colonnes qui n'appartiennent plus à aucune Definition.
        var selectedByTable = definitions.ToDictionary(
            d => d.TableName,
            d => new HashSet<string>(
                     d.Columns.Select(c => c.LogicalName),
                     StringComparer.OrdinalIgnoreCase),
            StringComparer.OrdinalIgnoreCase);

        AnsiConsole.MarkupLine($"[bold]{definitions.Count}[/] classe(s) Definition trouvée(s) dans le DLL.");

        // 1. Mettre à jour / créer les .table pour chaque Definition
        foreach (var def in definitions)
        {
            var tablePath = TablePath(def.TableName);
            SyncDefinition(def, tablePath);
        }

        // 2. Mode --clean : traiter les .table sans Definition correspondante
        if (clean)
        {
            CleanOrphanedTableFiles(selectedByTable);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Synchronisation d'une Definition vers son .table
    // ──────────────────────────────────────────────────────────────────────────

    private void SyncDefinition(DefinitionInfo def, string tablePath)
    {
        var isNew = !File.Exists(tablePath);
        var table = isNew ? CreateMinimalTable(def) : LoadTable(tablePath);

        var added = 0;
        var updated = 0;

        foreach (var colInfo in def.Columns)
        {
            var existing = table.Columns
                .FirstOrDefault(c => string.Equals(
                    c.LogicalName, colInfo.LogicalName, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                // Colonne absente du .table → création minimale
                var newCol = new Column
                {
                    LogicalName = colInfo.LogicalName,
                    Name = colInfo.Name,
                    Selected = true
                };
                table.Columns.Add(newCol);
                added++;
            }
            else if (!existing.Selected)
            {
                existing.Selected = true;
                updated++;
            }
            // Si déjà Selected=true : rien à faire
        }

        SaveTable(tablePath, table);

        if (isNew)
            AnsiConsole.MarkupLine(
                $"[green]Créé[/]    {def.TableName}.table " +
                $"([bold]{def.Columns.Count}[/] colonne(s))");
        else
            AnsiConsole.MarkupLine(
                $"[blue]Mis à jour[/] {def.TableName}.table " +
                $"(+{added} créée(s), {updated} activée(s))");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Mode --clean
    // ──────────────────────────────────────────────────────────────────────────

    private void CleanOrphanedTableFiles(
        Dictionary<string, HashSet<string>> selectedByTable)
    {
        var allTableFiles = Directory.GetFiles(_tablesDirectory, "*.table");

        // Ensemble des noms gérés par les Definitions du DLL
        var managedNames = new HashSet<string>(selectedByTable.Keys, StringComparer.OrdinalIgnoreCase);

        foreach (var tableFile in allTableFiles)
        {
            var tableName = Path.GetFileNameWithoutExtension(tableFile);

            if (managedNames.Contains(tableName))
            {
                // Ce fichier est géré → de-sélectionner les colonnes absentes de la Definition
                DeSelectOrphanColumns(tableFile, tableName, selectedByTable[tableName]);
            }
            else
            {
                // Aucune Definition correspondante dans le DLL → supprimer si outil-généré
                HandleFullyOrphanedFile(tableFile, tableName);
            }
        }
    }

    /// <summary>
    /// Met Select=false sur les colonnes d'un .table géré qui ne sont plus dans sa Definition.
    /// </summary>
    private void DeSelectOrphanColumns(string tableFile, string tableName,
        HashSet<string> definitionLogicalNames)
    {
        var table = LoadTable(tableFile);
        var deSelected = 0;

        foreach (var col in table.Columns)
        {
            if (col.Selected && !definitionLogicalNames.Contains(col.LogicalName))
            {
                col.Selected = false;
                deSelected++;
            }
        }

        if (deSelected > 0)
        {
            SaveTable(tableFile, table);
            AnsiConsole.MarkupLine(
                $"[yellow]Nettoyé[/]  {tableName}.table " +
                $"([bold]{deSelected}[/] colonne(s) de-sélectionnée(s))");
        }
    }

    /// <summary>
    /// Traite un .table sans Definition correspondante :
    /// supprime si entièrement outil-généré (aucune colonne avec des Labels CRM),
    /// sinon de-sélectionne toutes les colonnes et conserve le fichier.
    /// </summary>
    private void HandleFullyOrphanedFile(string tableFile, string tableName)
    {
        var table = LoadTable(tableFile);

        // Heuristique "outil-généré" : aucune colonne n'a de Labels (données CRM réelles).
        // Un .table produit par le DefinitionManager a toujours des Labels sur ses colonnes.
        var hasRealCrmData = table.Columns.Any(c => c.Labels.Count > 0);

        if (!hasRealCrmData)
        {
            File.Delete(tableFile);
            AnsiConsole.MarkupLine(
                $"[red]Supprimé[/] {tableName}.table " +
                "(aucune Definition, aucune donnée CRM réelle)");
        }
        else
        {
            // Fichier avec données CRM → conserver mais tout de-sélectionner
            var deSelected = 0;
            foreach (var col in table.Columns.Where(c => c.Selected))
            {
                col.Selected = false;
                deSelected++;
            }

            if (deSelected > 0)
            {
                SaveTable(tableFile, table);
                AnsiConsole.MarkupLine(
                    $"[yellow]Conservé[/] {tableName}.table " +
                    $"(aucune Definition — {deSelected} colonne(s) de-sélectionnée(s))");
            }
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Helpers I/O
    // ──────────────────────────────────────────────────────────────────────────

    private string TablePath(string tableName)
        => Path.Combine(_tablesDirectory, $"{tableName}.table");

    private static CoreTable CreateMinimalTable(DefinitionInfo def) => new()
    {
        LogicalName = def.EntityName,
        Name = def.TableName,
        CollectionName = def.EntityCollectionName ?? string.Empty
    };

    private static CoreTable LoadTable(string path)
    {
        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<CoreTable>(json)
               ?? throw new InvalidDataException($"Impossible de désérialiser {path}");
    }

    private static void SaveTable(string path, CoreTable table)
    {
        var json = JsonConvert.SerializeObject(table, SerializerSettings);
        File.WriteAllText(path, json);
    }
}
}
