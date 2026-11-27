// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Configuration;
using NUnit.Framework;
using XrmFramework.DeployUtils;
using XrmFramework.DeployUtils.Configuration;

namespace XrmFramework.DeployUtils.Tests.Configuration;

/// <summary>
///     A pipeline whose service connection carries a secret already has the credentials; writing
///     them into a <c>connectionStrings.config</c> first only adds a step and a file to clean up.
/// </summary>
[TestFixture]
public class ClientSecretConnectionTests
{
    private const string Url = "https://org.crm4.dynamics.com";
    private const string ClientId = "23fe1fd8-c88a-4b92-a73d-545b0e25c0ba";

    [Test]
    public void Build_FromTheCommandLine()
    {
        var connection = ClientSecretConnection.Build(
            new DeployOptions { EnvironmentUrl = Url, ApplicationId = ClientId, ClientSecret = "s3cr3t" },
            null,
            Empty);

        Assert.AreEqual($"AuthType=ClientSecret; Url={Url}; ClientId={ClientId}; ClientSecret=s3cr3t", connection);
    }

    [Test]
    public void Build_TakesTheSecretFromTheEnvironment()
    {
        // The better place for it: a command line is visible to every process on the agent.
        var connection = ClientSecretConnection.Build(
            new DeployOptions { EnvironmentUrl = Url, ApplicationId = ClientId },
            null,
            Reader(new Dictionary<string, string>
            {
                [ClientSecretConnection.SecretVariable] = "from-the-environment"
            }));

        Assert.IsTrue(connection.EndsWith("ClientSecret=from-the-environment"), connection);
    }

    [Test]
    public void Build_CommandLineWinsOverTheEnvironment()
    {
        var connection = ClientSecretConnection.Build(
            new DeployOptions { EnvironmentUrl = Url, ApplicationId = "cli-app", ClientSecret = "cli-secret" },
            null,
            Reader(new Dictionary<string, string>
            {
                [ClientSecretConnection.ClientIdVariable] = "env-app",
                [ClientSecretConnection.SecretVariable] = "env-secret"
            }));

        Assert.IsTrue(connection.Contains("ClientId=cli-app"), connection);
        Assert.IsTrue(connection.Contains("ClientSecret=cli-secret"), connection);
    }

    [Test]
    public void Build_FillsTheUrlFromADeclaredConnectionString()
    {
        // Only to fill in what the command line left out: a project that already declares its
        // environment should not have to repeat it to hand over a secret.
        var connection = ClientSecretConnection.Build(
            new DeployOptions { ApplicationId = ClientId, ClientSecret = "s3cr3t" },
            $"AuthType=ClientSecret; Url={Url}; ClientId=declared-app",
            Empty);

        Assert.IsTrue(connection.Contains($"Url={Url}"), connection);
        Assert.IsTrue(connection.Contains($"ClientId={ClientId}"), connection, "the command line still wins");
    }

    [Test]
    public void Build_WithoutTheSecret_SaysWhatToPass()
    {
        var error = Assert.Throws<ConfigurationErrorsException>(() => ClientSecretConnection.Build(
            new DeployOptions { EnvironmentUrl = Url, ApplicationId = ClientId }, null, Empty));

        Assert.IsTrue(error!.Message.Contains("--secret"), error.Message);
        Assert.IsTrue(error.Message.Contains(ClientSecretConnection.SecretVariable), error.Message);
    }

    [Test]
    public void Build_ListsEveryMissingValueAtOnce()
    {
        var error = Assert.Throws<ConfigurationErrorsException>(() => ClientSecretConnection.Build(
            new DeployOptions(), null, Empty));

        Assert.IsTrue(error!.Message.Contains("--environment"), error.Message);
        Assert.IsTrue(error.Message.Contains("--application-id"), error.Message);
        Assert.IsTrue(error.Message.Contains("--secret"), error.Message);
    }

    [Test]
    public void IsAvailable_OnlyWhenASecretWasHandedOver()
    {
        Assert.IsTrue(ClientSecretConnection.IsAvailable(
            new DeployOptions { ClientSecret = "s3cr3t" }, Empty));

        Assert.IsTrue(ClientSecretConnection.IsAvailable(
            new DeployOptions(),
            Reader(new Dictionary<string, string> { [ClientSecretConnection.SecretVariable] = "s3cr3t" })));

        Assert.IsFalse(ClientSecretConnection.IsAvailable(new DeployOptions(), Empty),
            "auto-detection must not fire for a pipeline that handed over nothing");

        Assert.IsFalse(ClientSecretConnection.IsAvailable(
            new DeployOptions { ClientSecret = "   " }, Empty), "a blank secret is not a secret");
    }

    [Test]
    public void Build_KeepsASecretContainingAnEqualsSign()
    {
        // Base64 secrets end in '=' often enough that mangling them would be a weekly bug.
        var connection = ClientSecretConnection.Build(
            new DeployOptions { EnvironmentUrl = Url, ApplicationId = ClientId, ClientSecret = "abc+def/gh=" },
            null,
            Empty);

        Assert.IsTrue(connection.EndsWith("ClientSecret=abc+def/gh="), connection);
    }

    private static readonly Func<string, string> Empty = _ => null;

    private static Func<string, string> Reader(IDictionary<string, string> environment)
        => name => environment.TryGetValue(name, out var value) ? value : null;
}
