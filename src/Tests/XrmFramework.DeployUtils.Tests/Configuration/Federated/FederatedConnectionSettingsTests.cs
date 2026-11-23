// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using XrmFramework.DeployUtils;
using XrmFramework.DeployUtils.Configuration.Federated;

namespace XrmFramework.DeployUtils.Tests.Configuration.Federated;

[TestFixture]
public class FederatedConnectionSettingsTests
{
    private const string Url = "https://org.crm4.dynamics.com";

    // ──────────────────────────────────────────────
    //  Recognising the connection string
    // ──────────────────────────────────────────────

    [Test]
    public void IsFederatedConnectionString_RecognisesTheAuthType()
    {
        Assert.IsTrue(FederatedConnectionSettings.IsFederatedConnectionString(
            "AuthType=WorkloadIdentity; Url=" + Url));

        Assert.IsTrue(FederatedConnectionSettings.IsFederatedConnectionString(
            "authtype=workloadidentity"), "the comparison is case-insensitive");

        Assert.IsTrue(FederatedConnectionSettings.IsFederatedConnectionString(
            "AuthenticationType=WorkloadIdentity"), "the long spelling is accepted too");
    }

    [Test]
    public void IsFederatedConnectionString_IgnoresEverythingElse()
    {
        Assert.IsFalse(FederatedConnectionSettings.IsFederatedConnectionString(
            "AuthType=ClientSecret; Url=" + Url + "; ClientId=x; ClientSecret=y"));

        Assert.IsFalse(FederatedConnectionSettings.IsFederatedConnectionString(null),
            "deciding how to authenticate happens before a connection string is known to exist");

        Assert.IsFalse(FederatedConnectionSettings.IsFederatedConnectionString(string.Empty));
        Assert.IsFalse(FederatedConnectionSettings.IsFederatedConnectionString("nonsense"));
    }

    // ──────────────────────────────────────────────
    //  Precedence: command line, connection string, environment
    // ──────────────────────────────────────────────

    [Test]
    public void Create_CommandLineWins()
    {
        var settings = FederatedConnectionSettings.Create(
            "AuthType=WorkloadIdentity; Url=https://from-config.crm4.dynamics.com; TenantId=config-tenant; ClientId=config-app",
            new DeployOptions
            {
                EnvironmentUrl = Url,
                TenantId = "cli-tenant",
                ApplicationId = "cli-app",
                ServiceConnectionId = "cli-connection"
            },
            Reader(new Dictionary<string, string>
            {
                [FederatedConnectionSettings.TenantIdVariable] = "env-tenant",
                [FederatedConnectionSettings.ClientIdVariable] = "env-app",
                [FederatedConnectionSettings.ServiceConnectionIdVariable] = "env-connection"
            }));

        Assert.AreEqual(Url, settings.Url);
        Assert.AreEqual("cli-tenant", settings.TenantId);
        Assert.AreEqual("cli-app", settings.ClientId);
        Assert.AreEqual("cli-connection", settings.ServiceConnectionId);
    }

    [Test]
    public void Create_ConnectionStringBeatsTheEnvironment()
    {
        var settings = FederatedConnectionSettings.Create(
            "AuthType=WorkloadIdentity; Url=" + Url + "; TenantId=config-tenant; ClientId=config-app",
            new DeployOptions(),
            Reader(new Dictionary<string, string>
            {
                [FederatedConnectionSettings.TenantIdVariable] = "env-tenant",
                [FederatedConnectionSettings.ClientIdVariable] = "env-app"
            }));

        Assert.AreEqual("config-tenant", settings.TenantId);
        Assert.AreEqual("config-app", settings.ClientId);
    }

    [Test]
    public void Create_FallsBackToThePipelineVariables()
    {
        // The pipeline case: no connectionStrings.config at all, the URL from --environment.
        var settings = FederatedConnectionSettings.Create(
            null,
            new DeployOptions { EnvironmentUrl = Url },
            Reader(new Dictionary<string, string>
            {
                [FederatedConnectionSettings.TenantIdVariable] = "env-tenant",
                [FederatedConnectionSettings.ClientIdVariable] = "env-app",
                [FederatedConnectionSettings.ServiceConnectionIdVariable] = "env-connection"
            }));

        Assert.AreEqual(Url, settings.Url);
        Assert.AreEqual("env-tenant", settings.TenantId);
        Assert.AreEqual("env-app", settings.ClientId);
        Assert.AreEqual("env-connection", settings.ServiceConnectionId);
    }

    [Test]
    public void Create_AcceptsTheAlternateFieldSpellings()
    {
        var settings = FederatedConnectionSettings.Create(
            "AuthType=WorkloadIdentity; ServiceUri=" + Url + "; AppId=config-app",
            new DeployOptions(),
            Empty);

        Assert.AreEqual(Url, settings.Url);
        Assert.AreEqual("config-app", settings.ClientId);
    }

    [Test]
    public void Create_TrimsTheTrailingSlashOfTheUrl()
    {
        var settings = FederatedConnectionSettings.Create(
            null, new DeployOptions { EnvironmentUrl = Url + "/" }, Empty);

        Assert.AreEqual(Url, settings.Url);
    }

