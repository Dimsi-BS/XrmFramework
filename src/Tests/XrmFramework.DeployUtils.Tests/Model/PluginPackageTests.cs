// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using NUnit.Framework;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Tests.Model;

[TestFixture]
public class PluginPackageTests
{
    /// <summary>
    /// The unique name is the publisher prefix, a separator, then the assembly name. Assembly
    /// names routinely contain underscores, so only the FIRST separator delimits the prefix.
    /// </summary>
    [TestCase("dim_MyProject.Plugins", "MyProject.Plugins")]
    [TestCase("dim_My_Project.Plugins", "My_Project.Plugins")]
    [TestCase("dim_a_b_c", "a_b_c")]
    public void AssemblyName_StripsThePrefixOnly(string uniqueName, string expected)
    {
        var package = new PluginPackage {Name = uniqueName};

        Assert.That(package.AssemblyName, Is.EqualTo(expected));
    }

    /// <summary>
    /// A name without a separator is not a prefixed name: returning it whole beats returning
    /// nothing, and it surfaces in the error message the server sends back.
    /// </summary>
    [Test]
    public void AssemblyName_WithoutPrefix_ReturnsTheWholeName()
    {
        var package = new PluginPackage {Name = "MyProject.Plugins"};

        Assert.That(package.AssemblyName, Is.EqualTo("MyProject.Plugins"));
    }

    /// <summary>
    /// A package is pushed on its own, before the diff's batches, so it must not appear in the
    /// component pool the diff walks.
    /// </summary>
    [Test]
    public void AssemblyInfo_DoesNotExposeThePackageAsAChild()
    {
        var assembly = new AssemblyInfo {Name = "MyProject.Plugins"};
        var package = new PluginPackage {Name = "dim_MyProject.Plugins"};

        assembly.AddChild(package);

        Assert.Multiple(() =>
        {
            Assert.That(assembly.Package, Is.SameAs(package));
            Assert.That(assembly.Children, Is.Empty);
        });
    }

    /// <summary>
    /// In package mode Dataverse owns the pluginassembly row. Writing it from here would replace
    /// what the package put there with the bare DLL, so the assembly is pinned to
    /// <see cref="RegistrationState.Computed" /> and stays out of every batch.
    /// </summary>
    [Test]
    public void AssemblyInfo_WithAPackage_IsNeverWritten()
    {
        var assembly = new AssemblyInfo {Name = "MyProject.Plugins"};

        assembly.RegistrationState = RegistrationState.ToUpdate;
        Assert.That(assembly.RegistrationState, Is.EqualTo(RegistrationState.ToUpdate));

        assembly.AddChild(new PluginPackage {Name = "dim_MyProject.Plugins"});

        Assert.Multiple(() =>
        {
            Assert.That(assembly.RegistrationState, Is.EqualTo(RegistrationState.Computed));

            // The pipeline walks components through the interface: a shadowed member would be
            // bypassed there, which is exactly why this one is an override.
            Assert.That(((ICrmComponent) assembly).RegistrationState,
                Is.EqualTo(RegistrationState.Computed));
        });
    }

    /// <summary>
    /// Without a package, nothing changes for the classic assembly deployment.
    /// </summary>
    [Test]
    public void AssemblyInfo_WithoutAPackage_KeepsItsState()
    {
        var assembly = new AssemblyInfo {Name = "MyProject.Plugins"};

        assembly.RegistrationState = RegistrationState.ToUpdate;

        Assert.That(assembly.RegistrationState, Is.EqualTo(RegistrationState.ToUpdate));
    }
}
