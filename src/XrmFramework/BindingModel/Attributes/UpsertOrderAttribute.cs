// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework.BindingModel
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UpsertOrderAttribute : Attribute
    {
        public UpsertOrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; private set; }
    }
}
