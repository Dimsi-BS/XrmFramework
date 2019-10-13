// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using Microsoft.Xrm.Sdk;

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
                    return (T)tempValue;
                }
            }

            return default(T);
        }

        public static T ParseExternalValue<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            foreach (var tempValue in Enum.GetValues(typeof(T)))
            {
                var attribute = typeof(T).GetField(tempValue.ToString()).GetCustomAttribute<ExternalValueAttribute>();
                if (attribute?.ExternalValue == value)
                {
                    return (T)tempValue;
                }
            }

            return default(T);
        }

        public static T ToEnum<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }
            return (T)Enum.Parse(typeof(T), value);
        }

        public static string GetDescription(this Enum enumValue)
        {
            var descriptionAttribute = enumValue.GetType().GetField(enumValue.ToString()).GetCustomAttribute<DescriptionAttribute>();

            return descriptionAttribute?.Description;
        }

        public static string GetExternalValue(this Enum enumValue)
        {
            var externalValueAttribute = enumValue.GetType().GetField(enumValue.ToString()).GetCustomAttribute<ExternalValueAttribute>();

            return externalValueAttribute?.ExternalValue;
        }

        public static int ToInt(this Enum enumValue)
        {
            return Convert.ToInt32(enumValue);
        }

        public static ICollection<object> ToEnumCollection(this IEnumerable<OptionSetValue> values, Type enumType)
        {
            var result = new List<object>();

            foreach (var value in values)
            {
                result.Add(Enum.ToObject(enumType, value.Value));
            }

            return result;
        }
    }
}
