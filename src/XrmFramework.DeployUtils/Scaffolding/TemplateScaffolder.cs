// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace XrmFramework.DeployUtils.Scaffolding
{
    /// <summary>
    /// Copies a template's file tree to an output directory, replacing a name token in file/directory
    /// names and in the content of text files — the in-process equivalent of what <c>dotnet new</c>'s
    /// <c>sourceName</c> substitution does, without depending on the Template Engine.
    /// </summary>
    public static class TemplateScaffolder
    {
        // Extensions (and the no-extension case, e.g. "gitignore") treated as text and searched for
        // the token. Anything else (the .snk strong-name key, images...) is copied byte for byte:
        // text-replacing a binary file would corrupt it.
        private static readonly HashSet<string> TextExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            "", ".cs", ".csproj", ".sln", ".config", ".sample", ".json", ".props", ".targets",
            ".xml", ".ts", ".js", ".html", ".css", ".md", ".txt", ".yml", ".yaml", ".editorconfig",
            ".gitignore"
        };

        /// <summary>
        /// Copies every file under <paramref name="sourceRoot"/> into <paramref name="destinationRoot"/>,
        /// replacing <paramref name="token"/> with <paramref name="replacement"/> in directory names,
        /// file names, and the content of recognized text files.
        /// </summary>
        public static void CopyAndReplace(string sourceRoot, string destinationRoot, string token, string replacement)
        {
            sourceRoot = Path.GetFullPath(sourceRoot);
            Directory.CreateDirectory(destinationRoot);

            foreach (var sourceDir in Directory.EnumerateDirectories(sourceRoot, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(Path.Combine(destinationRoot, MapRelativePath(sourceRoot, sourceDir, token, replacement)));
            }

            foreach (var sourceFile in Directory.EnumerateFiles(sourceRoot, "*", SearchOption.AllDirectories))
            {
                var destFile = Path.Combine(destinationRoot, MapRelativePath(sourceRoot, sourceFile, token, replacement));
                Directory.CreateDirectory(Path.GetDirectoryName(destFile) ?? destinationRoot);

                if (IsTextFile(sourceFile))
                {
                    var content = File.ReadAllText(sourceFile);
                    File.WriteAllText(destFile, content.Replace(token, replacement));
                }
                else
                {
                    File.Copy(sourceFile, destFile, overwrite: true);
                }
            }
        }

        // Path.GetRelativePath isn't available on net462 (this library multi-targets it), hence
        // the plain prefix strip rather than a BCL relative-path computation.
        private static string MapRelativePath(string root, string fullPath, string token, string replacement)
        {
            var relative = fullPath.Substring(root.Length)
                                    .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            return relative.Replace(token, replacement);
        }

        private static bool IsTextFile(string path) => TextExtensions.Contains(Path.GetExtension(path));
    }
}
