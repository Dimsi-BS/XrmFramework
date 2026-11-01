using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XrmFramework.Core.Tests
{
    [TestFixture]
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

        [SetUp]
        public void InitTests()
        {
            _columnCollection = new ColumnCollection();
        }

        [Test]
        public void ObjectInitialization()
        {
            Assert.IsNotNull(_columnCollection.GetEnumerator());

            Assert.AreEqual(0, _columnCollection.Count);

        }

        [Test]
        public void AddColumn()
        {
            _columnCollection.Add(null);


            _columnCollection.Add(_selectedColumn);

            Assert.AreEqual(1, _columnCollection.Count);
            Assert.That(_columnCollection.ToList(), Is.EquivalentTo(new List<Column> { _selectedColumn }));


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

            Assert.That(_columnCollection.ToList(), Is.EquivalentTo(new List<Column> { _otherColumn, _notSelectedColumn }));
        }

        // ──────────────────────────────────────────────────────────────────────
        // Folding the two copies a framework table comes in
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// <c>teamtype</c> as the XrmFramework package declares it: named for the code shipped
        /// alongside it (<c>TeamDefinition.Columns.Type</c>), and locked for that very reason.
        /// </summary>
        private static Column FrameworkTeamType(bool selected = true) => new Column
        {
            LogicalName = "teamtype",
            Name = "Type",
            IsLocked = true,
            Selected = selected
        };

        /// <summary>
        /// The same column as a project tracking its own copy of <c>team</c> pulled it: named after
        /// the CRM schema name, and unaware of the lock.
        /// </summary>
        private static Column ProjectTeamType(bool selected = true) => new Column
        {
            LogicalName = "teamtype",
            Name = "TeamType",
            Selected = selected
        };

        [Test]
        public void Add_LockedColumnThenUnlockedCopy_KeepsTheLockedName()
        {
            _columnCollection.Add(FrameworkTeamType());
            _columnCollection.Add(ProjectTeamType());

            var column = _columnCollection.Single();

            Assert.AreEqual("Type", column.Name);
            Assert.IsTrue(column.IsLocked);
            Assert.IsTrue(column.Selected);
        }

        /// <summary>
        /// The mirror case: the arbitration must not depend on the order the two copies reach the
        /// collection in, which is the order <c>AdditionalFiles</c> happens to list them in.
        /// </summary>
        [Test]
        public void Add_UnlockedCopyThenLockedColumn_KeepsTheLockedName()
        {
            _columnCollection.Add(ProjectTeamType());
            _columnCollection.Add(FrameworkTeamType());

            var column = _columnCollection.Single();

            Assert.AreEqual("Type", column.Name);
            Assert.IsTrue(column.IsLocked);
            Assert.IsTrue(column.Selected);
        }

        /// <summary>
        /// The branch that swaps the kept column for the incoming one: the lock describes the
        /// column, so it has to make the trip.
        /// </summary>
        [Test]
        public void Add_LockedColumnThenUnselectedUnlockedCopy_KeepsLockAndName()
        {
            _columnCollection.Add(FrameworkTeamType());
            _columnCollection.Add(ProjectTeamType(selected: false));

            var column = _columnCollection.Single();

            Assert.AreEqual("Type", column.Name);
            Assert.IsTrue(column.IsLocked);
            Assert.IsTrue(column.Selected);
        }

        [Test]
        public void Add_UnlockedCopyThenUnselectedLockedColumn_KeepsLockAndName()
        {
            _columnCollection.Add(ProjectTeamType());
            _columnCollection.Add(FrameworkTeamType(selected: false));

            var column = _columnCollection.Single();

            Assert.AreEqual("Type", column.Name);
            Assert.IsTrue(column.IsLocked);
            Assert.IsTrue(column.Selected);
        }

        /// <summary>
        /// Nothing locked: the selected copy coming in still names the column, as it always has.
        /// </summary>
        [Test]
        public void Add_TwoUnlockedSelectedCopies_TheIncomingCopyNamesTheColumn()
        {
            _columnCollection.Add(ProjectTeamType());
            _columnCollection.Add(new Column { LogicalName = "teamtype", Name = "Renamed", Selected = true });

            var column = _columnCollection.Single();

            Assert.AreEqual("Renamed", column.Name);
            Assert.IsFalse(column.IsLocked);
        }

        /// <summary>
        /// Two locked copies agree on the lock, so neither has a locked name to impose over the
        /// other: the ordinary rule applies again.
        /// </summary>
        [Test]
        public void Add_TwoLockedSelectedCopies_TheIncomingCopyNamesTheColumn()
        {
            _columnCollection.Add(FrameworkTeamType());
            _columnCollection.Add(new Column
            {
                LogicalName = "teamtype",
                Name = "Renamed",
                IsLocked = true,
                Selected = true
            });

            var column = _columnCollection.Single();

            Assert.AreEqual("Renamed", column.Name);
            Assert.IsTrue(column.IsLocked);
        }

        [Test]
        public void Add_LockedColumnWithoutAName_ImposesNothing()
        {
            _columnCollection.Add(new Column { LogicalName = "teamtype", IsLocked = true, Selected = true });
            _columnCollection.Add(ProjectTeamType());

            var column = _columnCollection.Single();

            Assert.AreEqual("TeamType", column.Name);
            Assert.IsTrue(column.IsLocked);
        }

        [Test]
        public void RemoveColumn()
        {
            _columnCollection.Add(_selectedColumn);

            Assert.IsFalse(_columnCollection.Remove(null));

            _columnCollection.Remove(_notSelectedColumn);

            Assert.AreEqual(0, _columnCollection.Count);
        }

        [Test]
        public void ContainsColumn()
        {
            _columnCollection.Add(_selectedColumn);

            Assert.IsFalse(_columnCollection.Contains(null));

            Assert.IsTrue(_columnCollection.Contains(_notSelectedColumn));
        }

        [Test]
        public void IsReadOnly()
        {
            Assert.IsFalse(_columnCollection.IsReadOnly);
        }

        [Test]
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

        [Test]
        public void GetEnumerator()
        {
            _columnCollection.Add(_selectedColumn);
            _columnCollection.Add(_otherColumn);

            var enumerator = ((IEnumerable)_columnCollection).GetEnumerator();

            Assert.Throws<InvalidOperationException>(() => { _ = enumerator.Current; });

            enumerator.MoveNext();

            Assert.AreEqual(_selectedColumn, enumerator.Current);

            enumerator.MoveNext();

            Assert.AreEqual(_otherColumn, enumerator.Current);
        }

        [Test]
        public void MergeColumns()
        {
            _columnCollection.Add(_selectedColumn);

            var list = new List<Column> { _selectedColumn, _otherColumn };

            _columnCollection.MergeColumns(list);

            Assert.AreEqual(list.Count, _columnCollection.Count);

            Assert.That(_columnCollection.ToList(), Is.EquivalentTo(list));
        }
    }
}