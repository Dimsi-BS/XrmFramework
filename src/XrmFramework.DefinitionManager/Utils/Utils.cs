// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using DefinitionManager;

namespace XrmFramework.DefinitionManager
{
    internal class IntermediateEntityListViewControl : CustomListViewControl<EntityDefinition> { }
    internal class EntityListViewControl : IntermediateEntityListViewControl { }

    internal class IntermediateAttributeListViewControl : CustomListViewControl<AttributeDefinition>
    {
    }
    internal class AttributeListViewControl : IntermediateAttributeListViewControl { }

    internal class IntermediateEnumListViewControl : CustomListViewControl<EnumValueDefinition>
    {
        public override string Label
        {
            get
            {
                return "Name";
            }
        }
        protected override string DefaultSortColumn
        {
            get
            {
                return "Value";
            }
        }
        public override bool ShowCheckBoxes
        {
            get
            {
                return false;
            }
        }
    }
    internal class EnumListViewControl : IntermediateEnumListViewControl { }
}
