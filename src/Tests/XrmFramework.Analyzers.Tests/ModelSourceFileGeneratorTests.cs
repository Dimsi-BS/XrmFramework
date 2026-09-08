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
    //  Attrs / Usings — attributes the emitter has no dedicated field for
    // ─────────────────────────────────────────────────────────────────────────

    [Test]
    public void Attrs_AreEmittedVerbatimOnTheProperty_WithUsingsInScope()
    {
        const string model = """
{
  "tName": "account",
  "Name": "AccountModel",
  "ns": "Contoso.Core.Model",
  "Usings": [ "System.ComponentModel.DataAnnotations", "System.Runtime.Serialization" ],
  "Cols": [
    {
      "Name": "Name",
      "Type": "string",
      "LogN": "name",
      "Attrs": [ "StringLength(100)", "DataMember(Name = \"Nom\")" ]
    }
  ]
}
""";

        var source = GenerateAccountModel(model);

        Assert.That(source, Does.Contain("using System.ComponentModel.DataAnnotations;"));
        Assert.That(source, Does.Contain("using System.Runtime.Serialization;"));
        Assert.That(source, Does.Contain("[StringLength(100)]"));
        Assert.That(source, Does.Contain("[DataMember(Name = \"Nom\")]"));
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

    // ─────────────────────────────────────────────────────────────────────────
    //  ExtendBindingModel — nesting another .model over the same record
    // ─────────────────────────────────────────────────────────────────────────

    private static string GenerateAccountWithCategoryModel()
    {
        var generated = TestHelper.Generate<ModelSourceFileGenerator>(
            ("Model/Definitions/Account.table", Resource("Account.table")),
            ("Model/Definitions/OptionSets.table", Resource("OptionSet.table")),
            ("Model/AccountCategoryModel.model", Resource("AccountCategoryModel.model")),
            ("Model/AccountWithCategoryModel.model", Resource("AccountWithCategoryModel.model")));

        Assert.That(generated.Keys, Does.Contain("AccountWithCategoryModel.model.cs"),
            "the generator produced: " + string.Join(", ", generated.Keys));

        return generated["AccountWithCategoryModel.model.cs"];
    }

    [Test]
    public void ExtendBindingModel_DeclaresThePropertyWithNoColumnMapping()
    {
        var source = GenerateAccountWithCategoryModel();

        Assert.That(source, Does.Contain("[ExtendBindingModel]"));
        Assert.That(source, Does.Contain("[JsonProperty(\"category\")]"));
        Assert.That(source, Does.Contain("public AccountCategoryModel CategoryInfo { get; set; }"));

        // No [CrmMapping] is written for it — it maps no column of its own.
        Assert.That(source, Does.Not.Contain("[CrmMapping(AccountDefinition.Columns.CategoryInfo"));
    }

    [Test]
    public void ExtendBindingModel_FillsAndMergesTheExtensionFromTheSameRecord()
    {
        var source = GenerateAccountWithCategoryModel();

        Assert.That(source, Does.Contain("model.CategoryInfo = AccountCategoryModel.ToBindingModel(entity);"));
        Assert.That(source, Does.Contain("entity.MergeWith(CategoryInfo?.ToEntity(service));"));
    }

    /// <summary>
    /// Both halves of an extension are filled from one row, so a model targeting a different
    /// table has nothing to fill it from.
    /// </summary>
    [Test]
    public void ExtendBindingModel_TargetingAnotherTable_IsReported()
    {
        const string parent = """
{
  "tName": "account",
  "Name": "AccountWithContactModel",
  "ns": "Contoso.Core.Model",
  "Cols": [ { "Name": "Contact", "Type": "ContactModel", "ExtendBindingModel": true } ]
}
""";

        var diagnostics = TestHelper.Diagnose<ModelSourceFileGenerator>(
            ("Model/Definitions/Account.table", Resource("Account.table")),
            ("Model/Definitions/Contact.table", Resource("Contact.table")),
            ("Model/Definitions/OptionSets.table", Resource("OptionSet.table")),
            ("Model/ContactModel.model", Resource("ContactModel.model")),
            ("Model/AccountWithContactModel.model", parent));

        Assert.That(diagnostics.Select(d => d.Id), Does.Contain("XRM1011"));
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  ChildRelationship — a one-to-many relationship as a List<T> property
    // ─────────────────────────────────────────────────────────────────────────

    private static string GenerateAccountWithContactsModel()
    {
        var generated = TestHelper.Generate<ModelSourceFileGenerator>(
            ("Model/Definitions/Account.table", Resource("Account.table")),
            ("Model/Definitions/Contact.table", Resource("Contact.table")),
            ("Model/Definitions/OptionSets.table", Resource("OptionSet.table")),
            ("Model/ContactModel.model", Resource("ContactModel.model")),
            ("Model/AccountWithContactsModel.model", Resource("AccountWithContactsModel.model")));

        Assert.That(generated.Keys, Does.Contain("AccountWithContactsModel.model.cs"),
            "the generator produced: " + string.Join(", ", generated.Keys));

        return generated["AccountWithContactsModel.model.cs"];
    }

    [Test]
    public void ChildRelationship_DeclaresAnInitializedListPropertyWithNoColumnMapping()
    {
        var source = GenerateAccountWithContactsModel();

        Assert.That(source, Does.Contain("[ChildRelationship(AccountDefinition.OneToManyRelationships.contact_account)]"));
        Assert.That(source, Does.Contain("public List<ContactModel> Contacts { get; set; } = new List<ContactModel>();"));

        // No [CrmMapping] is written for it — it maps no column of its own.
        Assert.That(source, Does.Not.Contain("[CrmMapping(AccountDefinition.Columns.Contacts"));
    }

    [Test]
    public void ChildRelationship_ReadsRelatedEntitiesByTheRelationshipSchemaName()
    {
        var source = GenerateAccountWithContactsModel();

        Assert.That(source, Does.Contain(
            "entity.RelatedEntities.FirstOrDefault(r => r.Key.SchemaName == AccountDefinition.OneToManyRelationships.contact_account)"));
        Assert.That(source, Does.Contain("var relatedModel = ContactModel.ToBindingModel(relatedEntity);"));
        Assert.That(source, Does.Contain("model.Contacts.Add(relatedModel);"));
    }

    [Test]
    public void ChildRelationship_WritesItsItemsAsRelatedEntities()
    {
        var source = GenerateAccountWithContactsModel();

        Assert.That(source, Does.Contain("if (Contacts != null)"));
        Assert.That(source, Does.Contain("relatedCollection.Entities.Add(item.ToEntity(service));"));
        Assert.That(source, Does.Contain(
            "entity.RelatedEntities[new Microsoft.Xrm.Sdk.Relationship(AccountDefinition.OneToManyRelationships.contact_account) "
            + "{ PrimaryEntityRole = Microsoft.Xrm.Sdk.EntityRole.Referenced }] = relatedCollection;"));
    }

    /// <summary>
    /// A relationship property is a collection of related records, not a single value: it has to
    /// be declared as <c>List&lt;T&gt;</c>, the same way a multi-select column already is.
    /// </summary>
    [Test]
    public void ChildRelationship_NotDeclaredAsAList_IsReported()
    {
        const string model = """
{
  "tName": "account",
  "Name": "AccountWithContactsModel",
  "ns": "Contoso.Core.Model",
  "Cols": [ { "Name": "Contacts", "Type": "ContactModel", "LogN": "contact_account" } ]
}
""";

        var diagnostics = TestHelper.Diagnose<ModelSourceFileGenerator>(
            ("Model/Definitions/Account.table", Resource("Account.table")),
            ("Model/Definitions/Contact.table", Resource("Contact.table")),
            ("Model/Definitions/OptionSets.table", Resource("OptionSet.table")),
            ("Model/ContactModel.model", Resource("ContactModel.model")),
            ("Model/AccountWithContactsModel.model", model));

        Assert.That(diagnostics.Select(d => d.Id), Does.Contain("XRM1006"));
    }
}
