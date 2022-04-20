﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace XrmFramework.Core
{
    public sealed class ColumnCollection : ICollection<Column>
    {
        internal readonly SortedList<string, Column> Columns = new();

        public void Add(Column? item)
        {
            if (item == null)
            {
                return;
            }

            if (Columns.TryGetValue(item.LogicalName, out var existingColumn))
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

                    Columns[item.LogicalName] = item;
                }
            }
            else
            {
                Columns.Add(item.LogicalName, item);
            }
        }

        public void RemoveAll(Func<Column, bool> predicate)
        {
            for (var i = Columns.Values.Count - 1; i >= 0; i--)
            {
                if (predicate(Columns.Values[i]))
                {
                    Columns.Remove(Columns.Values[i].LogicalName);
                }
            }
        }

        public void RemoveNonSelectedColumns()
        {
            List<string> keysToDelete = new List<string>();
            foreach (var column in Columns)
            {
                if (!column.Value.Selected)
                {
                    keysToDelete.Add(column.Key);

                }
            }
            foreach (var key in keysToDelete)
            {
                Columns.Remove(key);
            }
        }
        #region ICollection implementation

        public void Clear() => Columns.Clear();

        public IEnumerator<Column> GetEnumerator() => Columns.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(Column item)
        {
            if (item == null)
            {
                return false;
            }

            return Columns.ContainsKey(item.LogicalName);
        }

        public void CopyTo(Column[] array, int arrayIndex) => Columns.Values.CopyTo(array, arrayIndex);

        public bool Remove(Column item)
        {
            if (item == null)
            {
                return false;
            }

            return Columns.Remove(item.LogicalName);
        }

        public int Count => Columns.Count;

        public bool IsReadOnly => false;

        #endregion
    }
}
