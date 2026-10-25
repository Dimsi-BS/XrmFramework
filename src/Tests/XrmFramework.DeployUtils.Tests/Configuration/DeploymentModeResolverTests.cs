// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using NUnit.Framework;
using XrmFramework.DeployUtils.Configuration;

namespace XrmFramework.DeployUtils.Tests.Configuration;

/// <summary>
///     <see cref="DeploymentMode.Auto" /> decides which Dataverse entity a deployment writes, and
///     switching an already-deployed project between the two shapes re-creates its steps with new
///     ids. So the rules below are deliberately conservative: two corroborating signals to choose
///     the package, and a refusal to guess when they disagree.
/// </summary>
[TestFixture]
public class DeploymentModeResolverTests
{
    private string _root;
    private string _tfmFolder;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "xrmfw-mode-" + Guid.NewGuid().ToString("N"));
        _tfmFolder = Path.Combine(_root, "bin", "Debug", "net48");
        Directory.CreateDirectory(_tfmFolder);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, recursive: true);
    }

    // ── Auto ────────────────────────────────────────────────────────────────

    [Test]
    public void Auto_WithoutAPackage_IsAssembly()
    {
        var dll = WriteAssembly(signed: false);

        var resolution = DeploymentModeResolver.Resolve(DeploymentMode.Auto, dll);

        Assert.Multiple(() =>
        {
            Assert.That(resolution.Mode, Is.EqualTo(DeploymentMode.Assembly));
            Assert.That(resolution.PackagePath, Is.Null);
            Assert.That(resolution.Reason, Does.Contain("no plugin package"));
        });
    }

    /// <summary>
    /// The two signals package mode leaves behind: a .nupkg, and an unsigned assembly.
    /// </summary>
    [Test]
    public void Auto_WithAPackageAndAnUnsignedAssembly_IsPackage()
    {
        var dll = WriteAssembly(signed: false);
        var package = WritePackage(newerThan: dll);

        var resolution = DeploymentModeResolver.Resolve(DeploymentMode.Auto, dll);

        Assert.Multiple(() =>
        {
            Assert.That(resolution.Mode, Is.EqualTo(DeploymentMode.Package));
            Assert.That(resolution.PackagePath, Is.EqualTo(package));
        });
    }

    /// <summary>
    /// A signed assembly is what a bare registration needs and what package mode avoids
    /// producing. Signed plus a package reads as a leftover, and a leftover must not be able to
    /// change the shape of the deployment on its own.
    /// </summary>
    [Test]
    public void Auto_WithAPackageButASignedAssembly_IsAssembly()
    {
        var dll = WriteAssembly(signed: true);
        WritePackage(newerThan: dll);

        var resolution = DeploymentModeResolver.Resolve(DeploymentMode.Auto, dll);

        Assert.Multiple(() =>
        {
            Assert.That(resolution.Mode, Is.EqualTo(DeploymentMode.Assembly));
            Assert.That(resolution.Reason, Does.Contain("signed").And.Contain("leftover"));
        });
    }

    /// <summary>
    /// Unsigned and packaged, but the package predates the assembly: the last build did not
    /// refresh it. Pushing it would deploy code that is not the code just built - the one failure
    /// this feature must never produce silently.
    /// </summary>
    [Test]
    public void Auto_WithAStalePackage_Refuses()
    {
        var dll = WriteAssembly(signed: false);
        var package = WritePackage(newerThan: dll);
        File.SetLastWriteTimeUtc(package, File.GetLastWriteTimeUtc(dll).AddMinutes(-10));

        var ex = Assert.Throws<InvalidOperationException>(
            () => DeploymentModeResolver.Resolve(DeploymentMode.Auto, dll));

        Assert.That(ex!.Message, Does.Contain("older than the assembly"));
    }

    /// <summary>
    /// Naming a package is asking for package mode, whatever the assembly looks like.
    /// </summary>
    [Test]
    public void Auto_WithAnExplicitPackage_IsPackage()
    {
        var dll = WriteAssembly(signed: true);
        var package = WritePackage(newerThan: dll);

        var resolution = DeploymentModeResolver.Resolve(DeploymentMode.Auto, dll, package);

        Assert.Multiple(() =>
        {
            Assert.That(resolution.Mode, Is.EqualTo(DeploymentMode.Package));
            Assert.That(resolution.PackagePath, Is.EqualTo(package));
            Assert.That(resolution.Reason, Does.Contain("--package"));
        });
    }

    // ── Declared explicitly ─────────────────────────────────────────────────

    [Test]
    public void Package_IsHonouredEvenWithoutAPackageYet()
    {
        // The missing package is reported later, by the deployment, with instructions.
        var dll = WriteAssembly(signed: false);

        var resolution = DeploymentModeResolver.Resolve(DeploymentMode.Package, dll);

        Assert.That(resolution.Mode, Is.EqualTo(DeploymentMode.Package));
    }

    [Test]
    public void Assembly_IsHonouredEvenWhenTheBuildProducedAPackage()
    {
        var dll = WriteAssembly(signed: false);
        WritePackage(newerThan: dll);

        var resolution = DeploymentModeResolver.Resolve(DeploymentMode.Assembly, dll);

        Assert.Multiple(() =>
        {
            Assert.That(resolution.Mode, Is.EqualTo(DeploymentMode.Assembly));

            // Honoured, but said out loud: Dataverse rejects an unsigned bare assembly.
            Assert.That(resolution.Reason, Does.Contain("unsigned"));
        });
    }

    [Test]
    public void Assembly_StaysQuietWhenNothingLooksOdd()
    {
        var dll = WriteAssembly(signed: true);

        var resolution = DeploymentModeResolver.Resolve(DeploymentMode.Assembly, dll);

        Assert.Multiple(() =>
        {
            Assert.That(resolution.Mode, Is.EqualTo(DeploymentMode.Assembly));
            Assert.That(resolution.Reason, Does.Not.Contain("although"));
        });
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    /// <summary>
    ///     A real assembly is needed, because the signed/unsigned signal is read from metadata.
    ///     The test assembly itself stands in for a signed one; an arbitrary file cannot have its
    ///     identity read, which the resolver treats as unsigned.
    /// </summary>
    private string WriteAssembly(bool signed)
    {
        var path = Path.Combine(_tfmFolder, "MyProject.Plugins.dll");

        if (signed)
        {
            var strongNamed = typeof(Uri).Assembly.Location;
            Assume.That(strongNamed, Is.Not.Empty, "a strong-named assembly is needed as a fixture");
            File.Copy(strongNamed, path, overwrite: true);
        }
        else
        {
            File.WriteAllText(path, "not a readable assembly identity");
        }

        return path;
    }

    private string WritePackage(string newerThan)
    {
        var folder = Path.Combine(_root, "bin", "Debug");
        Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, "MyProject.Plugins.1.0.0.nupkg");
        File.WriteAllText(path, string.Empty);
        File.SetLastWriteTimeUtc(path, File.GetLastWriteTimeUtc(newerThan).AddSeconds(1));

        return path;
    }
}
