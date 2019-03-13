// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Model.Sdk;

namespace DefinitionManager.Definitions
{
    internal class RelationshipAttributeDefinition : AttributeDefinition
    {
        public EntityRole Role {get; set; }

        public string TargetEntityName { get; set; }

        public string NavigationPropertyName { get; set; }

        public string LookupFieldName { get; set; }

        protected override void MergeInternal(AbstractDefinition definition)
        {
            var d = (RelationshipAttributeDefinition) definition;

            Role = d.Role;
            TargetEntityName = d.TargetEntityName;
            NavigationPropertyName = d.NavigationPropertyName;
            LookupFieldName = d.LookupFieldName;
        }
    }
}
