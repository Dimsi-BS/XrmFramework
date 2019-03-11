using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinitionManager
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
