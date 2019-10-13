// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Linq;

namespace Model
{
    public static partial class EnumExtensions
    {
        public static OptionSetValue ToOptionSetValue(this Enum enumValue)
        {
            if (Enum.GetName(enumValue.GetType(), enumValue) == "Null" && enumValue.ToInt() == 0)
            {
                return null;
            }

            return new OptionSetValue(enumValue.ToInt());
        }
        public static OptionSetValueCollection ToOptionSetValueCollection<T>(this IEnumerable<T> enumValues) where T : Enum
        {
            var optionSetValues = new List<OptionSetValue>();

            foreach (var value in enumValues)
            {
                var o = value.ToOptionSetValue();

                if (o != null)
                {
                    optionSetValues.Add(o);
                }
            }

            return optionSetValues.Any() ? new OptionSetValueCollection(optionSetValues) : null;
        }
    }
}
