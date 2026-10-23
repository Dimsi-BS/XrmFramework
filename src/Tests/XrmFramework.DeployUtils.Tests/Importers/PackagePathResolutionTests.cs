// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using NUnit.Framework;
using XrmFramework.DeployUtils.Importers;

namespace XrmFramework.DeployUtils.Tests.Importers;

[TestFixture]
public class PackagePathResolutionTests
{
    private string _root;
    private string _tfmFolder;
    private string _dllPath;

    [SetUp]
    public void SetUp()
    {
        // bin/Debug/net48/MyProject.Plugins.dll, the layout of a real plugin project.
        _root = Path.Combine(Path.GetTempPath(), "xrmfw-pkg-" + Guid.NewGuid().ToString("N"));
        _tfmFolder = Path.Combine(_root, "bin", "Debug", "net48");
        Directory.CreateDirectory(_tfmFolder);

        _dllPath = Path.Combine(_tfmFolder, "MyProject.Plugins.dll");
        File.WriteAllText(_dllPath, string.Empty);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, recursive: true);
    }

    /// <summary>
    /// Where NuGet puts it by default: bin/&lt;Configuration&gt;/, one level above the assembly.
    /// </summary>
    [Test]
    public void FindsThePackageOneLevelAboveTheAssembly()
    {
        var expected = Touch(Path.Combine(_root, "bin", "Debug"), "MyProject.Plugins.1.2.3.nupkg");

        Assert.That(AssemblyImporter.ResolvePackagePath(_dllPath), Is.EqualTo(expected));
    }

    /// <summary>
    /// A project that redirects PackageOutputPath next to its output is supported too.
    /// </summary>
    [Test]
    public void FindsThePackageNextToTheAssembly()
    {
        var expected = Touch(_tfmFolder, "MyProject.Plugins.1.2.3.nupkg");

        Assert.That(AssemblyImporter.ResolvePackagePath(_dllPath), Is.EqualTo(expected));
    }

    /// <summary>
    /// The assembly folder wins over its parent: a package sitting beside the DLL was put there
    /// deliberately, where the one above is only NuGet's default.
    /// </summary>
    [Test]
    public void TheClosestPackageWins()
    {
        Touch(Path.Combine(_root, "bin", "Debug"), "MyProject.Plugins.1.0.0.nupkg");
        var expected = Touch(_tfmFolder, "MyProject.Plugins.2.0.0.nupkg");

        Assert.That(AssemblyImporter.ResolvePackagePath(_dllPath), Is.EqualTo(expected));
    }

    /// <summary>
    /// Only this assembly's package: a shared output folder holds one per project.
    /// </summary>
    [Test]
    public void IgnoresAnotherProjectsPackage()
    {
        Touch(Path.Combine(_root, "bin", "Debug"), "OtherProject.Plugins.1.0.0.nupkg");

        Assert.That(AssemblyImporter.ResolvePackagePath(_dllPath), Is.Null);
    }

    /// <summary>
    /// Several versions side by side - a rebuild without a clean - resolve to the freshest, which
    /// is the one the build just produced.
    /// </summary>
    [Test]
    public void SeveralVersions_ResolveToTheMostRecent()
    {
        var old = Touch(Path.Combine(_root, "bin", "Debug"), "MyProject.Plugins.1.0.0.nupkg");
        var recent = Touch(Path.Combine(_root, "bin", "Debug"), "MyProject.Plugins.1.0.1.nupkg");

        File.SetLastWriteTimeUtc(old, DateTime.UtcNow.AddHours(-1));
        File.SetLastWriteTimeUtc(recent, DateTime.UtcNow);

        Assert.That(AssemblyImporter.ResolvePackagePath(_dllPath), Is.EqualTo(recent));
    }

    /// <summary>
    /// Nothing built yet: returning null lets the caller explain how to produce the package,
    /// rather than failing later with a path that does not exist.
    /// </summary>
    [Test]
    public void NoPackage_ReturnsNull()
    {
        Assert.That(AssemblyImporter.ResolvePackagePath(_dllPath), Is.Null);
    }

    private static string Touch(string folder, string fileName)
    {
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, fileName);
        File.WriteAllText(path, string.Empty);
        return path;
    }
}
