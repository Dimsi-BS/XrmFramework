// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DefinitionManager
{
    class CustomListViewItem<T> : ListViewItem where T : AbstractDefinition
    {
        private Dictionary<string, ListViewSubItem> _subItems = new();

        public CustomListViewItem(T definition)
        {
            Definition = definition;

            Checked = Definition.IsSelected;

            Text = definition.Name;

            foreach (var column in definition.GetColumns())
            {
                if (column.Property.Name == "Name")
                {
                    continue;
                }
                var item = new ListViewSubItem(this, column.Property.GetValue(definition) as string);
                _subItems.Add(column.Property.Name, item);
                SubItems.Add(item);
            }

            definition.PropertyChanged += definition_PropertyChanged;
        }

        void definition_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_subItems.ContainsKey(e.PropertyName))
            {
                var item = _subItems[e.PropertyName];
                item.Text = typeof(T).GetProperty(e.PropertyName).GetValue(Definition) as string;
            }
            else if (e.PropertyName == "Name")
            {
                (new Thread(() =>
                {
                    Thread.Sleep(50);
                    SetText(Definition.Name);
                })).Start();
            }
        }

        delegate void SetTextCallback(string text);

        private void SetText(string text)
        {
            if (this.ListView == null)
            {
                return;
            }

            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.ListView.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.ListView.Invoke(d, new object[] { text });
            }
            else
            {
                this.Text = text;
            }
        }


        public T Definition { get; private set; }
    }
}
