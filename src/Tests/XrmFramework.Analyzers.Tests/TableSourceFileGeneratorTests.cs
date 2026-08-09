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
}
