// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace DefinitionManager
{
    internal class EnumDefinitionCollection : DefinitionCollection<EnumDefinition>
    {
        private static EnumDefinitionCollection _instance = new();
        public static EnumDefinitionCollection Instance { get { return _instance; } }
    }
}
