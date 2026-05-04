// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using NUnit.Framework;
using XrmFramework.DeployUtils.Configuration;

namespace XrmFramework.DeployUtils.Tests.Configuration;

[TestFixture]
public class ConnectionStringParserTests
{
    // ──────────────────────────────────────────────
    //  Parse — cas normaux
    // ──────────────────────────────────────────────

    [Test]
    public void Parse_FullConnectionString_ReturnsAllFields()
    {
        var cs = ConnectionStringParser.Parse("Url=https://org.crm.dynamics.com;Username=admin@org.onmicrosoft.com;Password=S3cr3t!");

        Assert.AreEqual("https://org.crm.dynamics.com", cs.Url);
        Assert.AreEqual("admin@org.onmicrosoft.com", cs.Username);
        Assert.AreEqual("S3cr3t!", cs.Password);
    }

    [Test]
    public void Parse_OnlyUrl_ReturnsUrlAndNullOthers()
    {
        var cs = ConnectionStringParser.Parse("Url=https://org.crm.dynamics.com");

        Assert.AreEqual("https://org.crm.dynamics.com", cs.Url);
        Assert.IsNull(cs.Username);
        Assert.IsNull(cs.Password);
    }

    [Test]
    public void Parse_UrlWithTrailingSpaces_TrimsValue()
    {
        var cs = ConnectionStringParser.Parse("Url= https://org.crm.dynamics.com ");

        Assert.AreEqual("https://org.crm.dynamics.com", cs.Url);
    }

    [Test]
    public void Parse_PasswordContainsEquals_PreservesEntirePassword()
    {
        // Password values may contain '=' (e.g. base64-encoded secrets)
        var cs = ConnectionStringParser.Parse("Url=https://org.crm.dynamics.com;Password=abc=def=");

        Assert.AreEqual("abc=def=", cs.Password);
    }

    [Test]
    public void Parse_EmptySegmentsBetweenSemicolons_Ignored()
    {
        // Double semi-colon produces an empty segment
        var cs = ConnectionStringParser.Parse("Url=https://org.crm.dynamics.com;;Username=user");

        Assert.AreEqual("https://org.crm.dynamics.com", cs.Url);
        Assert.AreEqual("user", cs.Username);
    }

    [Test]
    public void Parse_UnknownKey_IsIgnoredGracefully()
    {
        // 'Database' is not a known key; no exception should be thrown
        var cs = ConnectionStringParser.Parse("Url=https://org.crm.dynamics.com;Database=foo");

        Assert.AreEqual("https://org.crm.dynamics.com", cs.Url);
    }

    // ──────────────────────────────────────────────
    //  Parse — cas d'erreur
    // ──────────────────────────────────────────────

    [Test]
    public void Parse_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ConnectionStringParser.Parse(null!));
    }

    [Test]
    public void Parse_EmptyString_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ConnectionStringParser.Parse(string.Empty));
    }

    // ──────────────────────────────────────────────
    //  GetConnectionStringField
    // ──────────────────────────────────────────────

    [Test]
    public void GetConnectionStringField_ExistingField_ReturnsValue()
    {
        var value = ConnectionStringParser.GetConnectionStringField(
            "Url=https://org.crm.dynamics.com;Username=admin", "Url");

        Assert.AreEqual("https://org.crm.dynamics.com", value);
    }

    [Test]
    public void GetConnectionStringField_MissingField_ReturnsNull()
    {
        var value = ConnectionStringParser.GetConnectionStringField(
            "Url=https://org.crm.dynamics.com", "Username");

        Assert.IsNull(value);
    }

    [Test]
    public void GetConnectionStringField_UsernameField_ReturnsValue()
    {
        var value = ConnectionStringParser.GetConnectionStringField(
            "Url=https://org.crm.dynamics.com;Username=admin@org.onmicrosoft.com;Password=pass", "Username");

        Assert.AreEqual("admin@org.onmicrosoft.com", value);
    }

    [Test]
    public void GetConnectionStringField_FieldWithSpaces_TrimsResult()
    {
        var value = ConnectionStringParser.GetConnectionStringField(
            "Url= https://org.crm.dynamics.com ", "Url");

        Assert.AreEqual("https://org.crm.dynamics.com", value);
    }
}
