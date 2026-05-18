// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json;
using NUnit.Framework;

// ReSharper disable once CheckNamespace
namespace XrmFramework.Tests.Plugin;

/// <summary>
/// Tests unitaires pour <see cref="StepConfiguration"/>.
/// </summary>
[TestFixture]
public class StepConfigurationTests
{
    // ──────────────────────────────────────────────
    //  Initialisation par défaut
    // ──────────────────────────────────────────────

    [Test]
    public void DefaultConstructor_RegisteredMethodsIsEmpty()
    {
        var config = new StepConfiguration();

        Assert.IsNotNull(config.RegisteredMethods);
        Assert.AreEqual(0, config.RegisteredMethods.Count);
    }

    [Test]
    public void DefaultConstructor_BannedMethodsIsEmpty()
    {
        var config = new StepConfiguration();

        Assert.IsNotNull(config.BannedMethods);
        Assert.AreEqual(0, config.BannedMethods.Count);
    }

    [Test]
    public void DefaultConstructor_ConfigurationIsNull()
    {
        var config = new StepConfiguration();

        Assert.IsNull(config.Configuration);
    }

    [Test]
    public void DefaultConstructor_RelationshipNameIsNull()
    {
        var config = new StepConfiguration();

        Assert.IsNull(config.RelationshipName);
    }

    // ──────────────────────────────────────────────
    //  Sérialisation JSON
    // ──────────────────────────────────────────────

    [Test]
    public void Serialize_EmptyConfig_ProducesNonEmptyJson()
    {
        var config = new StepConfiguration();

        var json = JsonConvert.SerializeObject(config);

        Assert.IsFalse(string.IsNullOrWhiteSpace(json));
    }

    [Test]
    public void Serialize_WithRegisteredMethods_JsonContainsMethods()
    {
        var config = new StepConfiguration();
        config.RegisteredMethods.Add("OnCreate");
        config.RegisteredMethods.Add("OnUpdate");

        var json = JsonConvert.SerializeObject(config);

        Assert.IsTrue(json.Contains("OnCreate"), "JSON doit contenir 'OnCreate'");
        Assert.IsTrue(json.Contains("OnUpdate"), "JSON doit contenir 'OnUpdate'");
    }

    [Test]
    public void Serialize_UsesConfigurationPropertyName()
    {
        var config = new StepConfiguration { Configuration = "myconf" };

        var json = JsonConvert.SerializeObject(config);

        Assert.IsTrue(json.Contains("\"configuration\""),
            "Doit utiliser le nom de propriété JSON 'configuration'.");
        Assert.IsTrue(json.Contains("myconf"));
    }

    [Test]
    public void Serialize_UsesRelNamePropertyName()
    {
        var config = new StepConfiguration { RelationshipName = "myrel" };

        var json = JsonConvert.SerializeObject(config);

        Assert.IsTrue(json.Contains("\"relName\""),
            "Doit utiliser le nom de propriété JSON 'relName'.");
        Assert.IsTrue(json.Contains("myrel"));
    }

    [Test]
    public void Serialize_RegisteredMethodsUsesJsonKey()
    {
        var config = new StepConfiguration();
        config.RegisteredMethods.Add("Method1");

        var json = JsonConvert.SerializeObject(config);

        Assert.IsTrue(json.Contains("\"registeredMethods\""),
            "Doit utiliser le nom de propriété JSON 'registeredMethods'.");
    }

    [Test]
    public void Serialize_BannedMethodsUsesJsonKey()
    {
        var config = new StepConfiguration();
        config.BannedMethods.Add("BannedMethod");

        var json = JsonConvert.SerializeObject(config);

        Assert.IsTrue(json.Contains("\"bannedMethods\""),
            "Doit utiliser le nom de propriété JSON 'bannedMethods'.");
        Assert.IsTrue(json.Contains("BannedMethod"));
    }

    // ──────────────────────────────────────────────
    //  Désérialisation JSON
    // ──────────────────────────────────────────────

    [Test]
    public void Deserialize_ValidJson_RestoresConfiguration()
    {
        var json = @"{""configuration"":""myconf"",""relName"":""account_contact"",""registeredMethods"":[""OnCreate""],""bannedMethods"":[""OnDelete""]}";

        var config = JsonConvert.DeserializeObject<StepConfiguration>(json)!;

        Assert.AreEqual("myconf", config.Configuration);
        Assert.AreEqual("account_contact", config.RelationshipName);
        Assert.IsTrue(config.RegisteredMethods.Contains("OnCreate"));
        Assert.IsTrue(config.BannedMethods.Contains("OnDelete"));
    }

    [Test]
    public void RoundTrip_SerializeDeserialize_PreservesAllData()
    {
        var original = new StepConfiguration
        {
            Configuration = "conf",
            RelationshipName = "account_contact"
        };
        original.RegisteredMethods.Add("Method1");
        original.BannedMethods.Add("BannedMethod");

        var json = JsonConvert.SerializeObject(original);
        var restored = JsonConvert.DeserializeObject<StepConfiguration>(json)!;

        Assert.AreEqual(original.Configuration, restored.Configuration);
        Assert.AreEqual(original.RelationshipName, restored.RelationshipName);
        Assert.IsTrue(restored.RegisteredMethods.Contains("Method1"));
        Assert.IsTrue(restored.BannedMethods.Contains("BannedMethod"));
    }
}
