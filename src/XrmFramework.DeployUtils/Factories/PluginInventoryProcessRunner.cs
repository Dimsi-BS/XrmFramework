// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#if !NET462_OR_GREATER
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace XrmFramework.DeployUtils.Factories
{
    /// <summary>
    ///     Launches the net462 inventory tool (<c>XrmFramework.PluginInventory.exe</c>) out-of-process and
    ///     retrieves the JSON manifest from its standard output.
    /// </summary>
    /// <remarks>
    ///     Essential from a net8/net10 process: instantiating a net462 plugin (to execute its
    ///     step registration) requires the .NET Framework runtime. Deployment is therefore only possible
    ///     on Windows (or via a Mono launcher in development, see environment variables).
    /// </remarks>
    internal static class PluginInventoryProcessRunner
    {
        private const string ExeFileName = "XrmFramework.PluginInventory.exe";

        public static string Run(string dllPath)
        {
            var exe = LocateExe();

            var psi = new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = new UTF8Encoding(false),
                StandardErrorEncoding = new UTF8Encoding(false),
            };

            // On Windows, the net462 exe runs directly. In dev (macOS/Linux), a launcher
            // (e.g. "mono") can be provided via XRMFRAMEWORK_INVENTORY_LAUNCHER.
            var launcher = Environment.GetEnvironmentVariable("XRMFRAMEWORK_INVENTORY_LAUNCHER");
            if (!string.IsNullOrWhiteSpace(launcher))
            {
                psi.FileName = launcher;
                psi.ArgumentList.Add(exe);
            }
            else
            {
                psi.FileName = exe;
            }

            psi.ArgumentList.Add(dllPath);

            using var process = Process.Start(psi)
                ?? throw new InvalidOperationException($"Unable to start '{psi.FileName}'.");

            // Asynchronous reading of both streams to avoid any buffer deadlock.
            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            var stdout = stdoutTask.GetAwaiter().GetResult();
            var stderr = stderrTask.GetAwaiter().GetResult();

            if (process.ExitCode != 0)
                throw new InvalidOperationException(
                    $"The plugin inventory ({ExeFileName}) failed (code {process.ExitCode}).{Environment.NewLine}{stderr}");

            // Possible warnings (e.g. missing dependencies) without failure.
            if (!string.IsNullOrWhiteSpace(stderr))
                Console.Error.WriteLine(stderr);

            return stdout;
        }

        private static string LocateExe()
        {
            // 1) Explicit override.
            var overridePath = Environment.GetEnvironmentVariable("XRMFRAMEWORK_INVENTORY_EXE");
            if (!string.IsNullOrWhiteSpace(overridePath) && File.Exists(overridePath))
                return overridePath;

            // 2) Embedded next to the tool, under inventory/.
            var candidate = Path.Combine(AppContext.BaseDirectory, "inventory", ExeFileName);
            if (File.Exists(candidate))
                return candidate;

            // 3) Fallback: at the tool's root.
            candidate = Path.Combine(AppContext.BaseDirectory, ExeFileName);
            if (File.Exists(candidate))
                return candidate;

            throw new FileNotFoundException(
                $"Inventory tool not found ({ExeFileName}). Expected under " +
                $"'{Path.Combine(AppContext.BaseDirectory, "inventory")}'. Set the " +
                "XRMFRAMEWORK_INVENTORY_EXE environment variable to point to the net462 executable.");
        }
    }
}
#endif
