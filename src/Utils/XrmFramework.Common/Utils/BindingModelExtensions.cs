// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public static IEnumerable<IBindingModel> GetDiff<T>(this IEnumerable<T> sourceList, IEnumerable<T> targetEnumerable, IEqualityComparer<IBindingModel> comparer = null) where T : IBindingModel
        {
            return GetDiff(sourceList.Cast<IBindingModel>(), targetEnumerable.Cast<IBindingModel>(), typeof(T), comparer);
        }

        public static IEnumerable<IBindingModel> GetDiff(this IEnumerable<IBindingModel> sourceList, IEnumerable<IBindingModel> targetEnumerable, Type modelType, IEqualityComparer<IBindingModel> comparer = null)
        {
            var metadata = EntityMetadata.GetMetadata(modelType);

            comparer = comparer ?? new KeyEqualityComparer(modelType);

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


        public static void CopyField<TInput, TOutput>(this TInput input, TOutput output, string sourcePropertyName, string targetPropertyName, Type converterType = null) where TInput : BindingModelBase where TOutput : IBindingModel
        {
            var sourceProperty = typeof(TInput).GetProperty(sourcePropertyName);
            var targetProperty = typeof(TOutput).GetProperty(targetPropertyName);

            CopyFieldInternal(input, output, sourceProperty, targetProperty, converterType);
        }

        public static void CopyField<TInput, TOutput>(this TInput input, TOutput output, Expression<Func<TInput, object>> sourcePropertyExpression, Expression<Func<TOutput, object>> targetPropertyExpression, Type converterType = null) where TInput : BindingModelBase where TOutput : IBindingModel
        {
            var sourceProperty = GetPropertyInfo(sourcePropertyExpression);
            var targetProperty = GetPropertyInfo(targetPropertyExpression);

            CopyFieldInternal(input, output, sourceProperty, targetProperty, converterType);
        }

        private static void CopyFieldInternal<TInput, TOutput>(TInput input, TOutput output, PropertyInfo sourceProperty, PropertyInfo targetProperty, Type converterType) where TInput : BindingModelBase where TOutput : IBindingModel
        {
            if (sourceProperty == null || targetProperty == null || input.InitializedProperties.Contains(sourceProperty.Name) == false)
            {
                return;
            }

            var sourceValue = sourceProperty.GetMethod.Invoke(input, new object[] { });

            if (converterType != null)
            {
                var converter = (IConverter) Activator.CreateInstance(converterType);

                sourceValue = converter.ConvertFrom(sourceValue);
            }

            targetProperty.SetMethod?.Invoke(output, new[] {sourceValue});
        }

        private static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> propertyExpression)
        {
            if (propertyExpression.Body is UnaryExpression unaryExpression
                && unaryExpression.Operand.NodeType == ExpressionType.MemberAccess
                && unaryExpression.Operand is MemberExpression memberExpression
                && memberExpression.Member is PropertyInfo propertyInfo)
            {
                return propertyInfo;
            }
            return null;
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


    public class MultipleEqualityComparer<T> : IEqualityComparer<IBindingModel> where T : IBindingModel
    {
        private IList<PropertyInfo> Properties { get; }

        public MultipleEqualityComparer(params string[] propertyNames)
        {
            Properties = typeof(T).GetProperties().Where(p => propertyNames.Contains(p.Name)).ToList();
        }

        public bool Equals(IBindingModel x, IBindingModel y)
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

        public int GetHashCode(IBindingModel obj)
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
