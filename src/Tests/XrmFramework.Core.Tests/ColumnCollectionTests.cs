
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XrmFramework.Core.Tests.Comparers;

namespace XrmFramework.Core.Tests;

[TestClass]
public class ColumnCollectionTests
{
    private ColumnCollection _columnCollection = null!;

    private Column _notSelectedColumn = new()
    {
        LogicalName = "LogicalName",
        Name = "NotSelectedName"
    };

    private Column _otherColumn = new()
    {
        LogicalName = "OtherLogicalName",
        Name = "OtherName"
    };

    private Column _selectedColumn = new()
    {
        LogicalName = "LogicalName",
        Name = "SelectedName",
        Selected = true
    };

    [TestInitialize]
    public void InitTests()
    {
        _columnCollection = new ColumnCollection();
    }

    [TestMethod]
    public void ObjectInitialization()
    {
        Assert.IsNotNull(_columnCollection.GetEnumerator());
        Assert.AreEqual(0, _columnCollection.Count);
        Assert.IsFalse(_columnCollection.IsReadOnly);
    }

    [TestMethod]
    public void Add_NullColumn_DoesNotAdd()
    {
        _columnCollection.Add(null);
        Assert.AreEqual(0, _columnCollection.Count);
    }

    [TestMethod]
    public void Add_NewColumn_AddsSuccessfully()
    {
        _columnCollection.Add(_selectedColumn);

        Assert.AreEqual(1, _columnCollection.Count);
        CollectionAssert.AreEquivalent(new List<Column> { _selectedColumn }, _columnCollection.ToList());
    }

    [TestMethod]
    public void Add_ExistingColumn_SelectedOverridesNotSelected()
    {
        _columnCollection.Add(_notSelectedColumn);
        _columnCollection.Add(_selectedColumn);

        Assert.AreEqual(1, _columnCollection.Count);

        var retrievedColumn = _columnCollection.Single();
        Assert.AreEqual(_selectedColumn.Name, retrievedColumn.Name);
        Assert.IsTrue(retrievedColumn.Selected);
    }

    [TestMethod]
    public void Add_ExistingColumnWithSelectedFirst_KeepsSelected()
    {
        _columnCollection.Add(_selectedColumn);
        _columnCollection.Add(_notSelectedColumn);

        Assert.AreEqual(1, _columnCollection.Count);

        var retrievedColumn = _columnCollection.Single();
        Assert.AreEqual(_selectedColumn.Name, retrievedColumn.Name);
        Assert.IsTrue(retrievedColumn.Selected);
    }

    [TestMethod]
    public void Add_MultipleColumnsWithDifferentLogicalNames_AddsAll()
    {
        _columnCollection.Add(_selectedColumn);
        _columnCollection.Add(_otherColumn);

        Assert.AreEqual(2, _columnCollection.Count);
        CollectionAssert.AreEquivalent(new List<Column> { _selectedColumn, _otherColumn }, _columnCollection.ToList(), new ColumnEqualityComparer());
    }

    [TestMethod]
    public void MergeColumns_EmptyList_NoChanges()
    {
        _columnCollection.Add(_selectedColumn);

        _columnCollection.MergeColumns(new List<Column>());

        Assert.AreEqual(1, _columnCollection.Count);
    }

    [TestMethod]
    public void MergeColumns_WithNewColumns_AddsAll()
    {
        _columnCollection.Add(_selectedColumn);

        var list = new List<Column> { _otherColumn };
        _columnCollection.MergeColumns(list);

        Assert.AreEqual(2, _columnCollection.Count);
        Assert.IsTrue(_columnCollection.Contains(_selectedColumn));
        Assert.IsTrue(_columnCollection.Contains(_otherColumn));
    }

    [TestMethod]
    public void MergeColumns_WithDuplicates_MergesCorrectly()
    {
        var list = new List<Column> { _selectedColumn, _notSelectedColumn, _otherColumn };
        _columnCollection.MergeColumns(list);

        Assert.AreEqual(2, _columnCollection.Count);
        var mergedColumn = _columnCollection.First(c => c.LogicalName == "LogicalName");
        Assert.IsTrue(mergedColumn.Selected);
    }

    [TestMethod]
    public void RemoveAll_WithMatchingPredicate_RemovesItems()
    {
        _columnCollection.Add(_selectedColumn);
        _columnCollection.Add(_otherColumn);

        _columnCollection.RemoveAll(c => c.Selected);

        Assert.AreEqual(1, _columnCollection.Count);
        Assert.IsFalse(_columnCollection.Contains(_selectedColumn));
        Assert.IsTrue(_columnCollection.Contains(_otherColumn));
    }

    [TestMethod]
    public void RemoveAll_WithNoMatches_NoChanges()
    {
        _columnCollection.Add(_otherColumn);

        _columnCollection.RemoveAll(c => c.Selected);

        Assert.AreEqual(1, _columnCollection.Count);
        Assert.IsTrue(_columnCollection.Contains(_otherColumn));
    }

    [TestMethod]
    public void RemoveAll_RemovesAllMatchingItems()
    {
        var column1 = new Column { LogicalName = "col1", Selected = true };
        var column2 = new Column { LogicalName = "col2", Selected = true };
        var column3 = new Column { LogicalName = "col3", Selected = false };

        _columnCollection.Add(column1);
        _columnCollection.Add(column2);
        _columnCollection.Add(column3);

        _columnCollection.RemoveAll(c => c.Selected);

        Assert.AreEqual(1, _columnCollection.Count);
        Assert.IsTrue(_columnCollection.Contains(column3));
    }

