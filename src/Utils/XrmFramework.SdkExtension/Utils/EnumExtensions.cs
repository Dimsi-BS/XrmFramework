// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using System;
using System.Reflection;
using System.ComponentModel;

namespace Model
{
    public static partial class EnumExtensions
    {
        public static OptionSetValue ToOptionSetValue(this Enum enumValue)
        {
            return new OptionSetValue(enumValue.ToInt());
        }
    }
}
