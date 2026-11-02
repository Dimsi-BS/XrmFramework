// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using NUnit.Framework;

namespace XrmFramework.TypeScript.Tests;

/// <summary>
/// What the TypeScript definitions declare, and from what.
/// </summary>
/// <remarks>
/// <para>
/// The generator reads the same <c>.table</c> files as the C# source generator and has to fold them
/// by the same rules: the framework's copy of a table and the project's copy of it are two
/// declarations of one table, and only what the generated code declares belongs in a definition.
/// Both used to be decided here rather than in <c>XrmFramework.Core</c>, and both drifted -- one
/// project had <c>TeamType</c> in its generated C# and <c>TypeDEquipe</c> in its generated
/// TypeScript, and the definitions carried every column of every table whether the model activated
/// it or not.
/// </para>
/// <para>
/// The task is the thin MSBuild adapter over this; it holds no logic of its own.
/// </para>
/// </remarks>
[TestFixture]
public class TsGeneratorTests
{
    // ══════════════════════════════════════════════════════════════════════════
    // Folding the two copies a framework table comes in
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// The package's copy of <c>team</c>: <c>teamtype</c> under the name the framework's own code
    /// refers to, and locked for that reason.
    /// </summary>
    private static object FrameworkTeam() => new
    {
        LogName = "team",
        Name = "Team",
        CollName = "teams",
        Locked = true,
        Cols = new object[]
        {
            new { LogName = "teamtype", Name = "Type", Type = "Picklist", Capa = 13,
                  Locked = true, Select = true, EnumName = "team|teamtype" }
        },
        Enums = new object[]
        {
            new { LogName = "team|teamtype", Name = "TeamType", Locked = true,
                  Values = new object[] { new { Value = 0, Name = "Owner" } } }
        }
    };

    /// <summary>
    /// The project's copy, pulled from the environment: the CRM schema name, no lock, and a column
    /// of its own, which is what the copy exists for.
    /// </summary>
    private static object ProjectTeam() => new
    {
        LogName = "team",
        Name = "Team",
        CollName = "teams",
        Cols = new object[]
        {
            new { LogName = "teamtype", Name = "TeamType", Type = "Picklist", Capa = 13,
                  Select = true, EnumName = "team|teamtype" },
            new { LogName = "dpp_custom", Name = "Custom", Type = "String", Capa = 15, Select = true }
        },
        Enums = new object[]
        {
            new { LogName = "team|teamtype", Name = "TypeDEquipe",
                  Values = new object[] { new { Value = 0, Name = "Proprietaire" } } }
        }
    };

    [Test]
    public void Run_FoldsTwoCopiesOfOneTable_IntoOneDefinition()
    {
        using var harness = new GeneratorHarness()
            .WithTable("Team", FrameworkTeam())
            .WithTable("ZProjectTeam", ProjectTeam());

        Assert.IsTrue(harness.Run());

        Assert.That(harness.DefinitionNames(), Is.EqualTo(new[] { "TeamDefinition" }),
                    "two copies of team are one table, hence one definition");

        var columns = harness.Definition("TeamDefinition")["Cols"]!;

        Assert.AreEqual("Type", (string?)columns.Single(c => (string?)c["LogName"] == "teamtype")["Name"],
                        "the locked name belongs to the framework, not to the project's copy");
        Assert.IsTrue(columns.Any(c => (string?)c["LogName"] == "dpp_custom"),
                      "the project's own column is what its copy exists for");
    }

    /// <summary>
    /// The fold must not depend on the order the files reach the task in, which is the order
    /// <c>AdditionalFiles</c> happens to list them.
    /// </summary>
    [Test]
    public void Run_FoldsTwoCopies_WhateverTheOrder()
    {
        using var harness = new GeneratorHarness()
            .WithTable("AProjectTeam", ProjectTeam())
            .WithTable("Team", FrameworkTeam());

        Assert.IsTrue(harness.Run());

        var columns = harness.Definition("TeamDefinition")["Cols"]!;

        Assert.AreEqual("Type", (string?)columns.Single(c => (string?)c["LogName"] == "teamtype")["Name"]);
    }

    [Test]
    public void Run_DeclaresTheOptionSetUnderItsLockedName()
    {
        using var harness = new GeneratorHarness()
            .WithTable("Team", FrameworkTeam())
            .WithTable("ZProjectTeam", ProjectTeam());

        Assert.IsTrue(harness.Run());

        Assert.That(harness.EnumNames(), Is.EqualTo(new[] { "TeamType" }),
                    "one enum, under the name the framework locked");
    }

    /// <summary>
    /// The emitted enum must carry the members of the copy that names it: the framework's code
    /// writes <c>TeamType.Owner</c>, and the TypeScript has to offer the same member.
    /// </summary>
    [Test]
    public void Run_EmitsTheMembersOfTheCopyThatNamesTheOptionSet()
    {
        using var harness = new GeneratorHarness()
            .WithTable("ZProjectTeam", ProjectTeam())
            .WithTable("Team", FrameworkTeam());

        Assert.IsTrue(harness.Run());

        Assert.That(harness.Enums(), Does.Contain("Owner = 0,"));
        Assert.That(harness.Enums(), Does.Not.Contain("Proprietaire"),
                    "the project's member names belong to the name it did not win");
    }

