// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework.DefinitionManager.Definitions;

internal class EnumDefinitionCollection : DefinitionCollection<EnumDefinition>
{
    public static EnumDefinitionCollection Instance { get { return field; } } = new();
}