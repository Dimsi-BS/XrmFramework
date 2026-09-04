// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using XrmFramework.BindingModel.Tests.Models;

namespace XrmFramework.BindingModel.Tests;

/// <summary>
/// The binding models generated from the <c>.model</c> files beside this project, over the
/// framework's own <c>.table</c> files.
///
/// That these tests compile is already half the point: the tests over the generator assert on
/// emitted text, which cannot tell valid C# from a plausible-looking string. This project runs
/// the generators for real, so the models have to compile before a single assertion runs — and
/// the first build of it found a missing <c>using Microsoft.Xrm.Sdk</c> that no string assertion
/// would ever have caught.
/// </summary>
[TestFixture]
public class GeneratedModelTests
{
    private static readonly Guid SessionId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid DebugeeId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static Entity DebugSessionEntity()
    {
        var entity = new Entity(DebugSessionDefinition.EntityName, SessionId);

        entity[DebugSessionDefinition.Columns.Name] = "Session de Christophe";
        entity[DebugSessionDefinition.Columns.RelayUrl] = "https://contoso.servicebus.windows.net";
        entity[DebugSessionDefinition.Columns.DebugInfo] = "trace";
        entity[DebugSessionDefinition.Columns.SessionEnd] = new DateTime(2026, 9, 4, 18, 0, 0, DateTimeKind.Utc);
        entity[DebugSessionDefinition.Columns.DebugeeId] = new EntityReference(SystemUserDefinition.EntityName, DebugeeId);
        entity[DebugSessionDefinition.Columns.StateCode] = new OptionSetValue((int)DebugSessionState.Active);

        return entity;
    }

    // ── Entity -> model ───────────────────────────────────────────────────────

    [Test]
    public void ToBindingModel_ReadsScalarColumns()
    {
        var model = DebugSessionModel.ToBindingModel(DebugSessionEntity());

        Assert.AreEqual(SessionId, model.Id);
        Assert.AreEqual("Session de Christophe", model.Name);
        Assert.AreEqual("https://contoso.servicebus.windows.net", model.RelayUrl);
        Assert.AreEqual("trace", model.DebugInfo);
    }

    [Test]
    public void ToBindingModel_ReadsADateTime()
    {
        var model = DebugSessionModel.ToBindingModel(DebugSessionEntity());

        Assert.AreEqual(new DateTime(2026, 9, 4, 18, 0, 0, DateTimeKind.Utc), model.SessionEnd);
    }

    /// <summary>A lookup declared as a Guid is read off the EntityReference's id.</summary>
    [Test]
    public void ToBindingModel_ReadsALookupAsItsId()
    {
        var model = DebugSessionModel.ToBindingModel(DebugSessionEntity());

        Assert.AreEqual(DebugeeId, model.DebugeeId);
    }

    /// <summary>A State column is read as the enum the table generator emits for it.</summary>
    [Test]
    public void ToBindingModel_ReadsAnOptionSetAsItsEnum()
    {
        var model = DebugSessionModel.ToBindingModel(DebugSessionEntity());

        Assert.AreEqual(DebugSessionState.Active, model.State);
    }

    [Test]
    public void ToBindingModel_AnAbsentColumn_LeavesTheDefault()
    {
        var model = DebugSessionModel.ToBindingModel(new Entity(DebugSessionDefinition.EntityName, SessionId));

        Assert.IsNull(model.Name);
        Assert.IsNull(model.SessionEnd);
    }

    /// <summary>A different table must not be mapped, whatever its columns hold.</summary>
    [Test]
    public void ToBindingModel_AnotherEntity_ReturnsNull()
    {
        Assert.IsNull(DebugSessionModel.ToBindingModel(new Entity(SystemUserDefinition.EntityName, SessionId)));
    }

    // ── Model -> entity ───────────────────────────────────────────────────────

    [Test]
    public void ToEntity_WritesTheLogicalNameAndScalars()
    {
        var model = new DebugSessionModel { Name = "Nouvelle session", RelayUrl = "https://x" };

        var entity = model.ToEntity(null);

        Assert.AreEqual(DebugSessionDefinition.EntityName, entity.LogicalName);
        Assert.AreEqual("Nouvelle session", entity[DebugSessionDefinition.Columns.Name]);
    }

    /// <summary>A Guid lookup is written back as an EntityReference naming the target table.</summary>
    [Test]
    public void ToEntity_WritesALookupAsAnEntityReference()
    {
        var model = new DebugSessionModel { DebugeeId = DebugeeId };

        var reference = model.ToEntity(null)[DebugSessionDefinition.Columns.DebugeeId] as EntityReference;

        Assert.IsNotNull(reference);
        Assert.AreEqual(SystemUserDefinition.EntityName, reference!.LogicalName);
        Assert.AreEqual(DebugeeId, reference.Id);
    }

    [Test]
    public void ToEntity_WritesAnOptionSetValue()
    {
        var model = new DebugSessionModel { State = DebugSessionState.Inactive };

        var value = model.ToEntity(null)[DebugSessionDefinition.Columns.StateCode] as OptionSetValue;

        Assert.IsNotNull(value);
        Assert.AreEqual((int)DebugSessionState.Inactive, value!.Value);
    }

    /// <summary>Round-tripping must not lose what it read.</summary>
    [Test]
    public void RoundTrip_KeepsTheValues()
    {
        var entity = DebugSessionModel.ToBindingModel(DebugSessionEntity()).ToEntity(null);

        Assert.AreEqual("Session de Christophe", entity[DebugSessionDefinition.Columns.Name]);
        Assert.AreEqual(DebugeeId, ((EntityReference)entity[DebugSessionDefinition.Columns.DebugeeId]).Id);
    }

    // ── The polymorphic model ─────────────────────────────────────────────────

    /// <summary>
    /// <c>eventhandler</c> reaches both <c>plugintype</c> and <c>serviceendpoint</c>; the model
    /// names the first, and that is the table the generated mapping writes.
    /// </summary>
    [Test]
    public void PolymorphicLookup_WritesTheChosenTable()
    {
        var id = Guid.NewGuid();
        var model = new SdkMessageProcessingStepModel { EventHandlerId = id };

        var reference = model.ToEntity(null)[SdkMessageProcessingStepDefinition.Columns.EventHandler] as EntityReference;

        Assert.IsNotNull(reference);
        Assert.AreEqual(PluginTypeDefinition.EntityName, reference!.LogicalName);
    }

    [Test]
    public void PolymorphicModel_ReadsItsOptionSets()
    {
        var entity = new Entity(SdkMessageProcessingStepDefinition.EntityName, Guid.NewGuid());
        entity[SdkMessageProcessingStepDefinition.Columns.Name] = "Contact PostCreate";
        entity[SdkMessageProcessingStepDefinition.Columns.Stage] = new OptionSetValue((int)Stage.PostOperation);

        var model = SdkMessageProcessingStepModel.ToBindingModel(entity);

        Assert.AreEqual("Contact PostCreate", model.Name);
        Assert.AreEqual(Stage.PostOperation, model.Stage);
    }
}
