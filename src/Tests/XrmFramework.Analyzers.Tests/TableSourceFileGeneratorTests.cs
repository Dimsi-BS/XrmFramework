using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


using XrmFramework.Analyzers.Generators;

namespace XrmFramework.Analyzers.Tests;


[TestFixture]
public class TableSourceFileGeneratorTests
{
    private static byte[] LoadFixture(string fileName)
        => File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", fileName));

    [Test]
    public async Task CalculateTableFiles()
    {
        // No C# user code is needed: TableSourceFileGenerator only consumes AdditionalTexts.
        var source = string.Empty;

        // load all the files from the Resources folder
        
        var files = 
            Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Resources"))
                .Where(f => f.EndsWith(".table"))
                .Select(f => (path: f, content: LoadFixture(f)))
                .ToArray();
        
        
        
        await TestHelper.Verify<TableSourceFileGenerator>(source, files);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // A table declared by both the framework package and the project
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// The framework's copy, as shipped by the package: it selects the columns the framework itself
    /// needs, and declares only the option set behind them.
    /// </summary>
    private const string FrameworkSystemUserTable = """
        {
          "LogName": "systemuser",
          "Name": "SystemUser",
          "CollName": "systemusers",
          "Cols": [
            { "LogName": "systemuserid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
            { "LogName": "accessmode", "Name": "AccessMode", "Type": "Picklist", "EnumName": "systemuser|accessmode", "Select": true },
            { "LogName": "invitestatuscode", "Name": "InviteStatusCode", "Type": "Picklist", "EnumName": "systemuser|invitestatuscode", "Select": false }
          ],
          "Enums": [
            { "LogName": "systemuser|accessmode", "Name": "AccessMode", "Values": [ { "Value": 0, "Name": "ReadWrite" } ] }
          ]
        }
        """;

    /// <summary>
    /// The project's own copy of the same table: it selects one more column and declares the option
    /// set that column references, which the framework's copy knows nothing about.
    /// </summary>
    private const string ProjectSystemUserTable = """
        {
          "LogName": "systemuser",
          "Name": "SystemUser",
          "CollName": "systemusers",
          "Cols": [
            { "LogName": "invitestatuscode", "Name": "InviteStatusCode", "Type": "Picklist", "EnumName": "systemuser|invitestatuscode", "Select": true }
          ],
          "Enums": [
            {
              "LogName": "systemuser|invitestatuscode", "Name": "InviteStatus",
              "Values": [ { "Value": 1, "Name": "InvitationNotSent" }, { "Value": 2, "Name": "InvitationSent" } ]
            }
          ]
        }
        """;

    /// <summary>
    /// Both files describe <c>systemuser</c>, so a single Definition class comes out of them: the
    /// option sets each copy declares alone have to survive the merge, or the columns referencing
    /// them are generated with neither <c>[OptionSet]</c> attribute nor <c>enum</c>.
    /// </summary>
    /// <remarks>
    /// Both orders are covered because only one of them used to break, and it is not the project's
    /// to choose: <c>XrmFramework.props</c> declares the package's <c>.table</c> files as
    /// <c>AdditionalFiles</c> before the project's own.
    /// </remarks>
    [TestCase(true, TestName = "OptionSetsOfBothCopies_AreGenerated_FrameworkFileFirst")]
    [TestCase(false, TestName = "OptionSetsOfBothCopies_AreGenerated_ProjectFileFirst")]
    public void OptionSetsOfBothCopies_AreGenerated(bool frameworkFileFirst)
    {
        var framework = ("Framework/Systemuser.table", FrameworkSystemUserTable);
        var project = ("Model/Definitions/SystemUser.table", ProjectSystemUserTable);

        var generated = TestHelper.Generate<TableSourceFileGenerator>(
            frameworkFileFirst ? new[] { framework, project } : new[] { project, framework });

        var systemUser = generated["SystemUser.table.cs"];

        StringAssert.Contains("[OptionSet(typeof(InviteStatus))]", systemUser,
            "The column selected by the project's copy must carry the option set it references.");
        StringAssert.Contains("public const string InviteStatusCode = \"invitestatuscode\";", systemUser);
        StringAssert.Contains("public enum InviteStatus", systemUser);
        StringAssert.Contains("InvitationSent = 2,", systemUser);

        StringAssert.Contains("[OptionSet(typeof(AccessMode))]", systemUser,
            "Merging must not cost the framework's copy its own option sets either.");
        StringAssert.Contains("public enum AccessMode", systemUser);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Only the relationships a selected lookup stands behind become constants
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// <c>contact</c> selects the <c>accountid</c> lookup, and leaves the <c>parentcustomerid</c> one
    /// aside. Both relationships they carry are declared here.
    /// </summary>
    private const string ContactTable = """
        {
          "LogName": "contact", "Name": "Contact", "CollName": "contacts",
          "Cols": [
            { "LogName": "contactid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
            { "LogName": "accountid", "Name": "AccountId", "Type": "Lookup", "Select": true },
            { "LogName": "parentcustomerid", "Name": "ParentCustomerId", "Type": "Lookup", "Select": false }
          ],
          "NToOne": [
            { "Name": "contact_customer_accounts", "Etn": "account", "NavPropName": "accountid", "LookName": "accountid" },
            { "Name": "account_primary_contact", "Etn": "account", "NavPropName": "parentcustomerid", "LookName": "parentcustomerid" }
          ]
        }
        """;

    /// <summary>
    /// <c>account</c>, seen from the other end: it declares the 1:N of both relationships above, and
    /// selects no lookup of its own.
    /// </summary>
    private const string AccountTable = """
        {
          "LogName": "account", "Name": "Account", "CollName": "accounts",
          "Cols": [
            { "LogName": "accountid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true }
          ],
          "OneToN": [
            { "Name": "contact_customer_accounts", "Etn": "contact", "Role": "Referenced", "NavPropName": "contact_customer_accounts", "LookName": "accountid" },
            { "Name": "account_primary_contact", "Etn": "contact", "Role": "Referenced", "NavPropName": "account_primary_contact", "LookName": "parentcustomerid" }
          ]
        }
        """;

    [Test]
    public void OnlyTheRelationshipsBehindASelectedLookup_AreGenerated()
    {
        var generated = TestHelper.Generate<TableSourceFileGenerator>(
            ("Model/Definitions/Contact.table", ContactTable),
            ("Model/Definitions/Account.table", AccountTable));

        var contact = generated["Contact.table.cs"];
        var account = generated["Account.table.cs"];

        // Selecting contact.accountid produces the two ends of the relationship it carries.
        StringAssert.Contains(
            "[Relationship(AccountDefinition.EntityName, EntityRole.Referencing, \"accountid\", ContactDefinition.Columns.AccountId)]",
            contact, "The N:1 the selected lookup carries must reach the definition.");
        StringAssert.Contains("public const string contact_customer_accounts = \"contact_customer_accounts\";", contact);

        StringAssert.Contains(
            "[Relationship(ContactDefinition.EntityName, EntityRole.Referenced, \"contact_customer_accounts\", ContactDefinition.Columns.AccountId)]",
            account, "The account reaches its contacts through the very lookup contact selects.");
        StringAssert.Contains("public const string contact_customer_accounts = \"contact_customer_accounts\";", account);

        // parentcustomerid is not selected: neither end of its relationship is anybody's business.
        StringAssert.DoesNotContain("account_primary_contact", contact);
        StringAssert.DoesNotContain("account_primary_contact", account);
    }

    [Test]
    public void ATableWhoseLookupsAreAllUnselected_HasNoRelationshipClassAtAll()
    {
        var generated = TestHelper.Generate<TableSourceFileGenerator>(
            ("Model/Definitions/Account.table", AccountTable));

        // Contact is out of the compilation: nothing selects the lookups the 1:N rest on.
        StringAssert.DoesNotContain("OneToManyRelationships", generated["Account.table.cs"]);
    }

    /// <summary>
    /// The framework's copy of <c>systemuser</c>, which selects the lookup, and the project's copy,
    /// which declares the relationship behind it — neither file holds both.
    /// </summary>
    private const string FrameworkSystemUserLookupTable = """
        {
          "LogName": "systemuser", "Name": "SystemUser", "CollName": "systemusers",
          "Cols": [
            { "LogName": "systemuserid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
            { "LogName": "businessunitid", "Name": "BusinessUnitId", "Type": "Lookup", "Select": true }
          ]
        }
        """;

    private const string ProjectSystemUserRelationTable = """
        {
          "LogName": "systemuser", "Name": "SystemUser", "CollName": "systemusers",
          "NToOne": [
            { "Name": "business_unit_system_users", "Etn": "businessunit", "NavPropName": "businessunitid", "LookName": "businessunitid" }
          ]
        }
        """;

    /// <summary>
    /// The relationships to choose from are those of the merged table: a table declared by both the
    /// framework package and the project must answer for what either copy brings.
    /// </summary>
    [TestCase(true, TestName = "RelationshipsOfBothCopies_AreSelected_FrameworkFileFirst")]
    [TestCase(false, TestName = "RelationshipsOfBothCopies_AreSelected_ProjectFileFirst")]
    public void RelationshipsOfBothCopies_AreSelected(bool frameworkFileFirst)
    {
        var framework = ("Framework/Systemuser.table", FrameworkSystemUserLookupTable);
        var project = ("Model/Definitions/SystemUser.table", ProjectSystemUserRelationTable);

        var generated = TestHelper.Generate<TableSourceFileGenerator>(
            frameworkFileFirst ? new[] { framework, project } : new[] { project, framework });

        StringAssert.Contains("public const string business_unit_system_users = \"business_unit_system_users\";",
            generated["SystemUser.table.cs"],
            "The lookup selected by one copy must find the relationship declared by the other.");
    }
}
