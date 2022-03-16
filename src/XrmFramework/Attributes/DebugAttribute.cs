// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DebugAttribute : Attribute
    {
        public bool IsDebugFunction { get; private set; } 

        public DebugAttribute()
        {
            IsDebugFunction = true;
        }
        public DebugAttribute(bool isDebug)
        {
            IsDebugFunction = isDebug;
        }
    }
}
