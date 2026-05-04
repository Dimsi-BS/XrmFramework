// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;

// ReSharper disable once CheckNamespace
namespace XrmFramework.Tests.Plugin;

/// <summary>
/// Tests unitaires pour <see cref="CustomApiArgumentTypeMapper"/>.
/// </summary>
[TestFixture]
public class CustomApiArgumentTypeMapperTests
{
    // ──────────────────────────────────────────────
    //  Types connus → mapping correct
    // ──────────────────────────────────────────────

    [Test]
    public void TryMap_Bool_ReturnsBooleanAndTrue()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(bool), out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(CustomApiArgumentType.Boolean, result);
    }

    [Test]
    public void TryMap_DateTime_ReturnsDateTimeAndTrue()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(DateTime), out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(CustomApiArgumentType.DateTime, result);
    }

    [Test]
    public void TryMap_Decimal_ReturnsDecimalAndTrue()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(decimal), out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(CustomApiArgumentType.Decimal, result);
    }

    [Test]
    public void TryMap_Entity_ReturnsEntityAndTrue()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(Entity), out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(CustomApiArgumentType.Entity, result);
    }

    [Test]
    public void TryMap_EntityCollection_ReturnsEntityCollectionAndTrue()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(EntityCollection), out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(CustomApiArgumentType.EntityCollection, result);
    }

    [Test]
    public void TryMap_EntityReference_ReturnsEntityReferenceAndTrue()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(EntityReference), out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(CustomApiArgumentType.EntityReference, result);
    }

    [Test]
    public void TryMap_Float_ReturnsFloatAndTrue()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(float), out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(CustomApiArgumentType.Float, result);
    }

    [Test]
    public void TryMap_Int_ReturnsIntegerAndTrue()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(int), out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(CustomApiArgumentType.Integer, result);
    }

    [Test]
    public void TryMap_Money_ReturnsMoneyAndTrue()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(Money), out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(CustomApiArgumentType.Money, result);
    }

    [Test]
    public void TryMap_OptionSetValue_ReturnsPicklistAndTrue()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(OptionSetValue), out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(CustomApiArgumentType.Picklist, result);
    }

    [Test]
    public void TryMap_String_ReturnsStringAndTrue()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(string), out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(CustomApiArgumentType.String, result);
    }

    [Test]
    public void TryMap_StringArray_ReturnsStringArrayAndTrue()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(string[]), out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(CustomApiArgumentType.StringArray, result);
    }

    [Test]
    public void TryMap_Guid_ReturnsGuidAndTrue()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(Guid), out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(CustomApiArgumentType.Guid, result);
    }

    // ──────────────────────────────────────────────
    //  Enum → Picklist (cas spécial)
    // ──────────────────────────────────────────────

    [Test]
    public void TryMap_EnumType_ReturnsPicklistAndTrue()
    {
        // Any enum should be treated as Picklist
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(DayOfWeek), out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(CustomApiArgumentType.Picklist, result);
    }

    [Test]
    public void TryMap_CustomEnum_ReturnsPicklistAndTrue()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(CustomApiBindingType), out var result);

        Assert.IsTrue(found);
        Assert.AreEqual(CustomApiArgumentType.Picklist, result);
    }

    // ──────────────────────────────────────────────
    //  Type inconnu → String + retour false
    // ──────────────────────────────────────────────

    [Test]
    public void TryMap_UnknownType_ReturnsFalseAndDefaultsToString()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(Uri), out var result);

        Assert.IsFalse(found);
        Assert.AreEqual(CustomApiArgumentType.String, result);
    }

    [Test]
    public void TryMap_ObjectType_ReturnsFalseAndDefaultsToString()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(object), out var result);

        Assert.IsFalse(found);
        Assert.AreEqual(CustomApiArgumentType.String, result);
    }

    [Test]
    public void TryMap_ListOfString_ReturnsFalseAndDefaultsToString()
    {
        var found = CustomApiArgumentTypeMapper.TryMap(typeof(System.Collections.Generic.List<string>), out var result);

        Assert.IsFalse(found);
        Assert.AreEqual(CustomApiArgumentType.String, result);
    }
}
