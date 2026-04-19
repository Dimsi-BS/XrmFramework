// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XrmFramework.DeployUtils.Configuration;

namespace XrmFramework.DeployUtils.Tests.Configuration;

/// <summary>
/// Tests unitaires pour <see cref="ConnectionStringParser"/>.
/// </summary>
[TestClass]
public class ConnectionStringParserTests
{
    // ──────────────────────────────────────────────
    //  Parse — cas nominal
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Parse_WithAllFields_ReturnsCorrectConnectionString()
    {
        const string input = "Url=https://myorg.crm.dynamics.com;Username=admin@myorg.onmicrosoft.com;Password=P@ssw0rd!";

        var result = ConnectionStringParser.Parse(input);

        Assert.AreEqual("https://myorg.crm.dynamics.com", result.Url);
        Assert.AreEqual("admin@myorg.onmicrosoft.com", result.Username);
        Assert.AreEqual("P@ssw0rd!", result.Password);
    }

    [TestMethod]
    public void Parse_UrlOnly_ReturnsConnectionStringWithUrl()
    {
        const string input = "Url=https://myorg.crm.dynamics.com";

        var result = ConnectionStringParser.Parse(input);

        Assert.AreEqual("https://myorg.crm.dynamics.com", result.Url);
        Assert.IsNull(result.Username);
        Assert.IsNull(result.Password);
    }

    [TestMethod]
    public void Parse_UsernameAndPassword_ReturnsCorrectValues()
    {
        const string input = "Username=user@domain.com;Password=secret123";

        var result = ConnectionStringParser.Parse(input);

        Assert.IsNull(result.Url);
        Assert.AreEqual("user@domain.com", result.Username);
        Assert.AreEqual("secret123", result.Password);
    }

    // ──────────────────────────────────────────────
    //  Parse — gestion des espaces
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Parse_WithLeadingAndTrailingSpaces_TrimsValues()
    {
        const string input = "Url= https://myorg.crm.dynamics.com ;Username= admin ";

        var result = ConnectionStringParser.Parse(input);

        Assert.AreEqual("https://myorg.crm.dynamics.com", result.Url);
        Assert.AreEqual("admin", result.Username);
    }

    // ──────────────────────────────────────────────
    //  Parse — mot de passe avec caractères spéciaux
    // ──────────────────────────────────────────────

    [TestMethod]
    public void Parse_PasswordWithEqualsSign_PreservesFullValue()
    {
        // Le mot de passe contient un signe "=" : cas réel avec des tokens Base64 ou OAuth
        const string input = "Url=https://myorg.crm.dynamics.com;Password=abc=def==";

        var result = ConnectionStringParser.Parse(input);

        Assert.AreEqual("abc=def==", result.Password);
    }

    // ──────────────────────────────────────────────
    //  Parse — cas invalides
    // ──────────────────────────────────────────────

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Parse_NullInput_ThrowsArgumentNullException()
    {
        ConnectionStringParser.Parse(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Parse_EmptyString_ThrowsArgumentNullException()
    {
        ConnectionStringParser.Parse(string.Empty);
    }

    [TestMethod]
    public void Parse_UnknownKeys_AreIgnored()
    {
        const string input = "Url=https://myorg.crm.dynamics.com;UnknownKey=value";

        // Ne doit pas lever d'exception
        var result = ConnectionStringParser.Parse(input);

        Assert.AreEqual("https://myorg.crm.dynamics.com", result.Url);
    }
}
