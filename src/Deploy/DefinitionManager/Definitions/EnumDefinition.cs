// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Model.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinitionManager
{
    class EnumDefinition : AbstractDefinition
    {
        private DefinitionCollection<EnumValueDefinition> _values = new DefinitionCollection<EnumValueDefinition>();

        private HashSet<AttributeDefinition> _referencingAttributes = new HashSet<AttributeDefinition>(new DefinitionComparer<AttributeDefinition>());

        public IReadOnlyCollection<AttributeDefinition> ReferencedBy { get { return _referencingAttributes.ToList(); } }

        public void Register(AttributeDefinition definition)
        {
            _referencingAttributes.Add(definition);

            OnPropertyChanged("IsSelected");
        }

        public void UnRegister(AttributeDefinition definition)
        {
            _referencingAttributes.Remove(definition);

            OnPropertyChanged("IsSelected");
        }

        void Values_TextChanged(object sender, TextChangedEventArgs e)
        {
            Name = e.Text;
        }

        [Mergeable]
        public bool HasNullValue { get; set; }

        public override bool IsSelected
        {
            get
            {
                return _referencingAttributes.Any();
            }
            set
            {
            }
        }

        public DefinitionCollection<EnumValueDefinition> Values { get { return _values; } }

        public bool IsGlobal { get; set; }

        protected override void LoadInternal(bool attach = true)
        {
            var listView = CustomProvider.Instance.GetCustomList<CustomListViewControl<EnumValueDefinition>>();
            listView.Text = Name;
            Values.AttachListView(listView);
            Values.TextChanged += Values_TextChanged;
        }

        protected override void UnLoadInternal()
        {
            Values.TextChanged -= Values_TextChanged;
            Values.DetachListView();
        }

        protected override void MergeInternal(AbstractDefinition definition)
        {
            var def = (EnumDefinition)definition;

            Values.Merge(def.Values);
        }
    }
}
