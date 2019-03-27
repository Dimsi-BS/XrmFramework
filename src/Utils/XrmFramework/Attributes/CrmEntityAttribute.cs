// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CrmEntityAttribute : Attribute
    {
        public CrmEntityAttribute(string entityName)
        {
            EntityName = entityName;
        }

        public string EntityName { get; private set; }

        public bool ValidForCreate { get; set; } = true;

        public bool AllowDeactivation { get; set; } = true;
    }
}