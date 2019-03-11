using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Model.Sdk;

namespace Model
{
    public static class BindingModelExtensions
    {

        public static T GetDiffGeneric<T>(this T source, T target) where T : IBindingModel, new()
        {
            return (T)GetDiff(source, target);
        }

        public static IEnumerable<IBindingModel> GetDiff(IEnumerable<IBindingModel> sourceList, IEnumerable<IBindingModel> targetEnumerable, Type modelType)
        {
            var metadata = EntityMetadata.GetMetadata(modelType);

            var comparer = new KeyEqualityComparer(modelType);


            var targetList = targetEnumerable as IList<IBindingModel> ?? targetEnumerable.ToList();
            var objectsToUpsert = sourceList.Except(targetList, new DeepModelEqualityComparer(modelType));

            foreach (var obj in objectsToUpsert)
            {
                var existingObject = targetList.SingleOrDefault(o => comparer.Equals(obj, o));

                if (!metadata.IsValidForCreate && existingObject == null)
                {
                    continue;
                }

                yield return existingObject == null ? obj : GetDiff(obj, existingObject);
            }
        }

        public static IBindingModel GetDiff(this IBindingModel source, IBindingModel target)
        {
            if (target == null)
            {
                return source;
            }

            var metadata = EntityMetadata.GetMetadata(source.GetType());

            var result = (IBindingModel)Activator.CreateInstance(source.GetType());

            if (target.Id != Guid.Empty)
            {
                result.Id = target.Id;
            }
            else if (source.Id != Guid.Empty)
            {
                result.Id = source.Id;
            }

            foreach (var attribute in metadata.CrmAttributes.Where(a => a.CrmMapping.IsValidForUpdate))
            {
                if (source is BindingModelBase)
                {
                    if (!((BindingModelBase)source).InitializedProperties.Contains(attribute.PropertyName))
                    {
                        continue;
                    }
                }

                var valueSource = attribute.Property.GetValue(source);
                var valueTarget = attribute.Property.GetValue(target);


                switch (attribute.AttributeType)
                {
                    case AttributeTypeCode.DateTime:
                        if (attribute.DateTimeBehavior == DateTimeBehavior.UserLocal)
                        {
                            valueSource = ((DateTime?)valueSource)?.ToUniversalTime();
                            valueTarget = ((DateTime?)valueTarget)?.ToUniversalTime();
                        }
                        else
                        {
                            valueSource = valueSource != null ? (DateTime?)new DateTime(((DateTime)valueSource).Ticks, DateTimeKind.Utc) : null;
                        }
                        break;
                    case AttributeTypeCode.String:
                    case AttributeTypeCode.Memo:
                        if (string.Empty == (valueSource as string))
                        {
                            valueSource = null;
                        }
                        break;
                }

                if ((!attribute.IsKey || result.Id != Guid.Empty) &&
                       (
                           (valueSource == null && valueTarget == null)
                        || (valueSource != null && valueSource.Equals(valueTarget))
                        || (valueTarget != null && valueTarget.Equals(valueSource))
                       )
                   )
                {
                    continue;
                }

                attribute.Property.SetValue(result, valueSource);
            }

            return result;
        }
    }


        public class KeyEqualityComparer : IEqualityComparer<IBindingModel>
        {
            private PropertyInfo PropertyInfo { get; }

            private EntityMetadata Metadata { get; }

            private bool HasPreferedKey { get; }

            public KeyEqualityComparer(Type modelType)
            {
                Metadata = EntityMetadata.GetMetadata(modelType);
                HasPreferedKey = Metadata.HasPreferedKey;
            }

            public bool Equals(IBindingModel x, IBindingModel y)
            {
                var equals = true;

                foreach (var attributeMetada in Metadata.CrmAttributes.Where(a => (a.IsKey && !HasPreferedKey) || (HasPreferedKey && a.IsPreferedKey)))
                {
                    var valueX = attributeMetada.Property.GetValue(x);
                    var valueY = attributeMetada.Property.GetValue(y);

                    if (attributeMetada.AttributeType.ToInt() == AttributeTypeCode.String.ToInt())
                    {
                        equals &= valueX != null && valueY != null && string.Equals((string)valueX, (string)valueY, StringComparison.InvariantCultureIgnoreCase);
                    }
                    else
                    {
                        equals &= valueX != null && valueY != null && Equals(valueX, valueY);
                    }
                }
                return equals;
            }

