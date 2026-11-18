// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using XrmFramework.RemoteDebugger.Client;

namespace XrmFramework.PluginInventory.Tests;

/// <summary>
///     How a .NET runner reaches a .NET Framework plugin assembly it cannot reference.
/// </summary>
/// <remarks>
///     The fixtures this runs against are real net462/net48 plugin assemblies with their whole
///     output directory beside them - copy-local net462 <c>Microsoft.Xrm.Sdk</c> included, which is
///     what makes the second test here worth having.
/// </remarks>
[TestFixture]
public class PluginAssemblyLoaderTests
{
    private const string AccountPlugin = "XrmFramework.PluginInventory.Fixtures.AccountPlugin";
    private const string PluginAssembly = "XrmFramework.PluginInventory.Fixtures";

    private static string FixtureDirectory(string targetFramework)
        => Path.GetDirectoryName(InventoryHost.Fixture(targetFramework));

    // ── Doing nothing, when there is nothing to do ──────────────────────────────────

    /// <remarks>
    ///     Both answers are what let one startup file serve both runners: a .NET Framework runner
    ///     project-references the Plugins project and has no such folder, and must not be made to
    ///     care.
    /// </remarks>
    [Test]
    public void No_Directory_Is_Not_An_Error()
        => Assert.Multiple(() =>
        {
            Assert.That(PluginAssemblyLoader.UsePluginDirectory(null), Is.False);
            Assert.That(PluginAssemblyLoader.UsePluginDirectory(string.Empty), Is.False);
            Assert.That(PluginAssemblyLoader.UsePluginDirectory("   "), Is.False);
        });

    [Test]
    public void A_Directory_That_Is_Not_There_Is_Not_An_Error()
        => Assert.That(
            PluginAssemblyLoader.UsePluginDirectory(Path.Combine(Path.GetTempPath(), "xrmframework-absent-" + Guid.NewGuid().ToString("N"))),
            Is.False);

    // ── Reaching the plugin ─────────────────────────────────────────────────────────

    /// <remarks>
    ///     <para>
    ///         The whole point: after registration, the assembly-qualified name Dataverse sent
    ///         resolves, which is how <c>RemoteDebugger</c> gets from a context to a type.
    ///     </para>
    ///     <para>
    ///         And the plugin binds to the RUNNER's <c>Microsoft.Xrm.Sdk</c>, not to the net462 copy
    ///         sitting next to it in the fixture directory. That is the failure this guards: two
    ///         assemblies of that name in one process and the plugin rejects the
    ///         <c>IPluginExecutionContext</c> the runner hands it, with nothing in the cast failure
    ///         to say why. It holds because the runtime consults the resolver only for what it could
    ///         not resolve itself, so the SDK is never asked for.
    ///     </para>
    /// </remarks>
    [TestCase("net462")]
    [TestCase("net48")]
    public void A_Registered_Directory_Makes_The_Plugin_Resolvable(string targetFramework)
    {
        Assert.That(PluginAssemblyLoader.UsePluginDirectory(FixtureDirectory(targetFramework)), Is.True);

        // Version and PublicKeyToken stripped, the way RemoteDebugger.ExecutePluginType does it.
        var pluginType = Type.GetType($"{AccountPlugin}, {PluginAssembly}");

        Assert.Multiple(() =>
        {
            Assert.That(pluginType, Is.Not.Null,
                "The plugin type did not resolve, so the debugger would report it as unknown.");
            Assert.That(typeof(IPlugin).IsAssignableFrom(pluginType), Is.True,
                "The plugin does not implement the runner's IPlugin: a second Microsoft.Xrm.Sdk got "
                + "into the process.");

            Assert.That(
                AppDomain.CurrentDomain.GetAssemblies()
                    .Count(a => a.GetName().Name == "Microsoft.Xrm.Sdk"),
                Is.EqualTo(1),
                "More than one Microsoft.Xrm.Sdk is loaded.");
        });
    }

    /// <remarks>
    ///     Registering the same directory twice happens whenever a runner is restarted in-process or
    ///     a test fixture runs more than once; it must not pile up resolvers or start failing.
    /// </remarks>
    [Test]
    public void Registering_The_Same_Directory_Again_Is_Harmless()
    {
        var directory = FixtureDirectory("net48");

        Assert.Multiple(() =>
        {
            Assert.That(PluginAssemblyLoader.UsePluginDirectory(directory), Is.True);
            Assert.That(PluginAssemblyLoader.UsePluginDirectory(directory), Is.True);
            Assert.That(Type.GetType($"{AccountPlugin}, {PluginAssembly}"), Is.Not.Null);
        });
    }
}
