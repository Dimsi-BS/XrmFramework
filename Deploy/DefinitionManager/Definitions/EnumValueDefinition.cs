using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinitionManager
{
    class EnumValueDefinition : AbstractDefinition
    {
        [Column("Display Name", 0, 300)]
        public string DisplayName { get; set; }

        [Column("Value", 1)]
        public string Value { get; set; }

        public override bool IncludeLogicalNameColumn
        {
            get
            {
                return false;
            }
        }
    }
}
