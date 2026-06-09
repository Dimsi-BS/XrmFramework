// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using XrmFramework;
using XrmFramework.DeployUtils.Factories;
using XrmFramework.PluginInventory;

namespace XrmFramework.PluginInventory.Tests;

/// <summary>
/// Exerce le vrai moteur d'inventaire (<see cref="PluginInventoryEngine" />) contre des plugins
/// stub, en l'exécutant sur l'assembly de test elle-même. Couvre :
/// <list type="bullet">
///   <item>la découverte des plugins (public + internal) et l'exclusion des non-plugins ;</item>
///   <item>la transcription fidèle des steps (message/stage/mode, images, filtering, ordre…) ;</item>
///   <item>les custom APIs (binding, traitement autorisé, arguments typés) et les workflows ;</item>
///   <item>le round-trip producteur → <see cref="PluginInventoryReader" /> (consommateur réel).</item>
/// </list>
/// </summary>
[TestFixture]
public class PluginInventoryEngineTests
{
    private static string _json;
    private static JsonDocument _doc;
    private static JsonElement _root;

    [OneTimeSetUp]
    public void RunInventory()
    {
        var assemblyPath = typeof(Sample.AccountPlugin).Assembly.Location;
        _json = PluginInventoryEngine.BuildManifestJson(assemblyPath);
        _doc = JsonDocument.Parse(_json);
        _root = _doc.RootElement;
    }

    [OneTimeTearDown]
    public void Cleanup() => _doc?.Dispose();

    // ── Helpers JSON ────────────────────────────────────────────────────────

    private static JsonElement Plugin(string fullName) =>
        _root.GetProperty("plugins").EnumerateArray().Single(p => p.GetProperty("fullName").GetString() == fullName);

    private static JsonElement StepOf(JsonElement plugin, string methodName) =>
        plugin.GetProperty("steps").EnumerateArray().Single(s => s.GetProperty("methodName").GetString() == methodName);

    private static JsonElement CustomApi(string fullName) =>
        _root.GetProperty("customApis").EnumerateArray().Single(c => c.GetProperty("fullName").GetString() == fullName);

    private static string[] StrArray(JsonElement e, string prop) =>
        e.GetProperty(prop).EnumerateArray().Select(x => x.GetString()).ToArray();

    // ── Découverte des plugins ──────────────────────────────────────────────

    [Test]
    public void Inventories_Public_And_Internal_Plugins_And_Ignores_Others()
    {
        var fullNames = _root.GetProperty("plugins").EnumerateArray()
            .Select(p => p.GetProperty("fullName").GetString()).ToArray();

        Assert.That(fullNames, Does.Contain("Sample.AccountPlugin"));
        Assert.That(fullNames, Does.Contain("Sample.HiddenPlugin")); // internal
        Assert.That(fullNames, Has.Length.EqualTo(2));               // NotAPlugin exclu
    }

    // ── Steps : transcription fidèle (le moteur lit le vrai Step) ────────────

    [Test]
    public void Step_Create_IsTranscribed_WithPreImage_AndPostImageAllAttributes()
    {
        var step = StepOf(Plugin("Sample.AccountPlugin"), "OnCreate");

        Assert.Multiple(() =>
        {
            Assert.That(step.GetProperty("message").GetString(), Is.EqualTo("Create"));
            Assert.That(step.GetProperty("stage").GetString(), Is.EqualTo("PreOperation"));
            Assert.That(step.GetProperty("mode").GetString(), Is.EqualTo("Synchronous"));
            Assert.That(step.GetProperty("entityName").GetString(), Is.EqualTo("account"));
            Assert.That(step.GetProperty("order").GetInt32(), Is.EqualTo(1));
            Assert.That(StrArray(step, "methodNames"), Is.EqualTo(new[] { "OnCreate" }));

            Assert.That(step.GetProperty("preImage").GetProperty("allAttributes").GetBoolean(), Is.False);
            Assert.That(StrArray(step.GetProperty("preImage"), "attributes"),
                Is.EqualTo(new[] { "name", "accountnumber" }));

            Assert.That(step.GetProperty("postImage").GetProperty("allAttributes").GetBoolean(), Is.True);
            Assert.That(StrArray(step.GetProperty("postImage"), "attributes"), Is.Empty);
        });
    }

    [Test]
    public void Step_Update_IsTranscribed_WithFiltering_Order_Impersonation_AndConfig()
    {
        var step = StepOf(Plugin("Sample.AccountPlugin"), "OnUpdate");

        Assert.Multiple(() =>
        {
            Assert.That(step.GetProperty("message").GetString(), Is.EqualTo("Update"));
            Assert.That(step.GetProperty("stage").GetString(), Is.EqualTo("PostOperation"));
            Assert.That(step.GetProperty("mode").GetString(), Is.EqualTo("Asynchronous"));
            Assert.That(StrArray(step, "filteringAttributes"), Is.EqualTo(new[] { "name" }));
            Assert.That(step.GetProperty("order").GetInt32(), Is.EqualTo(5));
            Assert.That(step.GetProperty("impersonationUsername").GetString(), Is.EqualTo("admin"));
            Assert.That(step.GetProperty("unsecureConfig").GetString(), Is.EqualTo("{\"bannedMethods\":[\"Foo\"]}"));
        });
    }

