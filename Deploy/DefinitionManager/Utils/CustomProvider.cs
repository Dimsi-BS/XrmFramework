using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinitionManager
{
    internal static class CustomProvider
    {
        public static ICustomListProvider Instance { get; set; }
    }
}
