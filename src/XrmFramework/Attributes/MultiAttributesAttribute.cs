// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MultiAttributesAttribute : Attribute
    {
        public MultiAttributesAttribute(int startIndex, int endIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public int StartIndex { get; private set; }

        public int EndIndex { get; private set; }
    }
}