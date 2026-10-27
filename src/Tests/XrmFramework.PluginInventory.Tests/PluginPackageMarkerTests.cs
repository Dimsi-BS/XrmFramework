// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.Json;
using NUnit.Framework;
using XrmFramework.DeployUtils.Factories;

namespace XrmFramework.PluginInventory.Tests;

/// <summary>
///     The producer half of the plugin-package marker: the inventory reports, in its manifest,
///     whether the assembly it just walked was built as a Dataverse plugin package.
/// </summary>
/// <remarks>
///     This is what lets the deployment take the shape from the build instead of from a second
///     declaration in <c>xrmFramework.config</c>. The inventory is the right place to read it: it
///     already has the assembly loaded, while the deployment side deliberately reads a plugin DLL's
///     identity without loading it, so a net8 host can describe a net462 assembly.
/// </remarks>
[TestFixture]
public class PluginPackageMarkerTests
{
    private static string Manifest(bool usesPluginPackage) => ManifestJson.Build(
        new List<PluginInfo>(), new List<WorkflowInfo>(), new List<CustomApiInfo>(), usesPluginPackage);

    /// <summary>
    ///     The flag is emitted as a real boolean at the root of the manifest, next to the three
    ///     collections rather than inside them: it describes the assembly, not its contents.
    /// </summary>
    [TestCase(true)]
    [TestCase(false)]
    public void Manifest_CarriesTheFlagAtItsRoot(bool usesPluginPackage)
    {
        using var doc = JsonDocument.Parse(Manifest(usesPluginPackage));

        var flag = doc.RootElement.GetProperty("pluginPackage");

        Assert.That(flag.ValueKind,
            Is.EqualTo(usesPluginPackage ? JsonValueKind.True : JsonValueKind.False));
    }

    /// <summary>
    ///     Adding a root field must not disturb the collections the reader already consumes - the
    ///     manifest is hand-rolled JSON, so a stray comma is a real possibility.
    /// </summary>
    [Test]
    public void Manifest_StaysWellFormedWithTheNewField()
    {
        using var doc = JsonDocument.Parse(Manifest(true));

        Assert.Multiple(() =>
        {
            Assert.That(doc.RootElement.GetProperty("plugins").ValueKind, Is.EqualTo(JsonValueKind.Array));
            Assert.That(doc.RootElement.GetProperty("workflows").ValueKind, Is.EqualTo(JsonValueKind.Array));
            Assert.That(doc.RootElement.GetProperty("customApis").ValueKind, Is.EqualTo(JsonValueKind.Array));
        });
    }

    /// <summary>
    ///     The contract that matters: what the inventory writes is what the deployment reads. These
    ///     are two assemblies, built for different target frameworks, agreeing on one JSON field.
    /// </summary>
    [TestCase(true)]
    [TestCase(false)]
    public void ProducerAndConsumer_AgreeOnTheFlag(bool usesPluginPackage)
    {
        Assert.That(PluginInventoryReader.ReadUsesPluginPackage(Manifest(usesPluginPackage)),
            Is.EqualTo(usesPluginPackage));
    }

    /// <summary>
    ///     The real reflection path, on an assembly that carries no marker: this test assembly is
    ///     not built as a plugin package, and the engine has to say so rather than defaulting to
    ///     package mode - which would turn an ordinary deployment into a re-registration.
    /// </summary>
    /// <remarks>
    ///     The positive side of this path cannot be asserted here: it needs an assembly actually
    ///     built with <c>&lt;XrmFrameworkPluginPackage&gt;true&lt;/XrmFrameworkPluginPackage&gt;</c>,
    ///     which is an MSBuild concern. It is verified by building a probe project against the
    ///     packed XrmFramework.Plugin targets and reading the marker back out of the compiled DLL.
    /// </remarks>
    [Test]
    public void RealInventory_OfAnUnmarkedAssembly_ReportsAssemblyMode()
    {
        var assemblyPath = typeof(Sample.AccountPlugin).Assembly.Location;

        var json = PluginInventoryEngine.BuildManifestJson(assemblyPath);

        Assert.Multiple(() =>
        {
            Assert.That(PluginInventoryReader.ReadUsesPluginPackage(json), Is.False);
            Assert.That(JsonDocument.Parse(json).RootElement.GetProperty("pluginPackage").ValueKind,
                Is.EqualTo(JsonValueKind.False));
        });
    }
}