    [TestMethod]
    public void Clear_RemovesAllItems()
    {
        _columnCollection.Add(_selectedColumn);
        _columnCollection.Add(_otherColumn);

        _columnCollection.Clear();

        Assert.AreEqual(0, _columnCollection.Count);
    }

    [TestMethod]
    public void Contains_NullColumn_ReturnsFalse()
    {
        Assert.IsFalse(_columnCollection.Contains(null));
    }

    [TestMethod]
    public void Contains_ExistingColumn_ReturnsTrue()
    {
        _columnCollection.Add(_selectedColumn);

        Assert.IsTrue(_columnCollection.Contains(_notSelectedColumn));
        Assert.IsTrue(_columnCollection.Contains(_selectedColumn));
    }

    [TestMethod]
    public void Contains_NonExistingColumn_ReturnsFalse()
    {
        _columnCollection.Add(_selectedColumn);

        Assert.IsFalse(_columnCollection.Contains(_otherColumn));
    }

    [TestMethod]
    public void CopyTo_CopiesToArray()
    {
        _columnCollection.Add(_selectedColumn);
        _columnCollection.Add(_otherColumn);

        var array = new Column[2];
        _columnCollection.CopyTo(array, 0);

        Assert.AreEqual(2, array.Length);
        CollectionAssert.AreEquivalent(new List<Column> { _selectedColumn, _otherColumn }, array.ToList(), new ColumnEqualityComparer());
    }

    [TestMethod]
    public void CopyTo_WithArrayIndex_CopiesToCorrectPosition()
    {
        _columnCollection.Add(_selectedColumn);

        var array = new Column[3];
        _columnCollection.CopyTo(array, 1);

        Assert.IsNull(array[0]);
        Assert.AreEqual(_selectedColumn, array[1]);
        Assert.IsNull(array[2]);
    }

    [TestMethod]
    public void Remove_NullColumn_ReturnsFalse()
    {
        Assert.IsFalse(_columnCollection.Remove(null));
    }

    [TestMethod]
    public void Remove_ExistingColumn_RemovesAndReturnsTrue()
    {
        _columnCollection.Add(_selectedColumn);

        bool result = _columnCollection.Remove(_notSelectedColumn);

        Assert.IsTrue(result);
        Assert.AreEqual(0, _columnCollection.Count);
    }

    [TestMethod]
    public void Remove_NonExistingColumn_ReturnsFalse()
    {
        _columnCollection.Add(_selectedColumn);

        bool result = _columnCollection.Remove(_otherColumn);

        Assert.IsFalse(result);
        Assert.AreEqual(1, _columnCollection.Count);
    }

    [TestMethod]
    public void IsReadOnly_ReturnsFalse()
    {
        Assert.IsFalse(_columnCollection.IsReadOnly);
    }

    [TestMethod]
    public void GetEnumerator_Generic_EnumeratesCorrectly()
    {
        _columnCollection.Add(_selectedColumn);
        _columnCollection.Add(_otherColumn);

        using var enumerator = _columnCollection.GetEnumerator();

        var items = new List<Column>();
        while (enumerator.MoveNext())
        {
            items.Add(enumerator.Current);
        }

        Assert.AreEqual(2, items.Count);
        CollectionAssert.AreEquivalent(new List<Column> { _selectedColumn, _otherColumn }, items, new ColumnEqualityComparer());
    }

    [TestMethod]
    public void GetEnumerator_NonGeneric_EnumeratesCorrectly()
    {
        _columnCollection.Add(_selectedColumn);
        _columnCollection.Add(_otherColumn);

        var enumerator = ((IEnumerable)_columnCollection).GetEnumerator();

        var items = new List<Column>();
        while (enumerator.MoveNext())
        {
            items.Add((Column)enumerator.Current);
        }

        Assert.AreEqual(2, items.Count);
        CollectionAssert.AreEquivalent(new List<Column> { _selectedColumn, _otherColumn }, items, new ColumnEqualityComparer());
    }

    [TestMethod]
    public void Count_ReturnsCorrectValue()
    {
        Assert.AreEqual(0, _columnCollection.Count);

        _columnCollection.Add(_selectedColumn);
        Assert.AreEqual(1, _columnCollection.Count);

        _columnCollection.Add(_otherColumn);
        Assert.AreEqual(2, _columnCollection.Count);

        _columnCollection.Remove(_selectedColumn);
        Assert.AreEqual(1, _columnCollection.Count);
    }

    [TestMethod]
    public void SortedBehavior_MaintainsSortOrder()
    {
        var columnA = new Column { LogicalName = "a_column", Name = "A" };
        var columnC = new Column { LogicalName = "c_column", Name = "C" };
        var columnB = new Column { LogicalName = "b_column", Name = "B" };

        _columnCollection.Add(columnC);
        _columnCollection.Add(columnA);
        _columnCollection.Add(columnB);

        var list = _columnCollection.ToList();
        Assert.AreEqual("a_column", list[0].LogicalName);
        Assert.AreEqual("b_column", list[1].LogicalName);
        Assert.AreEqual("c_column", list[2].LogicalName);
    }
}
