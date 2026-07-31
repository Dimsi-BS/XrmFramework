// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using XrmFramework.Core;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// File-level side of the 2.* -> 3.1 migration: which <c>*Definition.cs</c> gets deleted, which becomes
/// a <c>.partial.cs</c>, and which is deliberately left alone.
/// </summary>
[TestFixture]
public class DefinitionFileMigratorTests
{
    private string _tempDir = string.Empty;

    [SetUp]
    public void SetUp()
    {
        _tempDir = Path.Combine(Path.GetTempPath(),
            "XrmFramework.DefinitionMigratorTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
        }
        catch { /* best effort */ }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Construction
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Constructor_ThrowsWhenDirectoryDoesNotExist()
    {
        var ghostPath = Path.Combine(Path.GetTempPath(),
            "XrmFramework.DefinitionMigratorTests_ghost_" + Guid.NewGuid().ToString("N"));

        Assert.Throws<DirectoryNotFoundException>(() => new DefinitionFileMigrator(ghostPath));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Deleting a fully generated Definition
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Migrate_DeletesDefinitionFile_WhenEverythingIsRegenerated()
    {
        WriteTable("Contact", "contact");
        WriteCs("ContactDefinition.cs", GeneratedOnly("ContactDefinition", "contact"));

        new DefinitionFileMigrator(_tempDir).Migrate();

        Assert.IsFalse(File.Exists(Path.Combine(_tempDir, "ContactDefinition.cs")));
        Assert.IsFalse(File.Exists(Path.Combine(_tempDir, "ContactDefinition.partial.cs")),
            "Nothing survives: no partial has any reason to be created.");
    }

    [Test]
    public void Migrate_MatchesTableByItsDeclaredName_NotByFileName()
    {
        // Systemuser.table declares "SystemUser": it is the declared name that drives the
        // generated class name, and therefore the file to migrate.
        WriteTable("Systemuser", "systemuser", tableName: "SystemUser");
        WriteCs("SystemUserDefinition.cs", GeneratedOnly("SystemUserDefinition", "systemuser"));

        new DefinitionFileMigrator(_tempDir).Migrate();

        Assert.IsFalse(File.Exists(Path.Combine(_tempDir, "SystemUserDefinition.cs")));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Converting a hand-edited Definition
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Migrate_ConvertsToPartial_WhenHandWrittenMembersRemain()
    {
        WriteTable("Contact", "contact");
        WriteCs("ContactDefinition.cs", WithHandWrittenConstant("ContactDefinition", "contact"));

        new DefinitionFileMigrator(_tempDir).Migrate();

        Assert.IsFalse(File.Exists(Path.Combine(_tempDir, "ContactDefinition.cs")),
            "The original file must not be left behind next to its replacement.");

        var partial = Path.Combine(_tempDir, "ContactDefinition.partial.cs");
        Assert.IsTrue(File.Exists(partial));

        var text = File.ReadAllText(partial);
        StringAssert.Contains("CustomLabel", text);
        StringAssert.Contains("public static partial class ContactDefinition", text);
        StringAssert.Contains("namespace XrmFramework", text);
        StringAssert.DoesNotContain("EntityName", text);
    }

    [Test]
    public void Migrate_LeavesFileAlone_WhenPartialAlreadyExists()
    {
        WriteTable("Contact", "contact");
        WriteCs("ContactDefinition.cs", WithHandWrittenConstant("ContactDefinition", "contact"));
        WriteCs("ContactDefinition.partial.cs", "// hand-written, do not clobber");

        new DefinitionFileMigrator(_tempDir).Migrate();

        Assert.IsTrue(File.Exists(Path.Combine(_tempDir, "ContactDefinition.cs")),
            "Rather than overwriting an existing partial, the migration reports and steps back.");
        Assert.AreEqual("// hand-written, do not clobber",
            File.ReadAllText(Path.Combine(_tempDir, "ContactDefinition.partial.cs")));
    }

    [Test]
    public void Migrate_IgnoresAlreadyMigratedPartialFiles()
    {
        WriteTable("Contact", "contact");
        WriteCs("ContactDefinition.partial.cs", WithHandWrittenConstant("ContactDefinition", "contact"));

        var skipped = new DefinitionFileMigrator(_tempDir).Migrate();

        Assert.AreEqual(0, skipped);
        Assert.IsTrue(File.Exists(Path.Combine(_tempDir, "ContactDefinition.partial.cs")),
            "A .partial.cs is the output of this migration, never its input.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Refusing to act
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Migrate_LeavesFileAlone_WhenNoTableBacksIt()
    {
        // No Ghost.table: the generator will produce nothing, so deleting the .cs would drop
        // the definition altogether.
        WriteCs("GhostDefinition.cs", GeneratedOnly("GhostDefinition", "ghost"));

        var skipped = new DefinitionFileMigrator(_tempDir).Migrate();

        Assert.AreEqual(1, skipped);
        Assert.IsTrue(File.Exists(Path.Combine(_tempDir, "GhostDefinition.cs")));
    }

    [Test]
    public void Migrate_LeavesFileAlone_WhenItCannotBeReadReliably()
    {
        WriteTable("Contact", "contact");
        WriteCs("ContactDefinition.cs", "namespace MyProject.Core { public static class ContactDefinition {");

        var skipped = new DefinitionFileMigrator(_tempDir).Migrate();

        Assert.AreEqual(1, skipped);
        Assert.IsTrue(File.Exists(Path.Combine(_tempDir, "ContactDefinition.cs")));
    }

    [Test]
    public void Migrate_MigratesFile_WhenItsNameDiffersFromTheClassInCasing()
    {
        // Sdkmessageprocessingstep.table declares "SdkMessageProcessingStep": file names and declared
        // names drift apart in casing across the framework, and the class is what matters.
        WriteTable("Contact", "contact");
        WriteCs("ContactDefinition.cs",
            GeneratedOnly("CONTACTDEFINITION", "contact"));

        var skipped = new DefinitionFileMigrator(_tempDir).Migrate();

        Assert.AreEqual(0, skipped);
        Assert.IsFalse(File.Exists(Path.Combine(_tempDir, "ContactDefinition.cs")));
    }

    [Test]
    public void Migrate_IgnoresFilesThatAreNotDefinitions()
    {
        WriteTable("Contact", "contact");
        WriteCs("Helpers.cs", "namespace MyProject.Core { public static class Helpers { } }");

        new DefinitionFileMigrator(_tempDir).Migrate();

        Assert.IsTrue(File.Exists(Path.Combine(_tempDir, "Helpers.cs")));
    }

    [Test]
    public void Migrate_ReportsNothingToDo_OnAnAlreadyMigratedDirectory()
    {
        WriteTable("Contact", "contact");

        Assert.AreEqual(0, new DefinitionFileMigrator(_tempDir).Migrate());
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Option set enums
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Migrate_RemovesEnum_WhenASelectedColumnReferencesIt()
    {
        WriteTable(BuildTable("Contact", "contact",
            new Column { LogicalName = "statuscode", Name = "StatusCode", Selected = true, EnumName = "contact_statuscode" },
            optionSet: new OptionSetEnum { LogicalName = "contact_statuscode", Name = "ContactStatus" }));

        WriteCs("ContactDefinition.cs", WithEnum("ContactDefinition", "contact", "ContactStatus"));

        new DefinitionFileMigrator(_tempDir).Migrate();

        Assert.IsFalse(File.Exists(Path.Combine(_tempDir, "ContactDefinition.cs")),
            "Class and enum are both regenerated: nothing is left to keep.");
    }

    [Test]
    public void Migrate_KeepsEnum_WhenNoSelectedColumnReferencesIt()
    {
        // The generator only emits an option set enum for a selected column. Here the column is
        // deselected, so the enum is not regenerated and must survive.
        WriteTable(BuildTable("Contact", "contact",
            new Column { LogicalName = "statuscode", Name = "StatusCode", Selected = false, EnumName = "contact_statuscode" },
            optionSet: new OptionSetEnum { LogicalName = "contact_statuscode", Name = "ContactStatus" }));

        WriteCs("ContactDefinition.cs", WithEnum("ContactDefinition", "contact", "ContactStatus"));

        new DefinitionFileMigrator(_tempDir).Migrate();

        var partial = Path.Combine(_tempDir, "ContactDefinition.partial.cs");
        Assert.IsTrue(File.Exists(partial));
        StringAssert.Contains("enum ContactStatus", File.ReadAllText(partial));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Helpers
    // ══════════════════════════════════════════════════════════════════════════

    private static string GeneratedOnly(string className, string logicalName) => $$"""
        using System.CodeDom.Compiler;
        using System.Diagnostics.CodeAnalysis;

        namespace MyProject.Core
        {
            [GeneratedCode("XrmFramework", "2.0")]
            [EntityDefinition]
            [ExcludeFromCodeCoverage]
            public static class {{className}}
            {
                public const string EntityName = "{{logicalName}}";
                public const string EntityCollectionName = "{{logicalName}}s";

                [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
                public static class Columns
                {
                    [AttributeMetadata(AttributeTypeCode.String)]
                    public const string Name = "name";
                }
            }
        }
        """;

    private static string WithHandWrittenConstant(string className, string logicalName) => $$"""
        namespace MyProject.Core
        {
            [GeneratedCode("XrmFramework", "2.0")]
            [EntityDefinition]
            public static class {{className}}
            {
                public const string EntityName = "{{logicalName}}";

                public const string CustomLabel = "added by hand";

                public static class Columns
                {
                    public const string Name = "name";
                }
            }
        }
        """;

    private static string WithEnum(string className, string logicalName, string enumName) => $$"""
        namespace MyProject.Core
        {
            [EntityDefinition]
            public static class {{className}}
            {
                public const string EntityName = "{{logicalName}}";

                public static class Columns
                {
                    public const string StatusCode = "statuscode";
                }
            }

            public enum {{enumName}}
            {
                Active = 0,
            }
        }
        """;

    private void WriteCs(string fileName, string content)
        => File.WriteAllText(Path.Combine(_tempDir, fileName), content);

    private void WriteTable(string fileName, string logicalName, string? tableName = null)
        => WriteTable(BuildTable(tableName ?? fileName, logicalName), fileName);

    private static Table BuildTable(string name, string logicalName,
                                    Column? column = null, OptionSetEnum? optionSet = null)
    {
        var table = new Table
        {
            Name = name,
            LogicalName = logicalName,
            CollectionName = logicalName + "s"
        };

        if (column != null) table.Columns.Add(column);
        if (optionSet != null) table.Enums.Add(optionSet);

        return table;
    }

    private void WriteTable(Table table, string? fileName = null)
    {
        var json = JsonConvert.SerializeObject(table, Formatting.Indented,
            new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });

        File.WriteAllText(Path.Combine(_tempDir, (fileName ?? table.Name) + ".table"), json);
    }
}
