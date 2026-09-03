// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace XrmFramework.DeployUtils.Tests;

/// <summary>
/// <c>deploy webresources</c>: which files the scan actually picks up. A Webresources project
/// that has been through <c>npm install</c> holds thousands of <c>.js</c>, <c>.css</c> and
/// <c>.png</c> files under <c>node_modules</c>, all of them carrying an extension Dataverse
/// accepts — filtering on the extension alone is not enough.
/// </summary>
[TestFixture]
public class WebResourceEnumerationTests
{
    private string _root = string.Empty;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "XrmFramework.WebResources_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            if (Directory.Exists(_root))
            {
                Directory.Delete(_root, true);
            }
        }
        catch (IOException)
        {
            // A temp folder left behind must not fail the run.
        }
    }

    private void WriteFile(params string[] segments)
    {
        var path = Path.Combine(new[] { _root }.Concat(segments).ToArray());
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, "// content");
    }

    private string[] Enumerate()
        => WebResourceHelper.EnumerateWebResourceFiles(new DirectoryInfo(_root))
            .Select(f => f.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

    [Test]
    public void Enumerate_SkipsNodeModules()
    {
        WriteFile("scripts", "form.js");
        WriteFile("node_modules", "webpack", "index.js");
        WriteFile("node_modules", "nested", "deep", "style.css");

        Assert.AreEqual(new[] { "form.js" }, Enumerate());
    }

    [Test]
    public void Enumerate_SkipsBuildOutputFolders()
    {
        WriteFile("page.html");
        WriteFile("bin", "Debug", "leftover.js");
        WriteFile("obj", "generated.js");
        WriteFile(".vs", "cache.js");
        WriteFile(".git", "hook.js");

        Assert.AreEqual(new[] { "page.html" }, Enumerate());
    }

    [Test]
    public void Enumerate_KeepsNestedWebResources()
    {
        WriteFile("scripts", "account", "form.js");
        WriteFile("html", "dialog.html");
        WriteFile("svg", "icon.svg");

        Assert.AreEqual(new[] { "dialog.html", "form.js", "icon.svg" }, Enumerate());
    }

    [Test]
    public void Enumerate_IgnoresNonWebResourceExtensions()
    {
        WriteFile("scripts", "form.ts");
        WriteFile("scripts", "form.js");
        WriteFile("tsconfig.json");
        WriteFile("package.json");

        Assert.AreEqual(new[] { "form.js" }, Enumerate());
    }

    [Test]
    public void Enumerate_ExclusionIsCaseInsensitive()
    {
        WriteFile("keep.js");
        WriteFile("Node_Modules", "dep.js");
        WriteFile("BIN", "out.js");

        Assert.AreEqual(new[] { "keep.js" }, Enumerate());
    }
}
