// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;
using XrmFramework.DeployUtils.Configuration;

namespace XrmFramework.DeployUtils.Tests.Configuration;

[TestFixture]
public class AuthenticationModeParserTests
{
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("auto")]
    [TestCase("AUTO")]
    public void TryParse_NothingOrAuto_IsAuto(string value)
    {
        Assert.IsTrue(AuthenticationModeParser.TryParse(value, out var mode));
        Assert.AreEqual(AuthenticationMode.Auto, mode);
    }

    [TestCase("connectionstring")]
    [TestCase("ConnectionString")]
    [TestCase("connection-string")]
    public void TryParse_ConnectionStringSpellings(string value)
    {
        Assert.IsTrue(AuthenticationModeParser.TryParse(value, out var mode));
        Assert.AreEqual(AuthenticationMode.ConnectionString, mode);
    }

    [TestCase("clientsecret")]
    [TestCase("ClientSecret")]
    [TestCase("client-secret")]
    [TestCase("secret")]
    public void TryParse_ClientSecretSpellings(string value)
    {
        Assert.IsTrue(AuthenticationModeParser.TryParse(value, out var mode));
        Assert.AreEqual(AuthenticationMode.ClientSecret, mode);
    }

    [TestCase("wif")]
    [TestCase("WIF")]
    [TestCase(" wif ")]
    [TestCase("workloadidentity")]
    [TestCase("workload-identity")]
    public void TryParse_FederationSpellings(string value)
    {
        Assert.IsTrue(AuthenticationModeParser.TryParse(value, out var mode));
        Assert.AreEqual(AuthenticationMode.WorkloadIdentity, mode);
    }

    [TestCase("oauth")]
    [TestCase("certificate")]
    [TestCase("pac")]
    public void TryParse_UnknownValue_Fails(string value)
    {
        Assert.IsFalse(AuthenticationModeParser.TryParse(value, out _));
    }
}