    // ── Custom APIs ─────────────────────────────────────────────────────────

    [Test]
    public void CustomApi_IsInventoried_WithBinding_Processing_AndTypedArguments()
    {
        var api = CustomApi("Sample.DoTheThing");

        Assert.Multiple(() =>
        {
            Assert.That(api.GetProperty("name").GetString(), Is.EqualTo("DoTheThing"));
            Assert.That(api.GetProperty("displayName").GetString(), Is.EqualTo("Do The Thing"));
            Assert.That(api.GetProperty("bindingType").GetString(), Is.EqualTo("Entity"));
            Assert.That(api.GetProperty("boundEntityLogicalName").GetString(), Is.EqualTo("account"));
            Assert.That(api.GetProperty("allowedCustomProcessing").GetString(), Is.EqualTo("AsyncOnly"));
            Assert.That(api.GetProperty("isFunction").GetBoolean(), Is.False);
        });

        var args = api.GetProperty("arguments").EnumerateArray().ToArray();
        Assert.That(args, Has.Length.EqualTo(2));

        var input = args.Single(a => a.GetProperty("name").GetString() == "Name");
        var output = args.Single(a => a.GetProperty("name").GetString() == "Count");

        Assert.Multiple(() =>
        {
            Assert.That(input.GetProperty("isInArgument").GetBoolean(), Is.True);
            Assert.That(input.GetProperty("typeFullName").GetString(), Is.EqualTo("System.String"));
            Assert.That(input.GetProperty("isEnum").GetBoolean(), Is.False);
            Assert.That(input.GetProperty("isOptional").GetBoolean(), Is.True);
            Assert.That(input.GetProperty("displayName").GetString(), Is.EqualTo("Le nom"));

            Assert.That(output.GetProperty("isInArgument").GetBoolean(), Is.False);
            Assert.That(output.GetProperty("typeFullName").GetString(), Is.EqualTo("System.Int32"));
        });
    }

    // ── Workflows ───────────────────────────────────────────────────────────

    [Test]
    public void Workflows_Use_DisplayName_OrFallBackToTypeName()
    {
        var workflows = _root.GetProperty("workflows").EnumerateArray().ToArray();

        var greeting = workflows.Single(w => w.GetProperty("fullName").GetString() == "Sample.GreetingWorkflow");
        var nameless = workflows.Single(w => w.GetProperty("fullName").GetString() == "Sample.NamelessWorkflow");

        Assert.Multiple(() =>
        {
            Assert.That(greeting.GetProperty("displayName").GetString(), Is.EqualTo("Say Hello"));
            Assert.That(nameless.GetProperty("displayName").GetString(), Is.EqualTo("NamelessWorkflow"));
        });
    }

    // ── Round-trip producteur → consommateur réel (PluginInventoryReader) ─────

    [Test]
    public void RoundTrip_ThroughPluginInventoryReader_MapsTheRealModel()
    {
        var account = PluginInventoryReader.ReadPlugins(_json).Single(p => p.FullName == "Sample.AccountPlugin");

        var create = account.Steps.Single(s => s.Message == Messages.Create);
        var update = account.Steps.Single(s => s.Message == Messages.Update);

        Assert.Multiple(() =>
        {
            Assert.That(create.Stage, Is.EqualTo(Stages.PreOperation));
            Assert.That(create.Mode, Is.EqualTo(Modes.Synchronous));
            Assert.That(create.EntityName, Is.EqualTo("account"));
            Assert.That(create.PreImage.Attributes, Is.EquivalentTo(new[] { "name", "accountnumber" }));
            Assert.That(create.PostImage.AllAttributes, Is.True);

            Assert.That(update.Stage, Is.EqualTo(Stages.PostOperation));
            Assert.That(update.Order, Is.EqualTo(5));
            Assert.That(update.ImpersonationUsername, Is.EqualTo("admin"));
            Assert.That(update.FilteringAttributes, Is.EquivalentTo(new[] { "name" }));
            Assert.That(update.StepConfiguration.BannedMethods, Does.Contain("Foo"));
        });
    }

    [Test]
    public void RoundTrip_CustomApi_ThroughPluginInventoryReader()
    {
        var api = PluginInventoryReader.ReadCustomApis(_json, "pref").Single(c => c.Name == "DoTheThing");

        Assert.Multiple(() =>
        {
            Assert.That(api.DisplayName, Is.EqualTo("Do The Thing"));
            Assert.That(api.BindingType.Value, Is.EqualTo(1));               // Entity
            Assert.That(api.BoundEntityLogicalName, Is.EqualTo("account"));
            Assert.That(api.Children.Count(), Is.EqualTo(2));                // 1 input + 1 output
        });
    }
}
