// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XrmFramework.DeployUtils.Scaffolding
{
    /// <summary>
    /// Copies a template's file tree to an output directory, replacing a name token in file/directory
    /// names and in the content of text files — the in-process equivalent of what <c>dotnet new</c>'s
    /// <c>sourceName</c> substitution does, without depending on the Template Engine.
    /// </summary>
    public static class TemplateScaffolder
    {
        /// <summary>
        /// Token the scaffolding templates carry in their &lt;TargetFramework(s)&gt; entries for the
        /// projects that run inside the Dataverse sandbox (plugins/workflows) or alongside them
        /// (Deploy.*, RemoteDebugger). It is literally the default moniker, so a template left
        /// unsubstituted still produces a valid project.
        /// </summary>
        public const string FrameworkToken = DefaultFramework;

        /// <summary>
        /// .NET Framework monikers a scaffolded plugin project may target. Dataverse supports
        /// both .NET Framework 4.6.2 and 4.8 for plugins and workflow activities.
        /// </summary>
        public static readonly string[] SupportedFrameworks = { "net48", "net462" };

        /// <summary>
        /// Default moniker used when the caller does not pick one: .NET Framework 4.8, the
        /// current target for Dataverse plugins and workflow activities. net462 stays available
        /// through <c>--framework</c> for solutions that must keep it.
        /// </summary>
        public const string DefaultFramework = "net48";

        /// <summary>
        /// Checks <paramref name="framework"/> against <see cref="SupportedFrameworks"/>, returning the
        /// normalized (lower-case) moniker, or <c>null</c> when it is not a supported value.
        /// </summary>
        public static string? NormalizeFramework(string? framework)
        {
            if (string.IsNullOrWhiteSpace(framework))
                return DefaultFramework;

            var normalized = framework!.Trim().ToLowerInvariant();

            return Array.IndexOf(SupportedFrameworks, normalized) >= 0 ? normalized : null;
        }

        // Extensions (and the no-extension case, e.g. "gitignore") treated as text and searched for
        // the token. Anything else (the .snk strong-name key, images...) is copied byte for byte:
        // text-replacing a binary file would corrupt it.
        private static readonly HashSet<string> TextExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            "", ".cs", ".csproj", ".sln", ".slnx", ".config", ".sample", ".json", ".props", ".targets",
            ".xml", ".ts", ".js", ".html", ".css", ".md", ".txt", ".yml", ".yaml", ".editorconfig",
            ".gitignore"
        };

        /// <summary>
        /// Copies every file under <paramref name="sourceRoot"/> into <paramref name="destinationRoot"/>,
        /// replacing <paramref name="token"/> with <paramref name="replacement"/> in directory names,
        /// file names, and the content of recognized text files.
        /// </summary>
        public static void CopyAndReplace(string sourceRoot, string destinationRoot, string token, string replacement)
            => CopyAndReplace(sourceRoot, destinationRoot, new[] { new KeyValuePair<string, string>(token, replacement) });

        /// <summary>
        /// Same as <see cref="CopyAndReplace(string, string, string, string)"/> but applies several
        /// token substitutions — the project name plus, for plugin templates, the target framework
        /// moniker the generated projects compile against.
        /// </summary>
        public static void CopyAndReplace(string sourceRoot, string destinationRoot, IEnumerable<KeyValuePair<string, string>> replacements)
        {
            var substitutions = replacements.ToArray();

            sourceRoot = Path.GetFullPath(sourceRoot);
            Directory.CreateDirectory(destinationRoot);

            foreach (var sourceDir in Directory.EnumerateDirectories(sourceRoot, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(Path.Combine(destinationRoot, MapRelativePath(sourceRoot, sourceDir, substitutions)));
            }

            foreach (var sourceFile in Directory.EnumerateFiles(sourceRoot, "*", SearchOption.AllDirectories))
            {
                var destFile = Path.Combine(destinationRoot, MapRelativePath(sourceRoot, sourceFile, substitutions));
                Directory.CreateDirectory(Path.GetDirectoryName(destFile) ?? destinationRoot);

                if (IsTextFile(sourceFile))
                {
                    File.WriteAllText(destFile, Replace(File.ReadAllText(sourceFile), substitutions));
                }
                else
                {
                    File.Copy(sourceFile, destFile, overwrite: true);
                }
            }
        }

        // Path.GetRelativePath isn't available on net462 (this library multi-targets it), hence
        // the plain prefix strip rather than a BCL relative-path computation.
        private static string MapRelativePath(string root, string fullPath, KeyValuePair<string, string>[] substitutions)
        {
            var relative = fullPath.Substring(root.Length)
                                    .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            return Replace(relative, substitutions);
        }

        private static string Replace(string value, KeyValuePair<string, string>[] substitutions)
        {
            foreach (var substitution in substitutions)
                value = value.Replace(substitution.Key, substitution.Value);

            return value;
        }

        private static bool IsTextFile(string path) => TextExtensions.Contains(Path.GetExtension(path));

        /// <summary>
        /// Finds the single solution file (<c>.sln</c> or the newer <c>.slnx</c>) directly under
        /// <paramref name="directory"/> — what <c>xrmframework new plugin/console/azurefunction</c>
        /// add a project to. <c>new solution</c> only ever creates <c>.slnx</c>, but a solution
        /// generated before that switch (or hand-created) may still be a classic <c>.sln</c>.
        /// </summary>
        /// <exception cref="FileNotFoundException">No solution file found.</exception>
        /// <exception cref="InvalidOperationException">More than one solution file found.</exception>
        public static string FindSingleSolutionFile(string directory)
        {
            if (!Directory.Exists(directory))
                throw new FileNotFoundException($"Solution directory not found: {directory}", directory);

            var solutionFiles = Directory.EnumerateFiles(directory, "*.sln")
                                          .Concat(Directory.EnumerateFiles(directory, "*.slnx"))
                                          .ToArray();

            return solutionFiles.Length switch
            {
                0 => throw new FileNotFoundException($"No .sln/.slnx file found under {directory}.", directory),
                1 => solutionFiles[0],
                _ => throw new InvalidOperationException($"Several solution files found under {directory}; expected exactly one."),
            };
        }
    }
}
