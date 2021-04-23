// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ExecutionOrderAttribute : Attribute
    {
        public int Order { get; private set; }

        public ExecutionOrderAttribute(int order)
        {
            Order = order;
        }
    }
}
