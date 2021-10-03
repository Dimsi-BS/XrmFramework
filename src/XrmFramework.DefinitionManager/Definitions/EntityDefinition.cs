// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace DefinitionManager
{
    class EntityDefinition : AbstractDefinition
    {
        [Mergeable]
        public bool IsActivity { get; set; }

        private readonly DefinitionCollection<AttributeDefinition> _attributes = new();
        public DefinitionCollection<AttributeDefinition> AttributesCollection { get { return _attributes; } }

        private readonly DefinitionCollection<ClassDefinition> _additionalClasses = new();
        public DefinitionCollection<ClassDefinition> AdditionalClassesCollection { get { return _additionalClasses; } }


        private DefinitionCollection<AttributeDefinition> _additionalInfos = new();
        public DefinitionCollection<AttributeDefinition> AdditionalInfoCollection { get { return _additionalInfos; } }

        public override string UnselectionWarning
        {
            get
            {
                return string.Format("The definition will be deleted.\r\nWould you like to continue ?");
            }
        }

        public string LogicalCollectionName { get; set; }

        public AttributeDefinition this[string attributeName]
        {
            get
            {
                return _attributes[attributeName];
            }
        }

        internal void Add(AttributeDefinition attributeDefinition)
        {
            _attributes.Add(attributeDefinition);
        }

        protected override void LoadInternal(bool attach = true)
        {
            if (!IsLoaded)
            {
                DataAccessManager.Instance.RetrieveAttributes(this, RetrieveAttributes);
            }

            if (attach)
            {
                AttributesCollection.AttachListView(CustomProvider.Instance.GetCustomList<CustomListViewControl<AttributeDefinition>>());
            }
        }

        protected override void UnLoadInternal()
        {
            AttributesCollection.DetachListView();
        }

        void RetrieveAttributes(object attr)
        {
            var attributes = (IEnumerable<AttributeDefinition>)attr;

            AttributesCollection.AddRange(attributes);
            IsLoaded = true;
        }

        protected override void MergeInternal(AbstractDefinition definition)
        {
            var def = definition as EntityDefinition;

            if (def == null) return;

            LogicalCollectionName = def.LogicalCollectionName;

            IsLoaded = definition.IsLoaded;

            _attributes.AddRange(def.AttributesCollection.Definitions);

            _additionalClasses.AddRange(def.AdditionalClassesCollection.Definitions);

            _additionalInfos.AddRange(def.AdditionalInfoCollection.Definitions);
        }

        public override bool CheckNameInternal(ref string name)
        {
            if (!name.EndsWith("Definition"))
            {
                name = string.Format("{0}Definition", name);
            }
            return true;
        }
    }
}
