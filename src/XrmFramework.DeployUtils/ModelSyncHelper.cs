// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Spectre.Console;
using XrmFramework.DeployUtils.ModelSync;
using CoreModel = XrmFramework.Core.Model;

namespace XrmFramework.DeployUtils
{
    /// <summary>
    /// Generates <c>.model</c> files from the hand-written binding models found in a compiled
    /// assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <c>.model</c> file is a declarative, generator-friendly stand-in for a hand-written
    /// <c>IBindingModel</c> class: <c>ModelSourceFileGenerator</c> reads it, together with the
    /// project's own <c>.table</c> files, and emits the exact same shape of class —
    /// <c>[CrmMapping]</c> / <c>[CrmLookup]</c> properties and the <c>ToBindingModel</c> /
    /// <c>ToEntity</c> pair — at compile time. This tool performs the reverse step: point it at an
    /// assembly that already has such classes, hand-written, and it reflects over each one to write
    /// out the <c>.model</c> file that reproduces it.
    /// </para>
    /// <para>
    /// Unlike <see cref="TableSyncHelper"/>, this is not a one-time version-upgrade tool — a
    /// <c>.model</c> file has no local edits or pulled CRM metadata to reconcile with, so each run
    /// simply overwrites the files for the classes still found. It can be run once to bulk-convert
    /// a project's binding models, or repeatedly while a project migrates a handful at a time.
    /// </para>
    /// </remarks>
    public static class ModelSyncHelper
    {
        private static readonly JsonSerializerSettings SerializerSettings = new()
        {
            Formatting = Formatting.Indented,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        /// <summary>
        /// Reflects over <paramref name="dllPath"/> and writes a <c>.model</c> file into
        /// <paramref name="modelsDirectory"/> for every hand-written <c>IBindingModel</c> class with
        /// a <c>[CrmEntity]</c> it finds.
        /// </summary>
        /// <param name="dllPath">Path to the compiled assembly to analyze.</param>
        /// <param name="modelsDirectory">
        /// Directory the <c>.model</c> files are written into — created if it does not exist yet.
        /// </param>
        /// <param name="sourceDirectory">
        /// When given, the directory holding the hand-written <c>.cs</c> files the classes came
        /// from: for each class a <c>.model</c> file was written for, its now-redundant
        /// <c>[CrmMapping]</c> / <c>[ChildRelationship]</c> / <c>[ExtendBindingModel]</c> properties
        /// are stripped from the source, leaving whatever else the project added by hand. Left
        /// <see langword="null"/>, only the <c>.model</c> files are written — the hand-written
        /// classes are not touched, and the project will not compile until they are reconciled by
        /// hand (duplicate members with the generated partial).
        /// </param>
        /// <returns>Exit code: <c>0</c> success, <c>2</c> file/directory not found, <c>3</c> unexpected error.</returns>
        public static int Sync(string dllPath, string modelsDirectory, string sourceDirectory = null)
        {
            AnsiConsole.MarkupLine("[bold]XrmFramework - generate .model files from a compiled assembly[/]");
            AnsiConsole.MarkupLine($"  DLL       : [cyan]{dllPath}[/]");
            AnsiConsole.MarkupLine($"  Directory : [cyan]{modelsDirectory}[/]");
            if (!string.IsNullOrEmpty(sourceDirectory))
                AnsiConsole.MarkupLine($"  Source    : [cyan]{sourceDirectory}[/] (hand-written classes will be stripped)");
            AnsiConsole.WriteLine();

            try
            {
                AnsiConsole.MarkupLine("Analyzing DLL...");
                var models = ModelDefinitionAnalyzer.ExtractModels(dllPath, out var skipped);

                foreach (var skip in skipped)
                    AnsiConsole.MarkupLine($"[yellow]Skipped[/] {Markup.Escape(skip.ClassName)} — {Markup.Escape(skip.Reason)}.");

                if (models.Count == 0)
                {
                    AnsiConsole.MarkupLine(skipped.Count > 0
                        ? "[yellow]Nothing was written — every candidate class had a custom base class and was skipped (see above).[/]"
                        : "[yellow]No hand-written binding model ([[CrmEntity]] + IBindingModel) found in the " +
                          "DLL: nothing was written. Check --dll points at the assembly that declares them.[/]");
                    return 0;
                }

                Directory.CreateDirectory(modelsDirectory);

                var written = 0;
                var writtenModels = new List<CoreModel>();

                foreach (var group in models.GroupBy(m => m.Name, StringComparer.Ordinal))
                {
                    var candidates = group.ToList();

                    if (candidates.Count > 1)
                    {
                        // Two classes named alike in different namespaces would otherwise silently
                        // overwrite one another under the same "{Name}.model" filename — left for a
                        // human to rename or move rather than guessed at here.
                        AnsiConsole.MarkupLine(
                            $"[yellow]Skipped[/] {group.Key}.model — {candidates.Count} classes share this " +
                            $"name ({string.Join(", ", candidates.Select(m => string.IsNullOrEmpty(m.ModelNamespace) ? "(no namespace)" : m.ModelNamespace))}); " +
                            "rename one of them and re-run.");
                        continue;
                    }

                    WriteModelFile(modelsDirectory, candidates[0]);
                    AnsiConsole.MarkupLine($"[green]Wrote[/] {group.Key}.model ({candidates[0].Properties.Count} propert{(candidates[0].Properties.Count == 1 ? "y" : "ies")})");
                    written++;
                    writtenModels.Add(candidates[0]);
                }

                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine(written > 0
                    ? $"[green]Done — {written} .model file(s) written.[/]"
                    : "[yellow]Nothing written.[/]");

                if (!string.IsNullOrEmpty(sourceDirectory) && writtenModels.Count > 0)
                {
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("[bold]Stripping the hand-written classes[/]");
                    new ModelSourceMigrator(sourceDirectory).Migrate(writtenModels);
                }

                return 0;
            }
            catch (FileNotFoundException ex)
            {
                AnsiConsole.MarkupLine($"[red]File not found:[/] {ex.FileName}");
                return 2;
            }
            catch (DirectoryNotFoundException ex)
            {
                AnsiConsole.MarkupLine($"[red]Directory not found:[/] {ex.Message}");
                return 2;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                return 3;
            }
        }

        private static void WriteModelFile(string modelsDirectory, CoreModel model)
        {
            var path = Path.Combine(modelsDirectory, $"{model.Name}.model");
            var json = JsonConvert.SerializeObject(model, SerializerSettings);
            File.WriteAllText(path, json);
        }
    }
}
