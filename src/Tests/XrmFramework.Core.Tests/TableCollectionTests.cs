using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace XrmFramework.Core.Tests;

[TestFixture]
public class TableCollectionTests
{
    private TableCollection _tableCollection = null!;

    private Table _table1 = null!;
    private Table _table2 = null!;
    private Table _table3 = null!;

    [SetUp]
    public void InitTests()
    {
        _tableCollection = new TableCollection();

        _table1 = new Table
        {
            LogicalName = "account",
            Name = "Account",
            CollectionName = "Accounts"
        };
        _table1.Columns.Add(new Column { LogicalName = "accountid", Name = "AccountId" });

        _table2 = new Table
        {
            LogicalName = "contact",
            Name = "Contact",
            CollectionName = "Contacts"
        };
        _table2.Columns.Add(new Column { LogicalName = "contactid", Name = "ContactId" });

        _table3 = new Table
        {
            LogicalName = "lead",
            Name = "Lead",
            CollectionName = "Leads"
        };
    }

    [Test]
    public void ObjectInitialization()
    {
        Assert.IsNotNull(_tableCollection);
        Assert.AreEqual(0, _tableCollection.Count);
        Assert.IsNotNull(_tableCollection.GetEnumerator());
    }

    [Test]
    public void Add_NullTable_DoesNotAdd()
    {
        _tableCollection.Add(null);

        Assert.AreEqual(0, _tableCollection.Count);
    }

    [Test]
    public void Add_NewTable_AddsSuccessfully()
    {
        _tableCollection.Add(_table1);

        Assert.AreEqual(1, _tableCollection.Count);
        Assert.IsTrue(_tableCollection.Contains(_table1));
    }

    [Test]
    public void Add_MultipleTables_AddsAll()
    {
        _tableCollection.Add(_table1);
        _tableCollection.Add(_table2);
        _tableCollection.Add(_table3);

        Assert.AreEqual(3, _tableCollection.Count);
        Assert.IsTrue(_tableCollection.Contains(_table1));
        Assert.IsTrue(_tableCollection.Contains(_table2));
        Assert.IsTrue(_tableCollection.Contains(_table3));
    }

    [Test]
    public void Add_DuplicateTable_MergesColumns()
    {
        var table1Copy = new Table
        {
            LogicalName = "account",
            Name = "Account",
            CollectionName = "Accounts"
        };
        table1Copy.Columns.Add(new Column { LogicalName = "name", Name = "Name" });

        _tableCollection.Add(_table1);
        _tableCollection.Add(table1Copy);

        Assert.AreEqual(1, _tableCollection.Count);
        
        // Verify that the first table has merged columns from the second
        var firstTable = _tableCollection.First(t => t.LogicalName == "account");
        Assert.AreEqual(2, firstTable.Columns.Count);
        Assert.IsTrue(firstTable.Columns.Any(c => c.LogicalName == "accountid"));
        Assert.IsTrue(firstTable.Columns.Any(c => c.LogicalName == "name"));
    }

    [Test]
    public void Add_DuplicateTableWithNoExisting_AddsOnce()
    {
        var table1Copy = new Table
        {
            LogicalName = "account",
            Name = "Account",
            CollectionName = "Accounts"
        };

        _tableCollection.Add(table1Copy);

        Assert.AreEqual(1, _tableCollection.Count);
    }

    [Test]
    public void Clear_RemovesAllTables()
    {
        _tableCollection.Add(_table1);
        _tableCollection.Add(_table2);
        _tableCollection.Add(_table3);

        _tableCollection.Clear();

        Assert.AreEqual(0, _tableCollection.Count);
    }

    [Test]
    public void Contains_ExistingTable_ReturnsTrue()
    {
        _tableCollection.Add(_table1);

        Assert.IsTrue(_tableCollection.Contains(_table1));
    }

    [Test]
    public void Contains_NonExistingTable_ReturnsFalse()
    {
        _tableCollection.Add(_table1);

        Assert.IsFalse(_tableCollection.Contains(_table2));
    }

    [Test]
    public void CopyTo_CopiesToArray()
    {
        _tableCollection.Add(_table1);
        _tableCollection.Add(_table2);

        var array = new Table[2];
        _tableCollection.CopyTo(array, 0);

        Assert.AreEqual(2, array.Length);
        Assert.IsNotNull(array[0]);
        Assert.IsNotNull(array[1]);
    }

    [Test]
    public void CopyTo_WithArrayIndex_CopiesToCorrectPosition()
    {
        _tableCollection.Add(_table1);

        var array = new Table[3];
        _tableCollection.CopyTo(array, 1);

        Assert.IsNull(array[0]);
        Assert.IsNotNull(array[1]);
        Assert.IsNull(array[2]);
    }

