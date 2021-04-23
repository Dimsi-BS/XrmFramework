// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinitionManager
{
    internal class ClassDefinition : AbstractDefinition
    {
        public bool IsEnum { get; set; }

        public IList<Attribute> ClassAttributes { get; } = new List<Attribute>();

        public DefinitionCollection<AttributeDefinition> Attributes { get; } = new DefinitionCollection<AttributeDefinition>();

        protected override void MergeInternal(AbstractDefinition definition)
        {
            Attributes.Merge(((ClassDefinition)definition).Attributes);
        }
    }
}
