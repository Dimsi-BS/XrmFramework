using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XrmFramework.Core
{
    public class TableCollection : ICollection<Table>
    {
        private readonly ISet<Table> _tables = new SortedSet<Table>();

        /// <summary>
        /// The table declared under <paramref name="logicalName" />, or <see langword="null" /> when
        /// the compilation knows nothing of it.
        /// </summary>
        public Table Get(string logicalName)
        => string.IsNullOrEmpty(logicalName)
               ? null
               : _tables.FirstOrDefault(t => string.Equals(t.LogicalName, logicalName,
                                                           StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Adds a table, or folds it into the copy the collection already holds for the same
        /// logical name.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <see cref="Table.MergeTo" /> folds everything the incoming copy declares into the one
        /// already in place, which is therefore the only one the collection keeps. Adding the
        /// incoming copy as well used to be a no-op — the set compares tables on their name — but
        /// silently produced a second entry for the same table as soon as the two copies disagreed
        /// on that name, hence two definition classes for one table.
        /// </para>
        /// <para>
        /// The name and the lock are the two things that fold cannot take from the incoming copy's
        /// columns and option sets, because they belong to the table itself. A locked name wins,
        /// just as it does for a column or an option set: <c>Locked</c> marks the tables the
        /// framework ships, whose generated classes its own code refers to. Should both copies — or
        /// neither — be locked, the copy already in place keeps its name, and two unlocked copies
        /// disagreeing on it are what the generator's <c>XRM1001</c> diagnostic reports.
        /// </para>
        /// </remarks>
        public void Add(Table table)
        {
            if (table == null)
            {
                return;
            }

            var existingTable = _tables.FirstOrDefault(e => e.LogicalName == table.LogicalName);

            // The collection already holds this very table: there is nothing to fold it into but
            // itself, and MergeTo would walk its own collections while writing to them.
            if (ReferenceEquals(existingTable, table))
            {
                return;
            }

            table.MergeTo(existingTable);

            if (existingTable == null)
            {
                _tables.Add(table);
                return;
            }

            var lockedName = LockArbitration.LockedName(existingTable.IsLocked, existingTable.Name,
                                                        table.IsLocked, table.Name);

            // Name is the key the set orders on: the table has to leave before that key changes, or
            // it stays filed under a name it no longer carries.
            _tables.Remove(existingTable);

            if (lockedName != null)
            {
                existingTable.Name = lockedName;
            }

            existingTable.IsLocked |= table.IsLocked;

            _tables.Add(existingTable);
        }

        #region ICollection Implementation

        /// <inheritdoc />
        public void Clear()
        => _tables.Clear();

        /// <inheritdoc />
        public bool Contains(Table item)
        => _tables.Contains(item);

        /// <inheritdoc />
        public void CopyTo(Table[] array, int arrayIndex)
        => _tables.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public bool Remove(Table item)
        => _tables.Remove(item);

        /// <inheritdoc />
        public int Count => _tables.Count;

        /// <inheritdoc />
        public bool IsReadOnly => _tables.IsReadOnly;

        public void AddRange(IEnumerable<Table> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        /// <inheritdoc />
        public IEnumerator<Table> GetEnumerator()
        => _tables.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)_tables).GetEnumerator();
        #endregion
    }
}
