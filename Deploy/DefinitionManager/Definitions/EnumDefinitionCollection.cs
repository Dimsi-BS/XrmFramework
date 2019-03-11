using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinitionManager
{
    internal class EnumDefinitionCollection : DefinitionCollection<EnumDefinition>
    {
        private static EnumDefinitionCollection _instance = new EnumDefinitionCollection();
        public static EnumDefinitionCollection Instance { get { return _instance; } }
    }
}