    [Test]
    public void Run_WritesOneTablesEntryPerLogicalName()
    {
        using var harness = new GeneratorHarness()
            .WithTable("Team", FrameworkTeam())
            .WithTable("ZProjectTeam", ProjectTeam());

        Assert.IsTrue(harness.Run());

        var entries = harness.Tables().Split('\n').Count(line => line.TrimStart().StartsWith("team:"));

        Assert.AreEqual(1, entries, "a duplicate key would make the Tables type ambiguous");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Naming
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// The definition is named after the table, as the generated C# class is: a table may be
    /// declared in two files, and a file is freely renamed.
    /// </summary>
    [Test]
    public void Run_NamesTheDefinitionAfterTheTable_NotTheFile()
    {
        using var harness = new GeneratorHarness().WithTable("SomeFileName", new
        {
            LogName = "ftp_contrat",
            Name = "Contrat",
            CollName = "ftp_contrats",
            Cols = new object[]
            {
                new { LogName = "ftp_nom", Name = "Nom", Type = "String", Capa = 15, Select = true }
            }
        });

        Assert.IsTrue(harness.Run());

        Assert.That(harness.DefinitionNames(), Is.EqualTo(new[] { "ContratDefinition" }));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Declaring only what the generated code declares
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Run_DeclaresOnlySelectedColumns()
    {
        using var harness = new GeneratorHarness().WithTable("Contrat", new
        {
            LogName = "ftp_contrat",
            Name = "Contrat",
            CollName = "ftp_contrats",
            Cols = new object[]
            {
                new { LogName = "ftp_nom", Name = "Nom", Type = "String", Capa = 15, Select = true },
                new { LogName = "ftp_inactif", Name = "Inactif", Type = "String", Capa = 15 }
            }
        });

        Assert.IsTrue(harness.Run());

        var columns = harness.Definition("ContratDefinition")["Cols"]!;

        Assert.AreEqual(1, columns.Count());
        Assert.AreEqual("ftp_nom", (string?)columns.First()["LogName"],
                        "an unselected column reaches no generated code");
    }

    [Test]
    public void Run_DeclaresOnlyOptionSetsCarriedByASelectedColumn()
    {
        using var harness = new GeneratorHarness().WithTable("Contrat", new
        {
            LogName = "ftp_contrat",
            Name = "Contrat",
            CollName = "ftp_contrats",
            Cols = new object[]
            {
                new { LogName = "ftp_statut", Name = "Statut", Type = "Picklist", Capa = 15,
                      Select = true, EnumName = "ftp_contrat|ftp_statut" },
                new { LogName = "ftp_type", Name = "Type", Type = "Picklist", Capa = 15,
                      EnumName = "ftp_contrat|ftp_type" }
            },
            Enums = new object[]
            {
                new { LogName = "ftp_contrat|ftp_statut", Name = "Statut",
                      Values = new object[] { new { Value = 1, Name = "Actif" } } },
                new { LogName = "ftp_contrat|ftp_type", Name = "TypeContrat",
                      Values = new object[] { new { Value = 1, Name = "Location" } } }
            }
        });

        Assert.IsTrue(harness.Run());

        Assert.That(harness.EnumNames(), Is.EqualTo(new[] { "Statut" }),
                    "TypeContrat is carried by no selected column, so no enum stands for it");
        Assert.AreEqual(1, harness.Definition("ContratDefinition")["Enums"]!.Count());
    }

    /// <summary>
    /// A 1:N rests on a lookup carried by the table at the far end: unselected there, the
    /// relationship reaches no generated code either.
    /// </summary>
    [Test]
    public void Run_DropsAOneToManyWhoseLookupIsNotSelectedAtTheFarEnd()
    {
        using var harness = new GeneratorHarness()
            .WithTable("Contrat", new
            {
                LogName = "ftp_contrat",
                Name = "Contrat",
                CollName = "ftp_contrats",
                Cols = new object[]
                {
                    new { LogName = "ftp_nom", Name = "Nom", Type = "String", Capa = 15, Select = true }
                },
                OneToN = new object[]
                {
                    new { Name = "ftp_contrat_ligne_selected", Etn = "ftp_ligne", Role = "Referenced",
                          NavPropName = "lignes", LookName = "ftp_contratid" },
                    new { Name = "ftp_contrat_ligne_dropped", Etn = "ftp_ligne", Role = "Referenced",
                          NavPropName = "autres", LookName = "ftp_autrecontratid" }
                }
            })
            .WithTable("Ligne", new
            {
                LogName = "ftp_ligne",
                Name = "Ligne",
                CollName = "ftp_lignes",
                Cols = new object[]
                {
                    new { LogName = "ftp_contratid", Name = "ContratId", Type = "Lookup", Capa = 15,
                          Select = true },
                    new { LogName = "ftp_autrecontratid", Name = "AutreContratId", Type = "Lookup",
                          Capa = 15 }
                }
            });

        Assert.IsTrue(harness.Run());

        var relations = harness.Definition("ContratDefinition")["OneToN"]!;

        Assert.AreEqual(1, relations.Count());
        Assert.AreEqual("ftp_contrat_ligne_selected", (string?)relations.First()["Name"]);
    }

    [Test]
    public void Run_KeepsAManyToOneWhoseLookupIsSelected()
    {
        using var harness = new GeneratorHarness().WithTable("Ligne", new
        {
            LogName = "ftp_ligne",
            Name = "Ligne",
            CollName = "ftp_lignes",
            Cols = new object[]
            {
                new { LogName = "ftp_contratid", Name = "ContratId", Type = "Lookup", Capa = 15,
                      Select = true }
            },
            NToOne = new object[]
            {
                new { Name = "ftp_ligne_contrat", Etn = "ftp_contrat", NavPropName = "contrat",
                      LookName = "ftp_contratid" },
                new { Name = "ftp_ligne_orphan", Etn = "ftp_contrat", NavPropName = "orphan",
                      LookName = "ftp_absent" }
            }
        });

        Assert.IsTrue(harness.Run());

        var relations = harness.Definition("LigneDefinition")["NToOne"]!;

        Assert.AreEqual(1, relations.Count());
        Assert.AreEqual("ftp_ligne_contrat", (string?)relations.First()["Name"]);
    }

    /// <summary>
    /// One constant per key name, so two keys claiming it declare it once.
    /// </summary>
    [Test]
    public void Run_DeclaresOneKeyPerName()
    {
        using var harness = new GeneratorHarness().WithTable("Contrat", new
        {
            LogName = "ftp_contrat",
            Name = "Contrat",
            CollName = "ftp_contrats",
            Cols = new object[]
            {
                new { LogName = "ftp_nom", Name = "Nom", Type = "String", Capa = 15, Select = true }
            },
            Keys = new object[]
            {
                new { LogicalName = "ftp_reference", Name = "Reference",
                      FieldNames = new[] { "ftp_nom" } },
                new { LogicalName = "ftp_reference_2", Name = "Reference",
                      FieldNames = new[] { "ftp_nom" } },
                new { LogicalName = (string?)null, Name = (string?)null,
                      FieldNames = new[] { "ftp_nom" } }
            }
        });

        Assert.IsTrue(harness.Run());

        var keys = harness.Definition("ContratDefinition")["Keys"]!;

        Assert.AreEqual(1, keys.Count());
        Assert.AreEqual("ftp_reference", (string?)keys.First()["LogicalName"]);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // What the task reports
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Run_WritesTheEnumMap()
    {
        using var harness = new GeneratorHarness()
            .WithTable("Team", FrameworkTeam())
            .WithTable("ZProjectTeam", ProjectTeam());

        Assert.IsTrue(harness.Run());

        Assert.That(harness.Enums(), Does.Contain("declare global { interface EnumMap {"));
        Assert.That(harness.Enums(), Does.Contain("TeamType: TeamType;"));
    }

    /// <summary>
    /// One C# name yields one enum; the C# generator fails the build over the clash (XRM1003), and
    /// a warning here says why an enum is missing rather than failing a second time.
    /// </summary>
    [Test]
    public void Run_WarnsWhenTwoOptionSetsClaimOneName()
    {
        using var harness = new GeneratorHarness().WithTable("Contrat", new
        {
            LogName = "ftp_contrat",
            Name = "Contrat",
            CollName = "ftp_contrats",
            Cols = new object[]
            {
                new { LogName = "ftp_a", Name = "A", Type = "Picklist", Capa = 15, Select = true,
                      EnumName = "ftp_contrat|ftp_a" },
                new { LogName = "ftp_b", Name = "B", Type = "Picklist", Capa = 15, Select = true,
                      EnumName = "ftp_contrat|ftp_b" }
            },
            Enums = new object[]
            {
                new { LogName = "ftp_contrat|ftp_a", Name = "Statut",
                      Values = new object[] { new { Value = 1, Name = "Actif" } } },
                new { LogName = "ftp_contrat|ftp_b", Name = "Statut",
                      Values = new object[] { new { Value = 2, Name = "Autre" } } }
            }
        });

        Assert.IsTrue(harness.Run());

        Assert.That(harness.EnumNames(), Is.EqualTo(new[] { "Statut" }));
        Assert.AreEqual(1, harness.Log.Warnings.Count(w => w.Contains("Statut")),
                        "the clash must be reported, not swallowed");
    }

    [Test]
    public void Run_ReportsAnUnreadableTable_AsAnError()
    {
        using var harness = new GeneratorHarness().WithRawTable("Broken", "{ not json");

        Assert.IsFalse(harness.Run(), "a table that cannot be read must fail the build");
        Assert.IsNotEmpty(harness.Log.Errors);
    }
}
