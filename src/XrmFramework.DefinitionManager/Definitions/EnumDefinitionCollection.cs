// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
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
