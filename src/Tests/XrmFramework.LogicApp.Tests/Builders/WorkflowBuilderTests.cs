// Copyright (c) DIMSI. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using NUnit.Framework;
using XrmFramework.LogicApp.Builders;
using XrmFramework.LogicApp.Models.Actions;
using XrmFramework.LogicApp.Models.Triggers;

namespace XrmFramework.LogicApp.Tests.Builders;

/// <summary>
/// Unit tests for <see cref="WorkflowBuilder"/>.
/// </summary>
[TestFixture]
public class WorkflowBuilderTests
{
    // ──────────────────────────────────────────────
    //  Build — initial state
    // ──────────────────────────────────────────────

    [Test]
    public void Build_EmptyBuilder_ReturnsEmptyDefinition()
    {
        var definition = new WorkflowBuilder().Build();

        Assert.IsNotNull(definition);
        Assert.AreEqual(0, definition.Triggers.Count);
        Assert.AreEqual(0, definition.Actions.Count);
    }

    // ──────────────────────────────────────────────
    //  AddTrigger
    // ──────────────────────────────────────────────

    [Test]
    public void AddTrigger_Instance_AddsToDefinition()
    {
        var trigger = new HttpRequestTrigger();
        var definition = new WorkflowBuilder()
            .AddTrigger("manual", trigger)
            .Build();

        Assert.IsTrue(definition.Triggers.ContainsKey("manual"));
        Assert.AreSame(trigger, definition.Triggers["manual"]);
    }

    [Test]
    public void AddTrigger_Factory_ConfiguresTriggerAndAddsToDefinition()
    {
        var schema = new { type = "object" };
        var definition = new WorkflowBuilder()
            .AddTrigger<HttpRequestTrigger>("manual", t => t.Schema = schema)
            .Build();

        Assert.IsTrue(definition.Triggers.ContainsKey("manual"));
        Assert.AreEqual(schema, ((HttpRequestTrigger)definition.Triggers["manual"]).Schema);
    }

    [Test]
    public void AddTrigger_MultipleTriggers_AllPresentInDefinition()
    {
        var definition = new WorkflowBuilder()
            .AddTrigger("trigger1", new HttpRequestTrigger())
            .AddTrigger("trigger2", new RecurrenceTrigger())
            .Build();

        Assert.AreEqual(2, definition.Triggers.Count);
    }

    // ──────────────────────────────────────────────
    //  AddAction
    // ──────────────────────────────────────────────

    [Test]
    public void AddAction_Instance_AddsToDefinition()
    {
        var action = new HttpAction { Method = "GET", Uri = "https://api.example.com" };
        var definition = new WorkflowBuilder()
            .AddAction("Call_API", action)
            .Build();

        Assert.IsTrue(definition.Actions.ContainsKey("Call_API"));
        Assert.AreSame(action, definition.Actions["Call_API"]);
    }

    [Test]
    public void AddAction_Factory_ConfiguresActionAndAddsToDefinition()
    {
        var definition = new WorkflowBuilder()
            .AddAction<HttpAction>("Call_API", a =>
            {
                a.Method = "POST";
                a.Uri = "https://api.example.com/data";
            })
            .Build();

        Assert.IsTrue(definition.Actions.ContainsKey("Call_API"));
        var httpAction = (HttpAction)definition.Actions["Call_API"];
        Assert.AreEqual("POST", httpAction.Method);
        Assert.AreEqual("https://api.example.com/data", httpAction.Uri);
    }

    [Test]
    public void AddAction_MultipleActions_AllPresentInDefinition()
    {
        var definition = new WorkflowBuilder()
            .AddAction("Step1", new ComposeAction())
            .AddAction("Step2", new HttpAction())
            .AddAction("Step3", new ResponseAction())
            .Build();

        Assert.AreEqual(3, definition.Actions.Count);
    }

    // ──────────────────────────────────────────────
    //  AddParameter
    // ──────────────────────────────────────────────

    [Test]
    public void AddParameter_Typed_AddsToDefinition()
    {
        var definition = new WorkflowBuilder()
            .AddParameter("myParam", "string", "defaultValue", "A test parameter")
            .Build();

        Assert.IsNotNull(definition.Parameters);
        Assert.IsTrue(definition.Parameters.ContainsKey("myParam"));
        Assert.AreEqual("string", definition.Parameters["myParam"].Type);
        Assert.AreEqual("defaultValue", definition.Parameters["myParam"].DefaultValue);
    }

    [Test]
    public void AddParameter_WithoutDefault_DefaultValueIsNull()
    {
        var definition = new WorkflowBuilder()
            .AddParameter("myParam", "integer")
            .Build();

        Assert.IsNull(definition.Parameters!["myParam"].DefaultValue);
    }

    // ──────────────────────────────────────────────
    //  AddOutput
    // ──────────────────────────────────────────────

    [Test]
    public void AddOutput_AddsToDefinition()
    {
        var definition = new WorkflowBuilder()
            .AddOutput("result", "@body('Call_API')")
            .Build();

        Assert.IsNotNull(definition.Outputs);
        Assert.IsTrue(definition.Outputs.ContainsKey("result"));
        Assert.AreEqual("@body('Call_API')", definition.Outputs["result"]);
    }

    // ──────────────────────────────────────────────
    //  Fluent chaining
    // ──────────────────────────────────────────────

    [Test]
    public void FluentChaining_ReturnsBuilderInstance()
    {
        var builder = new WorkflowBuilder();
        var returned = builder.AddTrigger("t", new HttpRequestTrigger());
        Assert.AreSame(builder, returned);
    }
}
