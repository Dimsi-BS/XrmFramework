// Copyright (c) DIMSI. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XrmFramework.LogicApp.Builders;
using XrmFramework.LogicApp.Models;
using XrmFramework.LogicApp.Models.Actions;
using XrmFramework.LogicApp.Models.Triggers;
using XrmFramework.LogicApp.Serialization;

namespace XrmFramework.LogicApp.Tests.Serialization;

/// <summary>
/// Unit tests for <see cref="LogicAppSerializer"/>.
/// </summary>
[TestFixture]
public class LogicAppSerializerTests
{
    // ──────────────────────────────────────────────
    //  SerializeConsumption
    // ──────────────────────────────────────────────

    [Test]
    public void SerializeConsumption_ReturnsValidJson()
    {
        var workflow = LogicAppBuilder
            .ForConsumption()
            .WithWorkflow(w => w.AddTrigger("manual", new HttpRequestTrigger()))
            .BuildConsumption();

        var json = LogicAppSerializer.SerializeConsumption(workflow);

        Assert.IsFalse(string.IsNullOrWhiteSpace(json));
        // Must be valid JSON
        var parsed = JObject.Parse(json);
        Assert.IsNotNull(parsed);
    }

    [Test]
    public void SerializeConsumption_ContainsDefinitionKey()
    {
        var workflow = LogicAppBuilder
            .ForConsumption()
            .WithWorkflow(w => w.AddTrigger("manual", new HttpRequestTrigger()))
            .BuildConsumption();

        var json = LogicAppSerializer.SerializeConsumption(workflow);
        var parsed = JObject.Parse(json);

        Assert.IsTrue(parsed.ContainsKey("definition"), "The JSON must contain the 'definition' key.");
    }

    [Test]
    public void SerializeConsumption_IsIndented()
    {
        var workflow = new ConsumptionWorkflow
        {
            Definition = new WorkflowDefinition()
        };

        var json = LogicAppSerializer.SerializeConsumption(workflow);

        // Indented JSON contains line breaks
        Assert.IsTrue(json.Contains(Environment.NewLine) || json.Contains("\n"),
            "The default JSON must be indented.");
    }

    [Test]
    public void SerializeConsumption_NullValuesOmitted()
    {
        var workflow = new ConsumptionWorkflow
        {
            Definition = new WorkflowDefinition(),
            Parameters = null
        };

        var json = LogicAppSerializer.SerializeConsumption(workflow);

        // Null values must not appear (NullValueHandling.Ignore)
        Assert.IsFalse(json.Contains("\"parameters\": null"),
            "Null keys must not be included in the JSON.");
    }

    [Test]
    public void SerializeConsumption_UsesCamelCase()
    {
        var workflow = LogicAppBuilder
            .ForConsumption()
            .WithWorkflow(w => w.AddTrigger("manual", new HttpRequestTrigger()))
            .BuildConsumption();

        var json = LogicAppSerializer.SerializeConsumption(workflow);

        // Properties must be in camelCase
        Assert.IsTrue(json.Contains("\"definition\""), "Properties must be in camelCase.");
    }

    // ──────────────────────────────────────────────
    //  SerializeStandard
    // ──────────────────────────────────────────────

    [Test]
    public void SerializeStandard_ReturnsValidJson()
    {
        var workflow = LogicAppBuilder
            .ForStandard()
            .WithWorkflow(w => w.AddTrigger("recurrence", new RecurrenceTrigger()))
            .BuildStandard();

        var json = LogicAppSerializer.SerializeStandard(workflow);

        Assert.IsFalse(string.IsNullOrWhiteSpace(json));
        var parsed = JObject.Parse(json);
        Assert.IsNotNull(parsed);
    }

