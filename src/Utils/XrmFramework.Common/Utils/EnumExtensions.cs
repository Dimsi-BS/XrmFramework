// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.ComponentModel;

namespace Model
{
    public static partial class EnumExtensions
    {
        public static T ToEnum<T>(this int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static T ParseDescription<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            foreach (var tempValue in Enum.GetValues(typeof(T)))
            {
                var attribute = typeof(T).GetField(tempValue.ToString()).GetCustomAttribute<DescriptionAttribute>();
                if (attribute?.Description == value)
                {
                     return (T) tempValue;
                }
            }

            return default(T);
        }

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static string GetDescription(this Enum enumValue)
        {
            var descriptionAttribute = enumValue.GetType().GetField(enumValue.ToString()).GetCustomAttribute<DescriptionAttribute>();
            
            return descriptionAttribute?.Description;
        }

        public static Int32 ToInt(this Enum enumValue)
        {
            return Convert.ToInt32(enumValue);
        }
    }
}
