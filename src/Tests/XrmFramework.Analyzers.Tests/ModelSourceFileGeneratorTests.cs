// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using XrmFramework.Analyzers.Generators;

namespace XrmFramework.Analyzers.Tests;

/// <summary>
/// <c>.model</c> + <c>.table</c> in, one complete binding model out: the class, its attributes,
/// its properties, and the <c>ToBindingModel</c> / <c>ToEntity</c> pair.
///
/// The mapping has to be emitted here rather than left to <see cref="MappingSourceGenerator"/>,
/// which only ever sees the compilation's own syntax trees — never a class another generator
/// produced. <see cref="GeneratorInteropTests"/> is what pins that down.
/// </summary>
[TestFixture]
public class ModelSourceFileGeneratorTests
{
    private static string Resource(string fileName)
        => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Resources", fileName));

    private static string GenerateAccountModel(string? modelContent = null)
    {
        var generated = TestHelper.Generate<ModelSourceFileGenerator>(
            ("Model/Definitions/Account.table", Resource("Account.table")),
            ("Model/Definitions/OptionSets.table", Resource("OptionSet.table")),
            ("Model/AccountModel.model", modelContent ?? Resource("AccountModel.model")));

        Assert.That(generated.Keys, Does.Contain("AccountModel.model.cs"),
            "the generator produced: " + string.Join(", ", generated.Keys));

        return generated["AccountModel.model.cs"];
    }

    [Test]
    public void Generates_TheClassWithItsCrmEntityAttribute()
    {
        var source = GenerateAccountModel();

        Assert.That(source, Does.Contain("[CrmEntity(typeof(AccountDefinition))]"));
        Assert.That(source, Does.Contain("public partial class AccountModel : BindingModelBase"));
        Assert.That(source, Does.Contain("namespace Contoso.Core.Model"));
    }

    [Test]
    public void Generates_ThePropertiesWithTheirColumnMapping()
    {
        var source = GenerateAccountModel();

        Assert.That(source, Does.Contain("[CrmMapping(AccountDefinition.Columns.Name)]"));
        Assert.That(source, Does.Contain("[CrmMapping(AccountDefinition.Columns.AccountCategoryCode)]"));
    }

    /// <summary>The half that was missing: the class was generated, the mapping never was.</summary>
    [Test]
    public void Generates_TheMappingMethods()
    {
        var source = GenerateAccountModel();

        Assert.That(source, Does.Contain("public static AccountModel ToBindingModel(Entity entity)"));
        Assert.That(source, Does.Contain("public Entity ToEntity("));
        Assert.That(source, Does.Contain("if (entity.LogicalName != AccountDefinition.EntityName)"));
    }

    [Test]
    public void Mapping_ReadsAndWritesEachMappedColumn()
    {
        var source = GenerateAccountModel();

        Assert.That(source, Does.Contain("entity.GetAttributeValue<string>(AccountDefinition.Columns.Name)"));
        Assert.That(source, Does.Contain("entity[AccountDefinition.Columns.Name]"));
    }

    [Test]
    public void UnknownColumn_IsReportedRatherThanSkipped()
    {
        const string model = """
{
  "tName": "account",
  "Name": "AccountModel",
  "ns": "Contoso.Core.Model",
  "Cols": [ { "Name": "Ghost", "Type": "string", "LogN": "nosuchcolumn" } ]
}
""";

        var diagnostics = TestHelper.Diagnose<ModelSourceFileGenerator>(
            ("Model/Definitions/Account.table", Resource("Account.table")),
            ("Model/Definitions/OptionSets.table", Resource("OptionSet.table")),
            ("Model/AccountModel.model", model));

        Assert.That(diagnostics.Select(d => d.Id), Does.Contain("XRM1006"));
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Column types beyond string and picklist
    // ─────────────────────────────────────────────────────────────────────────

    private static string GenerateContactModel()
    {
        var generated = TestHelper.Generate<ModelSourceFileGenerator>(
            ("Model/Definitions/Contact.table", Resource("Contact.table")),
            ("Model/Definitions/Account.table", Resource("Account.table")),
            ("Model/Definitions/OptionSets.table", Resource("OptionSet.table")),
            ("Model/ContactModel.model", Resource("ContactModel.model")));

        Assert.That(generated.Keys, Does.Contain("ContactModel.model.cs"),
            "the generator produced: " + string.Join(", ", generated.Keys));

        return generated["ContactModel.model.cs"];
    }

    [Test]
    public void DateTime_IsReadAndWrittenAsDateTime()
    {
        var source = GenerateContactModel();

        Assert.That(source, Does.Contain("entity.GetAttributeValue<DateTime?>(ContactDefinition.Columns.BirthDate)"));
        Assert.That(source, Does.Contain("entity[ContactDefinition.Columns.BirthDate]"));
    }

    [Test]
    public void Money_IsUnwrappedOnReadAndRewrappedOnWrite()
    {
        var source = GenerateContactModel();

        Assert.That(source, Does.Contain("entity.GetAttributeValue<Money>(ContactDefinition.Columns.Revenue)?.Value"));
        Assert.That(source, Does.Contain("new Money(Revenue.Value)"));
    }

    [Test]
    public void Lookup_ResolvesItsTargetThroughTheTablesManyToOneRelationship()
    {
        var source = GenerateContactModel();

        Assert.That(source, Does.Contain("entity.GetAttributeValue<EntityReference>(ContactDefinition.Columns.AccountId)"));

        // contact_account points at the account table, which is tracked here, so the target is
        // written as the definition constant rather than a bare literal.
        Assert.That(source, Does.Contain("AccountDefinition.EntityName"));
    }

    [Test]
    public void MultiSelectPicklist_UsesTheOptionSetCollectionHelpers()
    {
        var source = GenerateContactModel();

        Assert.That(source, Does.Contain("entity.GetOptionSetValues<ContactInterest>(ContactDefinition.Columns.Interests)"));
        Assert.That(source, Does.Contain("entity.SetOptionSetValues(ContactDefinition.Columns.Interests, Interests)"));
    }

    [Test]
    public void Boolean_IsMappedAsABoolean()
    {
        var source = GenerateContactModel();

        Assert.That(source, Does.Contain("entity.GetAttributeValue<bool>(ContactDefinition.Columns.IsActive)"));
    }

    /// <summary>
    /// A lookup column the table declares no relationship for cannot name its target, so it is
    /// reported rather than mapped against a guess.
    /// </summary>
    [Test]
    public void LookupWithoutRelationship_IsReported()
    {
        const string model = """
{
  "tName": "contact",
  "Name": "ContactModel",
  "ns": "Contoso.Core.Model",
  "Cols": [ { "Name": "Orphan", "Type": "Guid", "LogN": "orphanlookup" } ]
}
""";

        var diagnostics = TestHelper.Diagnose<ModelSourceFileGenerator>(
            ("Model/Definitions/Contact.table", Resource("Contact.table")),
            ("Model/Definitions/OptionSets.table", Resource("OptionSet.table")),
            ("Model/ContactModel.model", model));

        Assert.That(diagnostics.Select(d => d.Id), Does.Contain("XRM1007"));
    }
}
