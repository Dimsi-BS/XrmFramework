using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XrmFramework.Core
{
    public class TableCollection : ICollection<Table>
    {
        private readonly ISet<Table> _tables = new SortedSet<Table>();

        public void Add(Table table)
        {
            if (table == null)
            {
                return;
            }

            var existingTable = _tables.FirstOrDefault(e => e.LogicalName == table.LogicalName);

            table.MergeTo(existingTable);

            _tables.Add(table);
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
