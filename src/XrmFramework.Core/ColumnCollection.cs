using System.Collections;
using System.Collections.Generic;

namespace XrmFramework.Core
{
    public class ColumnCollection : ICollection<Column>
    {
        private readonly SortedList<string, Column> _columns = new();

        public void Add(Column item)
        {
            if (item == null)
            {
                return;
            }

            if (_columns.TryGetValue(item.LogicalName, out var existingColumn))
            {
                if (item.Selected)
                {
                    existingColumn.Name = item.Name;
                    existingColumn.Selected = true;
                }
                else if (existingColumn.Selected)
                {
                    item.Name = existingColumn.Name;
                    item.Selected = true;

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

        public void RemoveNonSelectedColumns()
        {
            List<string> keysToDelete = new List<string>();
            foreach(var column in _columns)
            {
                if(!column.Value.Selected)
                {
                    keysToDelete.Add(column.Key);
                    
                }
            }
            foreach(var key in keysToDelete)
            {
                _columns.Remove(key);
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

        public bool IsReadOnly => _columns.Values.IsReadOnly;

        #endregion
    }
}
