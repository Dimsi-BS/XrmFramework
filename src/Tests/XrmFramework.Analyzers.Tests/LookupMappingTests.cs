// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using NUnit.Framework;
using XrmFramework.Analyzers.Generators;

namespace XrmFramework.Analyzers.Tests;

/// <summary>
/// Lookups in a <c>.model</c>: which table a polymorphic one reaches, and the projection of a
/// column of the targeted record.
///
/// No <c>.table</c> in the repository declares a polymorphic lookup, so the tables here are
/// written inline — an <c>incident</c> whose <c>customerid</c> reaches both <c>account</c> and
/// <c>contact</c>, which is what the platform actually produces for a Customer column.
/// </summary>
[TestFixture]
public class LookupMappingTests
{
    private const string IncidentTable = """
{
  "LogName": "incident",
  "Name": "Incident",
  "CollName": "incidents",
  "Cols": [
    { "LogName": "incidentid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
    { "LogName": "title", "Name": "Title", "Type": "String", "Select": true },
    { "LogName": "customerid", "Name": "CustomerId", "Type": "Customer", "Select": true },
    { "LogName": "ownerid", "Name": "OwnerId", "Type": "Owner", "Select": true }
  ],
  "NToOne": [
    { "Name": "incident_customer_accounts", "Etn": "account", "LookName": "customerid", "NavPropName": "customerid_account" },
    { "Name": "incident_customer_contacts", "Etn": "contact", "LookName": "customerid", "NavPropName": "customerid_contact" },
    { "Name": "incident_owning_user", "Etn": "systemuser", "LookName": "ownerid", "NavPropName": "owninguser" }
  ]
}
""";

    private const string AccountTable = """
{
  "LogName": "account",
  "Name": "Account",
  "CollName": "accounts",
  "Cols": [
    { "LogName": "accountid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
    { "LogName": "name", "Name": "Name", "Type": "String", "PrimaryType": "Name", "Select": true },
    { "LogName": "revenue", "Name": "Revenue", "Type": "Money", "Select": true }
  ]
}
""";

    private const string ContactTable = """
{
  "LogName": "contact",
  "Name": "Contact",
  "CollName": "contacts",
  "Cols": [
    { "LogName": "contactid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
    { "LogName": "fullname", "Name": "FullName", "Type": "String", "Select": true }
  ]
}
""";

    private const string OptionSets = """{ "LogName": "globalEnums", "Name": "OptionSets", "Cols": [] }""";

    private static string Model(string properties) => $$"""
{
  "tName": "incident",
  "Name": "IncidentModel",
  "ns": "Contoso.Core.Model",
  "Cols": [ {{properties}} ]
}
""";

    private static string[] Diagnose(string properties)
        => TestHelper.Diagnose<ModelSourceFileGenerator>(
                ("Definitions/Incident.table", IncidentTable),
                ("Definitions/Account.table", AccountTable),
                ("Definitions/Contact.table", ContactTable),
                ("Definitions/OptionSets.table", OptionSets),
                ("Model/IncidentModel.model", Model(properties)))
            .Select(d => d.Id)
            .ToArray();

    private static string Generate(string properties)
        => TestHelper.Generate<ModelSourceFileGenerator>(
                ("Definitions/Incident.table", IncidentTable),
                ("Definitions/Account.table", AccountTable),
                ("Definitions/Contact.table", ContactTable),
                ("Definitions/OptionSets.table", OptionSets),
                ("Model/IncidentModel.model", Model(properties)))
            ["IncidentModel.model.cs"];

    // ── Polymorphic lookups ───────────────────────────────────────────────────

    /// <summary>
    /// customerid reaches account and contact. Choosing one arbitrarily would emit an
    /// EntityReference naming the wrong table for half the records.
    /// </summary>
    [Test]
    public void PolymorphicLookup_WithoutATarget_IsReported()
    {
        var diagnostics = Diagnose("""{ "Name": "Customer", "Type": "Guid?", "LogN": "customerid" }""");

        Assert.That(diagnostics, Does.Contain("XRM1010"));
    }

    [Test]
    public void PolymorphicLookup_WithATarget_NamesThatTable()
    {
        var code = Generate("""
{ "Name": "Customer", "Type": "Guid?", "LogN": "customerid", "LookupTargetTableLogicalName": "contact" }
""");

        Assert.That(code, Does.Contain("ContactDefinition.EntityName"));
        Assert.That(code, Does.Not.Contain("AccountDefinition.EntityName"));
    }

    [Test]
    public void LookupTarget_TheColumnDoesNotReach_IsReported()
    {
        var diagnostics = Diagnose("""
{ "Name": "Customer", "Type": "Guid?", "LogN": "customerid", "LookupTargetTableLogicalName": "systemuser" }
""");

        Assert.That(diagnostics, Does.Contain("XRM1010"));
    }

