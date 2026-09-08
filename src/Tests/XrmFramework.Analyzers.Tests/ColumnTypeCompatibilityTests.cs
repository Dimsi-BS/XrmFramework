// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using XrmFramework.Analyzers.Generators;

namespace XrmFramework.Analyzers.Tests;

/// <summary>
/// XRM1009 — the C# type a <c>.model</c> gives a property against the type of its column.
///
/// The emitter branches on the property's type name, so a mismatch does not fail: it falls
/// through to the generic path and emits code that compiles and reads the wrong thing. An
/// <c>int</c> on a Money column emits <c>GetAttributeValue&lt;int&gt;</c>, which returns zero
/// forever because the attribute holds a Money.
/// </summary>
[TestFixture]
public class ColumnTypeCompatibilityTests
{
    private static string Resource(string fileName)
        => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Resources", fileName));

    /// <summary>Builds a one-property model over Contact.table and returns what was reported.</summary>
    private static string[] DiagnoseProperty(string propertyName, string csharpType, string columnLogicalName)
    {
        var model = $$"""
{
  "tName": "contact",
  "Name": "ContactModel",
  "ns": "Contoso.Core.Model",
  "Cols": [ { "Name": "{{propertyName}}", "Type": "{{csharpType}}", "LogN": "{{columnLogicalName}}" } ]
}
""";

        return TestHelper.Diagnose<ModelSourceFileGenerator>(
                ("Model/Definitions/Contact.table", Resource("Contact.table")),
                ("Model/Definitions/Account.table", Resource("Account.table")),
                ("Model/Definitions/OptionSets.table", Resource("OptionSet.table")),
                ("Model/ContactModel.model", model))
            .Select(d => d.Id)
            .ToArray();
    }

    // ── Mismatches ────────────────────────────────────────────────────────────

    [TestCase("Revenue", "int", "revenue", TestName = "Money read as int")]
    [TestCase("Revenue", "string", "revenue", TestName = "Money read as string")]
    [TestCase("BirthDate", "string", "birthdate", TestName = "DateTime read as string")]
    [TestCase("IsActive", "int", "isactive", TestName = "Boolean read as int")]
    [TestCase("FullName", "int", "fullname", TestName = "String read as int")]
    [TestCase("AccountId", "string", "accountid", TestName = "Lookup read as string")]
    [TestCase("StatusCode", "string", "statuscode", TestName = "Status read as string")]
    [TestCase("Id", "string", "contactid", TestName = "Uniqueidentifier read as string")]
    public void IncompatibleType_IsReported(string property, string csharpType, string column)
    {
        Assert.That(DiagnoseProperty(property, csharpType, column), Does.Contain("XRM1009"));
    }

    /// <summary>A multi-select read into a single value keeps one option and drops the rest.</summary>
    [Test]
    public void MultiSelectColumn_MappedToASingleValue_IsReported()
    {
        Assert.That(DiagnoseProperty("Interests", "ContactInterest", "interests"), Does.Contain("XRM1009"));
    }

    // ── Accepted ──────────────────────────────────────────────────────────────

    [TestCase("Revenue", "decimal?", "revenue", TestName = "Money as nullable decimal")]
    [TestCase("Revenue", "decimal", "revenue", TestName = "Money as decimal")]
    [TestCase("Revenue", "Money", "revenue", TestName = "Money as Money")]
    [TestCase("BirthDate", "DateTime?", "birthdate", TestName = "DateTime as nullable DateTime")]
    [TestCase("IsActive", "bool", "isactive", TestName = "Boolean as bool")]
    [TestCase("FullName", "string", "fullname", TestName = "String as string")]
    [TestCase("AccountId", "Guid", "accountid", TestName = "Lookup as Guid")]
    [TestCase("AccountId", "Guid?", "accountid", TestName = "Lookup as nullable Guid")]
    [TestCase("AccountId", "EntityReference", "accountid", TestName = "Lookup as EntityReference")]
    [TestCase("StatusCode", "ContactStatus", "statuscode", TestName = "Status as its enum")]
    [TestCase("StatusCode", "ContactStatus?", "statuscode", TestName = "Status as nullable enum")]
    [TestCase("StatusCode", "int?", "statuscode", TestName = "Status as nullable int")]
    [TestCase("StatusCode", "OptionSetValue", "statuscode", TestName = "Status as OptionSetValue")]
    [TestCase("Id", "Guid", "contactid", TestName = "Uniqueidentifier as Guid")]
    [TestCase("Interests", "List<ContactInterest>", "interests", TestName = "Multi-select as a list of its enum")]
    public void CompatibleType_IsAccepted(string property, string csharpType, string column)
    {
        Assert.That(DiagnoseProperty(property, csharpType, column), Does.Not.Contain("XRM1009"),
            $"{csharpType} is a legitimate mapping for {column} and must not be reported");
    }

    /// <summary>
    /// The warning must not stop the property being mapped — that is the whole difference between
    /// it and the errors around it.
    /// </summary>
    [Test]
    public void IncompatibleType_StillGeneratesTheProperty()
    {
        var model = """
{
  "tName": "contact",
  "Name": "ContactModel",
  "ns": "Contoso.Core.Model",
  "Cols": [ { "Name": "Revenue", "Type": "int", "LogN": "revenue" } ]
}
""";

        var generated = TestHelper.Generate<ModelSourceFileGenerator>(
            ("Model/Definitions/Contact.table", Resource("Contact.table")),
            ("Model/Definitions/OptionSets.table", Resource("OptionSet.table")),
            ("Model/ContactModel.model", model));

        Assert.That(generated["ContactModel.model.cs"], Does.Contain("Revenue"));
    }
}