    [Test]
    public void Remove_ExistingTable_RemovesAndReturnsTrue()
    {
        _tableCollection.Add(_table1);
        _tableCollection.Add(_table2);

        bool result = _tableCollection.Remove(_table1);

        Assert.IsTrue(result);
        Assert.AreEqual(1, _tableCollection.Count);
        Assert.IsFalse(_tableCollection.Contains(_table1));
    }

    [Test]
    public void Remove_NonExistingTable_ReturnsFalse()
    {
        _tableCollection.Add(_table1);

        bool result = _tableCollection.Remove(_table2);

        Assert.IsFalse(result);
        Assert.AreEqual(1, _tableCollection.Count);
    }

    [Test]
    public void Count_ReturnsCorrectValue()
    {
        Assert.AreEqual(0, _tableCollection.Count);

        _tableCollection.Add(_table1);
        Assert.AreEqual(1, _tableCollection.Count);

        _tableCollection.Add(_table2);
        Assert.AreEqual(2, _tableCollection.Count);

        _tableCollection.Remove(_table1);
        Assert.AreEqual(1, _tableCollection.Count);

        _tableCollection.Clear();
        Assert.AreEqual(0, _tableCollection.Count);
    }

    [Test]
    public void IsReadOnly_ReturnsFalse()
    {
        Assert.IsFalse(_tableCollection.IsReadOnly);
    }

    [Test]
    public void AddRange_EmptyList_NoChanges()
    {
        _tableCollection.Add(_table1);

        _tableCollection.AddRange(new List<Table>());

        Assert.AreEqual(1, _tableCollection.Count);
    }

    [Test]
    public void AddRange_WithMultipleTables_AddsAll()
    {
        var tables = new List<Table> { _table1, _table2, _table3 };

        _tableCollection.AddRange(tables);

        Assert.AreEqual(3, _tableCollection.Count);
        Assert.IsTrue(_tableCollection.Contains(_table1));
        Assert.IsTrue(_tableCollection.Contains(_table2));
        Assert.IsTrue(_tableCollection.Contains(_table3));
    }

    [Test]
    public void AddRange_WithDuplicates_MergesCorrectly()
    {
        var table1Copy = new Table
        {
            LogicalName = "account",
            Name = "Account",
            CollectionName = "Accounts"
        };
        table1Copy.Columns.Add(new Column { LogicalName = "name", Name = "Name" });

        _tableCollection.Add(_table1);

        var tables = new List<Table> { table1Copy, _table2 };
        _tableCollection.AddRange(tables);

        Assert.AreEqual(2, _tableCollection.Count);

        var accountTable = _tableCollection.First(t => t.LogicalName == "account");
        Assert.AreEqual(2, accountTable.Columns.Count);
    }

    [Test]
    public void AddRange_WithNullTable_SkipsNull()
    {
        var tables = new List<Table> { _table1, null, _table2 };

        _tableCollection.AddRange(tables);

        Assert.AreEqual(2, _tableCollection.Count);
    }

    [Test]
    public void GetEnumerator_Generic_EnumeratesCorrectly()
    {
        _tableCollection.Add(_table1);
        _tableCollection.Add(_table2);

        var items = new List<Table>();
        using (var enumerator = _tableCollection.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                items.Add(enumerator.Current);
            }
        }

