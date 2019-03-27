// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DependentAttribute : Attribute
    {
        public DependentAttribute(string attributeName)
        {
            AttributeName = attributeName;
        }

        public string AttributeName { get; private set; }
    }
}
