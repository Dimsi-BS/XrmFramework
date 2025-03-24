// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace XrmFramework
{
    public static partial class EnumExtensions
    {
        public static T ToEnum<T>(this int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static T ToEnum<T>(this OptionSetValue value)
        {
            return value == null ? default : value.Value.ToEnum<T>();
        }

        public static T ToEnum<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }
            return (T)Enum.Parse(typeof(T), value);
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
        
        public static ICollection<T> ParseDescriptions<T>(this string stringList, char separator = ';')
        {
            var list = new HashSet<T>();

            if (string.IsNullOrEmpty(stringList))
            {
                return list;
            }

            var enumType = typeof(T);

            var dicEnum = new Dictionary<string, T>();

            foreach (T value in Enum.GetValues(enumType))
            {
                var description = enumType.GetField(value.ToString()).GetCustomAttribute<DescriptionAttribute>();
                if (description != null)
                {
                    dicEnum.Add(description.Description, value);
                }
            }

            var array = stringList.Split(separator);

            foreach (var stringEnum in array.Where(dicEnum.ContainsKey))
            {
                list.Add(dicEnum[stringEnum]);
            }

            return list;
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

            return optionSetValues.Any() ? new OptionSetValueCollection(optionSetValues) : new OptionSetValueCollection();
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
