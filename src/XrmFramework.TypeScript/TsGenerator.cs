// Copyright (c) Dimsi. All rights reserved.

namespace MsBuildTypeScript;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using XrmFramework.Core;

/// <summary>
/// Turns the <c>.table</c> files of a project into the TypeScript definitions its form and ribbon
/// scripts are typed on.
/// </summary>
/// <remarks>
/// The inputs are the ones the C# source generator reads, folded by the same rules — the framework's
/// copy of a table and the project's copy of it are two declarations of one table, and
/// <see cref="TableCollection" /> is what reconciles them. Anything decided here rather than there
/// drifts: a locked name honoured in the generated C# and ignored in the generated TypeScript left
/// one project with <c>TeamType</c> on one side and <c>TypeDEquipe</c> on the other.
/// </remarks>
internal class TsGenerator(TaskLoggingHelper log, string outputDirectory, string typePrefix)
{
    /// <summary>
    /// The settings <c>TableFileStore</c> writes <c>.table</c> files with. A definition carries its
    /// table's JSON verbatim, so a formatting divergence would churn the generated files.
    /// </summary>
    private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        DefaultValueHandling = DefaultValueHandling.Ignore
    };

    public void Generate(ITaskItem[] sourceFiles)
    {
        Directory.CreateDirectory(outputDirectory);
        Directory.CreateDirectory(Path.Combine(outputDirectory, "definitions"));

        var tables = ReadTables(sourceFiles);
        var selection = OptionSetSelection.Of(tables);

        WriteDefinitionFiles(tables, selection);
        WriteEnumsFile(tables, selection);
    }

    /// <summary>
    /// Reads every <c>.table</c> into one table per logical name.
    /// </summary>
    private TableCollection ReadTables(ITaskItem[] sourceFiles)
    {
        var tables = new TableCollection();

        foreach (var sourceFile in sourceFiles)
        {
            var sourcePath = sourceFile.GetMetadata("FullPath");

            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
            {
                log.LogWarning($"Source file not found: {sourcePath}");
                continue;
            }

            var content = File.ReadAllText(sourcePath, Encoding.UTF8);

            Table? table;

            try
            {
                table = JsonConvert.DeserializeObject<Table>(content);
            }
            catch (JsonException e)
            {
                log.LogError($"Failed to deserialize table from {sourcePath}: {e.Message}");
                continue;
            }

            if (table == null)
            {
                log.LogError($"Failed to deserialize table from {sourcePath}");
                continue;
            }

            tables.Add(table);
        }

        return tables;
    }

    /// <summary>
    /// Writes one definition per table, and the <c>Tables</c> map pointing at them.
    /// </summary>
    private void WriteDefinitionFiles(TableCollection tables, OptionSetSelection selection)
    {
        var tablesBuilder = new StringBuilder();
        var tableList = new List<string>();

        tablesBuilder.AppendLine("/// <reference path=\"./table.d.ts\" />");

        foreach (var table in tables)
        {
            // Named after the table, as the generated C# class {Name}Definition is, and not after
            // the file: one table may be declared in two files, and a file is freely renamed.
            var definitionName = $"{table.Name}Definition";

            tablesBuilder.AppendLine($"/// <reference path=\"./definitions/{definitionName}.d.ts\" />");
            tableList.Add($"    {table.LogicalName}: {definitionName} extends Table ? {definitionName} : never");

            WriteDefinitionFile(definitionName, Declared(tables, table, selection));
        }

        tablesBuilder
            .AppendLine()
            .AppendLine("type Tables = {")
            .AppendLine(string.Join(",\r\n", tableList))
            .AppendLine("};");

        var outputPath = Path.Combine(outputDirectory, "tables.d.ts");
        File.WriteAllText(outputPath, tablesBuilder.ToString(), Encoding.UTF8);
    }

    private void WriteDefinitionFile(string definitionName, Table table)
    {
        var outputPath = Path.Combine(outputDirectory, "definitions", $"{definitionName}.d.ts");

        var generated = new StringBuilder()
            .AppendLine($"{typePrefix} {definitionName} =")
            .AppendLine(JsonConvert.SerializeObject(table, SerializerSettings))
            .AppendLine();

        File.WriteAllText(outputPath, generated.ToString(), Encoding.UTF8);
        log.LogMessage(MessageImportance.High, $"Generated {outputPath}");
    }

    /// <summary>
    /// The part of a folded table that generated code declares.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The four rules are the C# generator's, asked of the same selectors: an unselected column
    /// reaches no generated code, and no more do the relationships resting on it, the option sets
    /// carried by it, or a key the CRM knows under no name. Writing the whole table out instead
    /// typed the scripts on columns the C# side declares nothing for — and carried, for one
    /// project, 2113 columns and 5191 relationships where 641 and 319 are declared.
    /// </para>
    /// <para>
    /// A projection rather than a pruning in place: <see cref="RelationSelector" /> reads the
    /// columns of the table at the far end of a 1:N, so dropping one table's unselected columns
    /// would change which relationships its neighbours keep, according to the order the tables
    /// happen to be written in.
    /// </para>
    /// </remarks>
    private static Table Declared(TableCollection tables, Table table, OptionSetSelection selection)
    {
        var declared = new Table
        {
            LogicalName = table.LogicalName,
            Name = table.Name,
            CollectionName = table.CollectionName,
            IsLocked = table.IsLocked
        };

        foreach (var column in table.Columns.Where(c => c.Selected))
        {
            declared.Columns.Add(column);
        }

        foreach (var generated in selection.In(table))
        {
            declared.Enums.Add(generated.OptionSet);
        }

        foreach (var key in KeySelector.Declared(table))
        {
            declared.Keys.Add(key);
        }

        declared.ManyToOneRelationships.AddRange(RelationSelector.ManyToOne(table));
        declared.OneToManyRelationships.AddRange(RelationSelector.OneToMany(tables, table));
        declared.ManyToManyRelationships.AddRange(RelationSelector.ManyToMany(tables, table));

        return declared;
    }

    /// <summary>
    /// Writes the enums the option sets yield, and the <c>EnumMap</c> naming them.
    /// </summary>
    /// <remarks>
    /// Which option sets those are is <see cref="OptionSetSelection" />'s to say — the one place the
    /// rule lives, and the one the C# generator asks as well: an option set becomes an enum when a
    /// selected column carries it, global ones across every table and local ones within the table
    /// declaring them, one enum per C# name.
    /// </remarks>
    private void WriteEnumsFile(TableCollection tables, OptionSetSelection selection)
    {
        foreach (var conflict in selection.Conflicts)
        {
            // The C# generator reports this as an error (XRM1003) and fails the build on its own. A
            // warning here says why an enum is missing from the TypeScript without failing twice.
            log.LogWarning(
                $"The name '{conflict.Name}' stands for several different option sets: "
                + $"{string.Join(", ", conflict.Claims.Select(c => c.OptionSet.LogicalName))}. "
                + "Only the first one is declared.");
        }

        var enumsBuilder = new StringBuilder();
        var enumMapContent = new StringBuilder();

        foreach (var table in tables)
        {
            foreach (var generated in selection.In(table))
            {
                AppendEnum(generated.OptionSet, enumsBuilder, enumMapContent);
            }
        }

        if (enumMapContent.Length > 0)
        {
            enumsBuilder
                .AppendLine("declare global { interface EnumMap {")
                .AppendLine(enumMapContent.ToString())
                .AppendLine("} }");
        }

        var outputPath = Path.Combine(outputDirectory, "enums.ts");
        File.WriteAllText(outputPath, enumsBuilder.ToString(), Encoding.UTF8);
    }

    private static void AppendEnum(OptionSetEnum optionSet, StringBuilder enumsBuilder,
                                   StringBuilder enumMapContent)
    {
        enumsBuilder
            .AppendLine($"export const enum {optionSet.Name}")
            .AppendLine("{");

        foreach (var value in optionSet.Values)
        {
            enumsBuilder.AppendLine($"    {value.Name} = {value.Value},");
        }

        enumsBuilder.AppendLine("}").AppendLine();
        enumMapContent.AppendLine($"    {optionSet.Name}: {optionSet.Name};");
    }
}
