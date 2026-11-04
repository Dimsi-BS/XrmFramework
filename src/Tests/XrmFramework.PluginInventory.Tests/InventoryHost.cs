// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace XrmFramework.PluginInventory.Tests;

/// <summary>
///     Runs the published inventory host the way the CLI does, and returns what it said.
/// </summary>
/// <remarks>
///     The host and the fixtures are copied into this project's output by the
///     <c>CopyInventoryHostAndFixtures</c> target; see the project file for why they are copied
///     rather than located in their own projects' <c>bin</c>.
/// </remarks>
internal static class InventoryHost
{
    internal sealed record Result(int ExitCode, string Stdout, string Stderr);

    private static string TestDirectory => TestContext.CurrentContext.TestDirectory;

    internal static string HostDll =>
        Path.Combine(TestDirectory, "inventory-host", "XrmFramework.PluginInventory.dll");

    internal static string ShimDll =>
        Path.Combine(TestDirectory, "inventory-host", "System.Activities.dll");

    /// <summary>The fixture plugin assembly built for <paramref name="targetFramework" />.</summary>
    internal static string Fixture(string targetFramework) => Path.Combine(
        TestDirectory, "fixtures", targetFramework, "XrmFramework.PluginInventory.Fixtures.dll");

    internal static Result Run(string pluginAssemblyPath) => RunWithHost(HostDll, pluginAssemblyPath);

    /// <summary>
    ///     Runs a specific copy of the host, for the tests that need to take something away from it.
    /// </summary>
    internal static Result RunWithHost(string hostDll, string pluginAssemblyPath)
    {
        var psi = new ProcessStartInfo
        {
            FileName = DotnetMuxer(),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = new UTF8Encoding(false),
            StandardErrorEncoding = new UTF8Encoding(false),
        };
        psi.ArgumentList.Add(hostDll);
        psi.ArgumentList.Add(pluginAssemblyPath);

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException($"Unable to start '{psi.FileName}'.");

        // Both streams read before waiting: a full pipe buffer would otherwise deadlock the child.
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        process.WaitForExit();

        return new Result(process.ExitCode, stdout.GetAwaiter().GetResult(), stderr.GetAwaiter().GetResult());
    }

    /// <summary>
    ///     The <c>dotnet</c> executable, derived from the runtime this test is running on.
    /// </summary>
    /// <remarks>
    ///     The same derivation the product uses, and for the same reason: DOTNET_HOST_PATH is only
    ///     set when the SDK is the parent process, so the dependable source is the runtime directory
    ///     we are already executing from - <c>&lt;root&gt;/shared/Microsoft.NETCore.App/&lt;version&gt;</c>,
    ///     three levels under the root the muxer sits in.
    /// </remarks>
    private static string DotnetMuxer()
    {
        var fromSdk = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH");
        if (!string.IsNullOrWhiteSpace(fromSdk) && File.Exists(fromSdk))
            return fromSdk;

        var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
        if (!string.IsNullOrEmpty(runtimeDir))
        {
            var root = Path.GetFullPath(Path.Combine(runtimeDir, "..", "..", ".."));
            var candidate = Path.Combine(root, OperatingSystem.IsWindows() ? "dotnet.exe" : "dotnet");
            if (File.Exists(candidate))
                return candidate;
        }

        return OperatingSystem.IsWindows() ? "dotnet.exe" : "dotnet";
    }

    /// <summary>Fails the test with the host's own diagnostics when it did not succeed.</summary>
    internal static string JsonOrFail(string pluginAssemblyPath)
    {
        var result = Run(pluginAssemblyPath);

        Assert.That(result.ExitCode, Is.Zero,
            $"The inventory host failed.{Environment.NewLine}{result.Stderr}");

        // stdout purity is a contract, not a nicety: the CLI pipes it straight into
        // PluginInventoryReader, so a stray diagnostic would be a parse error at deploy time.
        Assert.That(result.Stderr, Is.Empty, "The host wrote to stderr on a successful run.");

        return result.Stdout;
    }

    /// <summary>Every target framework the fixtures are built for.</summary>
    internal static string[] FixtureTargetFrameworks =>
        Directory.GetDirectories(Path.Combine(TestDirectory, "fixtures"))
            .Select(Path.GetFileName)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
}
