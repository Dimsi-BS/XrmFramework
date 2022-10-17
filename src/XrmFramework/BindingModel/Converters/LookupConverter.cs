// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Xrm.Sdk;
using System;
using System.ComponentModel;

namespace XrmFramework.BindingModel
{
    public class LookupConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(EntityReference);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || destinationType == typeof(EntityReference);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                return GetReferenceFromString((string)value);
            }

            return GetStringFromEntityReference((EntityReference)value);
        }

        private static EntityReference GetReferenceFromString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var split = value.Split('|');
            return new EntityReference
            {
                LogicalName = split[0],
                Id = new Guid(split[1]),
                Name = split[2]
            };
        }

        private static string GetStringFromEntityReference(EntityReference reference)
        {
            string result = string.Empty;
            if (reference != null)
            {
                result = $"{reference.LogicalName}|{reference.Id}|{reference.Name}";
            }
            return result;
        }
    }
}
