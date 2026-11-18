// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using XrmFramework.DeployUtils.Scaffolding;
using XrmFramework.DeployUtils.Tests.TableSync.Fixtures;

namespace XrmFramework.DeployUtils.Tests.Scaffolding;

/// <summary>
/// Covers the target framework a scaffolded solution compiles against. Dataverse supports .NET
/// Framework 4.6.2 through 4.8 for plugins and workflow activities, and net48 is what
/// <c>xrmframework new</c> produces unless the caller asks for net462.
/// </summary>
[TestFixture]
public class TemplateFrameworkTests
{
    /// <summary>
    /// Nothing asked for means net48 - including the shapes an unset command-line option takes.
    /// </summary>
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void NormalizeFramework_WithoutAValue_ReturnsTheDefault(string? framework)
    {
        Assert.That(TemplateScaffolder.NormalizeFramework(framework),
            Is.EqualTo(TemplateScaffolder.DefaultFramework));
    }

    /// <summary>
    /// A moniker typed on a command line arrives in whatever case and spacing the user used.
    /// </summary>
    [TestCase("net48", "net48")]
    [TestCase("NET48", "net48")]
    [TestCase("Net48", "net48")]
    [TestCase("  net48  ", "net48")]
    [TestCase("net462", "net462")]
    [TestCase("NET462", "net462")]
    [TestCase(" net462\t", "net462")]
    public void NormalizeFramework_AcceptsASupportedMoniker(string framework, string expected)
    {
        Assert.That(TemplateScaffolder.NormalizeFramework(framework), Is.EqualTo(expected));
    }

    /// <summary>
    /// Anything else is refused rather than written into a csproj: net472 and net481 are .NET
    /// Framework versions Dataverse does not support for plugins, and a .NET moniker is not a
    /// .NET Framework one at all. The scaffolders turn this null into exit code 2.
    /// </summary>
    [TestCase("net472")]
    [TestCase("net481")]
    [TestCase("net461")]
    [TestCase("net8.0")]
    [TestCase("net10.0")]
    [TestCase("netstandard2.0")]
    [TestCase("4.8")]
    [TestCase("net")]
    [TestCase("nonsense")]
    public void NormalizeFramework_RefusesAnythingElse(string framework)
    {
        Assert.That(TemplateScaffolder.NormalizeFramework(framework), Is.Null);
    }

    /// <summary>
    /// The token the templates carry IS the default moniker, which is what makes a template left
    /// unsubstituted still a valid project. Change one without the other and the templates stop
    /// compiling on their own.
    /// </summary>
    [Test]
    public void TheFrameworkTokenIsTheDefaultMoniker()
    {
        Assert.Multiple(() =>
        {
            Assert.That(TemplateScaffolder.FrameworkToken,
                Is.EqualTo(TemplateScaffolder.DefaultFramework));
            Assert.That(TemplateScaffolder.SupportedFrameworks,
                Contains.Item(TemplateScaffolder.DefaultFramework));
        });
    }

