using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xrm.Sdk;

namespace Model
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert given string to Guid
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string value)
        {
            return new Guid(value);
        }

        public static void ShouldNotBeNull(this string value, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(propertyName);
            }
        }

        public static EntityReference ToEntityReference(this string idOrkeyValue, string entityLogicalName, string keyAttributeName = null)
        {
            entityLogicalName.ShouldNotBeNull();

            if (string.IsNullOrEmpty(idOrkeyValue))
            {
                return null;
            }

            var reference = new EntityReference(entityLogicalName);

            if (string.IsNullOrEmpty(keyAttributeName))
            {
                reference.Id = new Guid(idOrkeyValue);
            }
            else
            {
                reference.KeyAttributes.Add(new KeyValuePair<string, object>(keyAttributeName, idOrkeyValue));
            }

            return reference;
        }

        public static Relationship ToRelationship(this string relationshipName)
        {
            if (string.IsNullOrEmpty(relationshipName))
            {
                return null;
            }

            return new Relationship(relationshipName);
        }
    }
}