// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using XrmFramework.DefinitionManager;

namespace DefinitionManager
{
    internal class DefinitionCollection<T> where T : AbstractDefinition, new()
    {
        public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);
        public event TextChangedEventHandler TextChanged;

        private SortedSet<T> _definitions = new(new DefinitionComparer<T>());

        private static object syncRoot = new();

        public IReadOnlyList<T> Definitions => _definitions.ToList();

        private CustomListViewControl<T> _listView;

        private T ActiveDefinition { get; set; }

        private string _filter = null;

        public void AttachListView(CustomListViewControl<T> listView)
        {
            if (_listView != listView)
            {
                _listView = listView;

                _listView.FilterTextChanged += _listView_FilterTextChanged;
                _listView.SelectionChanged += _listView_SelectionChanged;

                FillListView();
            }
        }

        internal void DetachListView()
        {
            if (_listView != null)
            {
                _listView.FilterTextChanged -= _listView_FilterTextChanged;
                _listView.SelectionChanged -= _listView_SelectionChanged;
                _listView.Clear();
            }
            _listView = null;
            UnLoadDefinitions();
        }

        private void UnLoadDefinitions()
        {
            foreach (var def in _definitions)
            {
                def.UnLoad();
            }
        }

        void _listView_FilterTextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.TextChanged != null)
            {
                this.TextChanged(this, new TextChangedEventArgs { Text = e.Text });
            }
            else
            {
                UnLoadDefinitions();

                _filter = e.Text;
                FillListView();
            }
        }

        void FillListView()
        {
            if (_listView != null)
            {
                _listView.SetVisible(_definitions.Where(d => d.Match(_filter)));
            }
        }

        public void AddRange(IEnumerable<T> definitions)
        {
            foreach (var definition in definitions)
            {
                if (_definitions.Contains(definition))
                {
                    _definitions.First(d => _definitions.Comparer.Compare(d, definition) == 0).Merge(definition);
                }
                else
                {
                    _definitions.Add(definition);
                }
            }
            FillListView();
        }

        void _listView_SelectionChanged(object sender, CustomListViewControl<T>.SelectionChangedEventArgs e)
        {
            e.Definition.IsActive = e.IsSelected;

            if (e.IsSelected)
            {
                e.Definition.Load();
            }
            else
            {
                e.Definition.UnLoad();
            }
        }

        public void Add(T definition)
        {
            if (_definitions.Contains(definition))
            {
                this[definition.LogicalName].Merge(definition);
            }
            else
            {
                _definitions.Add(definition);
            }

            FillListView();
        }

        public bool Contains(string logicalName)
        {
            return Definitions.Any(d => d.LogicalName == logicalName);
        }

        public T this[string logicalName]
        {
            get
            {
                var definition = _definitions.FirstOrDefault(e => e.LogicalName == logicalName);

                return definition;
            }
        }

        internal void Merge(DefinitionCollection<T> definitionCollection)
        {
            foreach (var def in definitionCollection.Definitions)
            {
                if (!_definitions.Contains(def))
                {
                    _definitions.Add(def);
                }
                else
                {
                    this[def.LogicalName].Merge(def);
                }
            }
        }

        public IEnumerable<T> SelectedDefinitions { get { return Definitions.Where(d => d.IsSelected); } }
    }
}