            public int GetHashCode(IBindingModel obj)
            {
                int hashCode = 0;

                foreach (var attributeMetada in Metadata.CrmAttributes.Where(a => (a.IsKey && !HasPreferedKey) || (HasPreferedKey && a.IsPreferedKey)))
                {

                    hashCode ^= attributeMetada.Property.GetValue(obj)?.GetHashCode() ?? 0;
                }
                return hashCode;
            }
        }


        public class MultipleEqualityComparer<T> : IEqualityComparer<T> where T : IBindingModel
        {
            private IList<PropertyInfo> Properties { get; }

            public MultipleEqualityComparer(params string[] propertyNames)
            {
                Properties = typeof(T).GetProperties().Where(p => propertyNames.Contains(p.Name)).ToList();
            }

            public bool Equals(T x, T y)
            {
                var equals = true;

                foreach (var property in Properties)
                {
                    var valueX = property.GetValue(x);
                    var valueY = property.GetValue(y);

                    if (property.PropertyType == typeof(string))
                    {
                        equals &= string.Equals((string)valueX, (string)valueY, StringComparison.InvariantCultureIgnoreCase);
                    }
                    else
                    {
                        equals &= (valueX != null && valueX.Equals(valueY)) || (valueX == null && valueY == null);
                    }
                    if (!equals)
                    {
                        break;
                    }
                }

                return equals;
            }

            public int GetHashCode(T obj)
            {
                var hash = 0;

                foreach (var property in Properties)
                {
                    var value = property.GetValue(obj);
                    if (value != null)
                    {
                        var s = value as string;
                        if (s != null)
                        {
                            hash ^= s.ToLowerInvariant().GetHashCode();
                        }
                        else
                        {
                            hash ^= value.GetHashCode();
                        }
                    }
                }

                return hash;
            }
        }

        public class ModelEqualityComparer<T> : IEqualityComparer<T> where T : IBindingModel
        {
            public bool Equals(T x, T y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(T obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        public class DeepModelEqualityComparer : IEqualityComparer<IBindingModel>
        {
            //private IList<PropertyInfo> Properties { get; set; }

            private EntityMetadata Metadata { get; }

            public DeepModelEqualityComparer(Type modelType)
            {
                Metadata = EntityMetadata.GetMetadata(modelType);
            }

            public bool Equals(IBindingModel x, IBindingModel y)
            {
                var equals = true;

                foreach (var attributeMetadata in Metadata.CrmAttributes.Where(a => a.CrmMapping.IsValidForUpdate))
                {
                    var valueX = attributeMetadata.Property.GetValue(x);
                    var valueY = attributeMetadata.Property.GetValue(y);

                    if (attributeMetadata.AttributeType == AttributeTypeCode.String || attributeMetadata.AttributeType == AttributeTypeCode.Memo)
                    {
                        equals &= string.Equals((string)valueX, (string)valueY, StringComparison.InvariantCultureIgnoreCase);
                    }
                    else
                    {
                        equals &= (valueX != null && valueX.Equals(valueY)) || (valueX == null && valueY == null);
                    }

                    if (!equals)
                    {
                        break;
                    }
                }

                return equals;
            }

            public int GetHashCode(IBindingModel obj)
            {
                var hash = 0;

                foreach (var attributeMetadata in Metadata.CrmAttributes.Where(a => a.CrmMapping.IsValidForUpdate))
                {
                    var value = attributeMetadata.Property.GetValue(obj);
                    if (value != null)
                    {
                        hash ^= value.GetHashCode();
                    }
                }

                return hash;
            }
        }

        public class DeepModelEqualityComparer<T> : IEqualityComparer<T> where T : IBindingModel
        {
            private IList<PropertyInfo> Properties { get; set; }
            public DeepModelEqualityComparer()
            {
                var allProperties = typeof(T).GetProperties();

                Properties = new List<PropertyInfo>(allProperties.Where(p => p.GetCustomAttribute<CrmMappingAttribute>() != null));
            }

            public bool Equals(T x, T y)
            {
                var equals = true;

                foreach (var property in Properties)
                {
                    var valueX = property.GetValue(x);
                    var valueY = property.GetValue(y);

                    equals &= (valueX != null && valueX.Equals(valueY)) || (valueX == null && valueY == null);

                    if (!equals)
                    {
                        break;
                    }
                }

                return equals;
            }

            public int GetHashCode(T obj)
            {
                var hash = 0;

                foreach (var property in Properties)
                {
                    var value = property.GetValue(obj);
                    if (value != null)
                    {
                        hash ^= value.GetHashCode();
                    }
                }

                return hash;
            }
        }
}
