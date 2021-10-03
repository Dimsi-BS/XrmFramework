// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DefinitionManager
{
    public abstract class AbstractDefinition : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _previousName = null;
        private string _name = null;

        [Column("Name", -1)]
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrEmpty(_previousName))
                {
                    _previousName = value;
                }
                _name = value;

                OnPropertyChanged("Name");
            }
        }

        private string _logicalName = null;

        [Column("Logical Name", 0)]
        public string LogicalName
        {
            get => _logicalName;
            set
            {
                _logicalName = value;

                OnPropertyChanged("LogicalName");
            }
        }

        public bool IsLoaded { get; set; }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public virtual bool IncludeLogicalNameColumn => true;

        private bool _isSelected = false;
        public virtual bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;

                OnPropertyChanged("IsSelected");
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        public string PreviousName => _previousName;

        public bool NameHasChanged => Name != PreviousName;

        public bool Match(string filter)
        {
            var match = false;

            if (string.IsNullOrEmpty(filter))
            {
                match = true;
            }
            else
            {
                match |= Name.Contains(filter);
                foreach (var column in GetColumns().Where(c => c.Property.PropertyType == typeof(string)))
                {
                    var value = column.Property.GetValue(this) as string;

                    match |= (!string.IsNullOrEmpty(value) && value.FormatText().ToLowerInvariant().Contains(filter.FormatText().ToLowerInvariant()));
                }
            }

            return match;
        }

        public IEnumerable<Column> GetColumns()
        {
            var type = this.GetType();

            var columns = new SortedSet<Column>(new ColumnSorter());

            foreach (var property in type.GetProperties())
            {
                var columnAttribute = property.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault() as ColumnAttribute;

                if (columnAttribute != null)
                {
                    columns.Add(new Column
                    {
                        Property = property,
                        Order = columnAttribute.Order,
                        DisplayName = columnAttribute.DisplayName,
                        Width = columnAttribute.Width
                    });
                }
            }

            return columns;
        }

        public virtual string UnselectionWarning => null;

        public class Column
        {
            public PropertyInfo Property { get; set; }
            public string DisplayName { get; set; }
            public int Order { get; set; }

            public int Width { get; set; }
        }

        private class ColumnSorter : IComparer<Column>
        {
            public int Compare(Column x, Column y)
            {
                return x.Order.CompareTo(y.Order);
            }
        }
        public void Load(bool attach = true)
        {
            LoadInternal(attach);
        }
        public void UnLoad()
        {
            UnLoadInternal();
        }

        protected virtual void LoadInternal(bool attach = true) { }

        protected virtual void UnLoadInternal() { }

        internal void Merge(AbstractDefinition definition)
        {
            if (this.GetType().Name != definition.GetType().Name)
            {
                throw new Exception("Only similar types can be merged.");
            }

            IsSelected = true;

            foreach (var column in this.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(MergeableAttribute), false).Any()))
            {
                if (column.GetSetMethod() == null)
                {
                    continue;
                }

                if (column.GetValue(this) == null || column.PropertyType == typeof(Boolean) || (typeof(System.Collections.IEnumerable).IsAssignableFrom(column.PropertyType) && column.PropertyType != typeof(string)))
                {
                    column.SetValue(this, column.GetValue(definition));
                }
            }

            MergeInternal(definition);
        }

        public bool CheckName(ref string name)
        {
            var ok = CheckNameInternal(ref name);

            if (name.Contains("'") || name.Contains("/") || name.Contains("(") || name.Contains(")") || name.Contains(",") || name.Contains(";") || name.Contains("\""))
            {
                ok = false;
            }

            name = name.Substring(0, 1).ToUpperInvariant() + name.Substring(1);

            return ok;
        }

        public virtual bool CheckNameInternal(ref string name)
        {
            return true;
        }

        protected virtual void MergeInternal(AbstractDefinition definition)
        {
        }
    }
}
