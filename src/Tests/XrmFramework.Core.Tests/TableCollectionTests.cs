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
}
