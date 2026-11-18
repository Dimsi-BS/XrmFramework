using System;
using System.Collections;
using System.Collections.Generic;

namespace XrmFramework.Core
{
    public sealed class ColumnCollection : ICollection<Column>
    {
        private readonly SortedList<string, Column> _columns = new();

        /// <summary>
        /// Adds a column, or folds it into the one the collection already holds for the same
        /// logical name.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A table shipped by the framework and tracked again by the project is declared twice, so
        /// the same column reaches this collection from both copies. The selected copy brings the
        /// C# name the generated code will carry: an unselected copy generates nothing and
        /// therefore has no say in the naming.
        /// </para>
        /// <para>
        /// Unless the column is locked. A locked column's name is shipped by the framework along
        /// with the code that refers to it — <c>TeamDefinition.Columns.Type</c> — and a copy the
        /// project keeps only in order to enrich the table must not rename it. The lock itself
        /// describes the column, not the copy that happened to be loaded first, so it has to
        /// survive the fold whichever way round the two arrive: without that, the arbitration
        /// rests on the order <c>AdditionalFiles</c> reaches the generator in.
        /// </para>
        /// </remarks>
        public void Add(Column item)
        {
            if (item == null)
            {
                return;
            }

            if (_columns.TryGetValue(item.LogicalName, out var existingColumn))
            {
                // Null when both copies agree on the lock: there is then no locked name to impose,
                // and the selected copy names the column as it otherwise does.
                var lockedName = LockArbitration.LockedName(existingColumn.IsLocked, existingColumn.Name,
                                                            item.IsLocked, item.Name);

                if (item.Selected)
                {
                    existingColumn.Name = lockedName ?? item.Name;
                    existingColumn.Selected = true;
                    existingColumn.IsLocked |= item.IsLocked;
                }
                else if (existingColumn.Selected)
                {
                    item.Name = lockedName ?? existingColumn.Name;
                    item.Selected = true;
                    item.IsLocked |= existingColumn.IsLocked;

                    _columns[item.LogicalName] = item;
                }
            }
            else
            {
                _columns.Add(item.LogicalName, item);
            }
        }

        public void MergeColumns(IEnumerable<Column> items)
        {
            foreach (var column in items)
            {
                Add(column);
            }
        }

        public void RemoveAll(Func<Column, bool> predicate)
        {
            for (var i = _columns.Values.Count - 1; i >= 0; i--)
            {
                if (predicate(_columns.Values[i]))
                {
                    _columns.Remove(_columns.Values[i].LogicalName);
                }
            }
        }

        #region ICollection implementation

        public void Clear() => _columns.Clear();

        public IEnumerator<Column> GetEnumerator() => _columns.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(Column item)
        {
            if (item == null)
            {
                return false;
            }

            return _columns.ContainsKey(item.LogicalName);
        }

        public void CopyTo(Column[] array, int arrayIndex) => _columns.Values.CopyTo(array, arrayIndex);

        public bool Remove(Column item)
        {
            if (item == null)
            {
                return false;
            }

            return _columns.Remove(item.LogicalName);
        }

        public int Count => _columns.Count;

        public bool IsReadOnly => false;

        #endregion
    }
}