    /// <summary>
    /// Substituting the moniker is a plain text replacement over the whole template, so the token
    /// must not appear anywhere but in a &lt;TargetFramework(s)&gt; element: a "net48" written in a
    /// template's README or in a lib/net48 path would silently become net462 when someone asks for
    /// net462. Reads the templates actually shipped, which is what the CLI copies.
    /// </summary>
    [Test]
    public void TheTemplatesOnlyCarryTheTokenInATargetFrameworkElement()
    {
        var scaffoldingRoot = RepositoryPaths.FindDirectory(
            Path.Combine("src", "XrmFramework.Cli", "Scaffolding"));

        var offenders = new List<string>();

        foreach (var file in Directory.EnumerateFiles(scaffoldingRoot, "*", SearchOption.AllDirectories))
        {
            // The template tree is checked-in sources; obj/bin only appear if someone built in place.
            if (file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                || file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
                continue;

            string content;
            try
            {
                content = File.ReadAllText(file);
            }
            catch (IOException)
            {
                continue;
            }

            var lineNumber = 0;
            foreach (var line in content.Split('\n'))
            {
                lineNumber++;

                if (!line.Contains(TemplateScaffolder.FrameworkToken))
                    continue;

                if (Regex.IsMatch(line, @"<TargetFrameworks?>[^<]*</TargetFrameworks?>"))
                    continue;

                offenders.Add($"{file.Substring(scaffoldingRoot.Length).TrimStart(Path.DirectorySeparatorChar)}:{lineNumber}: {line.Trim()}");
            }
        }

        Assert.That(offenders, Is.Empty,
            $"'{TemplateScaffolder.FrameworkToken}' outside a TargetFramework element would be rewritten "
            + $"by --framework net462:{System.Environment.NewLine}{string.Join(System.Environment.NewLine, offenders)}");
    }

    /// <summary>
    /// The substitution the scaffolders perform, end to end: the project name and the moniker both
    /// travel, in file content as well as in file and directory names.
    /// </summary>
    [Test]
    public void CopyAndReplace_AppliesBothTheNameAndTheFramework()
    {
        var source = CreateTemplate();
        var destination = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        try
        {
            TemplateScaffolder.CopyAndReplace(source, destination, new[]
            {
                new KeyValuePair<string, string>("$safeprojectname$", "Contoso"),
                new KeyValuePair<string, string>(TemplateScaffolder.FrameworkToken, "net462"),
            });

            var csproj = Path.Combine(destination, "Contoso.Plugins", "Contoso.Plugins.csproj");

            Assert.Multiple(() =>
            {
                Assert.That(File.Exists(csproj), Is.True, "the name token must travel in paths too");
                Assert.That(File.ReadAllText(csproj), Does.Contain("<TargetFramework>net462</TargetFramework>"));
                Assert.That(File.ReadAllText(csproj), Does.Not.Contain("net48"));
                Assert.That(File.ReadAllText(csproj), Does.Contain("Contoso.Core"));
            });
        }
        finally
        {
            Directory.Delete(source, recursive: true);
            if (Directory.Exists(destination)) Directory.Delete(destination, recursive: true);
        }
    }

    /// <summary>
    /// The default path leaves the moniker alone, since the token already is net48.
    /// </summary>
    [Test]
    public void CopyAndReplace_WithTheDefaultFramework_KeepsNet48()
    {
        var source = CreateTemplate();
        var destination = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        try
        {
            TemplateScaffolder.CopyAndReplace(source, destination, new[]
            {
                new KeyValuePair<string, string>("$safeprojectname$", "Contoso"),
                new KeyValuePair<string, string>(TemplateScaffolder.FrameworkToken,
                    TemplateScaffolder.NormalizeFramework(null)!),
            });

            var csproj = Path.Combine(destination, "Contoso.Plugins", "Contoso.Plugins.csproj");

            Assert.That(File.ReadAllText(csproj), Does.Contain("<TargetFramework>net48</TargetFramework>"));
        }
        finally
        {
            Directory.Delete(source, recursive: true);
            if (Directory.Exists(destination)) Directory.Delete(destination, recursive: true);
        }
    }

    /// <summary>
    /// A binary shipped by the templates - the strong-name key - is copied byte for byte. Running
    /// the text substitution over it would corrupt the key and break signing in every generated
    /// solution.
    /// </summary>
    [Test]
    public void CopyAndReplace_LeavesBinariesUntouched()
    {
        var source = CreateTemplate();
        var destination = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        // Bytes that happen to spell the tokens: a text replacement would rewrite them.
        var keyBytes = System.Text.Encoding.UTF8.GetBytes("net48 $safeprojectname$\0ÿþ");
        File.WriteAllBytes(Path.Combine(source, "Contoso.snk"), keyBytes);

        try
        {
            TemplateScaffolder.CopyAndReplace(source, destination, new[]
            {
                new KeyValuePair<string, string>("$safeprojectname$", "Contoso"),
                new KeyValuePair<string, string>(TemplateScaffolder.FrameworkToken, "net462"),
            });

            // The NAME is still substituted - only the content is off limits.
            var copied = Path.Combine(destination, "Contoso.snk");

            Assert.Multiple(() =>
            {
                Assert.That(File.Exists(copied), Is.True);
                Assert.That(File.ReadAllBytes(copied), Is.EqualTo(keyBytes));
            });
        }
        finally
        {
            Directory.Delete(source, recursive: true);
            if (Directory.Exists(destination)) Directory.Delete(destination, recursive: true);
        }
    }

    /// <summary>
    /// A minimal stand-in for the shipped templates: a project folder and csproj both named after
    /// the name token, with the framework token in the TargetFramework element.
    /// </summary>
    private static string CreateTemplate()
    {
        var root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var projectDirectory = Path.Combine(root, "$safeprojectname$.Plugins");
        Directory.CreateDirectory(projectDirectory);

        File.WriteAllText(
            Path.Combine(projectDirectory, "$safeprojectname$.Plugins.csproj"),
            """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net48</TargetFramework>
              </PropertyGroup>
              <ItemGroup>
                <ProjectReference Include="../$safeprojectname$.Core/$safeprojectname$.Core.csproj" />
              </ItemGroup>
            </Project>
            """);

        return root;
    }
}
