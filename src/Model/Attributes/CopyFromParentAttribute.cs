// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=true)]
    public class CopyFromParentAttribute : Attribute
    {
        public CopyFromParentAttribute(Type parentType, string parentPropertyName)
        {
            ParentType = parentType;
            ParentPropertyName = parentPropertyName;
        }

        public Type ParentType { get; set; }

        public string ParentPropertyName { get; set; }
    }
}