        Assert.AreEqual(2, items.Count);
    }

    [Test]
    public void GetEnumerator_NonGeneric_EnumeratesCorrectly()
    {
        _tableCollection.Add(_table1);
        _tableCollection.Add(_table2);

        var items = new List<Table>();
        var enumerator = ((IEnumerable)_tableCollection).GetEnumerator();
        
        while (enumerator.MoveNext())
        {
            items.Add((Table)enumerator.Current);
        }

        Assert.AreEqual(2, items.Count);
    }

    [Test]
    public void SortedBehavior_MaintainsSortOrder()
    {
        var tableC = new Table { LogicalName = "customer", Name = "Customer" };
        var tableA = new Table { LogicalName = "account", Name = "Account" };
        var tableB = new Table { LogicalName = "business", Name = "Business" };

        _tableCollection.Add(tableC);
        _tableCollection.Add(tableA);
        _tableCollection.Add(tableB);

        var list = _tableCollection.ToList();
        
        // Tables should be sorted by Name, then LogicalName (case-insensitive)
        Assert.AreEqual("Account", list[0].Name);
        Assert.AreEqual("Business", list[1].Name);
        Assert.AreEqual("Customer", list[2].Name);
    }

    [Test]
    public void SortedBehavior_SameNameDifferentLogicalName_SortsByLogicalName()
    {
        var table1 = new Table { LogicalName = "table_b", Name = "SameName" };
        var table2 = new Table { LogicalName = "table_a", Name = "SameName" };

        _tableCollection.Add(table1);
        _tableCollection.Add(table2);

        var list = _tableCollection.ToList();

        Assert.AreEqual("table_a", list[0].LogicalName);
        Assert.AreEqual("table_b", list[1].LogicalName);
    }

    [Test]
    public void Enumeration_AfterModification_ReflectsChanges()
    {
        _tableCollection.Add(_table1);
        
        var countBefore = _tableCollection.Count();
        Assert.AreEqual(1, countBefore);

        _tableCollection.Add(_table2);
        
        var countAfter = _tableCollection.Count();
        Assert.AreEqual(2, countAfter);
    }

    [Test]
    public void Add_TableWithoutColumns_AddsSuccessfully()
    {
        var emptyTable = new Table
        {
            LogicalName = "empty",
            Name = "Empty"
        };

        _tableCollection.Add(emptyTable);

        Assert.AreEqual(1, _tableCollection.Count);
        Assert.IsTrue(_tableCollection.Contains(emptyTable));
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Option sets of a table declared twice
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Add_DuplicateTable_MergesOptionSetsTheOtherCopyDeclaresAlone()
    {
        // A table shipped by the framework and tracked again by the project: the project's copy
        // selects a Picklist column and declares the option set behind it. Merging the column
        // without its option set would generate it with no [OptionSet] attribute and no enum.
        _table1.Enums.Add(new OptionSetEnum { LogicalName = "account|accountcategorycode", Name = "AccountCategory" });

        var projectCopy = new Table { LogicalName = "account", Name = "Account" };
        projectCopy.Columns.Add(new Column
        {
            LogicalName = "invitestatuscode", Name = "InviteStatusCode",
            Selected = true, EnumName = "account|invitestatuscode"
        });
        projectCopy.Enums.Add(new OptionSetEnum { LogicalName = "account|invitestatuscode", Name = "InviteStatus" });

        _tableCollection.Add(_table1);
        _tableCollection.Add(projectCopy);

        var merged = _tableCollection.Single(t => t.LogicalName == "account");

        Assert.AreEqual(2, merged.Enums.Count);
        Assert.IsTrue(merged.Enums.Any(e => e.LogicalName == "account|accountcategorycode"));
        Assert.IsTrue(merged.Enums.Any(e => e.LogicalName == "account|invitestatuscode"),
            "The option set referenced by the merged column must come along with it.");
    }

    [Test]
    public void Add_DuplicateTable_KeepsTheOptionSetAlreadyInPlace_OnConflict()
    {
        // Both copies declare the same option set under different C# names, neither on a column it
        // selects: nothing designates a winner, so the copy loaded first keeps the naming.
        _table1.Enums.Add(new OptionSetEnum { LogicalName = "systemuser|accessmode", Name = "AccessMode" });

        var projectCopy = new Table { LogicalName = "account", Name = "Account" };
        projectCopy.Enums.Add(new OptionSetEnum { LogicalName = "SystemUser|AccessMode", Name = "ModeDAcces" });

        _tableCollection.Add(_table1);
        _tableCollection.Add(projectCopy);

        var merged = _tableCollection.Single(t => t.LogicalName == "account");

        Assert.AreEqual(1, merged.Enums.Count);
        Assert.AreEqual("AccessMode", merged.Enums[0].Name);
    }

    [Test]
    public void Add_DuplicateTable_TakesTheNameOfTheCopySelectingTheColumn_OnConflict()
    {
        // The framework ships the column but selects it nowhere, so it never generates the enum and
        // no framework code names it: the project's own name is the one the compilation runs on.
        _table1.Columns.Add(new Column
        {
            LogicalName = "caltype", Name = "CALType", EnumName = "account|caltype"
        });
        _table1.Enums.Add(new OptionSetEnum { LogicalName = "account|caltype", Name = "ClientAccessLicenseType" });

        var projectCopy = new Table { LogicalName = "account", Name = "Account" };
        projectCopy.Columns.Add(new Column
        {
            LogicalName = "caltype", Name = "CALType", Selected = true, EnumName = "account|caltype"
        });
        projectCopy.Enums.Add(new OptionSetEnum { LogicalName = "account|caltype", Name = "CALTypes" });

        _tableCollection.Add(_table1);
        _tableCollection.Add(projectCopy);

        var merged = _tableCollection.Single(t => t.LogicalName == "account");

        Assert.AreEqual(1, merged.Enums.Count);
        Assert.AreEqual("CALTypes", merged.Enums[0].Name);
    }

    [Test]
    public void Add_DuplicateTable_TakesTheNameOfTheCopySelectingTheColumn_WhateverTheLoadingOrder()
    {
        _table1.Columns.Add(new Column
        {
            LogicalName = "caltype", Name = "CALType", EnumName = "account|caltype"
        });
        _table1.Enums.Add(new OptionSetEnum { LogicalName = "account|caltype", Name = "ClientAccessLicenseType" });

        var projectCopy = new Table { LogicalName = "account", Name = "Account" };
        projectCopy.Columns.Add(new Column
        {
            LogicalName = "caltype", Name = "CALType", Selected = true, EnumName = "account|caltype"
        });
        projectCopy.Enums.Add(new OptionSetEnum { LogicalName = "account|caltype", Name = "CALTypes" });

        // Reversed order: the project's copy is the one the collection keeps.
        _tableCollection.Add(projectCopy);
        _tableCollection.Add(_table1);

        var merged = _tableCollection.Single(t => t.LogicalName == "account");

        Assert.AreEqual(1, merged.Enums.Count);
        Assert.AreEqual("CALTypes", merged.Enums[0].Name);
    }

    [Test]
    public void Add_DuplicateTable_KeepsTheNameWhenTheCopyInPlaceSelectsTheColumn()
    {
        // accessmode is one of the columns the framework selects: it generates AccessMode and its own
        // code refers to it under that name, so a rename in the project's copy must not reach it.
        _table1.Columns.Add(new Column
        {
            LogicalName = "accessmode", Name = "AccessMode", Selected = true, EnumName = "account|accessmode"
        });
        _table1.Enums.Add(new OptionSetEnum { LogicalName = "account|accessmode", Name = "AccessMode" });

        var projectCopy = new Table { LogicalName = "account", Name = "Account" };
        projectCopy.Columns.Add(new Column
        {
            LogicalName = "accessmode", Name = "AccessMode", Selected = true, EnumName = "account|accessmode"
        });
        projectCopy.Enums.Add(new OptionSetEnum { LogicalName = "account|accessmode", Name = "ModeDAcces" });

        _tableCollection.Add(_table1);
        _tableCollection.Add(projectCopy);

        var merged = _tableCollection.Single(t => t.LogicalName == "account");

        Assert.AreEqual(1, merged.Enums.Count);
        Assert.AreEqual("AccessMode", merged.Enums[0].Name);
    }

    [Test]
    public void Add_DuplicateTable_MergesOptionSets_WhateverTheLoadingOrder()
    {
        var projectCopy = new Table { LogicalName = "account", Name = "Account" };
        projectCopy.Enums.Add(new OptionSetEnum { LogicalName = "account|invitestatuscode", Name = "InviteStatus" });

        _table1.Enums.Add(new OptionSetEnum { LogicalName = "account|accountcategorycode", Name = "AccountCategory" });

        // Reversed order: the project's copy is the one the collection keeps.
        _tableCollection.Add(projectCopy);
        _tableCollection.Add(_table1);

        var merged = _tableCollection.Single(t => t.LogicalName == "account");

        Assert.AreEqual(2, merged.Enums.Count);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // The name and the lock of a table declared twice
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// The framework locks the tables it ships and its own code refers to the classes generated
    /// from them, so its name has to survive a project copy pulled under the CRM schema name —
    /// whichever of the two the collection happens to keep.
    /// </summary>
    [Test]
    public void Add_DuplicateTable_KeepsTheLockedTableName([Values(true, false)] bool frameworkFirst)
    {
        var framework = new Table
        {
            LogicalName = "team",
            Name = "Team",
            CollectionName = "teams",
            IsLocked = true
        };

        var project = new Table { LogicalName = "team", Name = "Teams", CollectionName = "teams" };

        AddInOrder(frameworkFirst, framework, project);

        // One table, not two: a disagreement on the name used to file a second entry in the set.
        Assert.AreEqual(1, _tableCollection.Count);

        var team = _tableCollection.Get("team");

        Assert.AreEqual("Team", team!.Name);
        Assert.IsTrue(team.IsLocked);
    }

    /// <summary>
    /// The lock describes the table, not the copy that happened to be loaded first.
    /// </summary>
    [Test]
    public void Add_DuplicateTable_KeepsTheLock([Values(true, false)] bool frameworkFirst)
    {
        var framework = new Table { LogicalName = "team", Name = "Team", IsLocked = true };
        var project = new Table { LogicalName = "team", Name = "Team" };

        AddInOrder(frameworkFirst, framework, project);

        Assert.IsTrue(_tableCollection.Get("team")!.IsLocked);
    }

    /// <summary>
    /// A renamed table stays reachable under the name it ends up with: the set files its entries
    /// by that very name.
    /// </summary>
    [Test]
    public void Add_DuplicateTable_RenamedToTheLockedName_StaysSorted()
    {
        var framework = new Table { LogicalName = "team", Name = "Team", IsLocked = true };

        // Sorts ahead of "Account", where "Team" sorts behind it: a name changed in place would
        // leave the entry filed at the wrong end of the set.
        var project = new Table { LogicalName = "team", Name = "Aaa" };

        _tableCollection.Add(project);
        _tableCollection.Add(framework);
        _tableCollection.Add(new Table { LogicalName = "account", Name = "Account" });

        var names = _tableCollection.Select(t => t.Name).ToList();

        Assert.That(names, Is.EqualTo(new List<string> { "Account", "Team" }));

        // Looked up by name through the set's comparer, so an entry filed under a name it no
        // longer carries is unreachable.
        Assert.IsTrue(_tableCollection.Contains(project));
    }

    /// <summary>
    /// Neither copy locked: the one already in place keeps its name, and two copies disagreeing on
    /// it are the generator's <c>XRM1001</c> to report, not this collection's to arbitrate.
    /// </summary>
    [Test]
    public void Add_DuplicateTable_NeitherCopyLocked_KeepsTheNameAlreadyInPlace()
    {
        var first = new Table { LogicalName = "team", Name = "Team" };
        var second = new Table { LogicalName = "team", Name = "Teams" };

        _tableCollection.Add(first);
        _tableCollection.Add(second);

        Assert.AreEqual(1, _tableCollection.Count);
        Assert.AreEqual("Team", _tableCollection.Get("team")!.Name);
        Assert.IsFalse(_tableCollection.Get("team")!.IsLocked);
    }

    [Test]
    public void Add_DuplicateTable_BothCopiesLocked_KeepsTheNameAlreadyInPlace()
    {
        var first = new Table { LogicalName = "team", Name = "Team", IsLocked = true };
        var second = new Table { LogicalName = "team", Name = "Teams", IsLocked = true };

        _tableCollection.Add(first);
        _tableCollection.Add(second);

        Assert.AreEqual(1, _tableCollection.Count);
        Assert.AreEqual("Team", _tableCollection.Get("team")!.Name);
        Assert.IsTrue(_tableCollection.Get("team")!.IsLocked);
    }

    [Test]
    public void Add_DuplicateTable_LockedCopyWithoutAName_ImposesNothing()
    {
        var framework = new Table { LogicalName = "team", IsLocked = true };
        var project = new Table { LogicalName = "team", Name = "Teams" };

        _tableCollection.Add(project);
        _tableCollection.Add(framework);

        Assert.AreEqual(1, _tableCollection.Count);
        Assert.AreEqual("Teams", _tableCollection.Get("team")!.Name);
        Assert.IsTrue(_tableCollection.Get("team")!.IsLocked);
    }

    /// <summary>
    /// Adding the very same instance twice must not fold it into itself.
    /// </summary>
    [Test]
    public void Add_SameInstanceTwice_ChangesNothing()
    {
        _table1.Columns.Add(new Column { LogicalName = "name", Name = "Name", Selected = true });

        _tableCollection.Add(_table1);
        _tableCollection.Add(_table1);

        Assert.AreEqual(1, _tableCollection.Count);
        Assert.AreEqual("Account", _tableCollection.Get("account")!.Name);
        Assert.AreEqual(2, _tableCollection.Get("account")!.Columns.Count);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Locked option sets of a table declared twice
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Adds the two copies of a table in the order <c>AdditionalFiles</c> would list them in, or in
    /// the reverse one: nothing the fold decides may depend on that order.
    /// </summary>
    private void AddInOrder(bool frameworkFirst, Table framework, Table project)
    {
        if (frameworkFirst)
        {
            _tableCollection.Add(framework);
            _tableCollection.Add(project);
        }
        else
        {
            _tableCollection.Add(project);
            _tableCollection.Add(framework);
        }
    }

    /// <summary>
    /// Both copies select <c>teamtype</c>, so the selection rule alone cannot tell them apart and
    /// hands the naming to whichever was loaded first. The lock settles it: the framework's own
    /// code refers to <c>TeamType.Owner</c>, and the project's <c>TypeDEquipe</c> must not reach it.
    /// </summary>
    [Test]
    public void Add_DuplicateTable_KeepsTheLockedOptionSetName([Values(true, false)] bool frameworkFirst)
    {
        var framework = new Table { LogicalName = "team", Name = "Team", IsLocked = true };
        framework.Columns.Add(new Column
        {
            LogicalName = "teamtype", Name = "Type", IsLocked = true, Selected = true, EnumName = "team|teamtype"
        });
        framework.Enums.Add(new OptionSetEnum { LogicalName = "team|teamtype", Name = "TeamType", IsLocked = true });

        var project = new Table { LogicalName = "team", Name = "Team" };
        project.Columns.Add(new Column
        {
            LogicalName = "teamtype", Name = "TeamType", Selected = true, EnumName = "team|teamtype"
        });
        project.Enums.Add(new OptionSetEnum { LogicalName = "team|teamtype", Name = "TypeDEquipe" });

        AddInOrder(frameworkFirst, framework, project);

        var optionSet = _tableCollection.Get("team")!.Enums.Single();

        Assert.AreEqual("TeamType", optionSet.Name);
        Assert.IsTrue(optionSet.IsLocked);
    }

    /// <summary>
    /// The package declares <c>membershiptype</c>'s option set and locks it, but selects no column
    /// carrying it. Under the selection rule alone the project's rename carried the day — the lock
    /// now comes first.
    /// </summary>
    [Test]
    public void Add_DuplicateTable_KeepsTheLockedOptionSetName_EvenWhenOnlyTheProjectSelectsTheColumn(
        [Values(true, false)] bool frameworkFirst)
    {
        var framework = new Table { LogicalName = "team", Name = "Team", IsLocked = true };
        framework.Enums.Add(new OptionSetEnum
        {
            LogicalName = "team|membershiptype", Name = "MembershipType", IsLocked = true
        });

        var project = new Table { LogicalName = "team", Name = "Team" };
        project.Columns.Add(new Column
        {
            LogicalName = "membershiptype",
            Name = "MembershipType",
            Selected = true,
            EnumName = "team|membershiptype"
        });
        project.Enums.Add(new OptionSetEnum { LogicalName = "team|membershiptype", Name = "TypeDAppartenance" });

        AddInOrder(frameworkFirst, framework, project);

        var optionSet = _tableCollection.Get("team")!.Enums.Single();

        Assert.AreEqual("MembershipType", optionSet.Name);
        Assert.IsTrue(optionSet.IsLocked);
    }

    [Test]
    public void Add_DuplicateTable_LockedOptionSetWithoutAName_ImposesNothing()
    {
        var framework = new Table { LogicalName = "team", Name = "Team", IsLocked = true };
        framework.Enums.Add(new OptionSetEnum { LogicalName = "team|teamtype", IsLocked = true });

        var project = new Table { LogicalName = "team", Name = "Team" };
        project.Columns.Add(new Column
        {
            LogicalName = "teamtype", Name = "TeamType", Selected = true, EnumName = "team|teamtype"
        });
        project.Enums.Add(new OptionSetEnum { LogicalName = "team|teamtype", Name = "TypeDEquipe" });

        _tableCollection.Add(framework);
        _tableCollection.Add(project);

        var optionSet = _tableCollection.Get("team")!.Enums.Single();

        Assert.AreEqual("TypeDEquipe", optionSet.Name);
        Assert.IsTrue(optionSet.IsLocked);
    }

    /// <summary>
    /// Two locked copies have nothing to impose on each other: the selection decides, as it does
    /// for two unlocked ones.
    /// </summary>
    [Test]
    public void Add_DuplicateTable_TwoLockedOptionSets_TheSelectionStillDecides()
    {
        var first = new Table { LogicalName = "team", Name = "Team" };
        first.Columns.Add(new Column { LogicalName = "teamtype", Name = "Type", EnumName = "team|teamtype" });
        first.Enums.Add(new OptionSetEnum { LogicalName = "team|teamtype", Name = "TeamType", IsLocked = true });

        var second = new Table { LogicalName = "team", Name = "Team" };
        second.Columns.Add(new Column
        {
            LogicalName = "teamtype", Name = "Type", Selected = true, EnumName = "team|teamtype"
        });
        second.Enums.Add(new OptionSetEnum { LogicalName = "team|teamtype", Name = "Renamed", IsLocked = true });

        _tableCollection.Add(first);
        _tableCollection.Add(second);

        var optionSet = _tableCollection.Get("team")!.Enums.Single();

        Assert.AreEqual("Renamed", optionSet.Name);
        Assert.IsTrue(optionSet.IsLocked);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Relationships of a table declared twice
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Add_DuplicateTable_MergesRelationships()
    {
        // The framework's copy of the table knows the relationships the framework itself needs; the
        // project's copy, retrieved later, knows those of its own model. Selecting a lookup must find
        // the relationship behind it whichever of the two files declares it.
        _table1.ManyToOneRelationships.Add(new Relation
        {
            Name = "business_unit_accounts", EntityName = "businessunit", LookupFieldName = "owningbusinessunit"
        });

        var projectCopy = new Table { LogicalName = "account", Name = "Account" };
        projectCopy.ManyToOneRelationships.Add(new Relation
        {
            Name = "account_primary_contact", EntityName = "contact", LookupFieldName = "primarycontactid"
        });
        projectCopy.OneToManyRelationships.Add(new Relation
        {
            Name = "contact_customer_accounts", EntityName = "contact",
            Role = EntityRole.Referenced, LookupFieldName = "parentcustomerid"
        });
        projectCopy.ManyToManyRelationships.Add(new Relation { Name = "accountleads_association", EntityName = "lead" });

        _tableCollection.Add(_table1);
        _tableCollection.Add(projectCopy);

        var merged = _tableCollection.Single(t => t.LogicalName == "account");

        Assert.AreEqual(2, merged.ManyToOneRelationships.Count);
        Assert.IsTrue(merged.ManyToOneRelationships.Any(r => r.Name == "business_unit_accounts"));
        Assert.IsTrue(merged.ManyToOneRelationships.Any(r => r.Name == "account_primary_contact"));
        Assert.AreEqual(1, merged.OneToManyRelationships.Count);
        Assert.AreEqual(1, merged.ManyToManyRelationships.Count);
    }

    [Test]
    public void Add_DuplicateTable_KeepsTheRelationshipAlreadyInPlace()
    {
        // A relationship is identified by its schema name, which comes from the CRM and which no
        // project renames: two copies declaring it describe the same thing.
        _table1.ManyToOneRelationships.Add(new Relation
        {
            Name = "business_unit_accounts", EntityName = "businessunit", LookupFieldName = "owningbusinessunit"
        });

        var projectCopy = new Table { LogicalName = "account", Name = "Account" };
        projectCopy.ManyToOneRelationships.Add(new Relation
        {
            Name = "Business_Unit_Accounts", EntityName = "businessunit", LookupFieldName = "owningbusinessunit"
        });

        _tableCollection.Add(_table1);
        _tableCollection.Add(projectCopy);

        var merged = _tableCollection.Single(t => t.LogicalName == "account");

        Assert.AreEqual(1, merged.ManyToOneRelationships.Count);
        Assert.AreEqual("business_unit_accounts", merged.ManyToOneRelationships[0].Name);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Alternate keys of a table declared twice
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Add_DuplicateTable_MergesTheKeysTheOtherCopyDeclaresAlone()
    {
        // The framework ships systemuser with the single key it needs; the project tracks the same
        // table and knows the keys of its own model. Both have to reach AlternateKeyNames.
        _table1.Keys.Add(new Key { LogicalName = "aadobjectid", Name = "AADObjectid" });

        var projectCopy = new Table { LogicalName = "account", Name = "Account" };
        projectCopy.Keys.Add(new Key { LogicalName = "eco_reference", Name = "Reference" });

        _tableCollection.Add(_table1);
        _tableCollection.Add(projectCopy);

        var merged = _tableCollection.Single(t => t.LogicalName == "account");

        Assert.AreEqual(2, merged.Keys.Count);
        Assert.IsTrue(merged.Keys.Any(k => k.Name == "AADObjectid"));
        Assert.IsTrue(merged.Keys.Any(k => k.Name == "Reference"),
            "A key declared by the project's copy alone must survive the merge.");
    }

    [Test]
    public void Add_DuplicateTable_MergesKeys_WhateverTheLoadingOrder()
    {
        _table1.Keys.Add(new Key { LogicalName = "aadobjectid", Name = "AADObjectid" });

        var projectCopy = new Table { LogicalName = "account", Name = "Account" };
        projectCopy.Keys.Add(new Key { LogicalName = "eco_reference", Name = "Reference" });

        // Reversed order: the project's copy is the one the collection keeps.
        _tableCollection.Add(projectCopy);
        _tableCollection.Add(_table1);

        var merged = _tableCollection.Single(t => t.LogicalName == "account");

        Assert.AreEqual(2, merged.Keys.Count);
    }

    [Test]
    public void Add_DuplicateTable_KeepsTheKeyAlreadyInPlace()
    {
        // A key is identified by the logical name the CRM is queried on: two copies declaring it
        // describe the same key, and the code shipped with the copy in place names it already.
        _table1.Keys.Add(new Key { LogicalName = "aadobjectid", Name = "AADObjectid" });

        var projectCopy = new Table { LogicalName = "account", Name = "Account" };
        projectCopy.Keys.Add(new Key { LogicalName = "AADObjectId", Name = "AzureAdObjectId" });

        _tableCollection.Add(_table1);
        _tableCollection.Add(projectCopy);

        var merged = _tableCollection.Single(t => t.LogicalName == "account");

        Assert.AreEqual(1, merged.Keys.Count);
        Assert.AreEqual("AADObjectid", merged.Keys.Single().Name);
    }

    [Test]
    public void Add_DuplicateTable_TakesTheColumnsOfTheOtherCopy_WhenTheKeyInPlaceListsNone()
    {
        // Without the columns it rests on, the key annotates nothing: the copy in place is simply
        // older than the other, and the columns come from the CRM anyway.
        _table1.Keys.Add(new Key { LogicalName = "eco_reference", Name = "Reference" });

        var projectCopy = new Table { LogicalName = "account", Name = "Account" };
        var projectKey = new Key { LogicalName = "eco_reference", Name = "Reference" };
        projectKey.FieldNames.Add("eco_reference");
        projectCopy.Keys.Add(projectKey);

        _tableCollection.Add(_table1);
        _tableCollection.Add(projectCopy);

        var merged = _tableCollection.Single(t => t.LogicalName == "account");

        Assert.AreEqual(1, merged.Keys.Count);
        Assert.AreEqual("eco_reference", merged.Keys.Single().FieldNames.Single());
    }

    [Test]
    public void Add_DuplicateTable_IgnoresAKeyNamedNowhere()
    {
        _table1.Keys.Add(new Key { LogicalName = "aadobjectid", Name = "AADObjectid" });

        var projectCopy = new Table { LogicalName = "account", Name = "Account" };
        projectCopy.Keys.Add(new Key());

        _tableCollection.Add(_table1);
        _tableCollection.Add(projectCopy);

        var merged = _tableCollection.Single(t => t.LogicalName == "account");

        Assert.AreEqual(1, merged.Keys.Count);
    }

    [Test]
    public void Add_DuplicateTable_RecognizesAKeyDeclaredTheOldWay()
    {
        // A .table written before Key.LogicalName existed carries the logical name in Name. Both
        // copies below declare the very same key, and must not end up declaring it twice.
        _table1.Keys.Add(new Key { LogicalName = "aadobjectid_membershiptype", Name = "AADObjectidWithMembershiptype" });

        var projectCopy = new Table { LogicalName = "account", Name = "Account" };
        projectCopy.Keys.Add(new Key { Name = "aadobjectid_membershiptype" });
        projectCopy.Keys.Add(new Key { Name = "eco_reference" });

        _tableCollection.Add(_table1);
        _tableCollection.Add(projectCopy);

        var merged = _tableCollection.Single(t => t.LogicalName == "account");

        Assert.AreEqual(2, merged.Keys.Count);
        Assert.IsTrue(merged.Keys.Any(k => k.MemberName == "AADObjectidWithMembershiptype"));
        Assert.IsTrue(merged.Keys.Any(k => k.EffectiveLogicalName == "eco_reference"));
    }

    [Test]
    public void MergeTo_NullExistingTable_DoesNotThrow()
    {
        var newTable = new Table
        {
            LogicalName = "new",
            Name = "New"
        };
        newTable.Columns.Add(new Column { LogicalName = "col1", Name = "Column1" });

        // This should not throw
        _tableCollection.Add(newTable);

        Assert.AreEqual(1, _tableCollection.Count);
    }

    /// <summary>
    /// The two copies <c>team</c> reaches the generator in: the one the XrmFramework package ships
    /// and the one the project tracks to enrich the table. The package names <c>teamtype</c>
    /// <c>Type</c> and locks it, because the code it ships refers to
    /// <c>TeamDefinition.Columns.Type</c>; the project's copy, pulled from the CRM, calls it
    /// <c>TeamType</c> and knows nothing of the lock. The locked name has to win, and the lock has
    /// to survive, whichever order the two copies arrive in — that order being the one
    /// <c>AdditionalFiles</c> happens to list them in.
    /// </summary>
    [Test]
    public void Add_FrameworkTableAndProjectCopy_KeepsTheLockedColumnName(
        [Values(true, false)] bool frameworkFirst)
    {
        var framework = new Table
        {
            LogicalName = "team",
            Name = "Team",
            CollectionName = "teams",
            IsLocked = true
        };
        framework.Columns.Add(new Column
        {
            LogicalName = "teamtype",
            Name = "Type",
            IsLocked = true,
            Selected = true
        });

        var project = new Table
        {
            LogicalName = "team",
            Name = "Team",
            CollectionName = "teams"
        };
        project.Columns.Add(new Column { LogicalName = "teamtype", Name = "TeamType", Selected = true });
        project.Columns.Add(new Column { LogicalName = "membershiptype", Name = "MembershipType", Selected = true });

        AddInOrder(frameworkFirst, framework, project);

        var team = _tableCollection.Get("team");
        var teamType = team!.Columns.Single(c => c.LogicalName == "teamtype");

        Assert.AreEqual("Type", teamType.Name);
        Assert.IsTrue(teamType.IsLocked);

        // The point of the project's copy in the first place: its own columns still come through.
        Assert.AreEqual(2, team.Columns.Count);
    }
}
