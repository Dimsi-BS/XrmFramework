// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;
using XrmFramework.Analyzers.Generators;

namespace XrmFramework.Analyzers.Tests;

/// <summary>
/// A generated <c>ToEntity</c> populating <c>entity.KeyAttributes</c> from a <c>.table</c>'s
/// alternate keys, for an upsert-by-key request to have something to key on when nothing set a
/// real <c>Id</c>.
///
/// No <c>.table</c> already in the test resources declares an alternate key, so the table here is
/// written inline — a <c>contact</c>-shaped table with a single-column key (email) and a two-column
/// one (first + last name), to exercise the smallest-wins tie-break.
/// </summary>
[TestFixture]
public class AlternateKeyMappingTests
{
    private const string ContactTable = """
{
  "LogName": "contact",
  "Name": "Contact",
  "CollName": "contacts",
  "Cols": [
    { "LogName": "contactid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
    { "LogName": "emailaddress1", "Name": "Email", "Type": "String", "Select": true },
    { "LogName": "firstname", "Name": "FirstName", "Type": "String", "Select": true },
    { "LogName": "lastname", "Name": "LastName", "Type": "String", "Select": true }
  ],
  "Keys": [
    { "LogicalName": "email_key", "Name": "EmailKey", "FieldNames": [ "emailaddress1" ] },
    { "LogicalName": "name_key", "Name": "NameKey", "FieldNames": [ "firstname", "lastname" ] }
  ]
}
""";

    private const string OptionSets = """{ "LogName": "globalEnums", "Name": "OptionSets", "Cols": [] }""";

    private static string Model(string properties) => $$"""
{
  "tName": "contact",
  "Name": "ContactModel",
  "ns": "Contoso.Core.Model",
  "Cols": [ {{properties}} ]
}
""";

    private static string Generate(string properties)
        => TestHelper.Generate<ModelSourceFileGenerator>(
                ("Definitions/Contact.table", ContactTable),
                ("Definitions/OptionSets.table", OptionSets),
                ("Model/ContactModel.model", Model(properties)))
            ["ContactModel.model.cs"];

    private const string AllThreeColumns = """
{ "Name": "Email", "Type": "string", "LogN": "emailaddress1" },
{ "Name": "FirstName", "Type": "string", "LogN": "firstname" },
{ "Name": "LastName", "Type": "string", "LogN": "lastname" }
""";

    [Test]
    public void ToEntity_DeclaresBothAlternateKeysInTableOrder()
    {
        var code = Generate(AllThreeColumns);

        Assert.That(code, Does.Contain("var key0 = new[] { \"emailaddress1\" };"));
        Assert.That(code, Does.Contain("var key1 = new[] { \"firstname\", \"lastname\" };"));
    }

    [Test]
    public void ToEntity_OnlyResolvesTheKey_WhenIdIsEmpty()
    {
        var code = Generate(AllThreeColumns);

        Assert.That(code, Does.Contain("if (entity.Id == Guid.Empty)"));
    }

    /// <summary>A model mapping no column at all emits no key-resolution block.</summary>
    [Test]
    public void NoMappedColumns_EmitsNoAlternateKeyResolution()
    {
        var code = Generate("");

        Assert.That(code, Does.Not.Contain("smallestKey"));
    }

    /// <summary>
    /// A model mapping only the single-column key's column has nothing that could ever satisfy the
    /// two-column one — resolving it would be dead code, so only the satisfiable key is emitted.
    /// </summary>
    [Test]
    public void PartialCoverage_EmitsOnlyTheSatisfiableKey()
    {
        var code = Generate("""{ "Name": "Email", "Type": "string", "LogN": "emailaddress1" }""");

        Assert.That(code, Does.Contain("var key0 = new[] { \"emailaddress1\" };"));
        Assert.That(code, Does.Not.Contain("firstname"));
    }
}
