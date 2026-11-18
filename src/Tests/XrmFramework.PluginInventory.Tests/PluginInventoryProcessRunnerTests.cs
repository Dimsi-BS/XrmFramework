// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using XrmFramework.DeployUtils.Factories;

namespace XrmFramework.PluginInventory.Tests;

/// <summary>
///     Exercises the CLI's half of the inventory: locating the host, locating <c>dotnet</c>, running
///     the two, and turning a failure into something an operator can read.
/// </summary>
/// <remarks>
///     Driven through reflection because <c>PluginInventoryProcessRunner</c> is internal to
///     XrmFramework.DeployUtils, and it should stay that way - adding an
///     <c>InternalsVisibleTo</c> would change a shipped, strong-named assembly to suit a test.
///     Everything below goes through the runner's real entry point, so what is covered is the whole
///     path the CLI takes, muxer discovery included - the piece with the most ways to be wrong on a
///     machine that is not this one.
/// </remarks>
[TestFixture]
public class PluginInventoryProcessRunnerTests
{
    private const string HostOverrideVariable = "XRMFRAMEWORK_INVENTORY_DLL";

    private static readonly MethodInfo RunMethod = typeof(PluginInventoryReader).Assembly
        .GetType("XrmFramework.DeployUtils.Factories.PluginInventoryProcessRunner", throwOnError: true)!
        .GetMethod("Run", BindingFlags.Public | BindingFlags.Static)!;

    private string _previousOverride;

    [SetUp]
    public void PointTheRunnerAtTheCopiedHost()
    {
        _previousOverride = Environment.GetEnvironmentVariable(HostOverrideVariable);

        // The runner's normal lookup is relative to AppContext.BaseDirectory/inventory, which only
        // exists inside the built CLI. The override is the documented development hook, so using it
        // here exercises the same code an engineer uses against a local build of the host.
        Environment.SetEnvironmentVariable(HostOverrideVariable, InventoryHost.HostDll);
    }

    [TearDown]
    public void RestoreTheEnvironment()
        => Environment.SetEnvironmentVariable(HostOverrideVariable, _previousOverride);

    private static string Run(string pluginAssemblyPath)
    {
        try
        {
            return (string)RunMethod.Invoke(null, new object[] { pluginAssemblyPath });
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            throw ex.InnerException;
        }
    }

    /// <remarks>
    ///     The whole chain in one assertion: the runner found the host, found a <c>dotnet</c> to run
    ///     it with, and brought back a manifest the deployment's own reader can consume.
    /// </remarks>
    [Test]
    public void Runs_The_Host_And_Returns_A_Manifest_The_Reader_Accepts()
    {
        var json = Run(InventoryHost.Fixture("net48"));

        Assert.Multiple(() =>
        {
            Assert.That(PluginInventoryReader.ReadPlugins(json), Has.Count.EqualTo(1));
            Assert.That(PluginInventoryReader.ReadWorkflows(json), Has.Count.EqualTo(4));
        });
    }

    /// <remarks>
    ///     The host's stderr has to reach the operator. It is the only place the reason lives, and
    ///     the CLI shows nothing else when a deployment stops here.
    /// </remarks>
    [Test]
    public void A_Failing_Host_Surfaces_Its_Diagnostics()
    {
        var broken = Path.Combine(
            TestContext.CurrentContext.TestDirectory, "broken", "net48",
            "XrmFramework.PluginInventory.Fixtures.Broken.dll");

        var exception = Assert.Throws<InvalidOperationException>(() => Run(broken));

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Does.Contain("XrmFramework.PluginInventory.dll"));
            Assert.That(exception.Message, Does.Contain("BrokenActivity"));
        });
    }

    [Test]
    public void A_Missing_Host_Is_Reported_With_Where_It_Was_Expected()
    {
        Environment.SetEnvironmentVariable(HostOverrideVariable, null);

        // Nothing is installed under this test assembly's own inventory/ folder, so the runner
        // exhausts its lookup and has to say so rather than failing inside Process.Start.
        var exception = Assert.Throws<FileNotFoundException>(() => Run(InventoryHost.Fixture("net48")));

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Does.Contain("XrmFramework.PluginInventory.dll"));
            Assert.That(exception.Message, Does.Contain(HostOverrideVariable));
        });
    }
}
