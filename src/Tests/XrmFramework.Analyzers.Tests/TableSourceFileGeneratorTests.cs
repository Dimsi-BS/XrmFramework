using Microsoft.CodeAnalysis;
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
    // Which option sets become enums
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// <c>account</c> and <c>contact</c> each carry their own <c>preferredappointmenttimecode</c>, and
    /// the DefinitionManager names both <c>HeurePrivilegiee</c> — it derives the name from the label,
    /// which the two share. Only <c>contact</c> selects the column.
    /// </summary>
    /// <remarks>
    /// Tables are walked in name order, so <c>account</c> comes first: it used to claim the name and
    /// then decline to emit anything, and the enum <c>contact</c> would have declared never reached
    /// the compilation. Its own generated file typed a column on it all the same.
    /// </remarks>
    [Test]
    public void AnEnumSkippedByOneTable_IsStillDeclaredByTheTableSelectingIt()
    {
        const string account = """
            {
              "LogName": "account", "Name": "Account", "CollName": "accounts",
              "Cols": [
                { "LogName": "accountid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
                { "LogName": "preferredappointmenttimecode", "Name": "PreferredAppointmentTimeCode", "Type": "Picklist", "EnumName": "account|preferredappointmenttimecode", "Select": false }
              ],
              "Enums": [
                { "LogName": "account|preferredappointmenttimecode", "Name": "HeurePrivilegiee", "Values": [ { "Value": 1, "Name": "Matin" } ] }
              ]
            }
            """;

        const string contact = """
            {
              "LogName": "contact", "Name": "Contact", "CollName": "contacts",
              "Cols": [
                { "LogName": "contactid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
                { "LogName": "preferredappointmenttimecode", "Name": "PreferredAppointmentTimeCode", "Type": "Picklist", "EnumName": "contact|preferredappointmenttimecode", "Select": true }
              ],
              "Enums": [
                { "LogName": "contact|preferredappointmenttimecode", "Name": "HeurePrivilegiee", "Values": [ { "Value": 1, "Name": "Matin" }, { "Value": 2, "Name": "ApresMidi" } ] }
              ]
            }
            """;

        var generated = TestHelper.Generate<TableSourceFileGenerator>(
            ("Model/Definitions/Account.table", account),
            ("Model/Definitions/Contact.table", contact));

        StringAssert.Contains("[OptionSet(typeof(HeurePrivilegiee))]", generated["Contact.table.cs"]);
        StringAssert.Contains("public enum HeurePrivilegiee", generated["Contact.table.cs"],
            "The table selecting the column must declare the enum its own file types a column on.");
        StringAssert.Contains("ApresMidi = 2,", generated["Contact.table.cs"]);

        // account selects nothing carrying it: its own copy is nobody's business.
        StringAssert.DoesNotContain("public enum HeurePrivilegiee", generated["Account.table.cs"]);
    }

    /// <summary>
    /// A global option set is declared by the table listing it, on behalf of every table whose
    /// selected columns carry it — here <c>connectionrole</c>, which declares none of its own.
    /// </summary>
    [Test]
    public void AGlobalOptionSet_IsDeclaredForTheTablesCarryingIt()
    {
        const string globals = """
            {
              "LogName": "globalEnums", "Name": "OptionSet", "Cols": [],
              "Enums": [
                { "LogName": "connectionrole_category", "Name": "Categorie", "IsGlobal": true, "Values": [ { "Value": 1, "Name": "Business" } ] }
              ]
            }
            """;

        const string connectionRole = """
            {
              "LogName": "connectionrole", "Name": "Connectionrole", "CollName": "connectionroles",
              "Cols": [
                { "LogName": "connectionroleid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
                { "LogName": "category", "Name": "Category", "Type": "Picklist", "EnumName": "connectionrole_category", "Select": true }
              ]
            }
            """;

        var generated = TestHelper.Generate<TableSourceFileGenerator>(
            ("Model/Definitions/Connectionrole.table", connectionRole),
            ("Model/Definitions/OptionSet.table", globals));

        StringAssert.Contains("[OptionSetDefinition(\"connectionrole_category\")]", generated["OptionSet.table.cs"]);
        StringAssert.Contains("public enum Categorie", generated["OptionSet.table.cs"]);
        StringAssert.Contains("[OptionSet(typeof(Categorie))]", generated["Connectionrole.table.cs"]);
    }

    /// <summary>
    /// An option set no <c>.table</c> ever named has no enum to be attributed to: the column carrying
    /// it must come out plain rather than annotated <c>[OptionSet(typeof())]</c>.
    /// </summary>
    [Test]
    public void AnUnnamedOptionSet_LeavesTheColumnUnattributed()
    {
        const string table = """
            {
              "LogName": "account", "Name": "Account", "CollName": "accounts",
              "Cols": [
                { "LogName": "accountid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
                { "LogName": "statuscode", "Name": "StatusCode", "Type": "Picklist", "EnumName": "account|statuscode", "Select": true }
              ],
              "Enums": [
                { "LogName": "account|statuscode", "Values": [ { "Value": 1, "Name": "Actif" } ] }
              ]
            }
            """;

        var account = TestHelper.Generate<TableSourceFileGenerator>(
            ("Model/Definitions/Account.table", table))["Account.table.cs"];

        StringAssert.Contains("public const string StatusCode = \"statuscode\";", account);
        StringAssert.DoesNotContain("[OptionSet(typeof())]", account);
        StringAssert.DoesNotContain("public enum ", account);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // One name, several option sets: XRM1003
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Two option sets a selected column carries, named alike: one enum comes out, and the columns of
    /// the other table are typed on members it does not hold. Only the project can settle it.
    /// </summary>
    [Test]
    public void TwoOptionSetsGeneratedUnderOneName_FailTheBuild()
    {
        const string lead = """
            {
              "LogName": "lead", "Name": "Lead", "CollName": "leads",
              "Cols": [
                { "LogName": "leadid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
                { "LogName": "leadqualitycode", "Name": "LeadQualityCode", "Type": "Picklist", "EnumName": "lead|leadqualitycode", "Select": true }
              ],
              "Enums": [
                { "LogName": "lead|leadqualitycode", "Name": "Classement", "Values": [ { "Value": 1, "Name": "Chaud" } ] }
              ]
            }
            """;

        const string project = """
            {
              "LogName": "opportunity", "Name": "Project", "CollName": "opportunities",
              "Cols": [
                { "LogName": "opportunityid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
                { "LogName": "opportunityratingcode", "Name": "OpportunityRatingCode", "Type": "Picklist", "EnumName": "opportunity|opportunityratingcode", "Select": true }
              ],
              "Enums": [
                { "LogName": "opportunity|opportunityratingcode", "Name": "Classement", "Values": [ { "Value": 2, "Name": "Tiede" } ] }
              ]
            }
            """;

        var diagnostics = TestHelper.Diagnose<TableSourceFileGenerator>(
            ("Model/Definitions/Lead.table", lead),
            ("Model/Definitions/Project.table", project));

        var conflict = diagnostics.Single(d => d.Id == "XRM1003");

        Assert.AreEqual(DiagnosticSeverity.Error, conflict.Severity);

        var message = conflict.GetMessage();
        StringAssert.Contains("Classement", message);
        StringAssert.Contains("\"lead|leadqualitycode\" (Lead)", message,
            "Naming both option sets and their tables is what makes the conflict actionable.");
        StringAssert.Contains("\"opportunity|opportunityratingcode\" (Project)", message);
    }

    /// <summary>
    /// Only the option sets that really become enums can collide. A name shared with an option set no
    /// selected column carries costs nothing, and is how a CRM label repeated across tables normally
    /// reaches the <c>.table</c> files.
    /// </summary>
    [Test]
    public void ANameSharedWithAnOptionSetNobodySelects_IsNoConflict()
    {
        var diagnostics = TestHelper.Diagnose<TableSourceFileGenerator>(
            ("Model/Definitions/Account.table", """
                {
                  "LogName": "account", "Name": "Account", "CollName": "accounts",
                  "Cols": [
                    { "LogName": "preferredappointmenttimecode", "Name": "PreferredAppointmentTimeCode", "Type": "Picklist", "EnumName": "account|preferredappointmenttimecode", "Select": false }
                  ],
                  "Enums": [
                    { "LogName": "account|preferredappointmenttimecode", "Name": "HeurePrivilegiee", "Values": [ { "Value": 1, "Name": "Matin" } ] }
                  ]
                }
                """),
            ("Model/Definitions/Contact.table", """
                {
                  "LogName": "contact", "Name": "Contact", "CollName": "contacts",
                  "Cols": [
                    { "LogName": "preferredappointmenttimecode", "Name": "PreferredAppointmentTimeCode", "Type": "Picklist", "EnumName": "contact|preferredappointmenttimecode", "Select": true }
                  ],
                  "Enums": [
                    { "LogName": "contact|preferredappointmenttimecode", "Name": "HeurePrivilegiee", "Values": [ { "Value": 1, "Name": "Matin" } ] }
                  ]
                }
                """));

        Assert.IsEmpty(diagnostics.Where(d => d.Id == "XRM1003"));
    }

    /// <summary>
    /// The same option set declared by both copies of a table is one enum: de-duplicating it is what
    /// the name bookkeeping is for, and must not read as a conflict.
    /// </summary>
    [Test]
    public void OneOptionSetDeclaredByBothCopiesOfATable_IsNoConflict()
    {
        var diagnostics = TestHelper.Diagnose<TableSourceFileGenerator>(
            ("Framework/Systemuser.table", FrameworkSystemUserTable),
            ("Model/Definitions/SystemUser.table", ProjectSystemUserTable));

        Assert.IsEmpty(diagnostics.Where(d => d.Id == "XRM1003"));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Members the enum cannot declare: XRM1004
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// A member name reaches the <c>.table</c> derived from its CRM label, so it carries whatever the
    /// label held and a C# identifier cannot. Emitted as it stands, <c>PourInvest.Jeanbrun</c> is read
    /// as a member <c>PourInvest</c> the next such name declares all over again.
    /// </summary>
    [Test]
    public void AMemberNameThatIsNoIdentifier_IsStrippedDownToOne()
    {
        const string table = """
            {
              "LogName": "globalEnums", "Name": "OptionSet", "Cols": [
                { "LogName": "destination", "Name": "Destination", "Type": "Picklist", "EnumName": "cqc_destinationdubien", "Select": true }
              ],
              "Enums": [
                {
                  "LogName": "cqc_destinationdubien", "Name": "DestinationDuBien", "IsGlobal": true,
                  "Values": [
                    { "Value": 1, "Name": "PourInvest.Jeanbrun" },
                    { "Value": 2, "Name": "PourInvest.JeanbrunPlusLLI" },
                    { "Value": 3, "Name": "2Pieces" }
                  ]
                }
              ]
            }
            """;

        var generated = TestHelper.Generate<TableSourceFileGenerator>(
            ("Model/Definitions/OptionSet.table", table))["OptionSet.table.cs"];

        StringAssert.Contains("PourInvestJeanbrun = 1,", generated);
        StringAssert.Contains("PourInvestJeanbrunPlusLLI = 2,", generated);
        StringAssert.Contains("_2Pieces = 3,", generated,
            "A name starting with a digit is no identifier either.");

        // The label the name was derived from is what [Description] carries, untouched.
        StringAssert.Contains("[Description(\"PourInvest.Jeanbrun\")]", generated);
    }

    /// <summary>
    /// Two CRM options whose names land on one identifier: the enum cannot declare it twice, and
    /// dropping either silently would leave the code mapping one CRM value onto the other.
    /// </summary>
    [Test]
    public void TwoMembersUnderOneIdentifier_FailTheBuild()
    {
        const string table = """
            {
              "LogName": "lead", "Name": "Lead", "CollName": "leads",
              "Cols": [
                { "LogName": "leadsourcecode", "Name": "LeadSourceCode", "Type": "Picklist", "EnumName": "lead|leadsourcecode", "Select": true }
              ],
              "Enums": [
                {
                  "LogName": "lead|leadsourcecode", "Name": "SourceDuProspect",
                  "Values": [ { "Value": 2, "Name": "Web" }, { "Value": 8, "Name": "Web" } ]
                }
              ]
            }
            """;

        var diagnostics = TestHelper.Diagnose<TableSourceFileGenerator>(
            ("Model/Definitions/Lead.table", table));

        var duplicate = diagnostics.Single(d => d.Id == "XRM1004");

        Assert.AreEqual(DiagnosticSeverity.Error, duplicate.Severity);

        var message = duplicate.GetMessage();
        StringAssert.Contains("SourceDuProspect", message);
        StringAssert.Contains("'Web' (8)", message,
            "Naming the value is what tells the project which of the two to rename.");

        // The generated enum still parses: one Web, and the build stops on the diagnostic.
        var generated = TestHelper.Generate<TableSourceFileGenerator>(
            ("Model/Definitions/Lead.table", table))["Lead.table.cs"];

        Assert.AreEqual(1, CountOccurrences(generated, "Web = "));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // One table, several names: XRM1001
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// The two copies of a table are folded on its CRM logical name, but each distinct <c>Name</c>
    /// still makes the generator emit its own definition class, splitting the table's columns and
    /// option sets between them. Nothing downstream can recover from that, so the build stops.
    /// </summary>
    [Test]
    public void ATableDeclaredUnderTwoNames_FailsTheBuild()
    {
        // Exactly what a project upgrading to 3.1 hits on the global option sets: the package ships
        // OptionSet.table, the project kept OptionSets.table, and two classes come out of it.
        var diagnostics = TestHelper.Diagnose<TableSourceFileGenerator>(
            ("Framework/OptionSet.table", """{ "LogName": "globalEnums", "Name": "OptionSet" }"""),
            ("Model/Definitions/OptionSets.table", """{ "LogName": "globalEnums", "Name": "OptionSets" }"""));

        var conflict = diagnostics.Single(d => d.Id == "XRM1001");

        Assert.AreEqual(DiagnosticSeverity.Error, conflict.Severity);

        var message = conflict.GetMessage();
        StringAssert.Contains("globalEnums", message);
        StringAssert.Contains("\"OptionSet\" (OptionSet.table)", message,
            "Naming the file is what makes the conflict actionable.");
        StringAssert.Contains("\"OptionSets\" (OptionSets.table)", message);
    }

    /// <summary>
    /// Two names differing by case alone are two C# identifiers, hence two classes: the same
    /// conflict, and reported as one.
    /// </summary>
    [Test]
    public void NamesDifferingByCaseAlone_AreAConflictToo()
    {
        var diagnostics = TestHelper.Diagnose<TableSourceFileGenerator>(
            ("Framework/Systemuser.table", """{ "LogName": "systemuser", "Name": "Systemuser" }"""),
            ("Model/Definitions/SystemUser.table", """{ "LogName": "systemuser", "Name": "SystemUser" }"""));

        Assert.AreEqual(1, diagnostics.Count(d => d.Id == "XRM1001"));
    }

    /// <summary>
    /// The package names its own <c>.table</c> files and no project can rename them, so differing
    /// file names are the normal state of affairs. Only the <c>Name</c> they carry has to agree.
    /// </summary>
    [Test]
    public void TwoCopiesUnderDifferentFileNames_AgreeingOnTheName_AreNoConflict()
    {
        var diagnostics = TestHelper.Diagnose<TableSourceFileGenerator>(
            ("Framework/Systemuser.table", FrameworkSystemUserTable),
            ("Model/Definitions/SystemUser.table", ProjectSystemUserTable));

        Assert.IsEmpty(diagnostics.Where(d => d.Id == "XRM1001"),
            "Aligning file names is not something a project can do, nor does it need to.");
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

    // ══════════════════════════════════════════════════════════════════════════
    // Alternate keys
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// The framework's copy of <c>systemuser</c>, carrying the single alternate key the framework
    /// itself needs.
    /// </summary>
    private const string FrameworkSystemUserKeyTable = """
        {
          "LogName": "systemuser", "Name": "SystemUser", "CollName": "systemusers",
          "Cols": [
            { "LogName": "systemuserid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
            { "LogName": "azureactivedirectoryobjectid", "Name": "AzureActiveDirectoryObjectId", "Type": "Uniqueidentifier", "Select": true }
          ],
          "Keys": [
            { "LogicalName": "aadobjectid", "Name": "AADObjectid", "FieldNames": [ "azureactivedirectoryobjectid" ] }
          ]
        }
        """;

    /// <summary>
    /// The project's own copy of the same table: it tracks the keys of its model, and knows the
    /// framework's one as well since both come from the same environment.
    /// </summary>
    private const string ProjectSystemUserKeyTable = """
        {
          "LogName": "systemuser", "Name": "SystemUser", "CollName": "systemusers",
          "Cols": [
            { "LogName": "eco_reference", "Name": "Ref", "Type": "String", "Select": true }
          ],
          "Keys": [
            { "LogicalName": "eco_reference", "Name": "Reference", "FieldNames": [ "eco_reference" ] },
            { "LogicalName": "aadobjectid", "Name": "AADObjectid", "FieldNames": [ "azureactivedirectoryobjectid" ] }
          ]
        }
        """;

    /// <summary>
    /// A single Definition class comes out of both files, so <c>AlternateKeyNames</c> has to hold the
    /// keys of either copy: the project's own keys used to vanish behind the framework's.
    /// </summary>
    [TestCase(true, TestName = "KeysOfBothCopies_AreGenerated_FrameworkFileFirst")]
    [TestCase(false, TestName = "KeysOfBothCopies_AreGenerated_ProjectFileFirst")]
    public void KeysOfBothCopies_AreGenerated(bool frameworkFileFirst)
    {
        var framework = ("Framework/Systemuser.table", FrameworkSystemUserKeyTable);
        var project = ("Model/Definitions/SystemUser.table", ProjectSystemUserKeyTable);

        var generated = TestHelper.Generate<TableSourceFileGenerator>(
            frameworkFileFirst ? new[] { framework, project } : new[] { project, framework });

        var systemUser = generated["SystemUser.table.cs"];

        StringAssert.Contains("public const string Reference = \"eco_reference\";", systemUser,
            "A key declared by the project's copy alone must reach the definition.");
        StringAssert.Contains("public const string AADObjectid = \"aadobjectid\";", systemUser,
            "Merging must not cost the framework's copy its own key either.");

        // The key both copies declare is one key, and one constant.
        Assert.AreEqual(1, CountOccurrences(systemUser, "public const string AADObjectid = "));
    }

    [Test]
    public void AKeyAnnotatesTheColumnsItRestsOn()
    {
        var generated = TestHelper.Generate<TableSourceFileGenerator>(
            ("Framework/Systemuser.table", FrameworkSystemUserKeyTable),
            ("Model/Definitions/SystemUser.table", ProjectSystemUserKeyTable));

        var systemUser = generated["SystemUser.table.cs"];

        StringAssert.Contains("[AlternateKey(AlternateKeyNames.Reference)]", systemUser);
        StringAssert.Contains("[AlternateKey(AlternateKeyNames.AADObjectid)]", systemUser);
    }

    /// <summary>
    /// Two keys named alike would declare the constant twice, and a key naming itself nowhere would
    /// declare it against nothing. Neither may reach the generated code, whatever the
    /// <c>.table</c> holds.
    /// </summary>
    [Test]
    public void OnlyTheKeysTheGeneratedCodeCanStandFor_AreEmitted()
    {
        const string table = """
            {
              "LogName": "account", "Name": "Account", "CollName": "accounts",
              "Cols": [
                { "LogName": "accountid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
                { "LogName": "eco_reference", "Name": "Ref", "Type": "String", "Select": true }
              ],
              "Keys": [
                { "LogicalName": "eco_reference", "Name": "Reference", "FieldNames": [ "eco_reference" ] },
                { "LogicalName": "eco_reference_v2", "Name": "Reference", "FieldNames": [ "eco_reference" ] },
                { "FieldNames": [ "eco_reference" ] },
                { "LogicalName": "eco_unnamed", "FieldNames": [ "eco_reference" ] }
              ]
            }
            """;

        var account = TestHelper.Generate<TableSourceFileGenerator>(
            ("Model/Definitions/Account.table", table))["Account.table.cs"];
        var keyNames = AlternateKeyNamesOf(account);

        Assert.AreEqual(1, CountOccurrences(keyNames, "public const string Reference = "),
            "The second key claiming the Reference constant would not compile.");

        // A key the file leaves unnamed still gets a constant, under its logical name.
        StringAssert.Contains("public const string eco_unnamed = \"eco_unnamed\";", keyNames);
        StringAssert.Contains("[AlternateKey(AlternateKeyNames.eco_unnamed)]", account);

        // Of the four keys declared, only these two can be stood for.
        Assert.AreEqual(2, CountOccurrences(keyNames, "public const string "),
            "A key with neither name stands for nothing the CRM can be queried on.");

        // No attribute may name a constant the class does not declare.
        StringAssert.DoesNotContain("[AlternateKey(AlternateKeyNames.)]", account);
    }

    /// <summary>
    /// A <c>.table</c> written before <c>Key.LogicalName</c> existed carries the logical name in
    /// <c>Name</c>. The constant used to come out of one property and its value out of the other,
    /// so such a key generated <c>public const string eco_reference = ""</c> — a name the CRM
    /// matches nothing on.
    /// </summary>
    [Test]
    public void AKeyDeclaredTheOldWay_KeepsItsLogicalName()
    {
        const string framework = """
            {
              "LogName": "team", "Name": "Team", "CollName": "teams",
              "Cols": [
                { "LogName": "teamid", "Name": "Id", "Type": "Uniqueidentifier", "PrimaryType": "Id", "Select": true },
                { "LogName": "azureactivedirectoryobjectid", "Name": "AzureActiveDirectoryObjectId", "Type": "Uniqueidentifier", "Select": true }
              ],
              "Keys": [
                { "LogicalName": "aadobjectid", "Name": "AADObjectid", "FieldNames": [ "azureactivedirectoryobjectid" ] }
              ]
            }
            """;

        const string project = """
            {
              "LogName": "team", "Name": "Team", "CollName": "teams",
              "Cols": [
                { "LogName": "eco_reference", "Name": "Ref", "Type": "String", "Select": true }
              ],
              "Keys": [
                { "Name": "aadobjectid", "FieldNames": [ "azureactivedirectoryobjectid" ] },
                { "Name": "eco_reference", "FieldNames": [ "eco_reference" ] }
              ]
            }
            """;

        var team = TestHelper.Generate<TableSourceFileGenerator>(
            ("Framework/Team.table", framework),
            ("Model/Definitions/Team.table", project))["Team.table.cs"];

        StringAssert.Contains("public const string eco_reference = \"eco_reference\";", team,
            "The old format holds the logical name in Name: the constant must take its value from there.");
        StringAssert.Contains("[AlternateKey(AlternateKeyNames.eco_reference)]", team);

        // Both files declare the aadobjectid key, one under each format: it is one key.
        Assert.AreEqual(1, CountOccurrences(team, "= \"aadobjectid\";"));
        StringAssert.Contains("public const string AADObjectid = \"aadobjectid\";", team);
    }

    [Test]
    public void ATableWithoutKeys_HasNoAlternateKeyNamesClass()
    {
        var generated = TestHelper.Generate<TableSourceFileGenerator>(
            ("Model/Definitions/Account.table", AccountTable));

        StringAssert.DoesNotContain("AlternateKeyNames", generated["Account.table.cs"]);
    }

    /// <summary>
    /// The body of the generated <c>AlternateKeyNames</c> class, so that a count of constants is not
    /// thrown off by those of <c>Columns</c> — a key and the column it rests on often bear the same
    /// name.
    /// </summary>
    private static string AlternateKeyNamesOf(string source)
    {
        var start = source.IndexOf("class AlternateKeyNames", StringComparison.Ordinal);
        Assert.Greater(start, 0, "No AlternateKeyNames class was generated.");

        var end = source.IndexOf("public static class", start, StringComparison.Ordinal);

        return end < 0 ? source.Substring(start) : source.Substring(start, end - start);
    }

    private static int CountOccurrences(string source, string value)
    {
        var count = 0;

        for (var index = source.IndexOf(value, StringComparison.Ordinal);
             index >= 0;
             index = source.IndexOf(value, index + value.Length, StringComparison.Ordinal))
        {
            count++;
        }

        return count;
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
