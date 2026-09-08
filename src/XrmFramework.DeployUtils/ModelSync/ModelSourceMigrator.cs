// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Spectre.Console;
using CoreModel = XrmFramework.Core.Model;

namespace XrmFramework.DeployUtils.ModelSync
{
    /// <summary>
    /// Second half of a <c>migrate sync-models</c> run: once the <c>.model</c> files are written,
    /// strips the now-redundant properties from the hand-written classes they were read from.
    /// </summary>
    /// <remarks>
    /// A <c>.model</c> file names a class (<see cref="CoreModel.Name"/>) but not the source file it
    /// lives in — unlike a <c>.table</c>, whose companion <c>*Definition.cs</c> follows a fixed
    /// naming convention. Every <c>.cs</c> file under the source directory is therefore scanned for a
    /// class declaration matching one of the names, rather than assumed from a file name.
    /// </remarks>
    public sealed class ModelSourceMigrator
    {
        private readonly string _sourceDirectory;

        public ModelSourceMigrator(string sourceDirectory)
        {
            if (!Directory.Exists(sourceDirectory))
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDirectory}");

            _sourceDirectory = sourceDirectory;
        }

        /// <summary>
        /// Strips the mapped properties of every class named in <paramref name="models"/> from the
        /// <c>.cs</c> files under the source directory. Returns the number of classes found in no
        /// file, which is what the caller needs to warn about a partial migration.
        /// </summary>
        /// <remarks>
        /// File-centric, not model-centric: a single <c>.cs</c> file can declare several of the
        /// classes named in <paramref name="models"/> (a project is free to group more than one
        /// binding model, plus unrelated hand-written code, in one file). Every matching class is
        /// stripped from the file's evolving in-memory text before anything is written back, and the
        /// file's fate — deleted outright vs. rewritten as <c>*.partial.cs</c> — is decided once, from
        /// what is left of the whole file, never from a single class's outcome in isolation.
        /// </remarks>
        public int Migrate(IReadOnlyList<CoreModel> models)
        {
            if (models.Count == 0) return 0;

            var csFiles = Directory.GetFiles(_sourceDirectory, "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.EndsWith(".partial.cs", StringComparison.OrdinalIgnoreCase))
                .Where(path => !IsUnderBuildOutput(path))
                .ToList();

            var deleted = 0;
            var rewritten = 0;
            var foundNames = new HashSet<string>(StringComparer.Ordinal);

            foreach (var file in csFiles)
            {
                string source;
                try
                {
                    source = File.ReadAllText(file);
                }
                catch (IOException)
                {
                    continue;
                }

                var current = source;
                var matched = new List<(CoreModel Model, ModelSourceRewriteResult Result)>();

                foreach (var model in models)
                {
                    // A cheap textual pre-check before the real scan: most files in the directory do
                    // not declare this particular class at all.
                    if (current.IndexOf(model.Name, StringComparison.Ordinal) < 0)
                        continue;

                    var result = BindingModelSourceRewriter.Rewrite(current, model.Name);
                    if (result.Outcome == ModelSourceRewriteOutcome.Skipped)
                        continue;

                    matched.Add((model, result));
                    current = result.NewText;
                }

                if (matched.Count == 0)
                    continue;

                foreach (var entry in matched)
                    foundNames.Add(entry.Model.Name);

                ApplyFile(file, current, matched, ref deleted, ref rewritten);
            }

            var notFound = 0;

            foreach (var model in models)
            {
                if (foundNames.Contains(model.Name))
                    continue;

                notFound++;
                AnsiConsole.MarkupLine(
                    $"[yellow]No source file found[/] declaring class {Markup.Escape(model.Name)} " +
                    $"under {Markup.Escape(_sourceDirectory)} — its .model file was written, but the " +
                    "hand-written class was not stripped.");
            }

            AnsiConsole.MarkupLine(
                $"  [bold]{deleted}[/] file(s) deleted (nothing survived), " +
                $"[bold]{rewritten}[/] converted to .partial.cs" +
                (notFound > 0 ? $", [yellow]{notFound}[/] class(es) not found in any file" : string.Empty) + ".");

            return notFound;
        }

        /// <summary>
        /// Writes back the result of stripping every class <paramref name="matched"/> found in
        /// <paramref name="file"/>. The file is only ever deleted outright when nothing but boilerplate
        /// is left of it — a file that still declares other content (an unrelated class, a class not
        /// among <paramref name="matched"/>) is rewritten as <c>*.partial.cs</c> instead, even if every
        /// class that WAS matched turned out fully redundant on its own.
        /// </summary>
        private void ApplyFile(string file, string current,
            List<(CoreModel Model, ModelSourceRewriteResult Result)> matched, ref int deleted, ref int rewritten)
        {
            var fileName = Path.GetFileName(file);
            var classNames = string.Join(", ", matched.Select(m => m.Model.Name).ToArray());
            var removedCount = matched.Sum(m => m.Result.RemovedMembers.Count);

            if (BindingModelSourceRewriter.IsEmptyOfMembers(current))
            {
                File.Delete(file);
                deleted++;
                AnsiConsole.MarkupLine(
                    $"[red]Deleted[/] {Markup.Escape(fileName)} " +
                    $"(entirely regenerated from its .model file(s): {Markup.Escape(classNames)})");
                return;
            }

            var partialPath = Path.Combine(
                Path.GetDirectoryName(file) ?? _sourceDirectory,
                Path.GetFileNameWithoutExtension(file) + ".partial.cs");

            if (File.Exists(partialPath) && !string.Equals(partialPath, file, StringComparison.OrdinalIgnoreCase))
            {
                AnsiConsole.MarkupLine(
                    $"[yellow]Left as-is[/] {Markup.Escape(fileName)} " +
                    $"({Markup.Escape(Path.GetFileName(partialPath))} already exists — merge the two files by hand)");
                return;
            }

            File.WriteAllText(partialPath, current, DetectEncoding(file));

            if (!string.Equals(partialPath, file, StringComparison.OrdinalIgnoreCase))
                File.Delete(file);

            rewritten++;

            AnsiConsole.MarkupLine(
                $"[blue]Converted[/] {Markup.Escape(fileName)} -> {Markup.Escape(Path.GetFileName(partialPath))} " +
                $"([bold]{matched.Count}[/] class(es): {Markup.Escape(classNames)}, " +
                $"[bold]{removedCount}[/] propert{(removedCount == 1 ? "y" : "ies")} removed)");
        }

        /// <summary>
        /// A path under a <c>bin</c> or <c>obj</c> directory — build output and restored/generated
        /// sources, never a hand-written class worth scanning.
        /// </summary>
        private static bool IsUnderBuildOutput(string path)
        {
            foreach (var segment in path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
            {
                if (string.Equals(segment, "bin", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(segment, "obj", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>Reuses the source file's encoding so that migration alone does not add or drop a BOM.</summary>
        private static Encoding DetectEncoding(string path)
        {
            var preamble = new byte[3];

            using (var stream = File.OpenRead(path))
            {
                var read = stream.Read(preamble, 0, preamble.Length);
                var hasBom = read == 3 && preamble[0] == 0xEF && preamble[1] == 0xBB && preamble[2] == 0xBF;

                return new UTF8Encoding(hasBom);
            }
        }
    }
}
