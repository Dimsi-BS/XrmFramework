// Copyright (c) DIMSI. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using XrmFramework.LogicApp.Builders;
using XrmFramework.LogicApp.Models;
using XrmFramework.LogicApp.Models.Actions;
using XrmFramework.LogicApp.Models.Triggers;

namespace XrmFramework.LogicApp.Tests.Builders;

/// <summary>
/// Tests unitaires pour <see cref="LogicAppBuilder"/>.
/// </summary>
[TestClass]
public class LogicAppBuilderTests
{
    // ──────────────────────────────────────────────
    //  ForConsumption
    // ──────────────────────────────────────────────

    [TestMethod]
    public void ForConsumption_Build_ReturnsConsumptionWorkflow()
    {
        var result = LogicAppBuilder
            .ForConsumption()
            .Build();

        Assert.IsInstanceOfType<ConsumptionWorkflow>(result);
    }

    [TestMethod]
    public void ForConsumption_BuildConsumption_ReturnsConsumptionWorkflow()
    {
        var result = LogicAppBuilder
            .ForConsumption()
            .BuildConsumption();

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ForConsumption_BuildStandard_ThrowsInvalidOperationException()
    {
        LogicAppBuilder
            .ForConsumption()
            .BuildStandard();
    }

    // ──────────────────────────────────────────────
    //  ForStandard
    // ──────────────────────────────────────────────

    [TestMethod]
    public void ForStandard_Build_ReturnsStandardWorkflow()
    {
        var result = LogicAppBuilder
            .ForStandard()
            .Build();

        Assert.IsInstanceOfType<StandardWorkflow>(result);
    }

    [TestMethod]
    public void ForStandard_BuildStandard_ReturnsStandardWorkflow()
    {
        var result = LogicAppBuilder
            .ForStandard(WorkflowKind.Stateless)
            .BuildStandard();

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ForStandard_BuildConsumption_ThrowsInvalidOperationException()
    {
        LogicAppBuilder
            .ForStandard()
            .BuildConsumption();
    }

    // ──────────────────────────────────────────────
    //  WithWorkflow
    // ──────────────────────────────────────────────

    [TestMethod]
    public void WithWorkflow_AddsTrigger_DefinitionContainsTrigger()
    {
        var result = LogicAppBuilder
            .ForConsumption()
            .WithWorkflow(w => w.AddTrigger("manual", new HttpRequestTrigger()))
            .BuildConsumption();

        Assert.IsNotNull(result.Definition);
        Assert.IsTrue(result.Definition.Triggers.ContainsKey("manual"));
    }

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
    public void AddConnectionReference_SetsStandardConnectionReferences()
    {
        var result = LogicAppBuilder
            .ForStandard()
            .AddConnectionReference("myConn", new { api = new { id = "/managed/apis/office365" } })
            .BuildStandard();

        Assert.IsNotNull(result.ConnectionReferences);
        Assert.IsTrue(result.ConnectionReferences.ContainsKey("myConn"));
    }

    [TestMethod]
    public void AddConnectionReference_WithNoReferences_ConnectionReferencesIsNull()
    {
        var result = LogicAppBuilder
            .ForStandard()
            .BuildStandard();

        Assert.IsNull(result.ConnectionReferences);
    }

    // ──────────────────────────────────────────────
    //  Build — sans workflow configuré
    // ──────────────────────────────────────────────

    [TestMethod]
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
