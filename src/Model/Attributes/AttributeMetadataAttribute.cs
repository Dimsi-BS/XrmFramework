// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Model.Sdk;
using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AttributeMetadataAttribute : Attribute
    {
        public AttributeMetadataAttribute(AttributeTypeCode type)
        {
            Type = type;
        }

        public AttributeTypeCode Type { get; private set; }
    }
}
