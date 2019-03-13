// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Model;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UnsecureConfigAttribute : Attribute
    {
        public UnsecureConfigAttribute(Type resourceType, string propertyName)
        {
            ResourceType = resourceType;
            PropertyName = propertyName;
        }

        public UnsecureConfigAttribute(string unsecureConfig)
        {
            UnsecureConfig = unsecureConfig;
        }

        public Type ResourceType { get; private set; }

        public string PropertyName { get; private set; }

        public string UnsecureConfig { get; private set; }
    }
}
