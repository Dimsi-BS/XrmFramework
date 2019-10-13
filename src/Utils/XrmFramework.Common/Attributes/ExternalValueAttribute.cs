// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Model.Sdk;
using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ExternalValueAttribute : Attribute
    {
        public ExternalValueAttribute(string externalValue)
        {
            ExternalValue = externalValue;
        }

        public string ExternalValue { get; }
    }
}
