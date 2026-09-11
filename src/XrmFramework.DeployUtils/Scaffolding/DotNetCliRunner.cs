// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace XrmFramework.DeployUtils.Scaffolding
{
    /// <summary>
    /// Runs the <c>dotnet</c> CLI out-of-process for the two housekeeping steps scaffolding needs
    /// (adding a project to a solution, adding a project reference) — reusing its well-tested .sln
    /// and .csproj writers rather than hand-rolling them.
    /// </summary>
    internal static class DotNetCliRunner
    {
        /// <summary>Runs <c>dotnet sln &lt;solutionPath&gt; add &lt;projectPath&gt;</c>.</summary>
        public static (int ExitCode, string Output) SlnAdd(string solutionPath, string projectPath)
            => Run("sln", solutionPath, "add", projectPath);

        /// <summary>Runs <c>dotnet add &lt;projectPath&gt; reference &lt;referencedProjectPath&gt;</c>.</summary>
        public static (int ExitCode, string Output) AddReference(string projectPath, string referencedProjectPath)
            => Run("add", projectPath, "reference", referencedProjectPath);

        private static (int ExitCode, string Output) Run(params string[] args)
        {
            // ArgumentList isn't part of net462's reference surface (this library multi-targets it),
            // hence building a quoted Arguments string instead.
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = string.Join(" ", args.Select(QuoteArgument)),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = new UTF8Encoding(false),
                StandardErrorEncoding = new UTF8Encoding(false),
            };

            using var process = Process.Start(psi)
                ?? throw new InvalidOperationException("Unable to start 'dotnet'.");

            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            var output = stdoutTask.GetAwaiter().GetResult() + stderrTask.GetAwaiter().GetResult();
            return (process.ExitCode, output);
        }

        // None of these arguments come from arbitrary user input beyond file paths chosen by the
        // same user running the CLI, but quoting is still required for paths containing spaces.
        private static string QuoteArgument(string arg)
            => arg.Length > 0 && arg.IndexOfAny(new[] { ' ', '\t' }) < 0
                ? arg
                : "\"" + arg.Replace("\"", "\\\"") + "\"";
    }
}