    [Test]
    public void Create_ReadsAFieldWhoseValueContainsAnEqualsSign()
    {
        // The reason this does not go through ConnectionStringParser: that one splits the whole
        // segment on '=' and keeps the second piece, which truncates a URL carrying a query.
        var settings = FederatedConnectionSettings.Create(
            "AuthType=WorkloadIdentity; Url=https://org.crm4.dynamics.com/?a=b",
            new DeployOptions(),
            Empty);

        Assert.AreEqual("https://org.crm4.dynamics.com/?a=b", settings.Url);
    }

    [Test]
    public void Create_TreatsAnEmptyFieldAsAbsent()
    {
        var settings = FederatedConnectionSettings.Create(
            "AuthType=WorkloadIdentity; TenantId=",
            new DeployOptions(),
            Reader(new Dictionary<string, string>
            {
                [FederatedConnectionSettings.TenantIdVariable] = "env-tenant"
            }));

        Assert.AreEqual("env-tenant", settings.TenantId);
    }

    // ──────────────────────────────────────────────
    //  Validation
    // ──────────────────────────────────────────────

    [Test]
    public void Validate_ListsEveryMissingValueAtOnce()
    {
        var settings = FederatedConnectionSettings.Create(null, new DeployOptions(), Empty);

        var error = Assert.Throws<FederatedAuthenticationException>(() => settings.Validate());

        // All three in one run: a pipeline author should not have to discover them one deployment
        // at a time.
        Assert.IsTrue(error!.Message.Contains("--environment"), error.Message);
        Assert.IsTrue(error.Message.Contains("--tenant"), error.Message);
        Assert.IsTrue(error.Message.Contains("--application-id"), error.Message);
    }

    [Test]
    public void Validate_PassesOnceTheThreeAreKnown()
    {
        var settings = FederatedConnectionSettings.Create(
            null,
            new DeployOptions { EnvironmentUrl = Url, TenantId = "tenant", ApplicationId = "app" },
            Empty);

        Assert.DoesNotThrow(() => settings.Validate());
    }

    [Test]
    public void Validate_DoesNotRequireAServiceConnection()
    {
        // An endpoint given whole needs none; the provider is what complains when it does.
        var settings = FederatedConnectionSettings.Create(
            null,
            new DeployOptions { EnvironmentUrl = Url, TenantId = "tenant", ApplicationId = "app" },
            Empty);

        Assert.IsNull(settings.ServiceConnectionId);
        Assert.DoesNotThrow(() => settings.Validate());
    }

    // ──────────────────────────────────────────────
    //  Scope
    // ──────────────────────────────────────────────

    [Test]
    public void ScopeFor_UsesTheOrganisationsOwnDefaultScope()
    {
        var settings = FederatedConnectionSettings.Create(
            null, new DeployOptions { EnvironmentUrl = Url }, Empty);

        Assert.AreEqual(Url + "/.default", settings.ScopeFor(null));
        Assert.AreEqual(Url + "/.default", settings.ScopeFor("   "));
    }

    [Test]
    public void ScopeFor_PrefersTheInstanceTheClientIsCalling()
    {
        var settings = FederatedConnectionSettings.Create(
            null, new DeployOptions { EnvironmentUrl = Url }, Empty);

        Assert.AreEqual(
            "https://other.crm4.dynamics.com/.default",
            settings.ScopeFor("https://other.crm4.dynamics.com/"));
    }

    [Test]
    public void ScopeFor_KeepsOnlyTheAuthorityOfWhatServiceClientPasses()
    {
        // ServiceClient hands its token provider the SOAP endpoint it is about to call, not the
        // organization root. Appending /.default to that asks Entra for a scope that does not
        // exist, and Entra answers invalid_client - observed against a real tenant before this
        // was normalised.
        var settings = FederatedConnectionSettings.Create(
            null, new DeployOptions { EnvironmentUrl = Url }, Empty);

        Assert.AreEqual(
            Url + "/.default",
            settings.ScopeFor(Url + "/XRMServices/2011/Organization.svc/web?SDKClientVersion=9.2.49.17970"));
    }

    [Test]
    public void ScopeFor_KeepsAnExplicitPort()
    {
        var settings = FederatedConnectionSettings.Create(
            null, new DeployOptions { EnvironmentUrl = "https://crm.contoso.local:444/org" }, Empty);

        Assert.AreEqual("https://crm.contoso.local:444/.default", settings.ScopeFor(null));
    }

    [Test]
    public void ScopeFor_WithNoUrlAtAll_SaysWhatToPass()
    {
        var settings = FederatedConnectionSettings.Create(null, new DeployOptions(), Empty);

        var error = Assert.Throws<FederatedAuthenticationException>(() => settings.ScopeFor(null));

        Assert.IsTrue(error!.Message.Contains("--environment"), error.Message);
    }

    private static readonly Func<string, string> Empty = _ => null;

    private static Func<string, string> Reader(IDictionary<string, string> environment)
        => name => environment.TryGetValue(name, out var value) ? value : null;
}
