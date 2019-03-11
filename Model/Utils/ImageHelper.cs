using System;
using Plugins;

namespace Microsoft.Xrm.Sdk
{
    public static class ImageHelper
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

        public static T GetOptionSetValue<T>(this Entity newEntity, Entity preImage, string fieldName, T defaultValue)
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

        public static T GetOptionSetValue<T>(this Entity newEntity, Entity preImage, string fieldName)
        {
            return GetOptionSetValue<T>(newEntity, preImage, fieldName, default(T));
        }

        public static T GetOptionSetValue<T>(this Entity newEntity, string fieldName)
        {

            return GetOptionSetValue<T>(newEntity, null, fieldName);
        }

        public static T GetOptionSetValue<T>(this Entity newEntity, string fieldName, T defaultValue)
        {

            return GetOptionSetValue<T>(newEntity, null, fieldName, defaultValue);
        }

        public static void SetOptionSetValue<T>(this Entity entity, string fieldName, T value)
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

        public static void MergeWith(this Entity targetEntity, Entity sourceEntity)
        {
            foreach (var attributeName in sourceEntity.Attributes.Keys)
            {
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