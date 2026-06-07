// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using System.Text.Json;
using NUnit.Framework;

namespace XrmFramework.PluginManifest.Generator.Tests;

[TestFixture]
public class PluginManifestGeneratorTests
{
    private const string Source = @"
using XrmFramework;
namespace Sample
{
    public class TestPlugin : Plugin
    {
        protected override void AddSteps()
        {
            AddStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, ""account"", nameof(OnCreate));
            AddStep(Stages.PostOperation, Messages.Update, Modes.Asynchronous, ""contact"", nameof(OnUpdate), ""firstname"", ""lastname"");
        }
        [PreImage(true)] public void OnCreate(object ctx) {}
        [ExecutionOrder(5)] public void OnUpdate(object ctx) {}
    }

    public class DynamicPlugin : Plugin
    {
        protected override void AddSteps()
        {
            foreach (var e in new[] { ""a"", ""b"" })
                AddStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, e, nameof(OnAny));
        }
        public void OnAny(object ctx) {}
    }

    public class MyWorkflow : XrmFramework.Workflow.CustomWorkflowActivity
    {
        public MyWorkflow() { SetDisplayName(""Mon Workflow""); }
    }

    public enum Color { Red, Green }

    [CustomApi(CustomApiBindingType.Global, Name = ""my_api"", IsFunction = true, AllowedCustomProcessing = AllowedCustomProcessingStep.AsyncOnly)]
    public class MyCustomApi : CustomApi
    {
        public MyCustomApi() : base(""Run"") {}
        public void Run(object ctx) {}
        [CustomApiInput(Name = ""inText"", IsOptional = true)] public CustomApiInArgument<string>? InText { get; set; }
        [CustomApiInput] public CustomApiInArgument<Color>? InColor { get; set; }
        [CustomApiOutput] public CustomApiOutArgument<int>? OutNum { get; set; }
    }
}";

    private static JsonElement _root;
    private static Microsoft.CodeAnalysis.Diagnostic[] _diagnostics = null!;

    [OneTimeSetUp]
    public void RunGenerator()
    {
        var (json, diagnostics) = GeneratorTestHelper.Run(Source);
        Assert.That(json, Is.Not.Empty, "Le manifeste ne doit pas être vide.");
        _root = JsonDocument.Parse(json).RootElement.Clone();
        _diagnostics = diagnostics.ToArray();
    }

    private static JsonElement Plugin(string fullName)
        => _root.GetProperty("plugins").EnumerateArray()
            .Single(p => p.GetProperty("fullName").GetString() == fullName);

    private static JsonElement Step(JsonElement plugin, string method)
        => plugin.GetProperty("steps").EnumerateArray()
            .Single(s => s.GetProperty("methodName").GetString() == method);

    // ── Plugins / steps ─────────────────────────────────────────────────────

    [Test]
    public void Extracts_TwoPlugins()
    {
        var fullNames = _root.GetProperty("plugins").EnumerateArray()
            .Select(p => p.GetProperty("fullName").GetString()).ToList();
        Assert.That(fullNames, Does.Contain("Sample.TestPlugin"));
        Assert.That(fullNames, Does.Contain("Sample.DynamicPlugin"));
    }

    [Test]
    public void CreateStep_HasExpectedCoreData()
    {
        var step = Step(Plugin("Sample.TestPlugin"), "OnCreate");
        Assert.Multiple(() =>
        {
            Assert.That(step.GetProperty("message").GetString(), Is.EqualTo("Create"));
            Assert.That(step.GetProperty("stage").GetString(), Is.EqualTo("PreOperation"));
            Assert.That(step.GetProperty("mode").GetString(), Is.EqualTo("Synchronous"));
            Assert.That(step.GetProperty("entityName").GetString(), Is.EqualTo("account"));
            Assert.That(step.GetProperty("order").GetInt32(), Is.EqualTo(1));
        });
    }

    [Test]
    public void PreImageAttribute_AllColumns_IsRead()
    {
        var step = Step(Plugin("Sample.TestPlugin"), "OnCreate");
        Assert.That(step.GetProperty("preImage").GetProperty("allAttributes").GetBoolean(), Is.True);
    }

    [Test]
    public void UpdateStep_FilteringFromColumns_AndExecutionOrder()
    {
        var step = Step(Plugin("Sample.TestPlugin"), "OnUpdate");
        var filtering = step.GetProperty("filteringAttributes").EnumerateArray()
            .Select(x => x.GetString()).ToList();
        Assert.Multiple(() =>
        {
            Assert.That(step.GetProperty("message").GetString(), Is.EqualTo("Update"));
            Assert.That(filtering, Is.EquivalentTo(new[] { "firstname", "lastname" }));
            Assert.That(step.GetProperty("order").GetInt32(), Is.EqualTo(5));
        });
    }

    // ── Garde-fou enregistrement dynamique ──────────────────────────────────

    [Test]
    public void DynamicRegistration_ProducesNoStep_AndReportsError()
    {
        var dynamicPlugin = Plugin("Sample.DynamicPlugin");
        Assert.That(dynamicPlugin.GetProperty("steps").GetArrayLength(), Is.EqualTo(0),
            "Un AddStep dans une boucle ne doit produire aucun step.");

        var diagnostic = _diagnostics.Single(d => d.Id == "XRMMAN001");
        Assert.That(diagnostic.Severity, Is.EqualTo(Microsoft.CodeAnalysis.DiagnosticSeverity.Error),
            "L'enregistrement dynamique doit casser le build (erreur), faute de repli d'instanciation.");
    }

    // ── Workflows ───────────────────────────────────────────────────────────

    [Test]
    public void Workflow_IsDetected_WithDisplayName()
    {
        var workflow = _root.GetProperty("workflows").EnumerateArray()
            .Single(w => w.GetProperty("fullName").GetString() == "Sample.MyWorkflow");
        Assert.That(workflow.GetProperty("displayName").GetString(), Is.EqualTo("Mon Workflow"));
    }

    // ── Custom APIs ─────────────────────────────────────────────────────────

    [Test]
    public void CustomApi_AttributeFields_AreRead()
    {
        var api = _root.GetProperty("customApis").EnumerateArray()
            .Single(c => c.GetProperty("fullName").GetString() == "Sample.MyCustomApi");
        Assert.Multiple(() =>
        {
            Assert.That(api.GetProperty("name").GetString(), Is.EqualTo("my_api"));
            Assert.That(api.GetProperty("bindingType").GetString(), Is.EqualTo("Global"));
            Assert.That(api.GetProperty("isFunction").GetBoolean(), Is.True);
            Assert.That(api.GetProperty("allowedCustomProcessing").GetString(), Is.EqualTo("AsyncOnly"));
        });
    }

    [Test]
    public void CustomApi_Arguments_AreRead()
    {
        var api = _root.GetProperty("customApis").EnumerateArray()
            .Single(c => c.GetProperty("fullName").GetString() == "Sample.MyCustomApi");
        var args = api.GetProperty("arguments").EnumerateArray().ToList();

        var input = args.Single(a => a.GetProperty("name").GetString() == "inText");
        var output = args.Single(a => a.GetProperty("name").GetString() == "OutNum");

        var enumArg = args.Single(a => a.GetProperty("name").GetString() == "InColor");

        Assert.Multiple(() =>
        {
            Assert.That(input.GetProperty("isInArgument").GetBoolean(), Is.True);
            Assert.That(input.GetProperty("typeFullName").GetString(), Is.EqualTo("System.String"));
            Assert.That(input.GetProperty("isOptional").GetBoolean(), Is.True);
            Assert.That(input.GetProperty("isEnum").GetBoolean(), Is.False);

            Assert.That(output.GetProperty("isInArgument").GetBoolean(), Is.False);
            Assert.That(output.GetProperty("typeFullName").GetString(), Is.EqualTo("System.Int32"));

            Assert.That(enumArg.GetProperty("isEnum").GetBoolean(), Is.True);
        });
    }
}
