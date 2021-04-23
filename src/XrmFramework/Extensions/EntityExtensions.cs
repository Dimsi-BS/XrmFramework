// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace XrmFramework
{
    public static class EntityExtensions
    {
        public static T GetAttributeValue<T>(this Entity newEntity, Entity preEntity, string fieldName)
        {
            return GetAttributeValue<T>(newEntity, preEntity, fieldName, default(T));
        }
        public static T GetAttributeValue<T>(this Entity newEntity, string fieldName)
        {
            T value;
            if (typeof(T).IsEnum)
            {
                var tempValue = GetAttributeValue<OptionSetValue>(newEntity, null, fieldName, new OptionSetValue(0)).Value;

                value = (T)Enum.ToObject(typeof(T), tempValue);
            }
            else
            {
                value = GetAttributeValue<T>(newEntity, null, fieldName);
            }

            return GetAttributeValue<T>(newEntity, null, fieldName);
        }


        public static T GetAttributeValue<T>(this Entity newEntity, string fieldName, T defaultValue)
        {
            return GetAttributeValue<T>(newEntity, null, fieldName, defaultValue);
        }
        public static T GetAttributeValue<T>(this Entity newEntity, Entity preEntity, string fieldName, T defaultValue)
        {
            T value = defaultValue;
            if (newEntity != null && newEntity.Contains(fieldName))
            {
                value = newEntity.GetAttributeValue<T>(fieldName);
            }
            else if (preEntity != null && preEntity.Contains(fieldName))
            {
                value = preEntity.GetAttributeValue<T>(fieldName);
            }

            return value;
        }

        public static T GetAliasedValue<T>(this Entity newEntity, string fieldName)
        {
            return GetAliasedValue(newEntity, fieldName, default(T));
        }

        public static T GetAliasedValue<T>(this Entity newEntity, string fieldName, T defaultValue)
        {
            T value = defaultValue;
            if (newEntity != null && newEntity.Contains(fieldName))
            {
                value = (T)newEntity.GetAttributeValue<AliasedValue>(fieldName).Value;
            }

            return value;
        }

        public static T GetOptionSetValue<T>(this Entity newEntity, Entity preImage, string fieldName, T defaultValue) where T : Enum
        {
            T value = defaultValue;
            if (typeof(T).IsEnum)
            {
                OptionSetValue tempValue;
                var objectValue = GetAttributeValue<object>(newEntity, preImage, fieldName);

                if (objectValue is AliasedValue)
                {
                    tempValue = (OptionSetValue)((AliasedValue)objectValue).Value;
                }
                else
                {
                    tempValue = (OptionSetValue)objectValue;
                }

                if (tempValue != null)
                {
                    value = (T)Enum.ToObject(typeof(T), tempValue.Value);
                }
            }

            return value;
        }

        public static T GetOptionSetValue<T>(this Entity newEntity, Entity preImage, string fieldName) where T : Enum
        {
            return GetOptionSetValue<T>(newEntity, preImage, fieldName, default(T));
        }

        public static T GetOptionSetValue<T>(this Entity newEntity, string fieldName) where T : Enum
        {

            return GetOptionSetValue<T>(newEntity, null, fieldName);
        }

        public static T GetOptionSetValue<T>(this Entity newEntity, string fieldName, T defaultValue) where T : Enum
        {

            return GetOptionSetValue<T>(newEntity, null, fieldName, defaultValue);
        }

        public static void SetOptionSetValue<T>(this Entity entity, string fieldName, T value) where T : Enum
        {
            OptionSetValue optionSetValue = null;

            var intValue = (int)(object)value;
            if (typeof(T).IsEnum)
            {
                if (value.ToString() != "Null" || intValue != 0)
                {
                    optionSetValue = new OptionSetValue(intValue);
                }
            }
            else
            {
                optionSetValue = new OptionSetValue(intValue);
            }

            entity[fieldName] = optionSetValue;
        }

        public static ICollection<T> GetOptionSetValues<T>(this Entity newEntity, Entity preImage, string fieldName, params T[] defaultValues) where T : Enum
        {
            if (typeof(T).IsEnum)
            {
                OptionSetValueCollection tempValue;
                var objectValue = GetAttributeValue<object>(newEntity, preImage, fieldName);

                if (objectValue is AliasedValue aliasedValue)
                {
                    tempValue = (OptionSetValueCollection)aliasedValue.Value;
                }
                else
                {
                    tempValue = (OptionSetValueCollection)objectValue;
                }

                if (tempValue != null)
                {
                    var list = new List<T>();

                    foreach (var option in tempValue)
                    {
                        list.Add((T)Enum.ToObject(typeof(T), option.Value));
                    }

                    return list;
                }
            }

            return defaultValues == null ? new List<T>() : defaultValues.ToList();
        }

        public static ICollection<T> GetOptionSetValues<T>(this Entity newEntity, Entity preImage, string fieldName) where T : Enum
        {
            return GetOptionSetValues<T>(newEntity, preImage, fieldName, null);
        }

        public static ICollection<T> GetOptionSetValues<T>(this Entity newEntity, string fieldName) where T : Enum
        {

            return GetOptionSetValues<T>(newEntity, null, fieldName);
        }

        public static ICollection<T> GetOptionSetValues<T>(this Entity newEntity, string fieldName, T defaultValue) where T : Enum
        {

            return GetOptionSetValues<T>(newEntity, null, fieldName, defaultValue);
        }

        public static void SetOptionSetValues<T>(this Entity entity, string fieldName, ICollection<T> values) where T : Enum
        {
            SetOptionSetValues(entity, fieldName, values.ToArray());
        }

        /// <summary>
        /// Set multi-select picklist values
        /// </summary>
        /// <typeparam name="T">The type of the enum corresponding to the field</typeparam>
        /// <param name="entity"></param>
        /// <param name="fieldName"></param>
        /// <param name="values">List of selected values</param>
        public static void SetOptionSetValues<T>(this Entity entity, string fieldName, params T[] values) where T : Enum
        {
            OptionSetValueCollection optionSetValueCollection = null;

            if (values != null && values.Any())
            {
                optionSetValueCollection = new OptionSetValueCollection();

                foreach (var value in values)
                {
                    var intValue = value.ToInt();

                    if (value.ToString() != "Null" || intValue != 0)
                    {
                        optionSetValueCollection.Add(new OptionSetValue(intValue));
                    }
                }
            }

            entity[fieldName] = optionSetValueCollection;
        }

        public static EntityCollection EmptyIds(this EntityCollection collection, params string[] fieldNames)
        {
            foreach (var entity in collection.Entities)
            {
                entity.Id = Guid.Empty;
                if (fieldNames != null)
                {
                    foreach (var fieldName in fieldNames)
                    {
                        entity.Attributes.Remove(fieldName);
                    }
                }
            }

            return collection;
        }

        public static void MergeWith(this Entity targetEntity, Entity sourceEntity, bool copyOnlyIfFieldNotExist = false)
        {
            foreach (var attributeName in sourceEntity.Attributes.Keys)
            {
                if (targetEntity.Contains(attributeName) && copyOnlyIfFieldNotExist)
                {
                    continue;
                }

                CopyField(sourceEntity, targetEntity, attributeName, attributeName);
            }
        }

        public static void CopyField(this Entity sourceEntity, Entity targetEntity, string sourceColumnName, string targetColumnName)
        {
            CopyField(sourceEntity, null, targetEntity, sourceColumnName, targetColumnName);
        }

        public static void CopyField(this Entity sourceEntity, Entity preImage, Entity targetEntity, string sourceColumnName, string targetColumnName)
        {
            if (!sourceEntity.Contains(sourceColumnName)) return;

            var newValue = sourceEntity[sourceColumnName];

            if (preImage != null)
            {
                if (preImage.Contains(sourceColumnName))
                {
                    var oldValue = preImage[sourceColumnName];

                    if (Equals(oldValue, newValue))
                    {
                        return;
                    }
                }
            }


            targetEntity[targetColumnName] = newValue;
        }

        public static Entity Merge(this Entity sourceEntity, Entity preImage)
        {
            var newEntity = sourceEntity?.ToEntityReference().ToEntity() ?? preImage;

            if (sourceEntity != null)
            {
                foreach (var fieldName in sourceEntity.Attributes.Keys)
                {
                    newEntity[fieldName] = sourceEntity[fieldName];
                }
            }

            if (preImage == null)
            {
                return newEntity;
            }

            foreach (var fieldName in preImage.Attributes.Keys)
            {
                if (!newEntity.Contains(fieldName))
                {
                    newEntity[fieldName] = preImage[fieldName];
                }
            }

            return newEntity;
        }
    }
}