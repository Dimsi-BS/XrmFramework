// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using NUnit.Framework;
using XrmFramework.DeployUtils.Configuration;

namespace XrmFramework.DeployUtils.Tests.Configuration;

/// <summary>
///     <c>connectionStrings.config</c> holds secrets, so it is gitignored, so a pipeline that
///     authenticates through workload identity federation has no reason to carry one. Loading a
///     project configuration must therefore survive its absence - it used to throw - while a
///     deployment that does need a connection string still has to say so clearly.
/// </summary>
[TestFixture]
public class OptionalConnectionStringsConfigTests
{
    private string _projectRoot;

    [SetUp]
    public void SetUp()
    {
        _projectRoot = Path.Combine(Path.GetTempPath(), "xrmfw-wif-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_projectRoot, "Config"));
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_projectRoot)) Directory.Delete(_projectRoot, recursive: true);
    }

    [Test]
    public void UseProjectConfig_WithoutConnectionStrings_StillReadsTheSection()
    {
        WriteXrmConfig();

        Assert.DoesNotThrow(() => ConfigHelper.UseProjectConfig(_projectRoot));
        Assert.AreEqual("Xrm", ConfigHelper.GetSection().SelectedConnection);
    }

    [Test]
    public void TryGetSelectedConnectionString_WithoutConnectionStrings_AnswersNull()
    {
        WriteXrmConfig();
        ConfigHelper.UseProjectConfig(_projectRoot);

        Assert.IsNull(ConfigHelper.TryGetSelectedConnectionString());
    }

    [Test]
    public void TryGetSelectedConnectionString_WhenDeclared_AnswersIt()
    {
        WriteXrmConfig();
        File.WriteAllText(
            Path.Combine(_projectRoot, "Config", "connectionStrings.config"),
            """
            <connectionStrings>
              <add name="Xrm" connectionString="AuthType=WorkloadIdentity; Url=https://contoso.crm4.dynamics.com" />
            </connectionStrings>
            """);

        ConfigHelper.UseProjectConfig(_projectRoot);

        Assert.AreEqual(
            "AuthType=WorkloadIdentity; Url=https://contoso.crm4.dynamics.com",
            ConfigHelper.TryGetSelectedConnectionString());
    }

    [Test]
    public void UseProjectConfig_WithoutXrmFrameworkConfig_StillThrows()
    {
        // Only the connection strings became optional: without xrmFramework.config there is no
        // project to deploy and no solution to deploy it into.
        Assert.Throws<FileNotFoundException>(() => ConfigHelper.UseProjectConfig(_projectRoot));
    }

    private void WriteXrmConfig()
        => File.WriteAllText(
            Path.Combine(_projectRoot, "Config", "xrmFramework.config"),
            """
            <xrmFramework selectedConnection="Xrm">
              <entitySolution name="Entities" />
              <projects>
                <add name="Contoso.Plugins" targetSolution="Plugins" type="PluginsWorkflows" />
              </projects>
            </xrmFramework>
            """);
}
