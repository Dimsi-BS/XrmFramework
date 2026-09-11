// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using Spectre.Console;

namespace XrmFramework.DeployUtils.Scaffolding
{
    /// <summary>
    /// Adds a standalone project (console app, Azure Function) to an existing solution — shared by
    /// <c>xrmframework new console</c> and <c>xrmframework new azurefunction</c>, replacing the
    /// <c>xrmConsoleProject</c> / <c>xrmAzureFunction</c> dotnet-new templates and their identical
    /// <c>pwsh</c> postAction (<c>dotnet sln add</c> on the newly generated project).
    /// </summary>
    public static class SimpleProjectScaffolder
    {
        public const string NameToken = "$safeprojectname$";

        /// <returns>Exit code: <c>0</c> success, <c>2</c> invalid input / target not usable, <c>3</c> unexpected error.</returns>
        public static int Create(string templateRoot, string name, string solutionRoot, string kindLabel)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                AnsiConsole.MarkupLine("[red]A project name is required.[/]");
                return 2;
            }

            if (!Directory.Exists(templateRoot))
            {
                AnsiConsole.MarkupLine($"[red]Scaffolding content not found:[/] {templateRoot}");
                return 2;
            }

            string solutionFile;
            try
            {
                solutionFile = TemplateScaffolder.FindSingleSolutionFile(solutionRoot);
            }
            catch (Exception ex) when (ex is FileNotFoundException or InvalidOperationException)
            {
                AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
                return 2;
            }

            var projectDir = Path.Combine(solutionRoot, name);
            if (Directory.Exists(projectDir) && Directory.EnumerateFileSystemEntries(projectDir).Any())
            {
                AnsiConsole.MarkupLine($"[red]Directory already exists and is not empty:[/] {projectDir}");
                return 2;
            }

            AnsiConsole.MarkupLine($"[bold]XrmFramework - new {kindLabel}[/]");
            AnsiConsole.MarkupLine($"  Name    : [cyan]{name}[/]");
            AnsiConsole.MarkupLine($"  Solution: [cyan]{solutionFile}[/]");
            AnsiConsole.WriteLine();

            try
            {
                TemplateScaffolder.CopyAndReplace(templateRoot, projectDir, NameToken, name);

                var projectPath = Path.Combine(projectDir, $"{name}.csproj");
                var (exitCode, output) = DotNetCliRunner.SlnAdd(solutionFile, projectPath);
                if (exitCode != 0)
                    throw new InvalidOperationException($"'dotnet sln add {projectPath}' failed:{Environment.NewLine}{output}");

                AnsiConsole.MarkupLine($"[blue]Added to solution:[/] {name}.csproj");
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"[green]{char.ToUpperInvariant(kindLabel[0])}{kindLabel.Substring(1)} project created.[/]");
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
