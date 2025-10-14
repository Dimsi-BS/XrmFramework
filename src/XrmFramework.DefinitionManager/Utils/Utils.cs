// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using XrmFramework.DefinitionManager.Definitions;

namespace XrmFramework.DefinitionManager;

internal class IntermediateEntityListViewControl : CustomListViewControl<XrmFramework.DefinitionManager.Definitions.EntityDefinition> { }
internal class EntityListViewControl : IntermediateEntityListViewControl { }

internal class IntermediateAttributeListViewControl : CustomListViewControl<XrmFramework.DefinitionManager.Definitions.AttributeDefinition>
{
}
internal class AttributeListViewControl : IntermediateAttributeListViewControl { }

internal class IntermediateEnumListViewControl : CustomListViewControl<EnumValueDefinition>
{
    public override string Label => "Name";

    protected override string DefaultSortColumn => "Value";

    public override bool ShowCheckBoxes => false;
}
internal class EnumListViewControl : IntermediateEnumListViewControl { }