    [Test]
    public void SerializeStandard_ContainsKindKey()
    {
        var workflow = LogicAppBuilder
            .ForStandard(WorkflowKind.Stateful)
            .BuildStandard();

        var json = LogicAppSerializer.SerializeStandard(workflow);
        var parsed = JObject.Parse(json);

        Assert.IsTrue(parsed.ContainsKey("kind"), "The Standard JSON must contain the 'kind' key.");
        Assert.AreEqual("Stateful", parsed["kind"]!.Value<string>());
    }

    // ──────────────────────────────────────────────
    //  Serialize (generic)
    // ──────────────────────────────────────────────

    [Test]
    public void Serialize_WorkflowObject_ReturnsValidJson()
    {
        var workflowObject = LogicAppBuilder
            .ForConsumption()
            .Build();

        var json = LogicAppSerializer.Serialize(workflowObject);

        Assert.IsFalse(string.IsNullOrWhiteSpace(json));
        var parsed = JObject.Parse(json);
        Assert.IsNotNull(parsed);
    }

    // ──────────────────────────────────────────────
    //  WriteConsumption
    // ──────────────────────────────────────────────

    [Test]
    public void WriteConsumption_CreatesFileWithCorrectContent()
    {
        var workflow = LogicAppBuilder
            .ForConsumption()
            .WithWorkflow(w => w
                .AddTrigger("manual", new HttpRequestTrigger())
                .AddAction("Compose", new ComposeAction { Value = "Hello, World!" }))
            .BuildConsumption();

        var outputPath = Path.Combine(Path.GetTempPath(), $"test_consumption_{Guid.NewGuid():N}.json");
        try
        {
            LogicAppSerializer.WriteConsumption(workflow, outputPath);

            Assert.IsTrue(File.Exists(outputPath), "The file must be created.");
            var content = File.ReadAllText(outputPath);
            var parsed = JObject.Parse(content);
            Assert.IsNotNull(parsed["definition"]);
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    // ──────────────────────────────────────────────
    //  WriteStandard
    // ──────────────────────────────────────────────

    [Test]
    public void WriteStandard_CreatesWorkflowJsonInSubfolder()
    {
        var workflow = LogicAppBuilder
            .ForStandard()
            .WithWorkflow(w => w.AddTrigger("recurrence", new RecurrenceTrigger()))
            .BuildStandard();

        var outputDir = Path.Combine(Path.GetTempPath(), $"test_standard_{Guid.NewGuid():N}");
        try
        {
            var writtenPath = LogicAppSerializer.WriteStandard(workflow, "MyWorkflow", outputDir);

            Assert.IsTrue(File.Exists(writtenPath), "The workflow.json file must be created.");
            Assert.IsTrue(writtenPath.EndsWith("workflow.json"), "The file must be named workflow.json.");

            var content = File.ReadAllText(writtenPath);
            var parsed = JObject.Parse(content);
            Assert.IsNotNull(parsed["kind"]);
        }
        finally
        {
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, recursive: true);
        }
    }

    [Test]
    public void WriteStandard_ReturnsFullPathToWorkflowJson()
    {
        var workflow = LogicAppBuilder.ForStandard().BuildStandard();
        var outputDir = Path.Combine(Path.GetTempPath(), $"test_standard_{Guid.NewGuid():N}");

        try
        {
            var result = LogicAppSerializer.WriteStandard(workflow, "OrderProcessing", outputDir);
            var expectedPath = Path.Combine(outputDir, "OrderProcessing", "workflow.json");

            Assert.AreEqual(expectedPath, result);
        }
        finally
        {
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, recursive: true);
        }
    }

    // ──────────────────────────────────────────────
    //  Custom settings
    // ──────────────────────────────────────────────

    [Test]
    public void SerializeConsumption_WithCustomSettings_AppliesSettings()
    {
        var workflow = new ConsumptionWorkflow { Definition = new WorkflowDefinition() };
        var compactSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        var json = LogicAppSerializer.SerializeConsumption(workflow, compactSettings);

        // Compact JSON does not contain line breaks
        Assert.IsFalse(json.Contains("\n"), "With Formatting.None, the JSON must not be indented.");
    }
}
