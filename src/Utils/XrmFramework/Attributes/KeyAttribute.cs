// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class KeyAttribute : Attribute
    {
        public KeyAttribute(string keyName)
        {
            KeyName = keyName;
        }

        public string KeyName { get; private set; }
    }
}
