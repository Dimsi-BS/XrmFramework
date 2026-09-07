// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using XrmFramework.BindingModel.Tests.Models;
// This file's own namespace nests under XrmFramework, which declares its own same-named
// Relationship/EntityRole (see XrmFramework.Utils.Relationship) — enclosing-namespace lookup
// resolves a bare reference to those before the Microsoft.Xrm.Sdk ones this file imports, so the
// SDK types need an alias here, the same way the reflection mapper aliases them for the same reason.
using SdkRelationship = Microsoft.Xrm.Sdk.Relationship;
using SdkEntityRole = Microsoft.Xrm.Sdk.EntityRole;

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

    // ── ChildRelationship — a one-to-many relationship as a List<T> property ───

    private static readonly SdkRelationship PluginTypeSteps =
        new(PluginTypeDefinition.OneToManyRelationships.plugintypeid_sdkmessageprocessingstep)
        {
            PrimaryEntityRole = SdkEntityRole.Referenced
        };

    [Test]
    public void ToBindingModel_ReadsRelatedEntitiesIntoTheListProperty()
    {
        var stepId = Guid.NewGuid();
        var step = new Entity(SdkMessageProcessingStepDefinition.EntityName, stepId);
        step[SdkMessageProcessingStepDefinition.Columns.Name] = "Contact PostCreate";

        var pluginType = new Entity(PluginTypeDefinition.EntityName, Guid.NewGuid());
        pluginType[PluginTypeDefinition.Columns.TypeName] = "Contoso.Plugins.ContactPlugin";
        pluginType.RelatedEntities[PluginTypeSteps] = new EntityCollection(new List<Entity> { step });

        var model = PluginTypeModel.ToBindingModel(pluginType);

        Assert.AreEqual("Contoso.Plugins.ContactPlugin", model.TypeName);
        Assert.AreEqual(1, model.Steps.Count);
        Assert.AreEqual(stepId, model.Steps[0].Id);
        Assert.AreEqual("Contact PostCreate", model.Steps[0].Name);
    }

    [Test]
    public void ToBindingModel_NoRelatedEntities_LeavesTheListEmpty()
    {
        var model = PluginTypeModel.ToBindingModel(new Entity(PluginTypeDefinition.EntityName, Guid.NewGuid()));

        Assert.IsEmpty(model.Steps);
    }

    [Test]
    public void ToEntity_WritesTheListAsRelatedEntities()
    {
        var model = new PluginTypeModel
        {
            Steps = new List<SdkMessageProcessingStepModel>
            {
                new() { Name = "Contact PostCreate" }
            }
        };

        var entity = model.ToEntity(null);

        Assert.IsTrue(entity.RelatedEntities.TryGetValue(PluginTypeSteps, out var related));
        Assert.AreEqual(1, related!.Entities.Count);
        Assert.AreEqual("Contact PostCreate", related.Entities[0][SdkMessageProcessingStepDefinition.Columns.Name]);
    }

    [Test]
    public void ToEntity_AnEmptyList_StillWritesAnEmptyRelatedCollection()
    {
        var model = new PluginTypeModel();

        var entity = model.ToEntity(null);

        Assert.IsTrue(entity.RelatedEntities.TryGetValue(PluginTypeSteps, out var related));
        Assert.IsEmpty(related!.Entities);
    }

    // ── LookupTargetModel — embedding another binding model behind a lookup ───

    [Test]
    public void ToBindingModel_EmbedsTheTargetFromAliasedColumns()
    {
        var pluginTypeId = Guid.NewGuid();
        var entity = new Entity(SdkMessageProcessingStepDefinition.EntityName, Guid.NewGuid());
        entity[SdkMessageProcessingStepDefinition.Columns.PluginTypeId] =
            new EntityReference(PluginTypeDefinition.EntityName, pluginTypeId);
        entity["plugintypeid.typename"] = new AliasedValue(
            PluginTypeDefinition.EntityName, PluginTypeDefinition.Columns.TypeName, "Contoso.Plugins.ContactPlugin");

        var model = SdkMessageProcessingStepModel.ToBindingModel(entity);

        Assert.IsNotNull(model.PluginType);
        Assert.AreEqual(pluginTypeId, model.PluginType!.Id);
        Assert.AreEqual("Contoso.Plugins.ContactPlugin", model.PluginType.TypeName);
    }

    [Test]
    public void ToBindingModel_EmbeddedTarget_FallsBackToRelatedEntities()
    {
        var pluginTypeId = Guid.NewGuid();
        var entity = new Entity(SdkMessageProcessingStepDefinition.EntityName, Guid.NewGuid());
        entity[SdkMessageProcessingStepDefinition.Columns.PluginTypeId] =
            new EntityReference(PluginTypeDefinition.EntityName, pluginTypeId);

        var pluginType = new Entity(PluginTypeDefinition.EntityName, pluginTypeId);
        pluginType[PluginTypeDefinition.Columns.TypeName] = "Contoso.Plugins.ContactPlugin";
        entity.RelatedEntities[new SdkRelationship("plugintypeid_sdkmessageprocessingstep")
        {
            PrimaryEntityRole = SdkEntityRole.Referenced
        }] = new EntityCollection(new List<Entity> { pluginType });

        var model = SdkMessageProcessingStepModel.ToBindingModel(entity);

        Assert.IsNotNull(model.PluginType);
        Assert.AreEqual(pluginTypeId, model.PluginType!.Id);
        Assert.AreEqual("Contoso.Plugins.ContactPlugin", model.PluginType.TypeName);
    }

    [Test]
    public void ToBindingModel_NoLookupValue_LeavesTheEmbeddedTargetNull()
    {
        var model = SdkMessageProcessingStepModel.ToBindingModel(
            new Entity(SdkMessageProcessingStepDefinition.EntityName, Guid.NewGuid()));

        Assert.IsNull(model.PluginType);
    }

    /// <summary>
    /// <c>eventhandler</c> is polymorphic, so the aliased columns the query brings back for the
    /// chosen target carry its logical name in the alias, to keep them apart from another
    /// candidate's columns under the same join.
    /// </summary>
    [Test]
    public void ToBindingModel_PolymorphicEmbeddedTarget_ReadsThePolymorphicAlias()
    {
        var pluginTypeId = Guid.NewGuid();
        var entity = new Entity(SdkMessageProcessingStepDefinition.EntityName, Guid.NewGuid());
        entity[SdkMessageProcessingStepDefinition.Columns.EventHandler] =
            new EntityReference(PluginTypeDefinition.EntityName, pluginTypeId);
        entity["eventhandler__plugintype.typename"] = new AliasedValue(
            PluginTypeDefinition.EntityName, PluginTypeDefinition.Columns.TypeName, "Contoso.Plugins.ContactPlugin");

        var model = SdkMessageProcessingStepModel.ToBindingModel(entity);

        Assert.IsNotNull(model.EventHandlerPluginType);
        Assert.AreEqual("Contoso.Plugins.ContactPlugin", model.EventHandlerPluginType!.TypeName);
    }

    [Test]
    public void ToEntity_NeverWritesTheEmbeddedTargetBackToTheLookupColumn()
    {
        var model = new SdkMessageProcessingStepModel { PluginType = new PluginTypeModel { TypeName = "X" } };

        var entity = model.ToEntity(null);

        Assert.IsFalse(entity.Contains(SdkMessageProcessingStepDefinition.Columns.PluginTypeId));
    }

    /// <summary>
    /// The same embedding, from a hand-written class with no <c>[CrmLookup]</c> of its own —
    /// <c>MappingSourceGenerator</c> has to resolve it the same way <c>ModelSourceFileGenerator</c>
    /// does for a <c>.model</c> file, from <see cref="PluginTypeModel"/>'s own <c>[CrmEntity]</c>.
    /// </summary>
    [Test]
    public void HandWrittenModel_EmbedsTheTargetFromAliasedColumns()
    {
        var pluginTypeId = Guid.NewGuid();
        var entity = new Entity(SdkMessageProcessingStepDefinition.EntityName, Guid.NewGuid());
        entity[SdkMessageProcessingStepDefinition.Columns.PluginTypeId] =
            new EntityReference(PluginTypeDefinition.EntityName, pluginTypeId);
        entity["plugintypeid.typename"] = new AliasedValue(
            PluginTypeDefinition.EntityName, PluginTypeDefinition.Columns.TypeName, "Contoso.Plugins.ContactPlugin");

        var model = SdkMessageProcessingStepManualModel.ToBindingModel(entity);

        Assert.IsNotNull(model.PluginType);
        Assert.AreEqual(pluginTypeId, model.PluginType!.Id);
        Assert.AreEqual("Contoso.Plugins.ContactPlugin", model.PluginType.TypeName);
    }

    [Test]
    public void HandWrittenModel_NoLookupValue_LeavesTheEmbeddedTargetNull()
    {
        var model = SdkMessageProcessingStepManualModel.ToBindingModel(
            new Entity(SdkMessageProcessingStepDefinition.EntityName, Guid.NewGuid()));

        Assert.IsNull(model.PluginType);
    }

    // ── Alternate key — populated on ToEntity when nothing set a real Id ──────

    [Test]
    public void ToEntity_NoId_PopulatesKeyAttributesFromTheAlternateKey()
    {
        var aadId = Guid.NewGuid();
        var model = new SystemUserModel { AzureActiveDirectoryObjectId = aadId };

        var entity = model.ToEntity(null);

        Assert.IsTrue(entity.KeyAttributes.Contains(SystemUserDefinition.Columns.AzureActiveDirectoryObjectId));
        Assert.AreEqual(aadId, entity.KeyAttributes[SystemUserDefinition.Columns.AzureActiveDirectoryObjectId]);
    }

    [Test]
    public void ToEntity_WithAnId_LeavesKeyAttributesEmpty()
    {
        var model = new SystemUserModel
        {
            Id = Guid.NewGuid(),
            AzureActiveDirectoryObjectId = Guid.NewGuid()
        };

        var entity = model.ToEntity(null);

        Assert.IsEmpty(entity.KeyAttributes);
    }

    [Test]
    public void ToEntity_TheKeyColumnNotSet_LeavesKeyAttributesEmpty()
    {
        var model = new SystemUserModel { FullName = "Christophe" };

        var entity = model.ToEntity(null);

        Assert.IsEmpty(entity.KeyAttributes);
    }

    [Test]
    public void HandWrittenModel_ToEntity_NoId_PopulatesKeyAttributesFromTheAlternateKey()
    {
        var aadId = Guid.NewGuid();
        var model = new SystemUserManualModel { AzureActiveDirectoryObjectId = aadId };

        var entity = model.ToEntity(null);

        Assert.IsTrue(entity.KeyAttributes.Contains(SystemUserDefinition.Columns.AzureActiveDirectoryObjectId));
        Assert.AreEqual(aadId, entity.KeyAttributes[SystemUserDefinition.Columns.AzureActiveDirectoryObjectId]);
    }

    // ── migrate sync-models round trip ─────────────────────────────────────────
    //
    // PluginTypeRoundtripModel.model / SdkMessageProcessingStepRoundtripModel.model were produced
    // by `migrate sync-models` reflecting over SdkMessageProcessingStepManualModel /
    // PluginTypeManualModel (both hand-written, just above) — proving the round trip compiles and
    // behaves the same as the class it was generated from.

    [Test]
    public void RoundtripModel_EmbedsTheTargetTheSameWayTheHandWrittenOneDid()
    {
        var pluginTypeId = Guid.NewGuid();
        var entity = new Entity(SdkMessageProcessingStepDefinition.EntityName, Guid.NewGuid());
        entity[SdkMessageProcessingStepDefinition.Columns.PluginTypeId] =
            new EntityReference(PluginTypeDefinition.EntityName, pluginTypeId);
        entity["plugintypeid.typename"] = new AliasedValue(
            PluginTypeDefinition.EntityName, PluginTypeDefinition.Columns.TypeName, "Contoso.Plugins.ContactPlugin");

        var model = SdkMessageProcessingStepRoundtripModel.ToBindingModel(entity);

        Assert.IsNotNull(model.PluginType);
        Assert.AreEqual(pluginTypeId, model.PluginType!.Id);
        Assert.AreEqual("Contoso.Plugins.ContactPlugin", model.PluginType.TypeName);
    }
}
