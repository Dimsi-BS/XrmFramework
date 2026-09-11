// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using Spectre.Console;

namespace XrmFramework.DeployUtils.Scaffolding
{
    /// <summary>
    /// Creates a new XrmFramework solution (Core, Plugins, Utils, Webresources) from the embedded
    /// scaffolding content — the <c>xrmframework new solution</c> replacement for the
    /// <c>xrmSolution</c> dotnet-new template and its <c>initXrm.ps1</c> postAction.
    /// </summary>
    public static class SolutionScaffolder
    {
        /// <summary>Token replaced by the solution name throughout the scaffolded content.</summary>
        public const string NameToken = "$safeprojectname$";

        /// <summary>
        /// Scaffolds a new solution named <paramref name="name"/> under <paramref name="outputDirectory"/>.
        /// </summary>
        /// <param name="templateRoot">Directory holding the embedded "Solution" scaffolding content.</param>
        /// <param name="name">Solution / root project name — replaces <see cref="NameToken"/>.</param>
        /// <param name="outputDirectory">Directory the solution's own folder is created into.</param>
        /// <returns>Exit code: <c>0</c> success, <c>2</c> invalid input / target not usable, <c>3</c> unexpected error.</returns>
        public static int Create(string templateRoot, string name, string outputDirectory)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                AnsiConsole.MarkupLine("[red]A solution name is required.[/]");
                return 2;
            }

            if (!Directory.Exists(templateRoot))
            {
                AnsiConsole.MarkupLine($"[red]Scaffolding content not found:[/] {templateRoot}");
                return 2;
            }

            var target = Path.Combine(outputDirectory, name);

            if (Directory.Exists(target) && Directory.EnumerateFileSystemEntries(target).Any())
            {
                AnsiConsole.MarkupLine($"[red]Directory already exists and is not empty:[/] {target}");
                return 2;
            }

            AnsiConsole.MarkupLine("[bold]XrmFramework - new solution[/]");
            AnsiConsole.MarkupLine($"  Name  : [cyan]{name}[/]");
            AnsiConsole.MarkupLine($"  Output: [cyan]{target}[/]");
            AnsiConsole.WriteLine();

            try
            {
                TemplateScaffolder.CopyAndReplace(templateRoot, target, NameToken, name);

                // Equivalent of dotnet-new's declarative "rename" map: a leading-dot file can't be
                // checked in as such without every tool along the way (git included) treating it
                // as the real thing, so the template ships it as "gitignore" and it's renamed here.
                var gitignore = Path.Combine(target, "gitignore");
                if (File.Exists(gitignore))
                    File.Move(gitignore, Path.Combine(target, ".gitignore"));

                // The sample stays as a reference; the real, gitignored config is materialized from it.
                // (dotnet new has no "copy to two outputs" primitive — see the .sample file itself.)
                var sample = Path.Combine(target, "Config", "connectionStrings.config.sample");
                var connectionStrings = Path.Combine(target, "Config", "connectionStrings.config");
                if (File.Exists(sample) && !File.Exists(connectionStrings))
                    File.Copy(sample, connectionStrings);

                AnsiConsole.MarkupLine("[green]Solution created.[/]");
                return 0;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                return 3;
            }
        }
    }
}
