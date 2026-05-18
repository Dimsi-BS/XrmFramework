// Copyright (c) Dimsi. All rights reserved.

namespace MsBuildTypeScript;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using System.Text;

internal class TsGenerator(TaskLoggingHelper log, string outputDirectory, string typePrefix)
{
    public void Generate(ITaskItem[] sourceFiles)
    {
        Directory.CreateDirectory(outputDirectory);
        Directory.CreateDirectory(Path.Combine(outputDirectory, "definitions"));

        var tablesBuilder = new StringBuilder();
        var tableList = new List<string>();
        tablesBuilder.AppendLine($"/// <reference path=\"./table.d.ts\" />\");");

        var enumsBuilder = new StringBuilder();
        var enumMapContent = new StringBuilder();

        foreach (var sourceFile in sourceFiles)
        {
            ProcessSourceFile(sourceFile, tablesBuilder, tableList, enumsBuilder, enumMapContent);
        }

        WriteTablesFile(tablesBuilder, tableList);
        WriteEnumsFile(enumsBuilder, enumMapContent);
    }

    private void ProcessSourceFile(
        ITaskItem sourceFile,
        StringBuilder tablesBuilder,
        List<string> tableList,
        StringBuilder enumsBuilder,
        StringBuilder enumMapContent)
    {
        var sourcePath = sourceFile.GetMetadata("FullPath");
        if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
        {
            log.LogWarning($"Source file not found: {sourcePath}");
            return;
        }

        var fileName = Path.GetFileNameWithoutExtension(sourcePath);
        var content = File.ReadAllText(sourcePath, Encoding.UTF8);

        var table = JsonConvert.DeserializeObject<Table>(content);
        if (table == null)
        {
            log.LogError($"Failed to deserialize table from {sourcePath}");
            return;
        }

        tablesBuilder.AppendLine($"/// <reference path=\"./definitions/{fileName}Definition.d.ts\" />");
        tableList.Add($"    {table.LogName}: {fileName}Definition extends Table ? {fileName}Definition : never");

        WriteDefinitionFile(fileName, content);
        AppendEnums(table.Enums, enumsBuilder, enumMapContent);
    }

    private void WriteDefinitionFile(string fileName, string content)
    {
        var outputPath = Path.Combine(outputDirectory, $"./definitions/{fileName}Definition.d.ts");

        var generated = new StringBuilder();
        generated
            .AppendLine($"{typePrefix} {fileName}Definition =")
            .AppendLine(content)
            .AppendLine();

        File.WriteAllText(outputPath, generated.ToString(), Encoding.UTF8);
        log.LogMessage(MessageImportance.High, $"Generated {outputPath}");
    }

    private static void AppendEnums(List<OptionSet> optionSets, StringBuilder enumsBuilder, StringBuilder enumMapContent)
    {
        foreach (var optionSet in optionSets)
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

    private void WriteTablesFile(StringBuilder tablesBuilder, List<string> tableList)
    {
        tablesBuilder
            .AppendLine()
            .AppendLine("type Tables = {")
            .AppendLine(string.Join(",\r\n", tableList))
            .AppendLine("};");

        var outputPath = Path.Combine(outputDirectory, "tables.d.ts");
        File.WriteAllText(outputPath, tablesBuilder.ToString(), Encoding.UTF8);
    }

    private void WriteEnumsFile(StringBuilder enumsBuilder, StringBuilder enumMapContent)
    {
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
}
