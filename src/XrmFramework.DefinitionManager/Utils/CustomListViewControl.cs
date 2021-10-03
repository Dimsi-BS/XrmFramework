// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace DefinitionManager
{
    internal partial class CustomListViewControl<T> : UserControl where T : AbstractDefinition, new()
    {
        public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);
        public delegate void FilterTextChangedEventHandler(object sender, TextChangedEventArgs e);

        public event SelectionChangedEventHandler SelectionChanged;
        public event FilterTextChangedEventHandler FilterTextChanged;

        private SortedList<T, CustomListViewItem<T>> _customItems = new(new DefinitionComparer<T>());

        private IEnumerable<T> _currentlyVisibleDefinitions;

        private readonly DefinitionComparer<T> _sortComparer;

        public CustomListViewControl()
        {
            InitializeComponent();

            this.listView.CheckBoxes = ShowCheckBoxes;

            _sortComparer = new DefinitionComparer<T>(DefaultSortColumn);

            var t = new T();

            var columns = t.GetColumns();

            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            foreach (var column in columns)
            {
                var columnHeader = new System.Windows.Forms.ColumnHeader();
                columnHeader.Name = column.Property.Name;
                columnHeader.Width = column.Width;
                columnHeader.Text = column.DisplayName;

                this.listView.Columns.Add(columnHeader);
            }

            if (!DisplayFilter)
            {
                this.tableLayoutPanel.RowStyles[0].Height = 0;
            }

            this.textBoxLabel.Text = Label;

            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        protected virtual string DefaultSortColumn { get { return "Name"; } }
        void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var columnName = this.listView.Columns[e.Column].Name;

            if (_sortComparer.PropertyName == columnName)
            {
                _sortComparer.AscendingOrder = !_sortComparer.AscendingOrder;
            }
            else
            {
                _sortComparer.AscendingOrder = true;
                _sortComparer.PropertyName = columnName;
            }
            this.listView.Items.Clear();

            Render();
        }

        private void Render()
        {
            //listView.SuspendLayout();

            this.listView.Items.Clear();

            var sw = new Stopwatch();
            sw.Start();

            var items = _customItems.Keys.Intersect(_currentlyVisibleDefinitions).ToList();

            var filterTime = sw.Elapsed;
            sw.Restart();

            var items2 = items.OrderBy(s => s, _sortComparer).ToList();

            var sortTime = sw.Elapsed;
            sw.Restart();

            var items3 = items2.Select(key => _customItems[key]).ToList();

            var selectTime = sw.Elapsed;
            sw.Restart();

            var items4 = items3.ToArray();

            var toArrayTime = sw.Elapsed;

            this.listView.Items.AddRange(items4);

            sw.Stop();
            Console.WriteLine(string.Format("Sort : {0}, Filter : {1}, Select : {2}, ToArray : {3}", sortTime, filterTime, selectTime, toArrayTime));
            //listView.ResumeLayout(true);
        }

        public void Clear()
        {
            Text = null;
            _customItems.Clear();
            listView.Items.Clear();
            _sortComparer.PropertyName = DefaultSortColumn;
            _sortComparer.AscendingOrder = true;
        }

        public void SetVisible(IEnumerable<T> definitions)
        {
            foreach (var def in definitions)
            {
                Add(def);
            }

            _currentlyVisibleDefinitions = definitions;

            Render();
        }

        private void Add(T item)
        {
            CustomListViewItem<T> listViewItem;

            if (!_customItems.ContainsKey(item))
            {
                listViewItem = new CustomListViewItem<T>(item);
                _customItems[item] = listViewItem;
            }
        }

        public virtual bool DisplayFilter
        {
            get { return true; }
        }

        private void listView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Label))
            {
                e.CancelEdit = true;
                return;
            }

            var item = this.listView.Items[e.Item] as CustomListViewItem<T>;

            var label = e.Label;

            if (!item.Definition.CheckName(ref label))
            {
                e.CancelEdit = true;
                return;
            }
            
            item.Definition.Name = label;
        }

        private void filterTextBox_TextChanged(object sender, EventArgs e)
        {
            RaiseFilterTextChanged(this.filterTextBox.Text);
        }

        public class SelectionChangedEventArgs
        {
            public T Definition { get; set; }

            public bool IsSelected { get; set; }
        }

        private void listView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var item = e.Item as CustomListViewItem<T>;

            RaiseSelectionChanged(item.Definition, e.IsSelected);
        }

        private void RaiseSelectionChanged(T definition, bool isSelected)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, new SelectionChangedEventArgs { Definition = definition, IsSelected = isSelected });
            }
        }

        private void RaiseFilterTextChanged(string filterText)
        {
            if (FilterTextChanged != null)
            {
                FilterTextChanged(this, new TextChangedEventArgs { Text = filterText });
            }
        }

        private void listView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var item = listView.Items[e.Index] as CustomListViewItem<T>;

            if (e.NewValue == CheckState.Unchecked && !string.IsNullOrEmpty(item.Definition.UnselectionWarning))
            {
                var result = MessageBox.Show(item.Definition.UnselectionWarning, "Warning", MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                {
                    e.NewValue = CheckState.Checked;
                }
            }

            item.Definition.IsSelected = e.NewValue == CheckState.Checked;
        }

        public virtual string Label { get { return "Filter"; } }

        public new string Text { get { return this.filterTextBox.Text; } set { this.filterTextBox.Text = value; RaiseFilterTextChanged(value); } }

        private void filterTextBox_KeyUp(object sender, KeyEventArgs e)
        {

        }

        public virtual bool ShowCheckBoxes { get { return true; } }
    }
}
