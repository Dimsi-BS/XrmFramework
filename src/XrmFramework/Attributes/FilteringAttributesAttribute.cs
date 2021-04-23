// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FilteringAttributesAttribute : Attribute
    {
        public string[] Attributes { get; private set; }

        public FilteringAttributesAttribute(params string[] attributes)
        {
            Attributes = attributes;
        }
    }
}
