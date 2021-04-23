// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PrimaryAttributeAttribute : Attribute
    {
        public PrimaryAttributeAttribute(PrimaryAttributeType type)
        {
            Type = type;
        }

        public PrimaryAttributeType Type { get; private set; }
    }

    public enum PrimaryAttributeType
    {
        Id,
        Name,
        Image
    }
}
