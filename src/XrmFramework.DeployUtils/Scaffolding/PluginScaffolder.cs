// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Spectre.Console;

namespace XrmFramework.DeployUtils.Scaffolding
{
    /// <summary>
    /// Adds a new plugin project (and its Deploy.* companion) to an existing XrmFramework solution —
    /// the <c>xrmframework new plugin</c> replacement for the <c>xrmPluginProject</c> dotnet-new
    /// template and its <c>initXrm.ps1</c> postAction.
    /// </summary>
    /// <remarks>
    /// The dotnet-new template had to generate into a wrapper folder then <c>mv</c> its content up
    /// into the already-existing solution, because the Template Engine always writes into its own
    /// output directory. Writing the files straight to their final path under
    /// <paramref name="solutionRoot"/> — the whole point of leaving dotnet-new — makes that move a
    /// non-issue: the token-bearing folder names already produce the right layout.
    /// </remarks>
    public static class PluginScaffolder
    {
        public const string NameToken = "$safeprojectname$";

        /// <returns>Exit code: <c>0</c> success, <c>2</c> invalid input / target not usable, <c>3</c> unexpected error.</returns>
        public static int Create(string templateRoot, string name, string solutionRoot, string? solutionUniqueName)
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
                solutionFile = FindSingleSolutionFile(solutionRoot);
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

            var configFileName = Path.Combine(solutionRoot, "Config", "xrmFramework.config");
            if (!File.Exists(configFileName))
            {
                AnsiConsole.MarkupLine($"[red]Configuration file not found:[/] {configFileName}");
                return 2;
            }

            if (string.IsNullOrWhiteSpace(solutionUniqueName))
                solutionUniqueName = AnsiConsole.Ask<string>("Solution unique name:");

            AnsiConsole.MarkupLine("[bold]XrmFramework - new plugin[/]");
            AnsiConsole.MarkupLine($"  Name    : [cyan]{name}[/]");
            AnsiConsole.MarkupLine($"  Solution: [cyan]{solutionFile}[/]");
            AnsiConsole.WriteLine();

            try
            {
                // The template's own folder names already carry the token ($safeprojectname$ and
                // Deploy.$safeprojectname$), so copying straight into solutionRoot lands everything
                // at its final path — no wrapper folder to unwrap or merge afterwards.
                TemplateScaffolder.CopyAndReplace(templateRoot, solutionRoot, NameToken, name);

                var projectPath = Path.Combine(solutionRoot, name, $"{name}.csproj");
                var deployProjectPath = Path.Combine(solutionRoot, "Utils", $"Deploy.{name}", $"Deploy.{name}.csproj");
                var remoteDebuggerProjectPath = Path.Combine(solutionRoot, "Utils", "RemoteDebugger", "RemoteDebugger.csproj");

                AddToSolution(solutionFile, projectPath);
                AddToSolution(solutionFile, deployProjectPath);

                if (File.Exists(remoteDebuggerProjectPath))
                    AddReference(remoteDebuggerProjectPath, projectPath);
                else
                    AnsiConsole.MarkupLine(
                        $"[yellow]RemoteDebugger project not found ({remoteDebuggerProjectPath}) — reference not added.[/]");

                RegisterProject(configFileName, name, solutionUniqueName!);

                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[green]Plugin project created.[/]");
                return 0;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                return 3;
            }
        }

        private static string FindSingleSolutionFile(string solutionRoot)
        {
            if (!Directory.Exists(solutionRoot))
                throw new FileNotFoundException($"Solution directory not found: {solutionRoot}", solutionRoot);

            var solutionFiles = Directory.GetFiles(solutionRoot, "*.sln");

            return solutionFiles.Length switch
            {
                0 => throw new FileNotFoundException($"No .sln file found under {solutionRoot}.", solutionRoot),
                1 => solutionFiles[0],
                _ => throw new InvalidOperationException($"Several .sln files found under {solutionRoot}; expected exactly one."),
            };
        }

        private static void AddToSolution(string solutionFile, string projectPath)
        {
            var (exitCode, output) = DotNetCliRunner.SlnAdd(solutionFile, projectPath);
            if (exitCode != 0)
                throw new InvalidOperationException($"'dotnet sln add {projectPath}' failed:{Environment.NewLine}{output}");

            AnsiConsole.MarkupLine($"[blue]Added to solution:[/] {Path.GetFileName(projectPath)}");
        }

        private static void AddReference(string projectPath, string referencedProjectPath)
        {
            var (exitCode, output) = DotNetCliRunner.AddReference(projectPath, referencedProjectPath);
            if (exitCode != 0)
                throw new InvalidOperationException($"'dotnet add {projectPath} reference {referencedProjectPath}' failed:{Environment.NewLine}{output}");

            AnsiConsole.MarkupLine($"[blue]Reference added:[/] {Path.GetFileName(projectPath)} -> {Path.GetFileName(referencedProjectPath)}");
        }

        /// <summary>
        /// Appends an <c>&lt;add name="..." targetSolution="..." type="PluginsWorkflows"/&gt;</c>
        /// element under &lt;xrmFramework&gt;/&lt;projects&gt; in <paramref name="configFileName"/> —
        /// the schema <see cref="Configuration.XrmFrameworkSection"/> reads back at deploy time.
        /// </summary>
        private static void RegisterProject(string configFileName, string name, string solutionUniqueName)
        {
            var xmlDoc = XDocument.Load(configFileName);
            var projects = xmlDoc.Root?.Element("projects")
                ?? throw new InvalidOperationException($"'{configFileName}' has no <projects> element under <xrmFramework>.");

            projects.Add(new XElement("add",
                new XAttribute("name", name),
                new XAttribute("targetSolution", solutionUniqueName),
                new XAttribute("type", "PluginsWorkflows")));

            xmlDoc.Save(configFileName);

            AnsiConsole.MarkupLine($"[blue]Registered in xrmFramework.config:[/] {name} -> {solutionUniqueName}");
        }
    }
}
