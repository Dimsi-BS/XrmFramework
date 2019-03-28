// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Model
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