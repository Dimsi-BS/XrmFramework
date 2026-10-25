// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using XrmFramework.DeployUtils.Configuration;

namespace XrmFramework.DeployUtils.Tests.Configuration;

/// <summary>
///     The <c>deployment</c> attribute was added to <c>xrmFramework.config</c> in 3.2. Every config
///     written before that does not carry it, on any project - plugins, workflow activities, web
///     resources - so reading it has to resolve to something sane rather than throw. A parsing
///     failure here would break the deployment of every existing solution.
/// </summary>
[TestFixture]
public class DeploymentModeConfigTests
{
    private string _projectRoot;

    [SetUp]
    public void SetUp()
    {
        _projectRoot = Path.Combine(Path.GetTempPath(), "xrmfw-cfg-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_projectRoot, "Config"));

        File.WriteAllText(
            Path.Combine(_projectRoot, "Config", "connectionStrings.config"),
            """
            <connectionStrings>
              <add name="Xrm" connectionString="Url=https://contoso.crm4.dynamics.com" />
            </connectionStrings>
            """);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_projectRoot)) Directory.Delete(_projectRoot, recursive: true);
    }

    /// <summary>
    /// A config written before 3.2 carries no attribute at all, on any project type. It resolves
    /// to <see cref="DeploymentMode.Auto" />, which is only backward compatible because such a
    /// project's build produces a signed assembly and no package - so Auto lands on Assembly.
    /// Both halves of that are asserted here; the first one alone would not be the guarantee.
    /// </summary>
    [Test]
    public void LegacyConfig_WithoutTheAttribute_StillDeploysAsAnAssembly()
    {
        WriteXrmConfig("""
                       <xrmFramework selectedConnection="Xrm">
                         <entitySolution name="Entities" />
                         <projects>
                           <add name="Contoso.Plugins" targetSolution="Plugins" type="PluginsWorkflows" />
                           <add name="Contoso.Workflows" targetSolution="Plugins" type="PluginsWorkflows" />
                         </projects>
                       </xrmFramework>
                       """);

        var projects = LoadProjects();

        Assert.Multiple(() =>
        {
            Assert.That(projects, Has.Count.EqualTo(2));
            Assert.That(projects.All(p => p.Deployment == DeploymentMode.Auto), Is.True);
        });

        // What a pre-3.2 build leaves on disk: an assembly, no package next to it.
        var legacyOutput = Path.Combine(_projectRoot, "bin", "Debug", "net48");
        Directory.CreateDirectory(legacyOutput);
        var dll = Path.Combine(legacyOutput, "Contoso.Plugins.dll");
        File.Copy(typeof(Uri).Assembly.Location, dll, overwrite: true);

        Assert.That(DeploymentModeResolver.Resolve(DeploymentMode.Auto, dll).Mode,
            Is.EqualTo(DeploymentMode.Assembly));
    }

    [Test]
    public void BothModes_CanCoexistInOneConfig()
    {
        // The split a solution with custom workflow activities has to make: activities cannot ship
        // in a package, so they stay on an assembly while the plugins move to a package.
        WriteXrmConfig("""
                       <xrmFramework selectedConnection="Xrm">
                         <entitySolution name="Entities" />
                         <projects>
                           <add name="Contoso.Plugins" targetSolution="Plugins" type="PluginsWorkflows" deployment="package" />
                           <add name="Contoso.Workflows" targetSolution="Plugins" type="PluginsWorkflows" deployment="assembly" />
                         </projects>
                       </xrmFramework>
                       """);

        var projects = LoadProjects();

        Assert.Multiple(() =>
        {
            Assert.That(projects.Single(p => p.Name == "Contoso.Plugins").Deployment,
                Is.EqualTo(DeploymentMode.Package));
            Assert.That(projects.Single(p => p.Name == "Contoso.Workflows").Deployment,
                Is.EqualTo(DeploymentMode.Assembly));
        });
    }

    /// <summary>
    ///     Casing must not matter. System.Configuration parses enums case-sensitively, so a
    ///     hand-written (and perfectly reasonable) lowercase value used to blow up with
    ///     "The enumeration value must be one of the following: Assembly, Package" before the
    ///     deployment even connected.
    /// </summary>
    [TestCase("package", DeploymentMode.Package)]
    [TestCase("Package", DeploymentMode.Package)]
    [TestCase("PACKAGE", DeploymentMode.Package)]
    [TestCase("assembly", DeploymentMode.Assembly)]
    [TestCase("Assembly", DeploymentMode.Assembly)]
    [TestCase(" Package ", DeploymentMode.Package)]
    [TestCase("auto", DeploymentMode.Auto)]
    [TestCase("Auto", DeploymentMode.Auto)]
    public void TheAttribute_IsReadWhateverItsCasing(string raw, DeploymentMode expected)
    {
        WriteXrmConfig($"""
                        <xrmFramework selectedConnection="Xrm">
                          <entitySolution name="Entities" />
                          <projects>
                            <add name="Contoso.Plugins" targetSolution="Plugins" type="PluginsWorkflows" deployment="{raw}" />
                          </projects>
                        </xrmFramework>
                        """);

        Assert.That(LoadProjects().Single().Deployment, Is.EqualTo(expected));
    }

    /// <summary>
    ///     A typo gets an error naming the project and both valid values, rather than the
    ///     framework silently falling back to assembly mode and deploying the wrong shape.
    /// </summary>
    [Test]
    public void AnUnknownValue_IsRejectedWithAUsableMessage()
    {
        WriteXrmConfig("""
                       <xrmFramework selectedConnection="Xrm">
                         <entitySolution name="Entities" />
                         <projects>
                           <add name="Contoso.Plugins" targetSolution="Plugins" type="PluginsWorkflows" deployment="packages" />
                         </projects>
                       </xrmFramework>
                       """);

        var project = LoadProjects().Single();

        var ex = Assert.Throws<System.Configuration.ConfigurationErrorsException>(
            () => _ = project.Deployment);

        Assert.Multiple(() =>
        {
            Assert.That(ex!.Message, Does.Contain("Contoso.Plugins"));
            Assert.That(ex.Message, Does.Contain("packages"));
            Assert.That(ex.Message, Does.Contain("Assembly").And.Contain("Package"));
        });
    }

    private void WriteXrmConfig(string content)
        => File.WriteAllText(Path.Combine(_projectRoot, "Config", "xrmFramework.config"), content);

    private System.Collections.Generic.List<ProjectElement> LoadProjects()
    {
        ConfigHelper.UseProjectConfig(_projectRoot);

        return ConfigHelper.GetSection().Projects.OfType<ProjectElement>().ToList();
    }
}
