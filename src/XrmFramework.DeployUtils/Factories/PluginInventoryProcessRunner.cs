// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#if !NET462_OR_GREATER
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace XrmFramework.DeployUtils.Factories
{
    /// <summary>
    ///     Launches the inventory host (<c>XrmFramework.PluginInventory.dll</c>) out of process and
    ///     retrieves the JSON manifest from its standard output.
    /// </summary>
    /// <remarks>
    ///     The host is a .NET application, so this works wherever the CLI works - Windows, macOS and
    ///     Linux alike. It needs no .NET Framework and no Mono: a net462/net48 plugin assembly loads
    ///     on a modern runtime, and the host supplies the two things that runtime lacks (a
    ///     <c>System.Activities</c> stand-in, and System.Configuration's implementation).
    ///     <para>
    ///         Out of process on purpose, and not for want of an alternative: this process has
    ///         <c>Microsoft.PowerPlatform.Dataverse.Client</c> loaded, and therefore its own .NET
    ///         <c>Microsoft.Xrm.Sdk</c>. Loading the plugin's net4x one alongside it would put two
    ///         assemblies of the same name, with different types, in the same place.
    ///     </para>
    ///     <para>
    ///         Launched through the <c>dotnet</c> muxer rather than an executable of its own: an
    ///         apphost is platform-specific, and the CLI ships as a single RID-less tool package, so
    ///         there is no one native launcher that would work everywhere it is installed.
    ///     </para>
    /// </remarks>
    internal static class PluginInventoryProcessRunner
    {
        private const string HostFileName = "XrmFramework.PluginInventory.dll";

        /// <summary>Points at the host assembly to run, overriding the one inside the tool.</summary>
        private const string HostOverrideVariable = "XRMFRAMEWORK_INVENTORY_DLL";

        /// <summary>Points at the <c>dotnet</c> executable to run it with.</summary>
        private const string MuxerOverrideVariable = "XRMFRAMEWORK_INVENTORY_DOTNET";

        public static string Run(string dllPath)
        {
            WarnAboutRetiredVariables();

            var host = LocateHost();

            var psi = new ProcessStartInfo
            {
                FileName = LocateDotnetMuxer(host),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = new UTF8Encoding(false),
                StandardErrorEncoding = new UTF8Encoding(false),
            };

            psi.ArgumentList.Add(host);
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
                    $"The plugin inventory ({HostFileName}) failed (code {process.ExitCode})."
                    + $"{Environment.NewLine}{stderr}");

            // Possible warnings (e.g. missing dependencies) without failure.
            if (!string.IsNullOrWhiteSpace(stderr))
                Console.Error.WriteLine(stderr);

            return stdout;
        }

        private static string LocateHost()
        {
            // 1) Explicit override, for a development build of the host.
            var overridePath = Environment.GetEnvironmentVariable(HostOverrideVariable);
            if (!string.IsNullOrWhiteSpace(overridePath) && File.Exists(overridePath))
                return overridePath;

            // 2) Shipped inside the tool, under inventory/, with its whole dependency closure.
            var candidate = Path.Combine(AppContext.BaseDirectory, "inventory", HostFileName);
            if (File.Exists(candidate))
                return candidate;

            // 3) Fallback: at the tool's root.
            candidate = Path.Combine(AppContext.BaseDirectory, HostFileName);
            if (File.Exists(candidate))
                return candidate;

            throw new FileNotFoundException(
                $"Inventory host not found ({HostFileName}). Expected under "
                + $"'{Path.Combine(AppContext.BaseDirectory, "inventory")}'. Reinstall the tool, or set "
                + $"{HostOverrideVariable} to point at the host assembly.");
        }

        /// <summary>
        ///     Finds the <c>dotnet</c> executable used to run the host.
        /// </summary>
        /// <remarks>
        ///     The dependable source is the third one, not the environment variables: this process is
        ///     already running on a .NET runtime, so the muxer that started it sits three directories
        ///     above that runtime's own folder. <c>DOTNET_HOST_PATH</c> is only set when the SDK is the
        ///     parent process, and <c>DOTNET_ROOT</c> only when someone set it - neither is normally
        ///     present when a user runs an installed tool from a shell.
        /// </remarks>
        internal static string LocateDotnetMuxer(string host)
        {
            var attempts = new List<string>();

            foreach (var candidate in MuxerCandidates(attempts))
                if (!string.IsNullOrWhiteSpace(candidate) && File.Exists(candidate))
                    return candidate;

            throw new FileNotFoundException(
                $"Unable to locate the 'dotnet' executable, needed to run the plugin inventory host "
                + $"('{host}')." + Environment.NewLine
                + "Tried, in order:" + Environment.NewLine
                + string.Join(Environment.NewLine, attempts) + Environment.NewLine
                + $"Install the .NET runtime, or set {MuxerOverrideVariable} to a dotnet executable.");
        }

        private static IEnumerable<string> MuxerCandidates(List<string> attempts)
        {
            var explicitPath = Environment.GetEnvironmentVariable(MuxerOverrideVariable);
            attempts.Add($"  {MuxerOverrideVariable,-30} {Describe(explicitPath)}");
            yield return explicitPath;

            // Set by the SDK when MSBuild, `dotnet test` or `dotnet run` is the parent process.
            var fromSdk = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH");
            attempts.Add($"  {"DOTNET_HOST_PATH",-30} {Describe(fromSdk)}");
            yield return fromSdk;

            var fromRuntime = MuxerBesideTheRunningRuntime();
            attempts.Add($"  {"the running runtime",-30} {Describe(fromRuntime)}");
            yield return fromRuntime;

            var root = Environment.GetEnvironmentVariable("DOTNET_ROOT");
            var fromRoot = string.IsNullOrWhiteSpace(root) ? null : Path.Combine(root, MuxerFileName());
            attempts.Add($"  {"DOTNET_ROOT",-30} {Describe(fromRoot)}");
            yield return fromRoot;

            // Scanned by hand rather than left to the OS: handing Process.Start a bare "dotnet"
            // turns a missing runtime into a Win32Exception with none of this context.
            var fromPath = MuxerOnPath();
            attempts.Add($"  {"PATH",-30} {Describe(fromPath)}");
            yield return fromPath;
        }

        /// <summary>
        ///     Derives the muxer from the runtime this process is executing on:
        ///     <c>&lt;root&gt;/shared/Microsoft.NETCore.App/&lt;version&gt;</c> sits three levels under the
        ///     root the executable lives in.
        /// </summary>
        private static string MuxerBesideTheRunningRuntime()
        {
            var runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);

            // Empty for a single-file or trimmed host, where there is nothing to derive from.
            if (string.IsNullOrEmpty(runtimeDirectory))
                return null;

            var root = Path.GetFullPath(Path.Combine(runtimeDirectory, "..", "..", ".."));
            return Path.Combine(root, MuxerFileName());
        }

        private static string MuxerOnPath()
        {
            var path = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrWhiteSpace(path))
                return null;

            foreach (var directory in path.Split(Path.PathSeparator))
            {
                if (string.IsNullOrWhiteSpace(directory))
                    continue;

                var candidate = Path.Combine(directory.Trim(), MuxerFileName());
                if (File.Exists(candidate))
                    return candidate;
            }

            return null;
        }

        private static string MuxerFileName()
            => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "dotnet.exe" : "dotnet";

        private static string Describe(string value)
            => string.IsNullOrWhiteSpace(value)
                ? "(not set)"
                : File.Exists(value) ? value : value + "  (does not exist)";

        /// <summary>
        ///     Tells anyone still carrying the .NET Framework era's variables that they do nothing.
        /// </summary>
        /// <remarks>
        ///     Both used to be the only way to deploy from macOS or Linux, so they are sitting in
        ///     shell profiles and CI scripts. Silently ignoring them would leave someone convinced
        ///     they still need Mono installed.
        /// </remarks>
        private static void WarnAboutRetiredVariables()
        {
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("XRMFRAMEWORK_INVENTORY_LAUNCHER")))
                Console.Error.WriteLine(
                    "XRMFRAMEWORK_INVENTORY_LAUNCHER is ignored: the plugin inventory is a .NET "
                    + "application and no longer needs Mono. You can unset it.");

            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("XRMFRAMEWORK_INVENTORY_EXE")))
                Console.Error.WriteLine(
                    "XRMFRAMEWORK_INVENTORY_EXE is ignored: the plugin inventory is no longer a "
                    + $"standalone executable. Use {HostOverrideVariable} to point at "
                    + $"{HostFileName} instead.");
        }
    }
}
#endif
