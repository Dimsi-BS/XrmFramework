// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;
using XrmFramework.DeployUtils.Factories;

namespace XrmFramework.DeployUtils.Tests.Factories;

/// <summary>
///     The consumer half of the plugin-package marker: how the deployment reads, out of the
///     inventory manifest, whether the assembly in front of it was built as a plugin package.
/// </summary>
/// <remarks>
///     The producer half - the packaging targets stamping
///     <c>AssemblyMetadata("XrmFrameworkPluginPackage", "true")</c> into the assembly, and the
///     inventory engine reading it back - is covered in XrmFramework.PluginInventory.Tests, which
///     runs the real engine.
/// </remarks>
[TestFixture]
public class PluginPackageMarkerTests
{
    private const string MarkedManifest =
        @"{""pluginPackage"":true,""plugins"":[],""workflows"":[],""customApis"":[]}";

    private const string UnmarkedManifest =
        @"{""pluginPackage"":false,""plugins"":[],""workflows"":[],""customApis"":[]}";

    /// <summary>
    ///     An assembly built as a package says so, and that is what puts the deployment into
    ///     package mode without anything being declared in xrmFramework.config.
    /// </summary>
    [Test]
    public void MarkedAssembly_IsReportedAsPackage()
    {
        Assert.That(PluginInventoryReader.ReadUsesPluginPackage(MarkedManifest), Is.True);
    }

    /// <summary>
    ///     A project that never opted in must not be pushed as a package: that switch is a
    ///     re-registration, with new plugin type rows and new step ids.
    /// </summary>
    [Test]
    public void UnmarkedAssembly_IsReportedAsAssembly()
    {
        Assert.That(PluginInventoryReader.ReadUsesPluginPackage(UnmarkedManifest), Is.False);
    }

    /// <summary>
    ///     Backward compatibility, and the case that will be the common one for a while: an
    ///     assembly built before the marker existed produces a manifest without the field. It has
    ///     to read as "not a package", leaving <c>deployment="package"</c> to force the mode for
    ///     those projects rather than the deployment guessing.
    /// </summary>
    [Test]
    public void ManifestWithoutTheField_ReadsAsAssembly()
    {
        const string legacyManifest = @"{""plugins"":[],""workflows"":[],""customApis"":[]}";

        Assert.That(PluginInventoryReader.ReadUsesPluginPackage(legacyManifest), Is.False);
    }

    /// <summary>
    ///     The flag is read before anything is written, so a missing inventory must not throw here:
    ///     the pipeline reports an empty inventory with its own, explicit error.
    /// </summary>
    [Test]
    public void AbsentManifest_ReadsAsAssemblyWithoutThrowing()
    {
        Assert.Multiple(() =>
        {
            Assert.That(PluginInventoryReader.ReadUsesPluginPackage(null), Is.False);
            Assert.That(PluginInventoryReader.ReadUsesPluginPackage(string.Empty), Is.False);
            Assert.That(PluginInventoryReader.ReadUsesPluginPackage("   "), Is.False);
        });
    }

    /// <summary>
    ///     The field is read independently of the rest of the manifest: a package assembly is
    ///     recognised as one even when it carries no plugin at all, which is what a project holding
    ///     only Custom APIs looks like.
    /// </summary>
    [Test]
    public void MarkerIsIndependentOfTheInventoryContents()
    {
        const string customApisOnly =
            @"{""pluginPackage"":true,""plugins"":[],""workflows"":[],""customApis"":[
                 {""fullName"":""My.Ns.Approve"",""name"":""dim_Approve"",""arguments"":[]}]}";

        Assert.Multiple(() =>
        {
            Assert.That(PluginInventoryReader.ReadUsesPluginPackage(customApisOnly), Is.True);
            Assert.That(PluginInventoryReader.ReadCustomApis(customApisOnly, "dim"), Is.Not.Empty);
        });
    }
}
