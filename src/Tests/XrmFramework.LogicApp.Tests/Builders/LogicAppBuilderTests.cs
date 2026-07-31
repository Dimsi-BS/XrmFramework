// Copyright (c) DIMSI. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using NUnit.Framework;
using XrmFramework.LogicApp.Builders;
using XrmFramework.LogicApp.Models;
using XrmFramework.LogicApp.Models.Actions;
using XrmFramework.LogicApp.Models.Triggers;

namespace XrmFramework.LogicApp.Tests.Builders;

/// <summary>
/// Unit tests for <see cref="LogicAppBuilder"/>.
/// </summary>
[TestFixture]
public class LogicAppBuilderTests
{
    // ──────────────────────────────────────────────
    //  ForConsumption
    // ──────────────────────────────────────────────

    [Test]
    public void ForConsumption_Build_ReturnsConsumptionWorkflow()
    {
        var result = LogicAppBuilder
            .ForConsumption()
            .Build();

        Assert.That(result, Is.InstanceOf<ConsumptionWorkflow>());
    }

    [Test]
    public void ForConsumption_BuildConsumption_ReturnsConsumptionWorkflow()
    {
        var result = LogicAppBuilder
            .ForConsumption()
            .BuildConsumption();

        Assert.IsNotNull(result);
    }

    [Test]
    public void ForConsumption_BuildStandard_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            LogicAppBuilder
                .ForConsumption()
                .BuildStandard());
    }

    // ──────────────────────────────────────────────
    //  ForStandard
    // ──────────────────────────────────────────────

    [Test]
    public void ForStandard_Build_ReturnsStandardWorkflow()
    {
        var result = LogicAppBuilder
            .ForStandard()
            .Build();

        Assert.That(result, Is.InstanceOf<StandardWorkflow>());
    }

    [Test]
    public void ForStandard_BuildStandard_ReturnsStandardWorkflow()
    {
        var result = LogicAppBuilder
            .ForStandard(WorkflowKind.Stateless)
            .BuildStandard();

        Assert.IsNotNull(result);
    }

    [Test]
    public void ForStandard_BuildConsumption_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            LogicAppBuilder
                .ForStandard()
                .BuildConsumption());
    }

    // ──────────────────────────────────────────────
    //  WithWorkflow
    // ──────────────────────────────────────────────

    [Test]
    public void WithWorkflow_AddsTrigger_DefinitionContainsTrigger()
    {
        var result = LogicAppBuilder
            .ForConsumption()
            .WithWorkflow(w => w.AddTrigger("manual", new HttpRequestTrigger()))
            .BuildConsumption();

        Assert.IsNotNull(result.Definition);
        Assert.IsTrue(result.Definition.Triggers.ContainsKey("manual"));
    }

    [Test]
    public void WithWorkflow_AddsAction_DefinitionContainsAction()
    {
        var result = LogicAppBuilder
            .ForConsumption()
            .WithWorkflow(w => w
                .AddTrigger("manual", new HttpRequestTrigger())
                .AddAction("Send_HTTP", new HttpAction { Method = "POST", Uri = "https://example.com" }))
            .BuildConsumption();

        Assert.IsTrue(result.Definition.Actions.ContainsKey("Send_HTTP"));
    }

    // ──────────────────────────────────────────────
    //  AddConnection (Consumption)
    // ──────────────────────────────────────────────

    [Test]
    public void AddConnection_SetsConsumptionParameters()
    {
        var result = LogicAppBuilder
            .ForConsumption()
            .AddConnection(
                "servicebus",
                "/subscriptions/xxx/resourceGroups/rg/providers/Microsoft.Web/connections/servicebus",
                "/subscriptions/xxx/providers/Microsoft.Web/locations/westeurope/managedApis/servicebus")
            .BuildConsumption();

        Assert.IsNotNull(result.Parameters);
        Assert.IsTrue(result.Parameters.ContainsKey("$connections"));
    }

    [Test]
    public void AddConnection_WithNoConnections_ParametersIsNull()
    {
        var result = LogicAppBuilder
            .ForConsumption()
            .BuildConsumption();

        Assert.IsNull(result.Parameters);
    }

    // ──────────────────────────────────────────────
    //  AddConnectionReference (Standard)
    // ──────────────────────────────────────────────

    [Test]
    public void AddConnectionReference_SetsStandardConnectionReferences()
    {
        var result = LogicAppBuilder
            .ForStandard()
            .AddConnectionReference("myConn", new { api = new { id = "/managed/apis/office365" } })
            .BuildStandard();

        Assert.IsNotNull(result.ConnectionReferences);
        Assert.IsTrue(result.ConnectionReferences.ContainsKey("myConn"));
    }

    [Test]
    public void AddConnectionReference_WithNoReferences_ConnectionReferencesIsNull()
    {
        var result = LogicAppBuilder
            .ForStandard()
            .BuildStandard();

        Assert.IsNull(result.ConnectionReferences);
    }

    // ──────────────────────────────────────────────
    //  Build — without a configured workflow
    // ──────────────────────────────────────────────

    [Test]
    public void Build_WithoutWorkflow_ReturnsEmptyDefinition()
    {
        var result = LogicAppBuilder
            .ForConsumption()
            .BuildConsumption();

        Assert.IsNotNull(result.Definition);
        Assert.AreEqual(0, result.Definition.Triggers.Count);
        Assert.AreEqual(0, result.Definition.Actions.Count);
    }
}