    /// <summary>A lookup reaching a single table needs no target and must not be reported.</summary>
    [Test]
    public void SingleTargetLookup_NeedsNoTarget()
    {
        var diagnostics = Diagnose("""{ "Name": "OwnerId", "Type": "Guid?", "LogN": "ownerid" }""");

        Assert.That(diagnostics, Does.Not.Contain("XRM1010"));
    }

    // ── Projections through a lookup ──────────────────────────────────────────

    /// <summary>
    /// The pattern behind [CrmMapping(lookupColumn)] + [CrmLookup(target, targetColumn)]: the
    /// declared type is that of the projected column, not of the lookup. Checking it against the
    /// lookup reported every such property as expecting a Guid.
    /// </summary>
    [Test]
    public void ProjectedColumn_IsCheckedAgainstTheProjectedColumn()
    {
        var diagnostics = Diagnose("""
{ "Name": "CustomerName", "Type": "string", "LogN": "customerid",
  "LookupTargetTableLogicalName": "account", "LookupTargetColumnLogicalName": "name" }
""");

        Assert.That(diagnostics, Does.Not.Contain("XRM1009"),
            "a string projecting account.name is correct; only the lookup column is a Guid");
    }

    [Test]
    public void ProjectedColumn_WithTheWrongType_IsStillReported()
    {
        var diagnostics = Diagnose("""
{ "Name": "CustomerName", "Type": "int", "LogN": "customerid",
  "LookupTargetTableLogicalName": "account", "LookupTargetColumnLogicalName": "name" }
""");

        Assert.That(diagnostics, Does.Contain("XRM1009"),
            "account.name is a String, so an int projection is still a mistake");
    }

    [Test]
    public void ProjectedMoneyColumn_AsDecimal_IsAccepted()
    {
        var diagnostics = Diagnose("""
{ "Name": "CustomerRevenue", "Type": "decimal?", "LogN": "customerid",
  "LookupTargetTableLogicalName": "account", "LookupTargetColumnLogicalName": "revenue" }
""");

        Assert.That(diagnostics, Does.Not.Contain("XRM1009"));
    }

    /// <summary>A related model carries no column type at all, so nothing is checked.</summary>
    [Test]
    public void RelatedModel_IsNotTypeChecked()
    {
        var diagnostics = Diagnose("""
{ "Name": "Customer", "Type": "AccountModel", "LogN": "customerid",
  "LookupTargetTableLogicalName": "account", "LookupTargetModel": "Contoso.Core.Model.AccountModel" }
""");

        Assert.That(diagnostics, Does.Not.Contain("XRM1009"));
    }

    // ── What the generated class declares ─────────────────────────────────────

    [Test]
    public void ProjectedColumn_EmitsTheCrmLookupAttribute()
    {
        var code = Generate("""
{ "Name": "CustomerName", "Type": "string", "LogN": "customerid",
  "LookupTargetTableLogicalName": "account", "LookupTargetColumnLogicalName": "name" }
""");

        Assert.That(code, Does.Contain("[CrmLookup(AccountDefinition.EntityName, AccountDefinition.Columns.Name)]"));
    }

    [Test]
    public void AllowNotExisting_IsCarriedToTheAttribute()
    {
        var code = Generate("""
{ "Name": "CustomerName", "Type": "string", "LogN": "customerid", "AllowNotExisting": true,
  "LookupTargetTableLogicalName": "account", "LookupTargetColumnLogicalName": "name" }
""");

        Assert.That(code, Does.Contain("AccountDefinition.Columns.Name, true)]"));
    }

    /// <summary>A lookup reaching one table and projecting nothing needs no attribute at all.</summary>
    [Test]
    public void PlainSingleTargetLookup_EmitsNoCrmLookup()
    {
        var code = Generate("""{ "Name": "OwnerId", "Type": "Guid?", "LogN": "ownerid" }""");

        Assert.That(code, Does.Not.Contain("[CrmLookup("));
    }

    [Test]
    public void FollowLink_IsCarriedToTheMapping()
    {
        var code = Generate("""
{ "Name": "CustomerName", "Type": "string", "LogN": "customerid", "FollowLink": true,
  "LookupTargetTableLogicalName": "account", "LookupTargetColumnLogicalName": "name" }
""");

        Assert.That(code, Does.Contain("FollowLink = true"));
    }

    [Test]
    public void JsonIgnore_EmitsTheAttribute()
    {
        var code = Generate("""{ "Name": "Title", "Type": "string", "LogN": "title", "JsonIgnore": true }""");

        Assert.That(code, Does.Contain("[JsonIgnore]"));
    }

    /// <summary>The property carries the related model, not the lookup's Guid.</summary>
    [Test]
    public void RelatedModel_BecomesThePropertyType()
    {
        var code = Generate("""
{ "Name": "Customer", "Type": "Guid?", "LogN": "customerid",
  "LookupTargetTableLogicalName": "account", "LookupTargetModel": "Contoso.Core.Model.AccountModel" }
""");

        Assert.That(code, Does.Contain("Contoso.Core.Model.AccountModel Customer"));
        Assert.That(code, Does.Not.Contain("Guid? Customer"));
    }
}
