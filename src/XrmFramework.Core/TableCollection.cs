﻿using System.Collections;
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
            if(existingTable != null)
            {
                if (table.isLocked || table.Selected)
                {
                    existingTable.Name = table.Name;
                    existingTable.Selected = true;
                }
                else if (existingTable.isLocked || existingTable.Selected)
                {
                    table.Name = existingTable.Name;
                    table.Selected = true;

                }

                foreach(var en in table.Enums)
                {
                    var correspondingEnum = existingTable.Enums.FirstOrDefault(e => e.LogicalName == en.LogicalName);
                    if(correspondingEnum != null)
                    {
                        if(correspondingEnum.IsLocked)
                        {
                            en.Name = correspondingEnum.Name;

                        }
                        else if(en.IsLocked)
                        {
                            correspondingEnum.Name = en.Name;
                        }
                    }
                }


                table.MergeTo(existingTable);
            }
            
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
