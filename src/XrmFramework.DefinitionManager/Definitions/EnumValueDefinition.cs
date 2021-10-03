// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace DefinitionManager
{
    class EnumValueDefinition : AbstractDefinition
    {
        [Column("Display Name", 0, 300)]
        public string DisplayName { get; set; }

        [Column("Value", 1)]
        public string Value { get; set; }

        [Column("External Value", 2)]
        public string ExternalValue { get; set; }

        public override bool IncludeLogicalNameColumn
        {
            get
            {
                return false;
            }
        }
    }
}
