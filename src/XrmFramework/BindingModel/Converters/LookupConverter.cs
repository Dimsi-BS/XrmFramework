// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Xrm.Sdk;
using System;

namespace XrmFramework.BindingModel
{
    public class LookupConverter : ModelPropertyConverter
    {
        public override object ConvertFrom(object value)
            => value switch
            {
                string stringValue => GetReferenceFromString(stringValue),
                EntityReference refValue => GetStringFromEntityReference(refValue),
                _ => throw new ArgumentException(@"The value must be a string or an EntityReference", nameof(value))
            };

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
            var result = string.Empty;
            if (reference != null)
            {
                result = $"{reference.LogicalName}|{reference.Id}|{reference.Name}";
            }
            return result;
        }
    }
}
