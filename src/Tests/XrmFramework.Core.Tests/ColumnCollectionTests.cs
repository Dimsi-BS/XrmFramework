using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XrmFramework.Core.Tests
{
    [TestClass]
    public class ColumnCollectionTests
    {
        private ColumnCollection _columnCollection = null!;

        private Column _selectedColumn = new Column
        {
            LogicalName = "LogicalName",
            Name = "SelectedName",
            Selected = true
        };

        private Column _notSelectedColumn = new Column
        {
            LogicalName = "LogicalName",
            Name = "NotSelectedName"
        };

        private Column _otherColumn = new Column
        {
            LogicalName = "OtherLogicalName",
            Name = "OtherName"
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

        }

        [TestMethod]
        public void AddColumn()
        {
            _columnCollection.Add(null);


            _columnCollection.Add(_selectedColumn);

            Assert.AreEqual(1, _columnCollection.Count);
            CollectionAssert.AreEquivalent(new List<Column> { _selectedColumn }, _columnCollection.ToList());


            _columnCollection.Add(_notSelectedColumn);

            Assert.AreEqual(1, _columnCollection.Count);

            var retrievedColumn = _columnCollection.Single();

            Assert.AreEqual(_selectedColumn.Name, retrievedColumn.Name);
            Assert.IsTrue(retrievedColumn.Selected);

            _columnCollection.Clear();

            Assert.AreEqual(0, _columnCollection.Count);

            _columnCollection.Add(_notSelectedColumn);
            _columnCollection.Add(_selectedColumn);

            retrievedColumn = _columnCollection.Single();

            Assert.AreEqual(_selectedColumn.Name, retrievedColumn.Name);
            Assert.IsTrue(retrievedColumn.Selected);

            _columnCollection.Add(_otherColumn);

            CollectionAssert.AreEquivalent(new List<Column> { _otherColumn, _notSelectedColumn }, _columnCollection.ToList());
        }

        [TestMethod]
        public void RemoveColumn()
        {
            _columnCollection.Add(_selectedColumn);

            Assert.IsFalse(_columnCollection.Remove(null));

            _columnCollection.Remove(_notSelectedColumn);

            Assert.AreEqual(0, _columnCollection.Count);
        }

        [TestMethod]
        public void ContainsColumn()
        {
            _columnCollection.Add(_selectedColumn);

            Assert.IsFalse(_columnCollection.Contains(null));

            Assert.IsTrue(_columnCollection.Contains(_notSelectedColumn));
        }

        [TestMethod]
        public void IsReadOnly()
        {
            Assert.IsTrue(_columnCollection.IsReadOnly);
        }

        [TestMethod]
        public void GetEnumeratorGeneric()
        {
            _columnCollection.Add(_selectedColumn);
            _columnCollection.Add(_otherColumn);

            using var enumerator = _columnCollection.GetEnumerator();

            Assert.IsNull(enumerator.Current);

            enumerator.MoveNext();

            Assert.AreEqual(_selectedColumn, enumerator.Current);

            enumerator.MoveNext();

            Assert.AreEqual(_otherColumn, enumerator.Current);
        }

        [TestMethod]
        public void GetEnumerator()
        {
            _columnCollection.Add(_selectedColumn);
            _columnCollection.Add(_otherColumn);

            var enumerator = ((IEnumerable)_columnCollection).GetEnumerator();

            Assert.ThrowsException<InvalidOperationException>(() => enumerator.Current);

            enumerator.MoveNext();

            Assert.AreEqual(_selectedColumn, enumerator.Current);

            enumerator.MoveNext();

            Assert.AreEqual(_otherColumn, enumerator.Current);
        }

        [TestMethod]
        public void MergeColumns()
        {
            _columnCollection.Add(_selectedColumn);

            var list = new List<Column> { _selectedColumn, _otherColumn };

            _columnCollection.MergeColumns(list);

            Assert.AreEqual(list.Count, _columnCollection.Count);

            CollectionAssert.AreEquivalent(list, _columnCollection.ToList());
        }
    }
}