// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;

namespace XrmFramework.BindingModel
{
    /// <summary>
    /// Converts between plain DTO objects (marked with <see cref="DtoFieldMappingAttribute"/>)
    /// and <see cref="IBindingModel"/> instances.
    /// </summary>
    internal static class DtoBindingModelMapper
    {
        public static IBindingModel FromDto(object dto, Type bindingType = null)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var dtoType = dto.GetType();
            bindingType ??= GetCorrespondingBindingType(dtoType);

            if (bindingType == null)
            {
                throw new Exception($"No BindingModel associated with type {dtoType.Name}");
            }

            var bindingModel = bindingType.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());

            foreach (var bindingProperty in bindingType.GetProperties())
            {
                var mapping = bindingProperty.GetCustomAttribute<DtoFieldMappingAttribute>();
                if (mapping == null)
                {
                    continue;
                }

                var dtoProperty = GetMappedDtoProperty(dtoType, mapping);
                var rawValue = dtoProperty.GetValue(dto);

                // ToBindingModel(object,Type) in the original only called SetValue when it had
                // a real mapping: matching types, or a converter. We preserve that.
                if (bindingProperty.PropertyType == dtoProperty.PropertyType)
                {
                    bindingProperty.SetValue(bindingModel, rawValue);
                }
                else if (mapping.ConverterType != null)
                {
                    var converter = CreateConverter(mapping.ConverterType);
                    bindingProperty.SetValue(bindingModel, converter.ConvertFromDtoAttribute(rawValue));
                }
            }

            return (IBindingModel)bindingModel;
        }

        public static T ToDto<T>(IBindingModel model) where T : new()
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var dto = new T();
            var dtoType = typeof(T);
            var bindingType = model.GetType();

            foreach (var bindingProperty in bindingType.GetProperties())
            {
                var mapping = bindingProperty.GetCustomAttribute<DtoFieldMappingAttribute>();
                if (mapping == null)
                {
                    continue;
                }

                var dtoProperty = GetMappedDtoProperty(dtoType, mapping);
                var rawValue = bindingProperty.GetValue(model);

                dtoProperty.SetValue(dto, ConvertToDto(bindingProperty, dtoProperty, mapping, rawValue));
            }

            return dto;
        }

        public static U FromDtoStrict<T, U>(T dto) where T : new() where U : IBindingModel, new()
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var model = new U();
            var bindingType = typeof(U);
            var dtoType = typeof(T);

            foreach (var bindingProperty in bindingType.GetProperties())
            {
                var mapping = bindingProperty.GetCustomAttribute<DtoFieldMappingAttribute>();
                if (mapping == null)
                {
                    continue;
                }

                var dtoProperty = GetMappedDtoProperty(dtoType, mapping);
                var rawValue = dtoProperty.GetValue(dto);

                bindingProperty.SetValue(model, ConvertFromDto(bindingProperty, dtoProperty, mapping, rawValue));
            }

            return model;
        }

        public static Type GetCorrespondingBindingType(Type dtoType)
        {
            return typeof(DtoBindingModelMapper).Assembly.GetTypes()
                .Where(t => typeof(IXmlModel).IsAssignableFrom(t))
                .FirstOrDefault(t => t.GetCustomAttribute<DtoObjectMappingAttribute>()?.RelativePath == dtoType.Name);
        }

        // ---------------------------------------------------------------------

        private static PropertyInfo GetMappedDtoProperty(Type dtoType, DtoFieldMappingAttribute mapping)
        {
            var dtoProperty = dtoType.GetProperty(mapping.RelativePath);
            if (dtoProperty == null)
            {
                throw new Exception(
                    $"The Property {mapping.RelativePath} does not exist on type {dtoType.Name}, modify BindingModel accordingly");
            }
            return dtoProperty;
        }

        private static object ConvertToDto(PropertyInfo bindingProperty, PropertyInfo dtoProperty, DtoFieldMappingAttribute mapping, object rawValue)
        {
            if (bindingProperty.PropertyType == dtoProperty.PropertyType)
            {
                return rawValue;
            }

            if (mapping.ConverterType != null)
            {
                return CreateConverter(mapping.ConverterType).ConvertToDtoAttribute(rawValue);
            }

            if (dtoProperty.PropertyType == typeof(string) && rawValue != null)
            {
                return rawValue.ToString();
            }

            return null;
        }

        private static object ConvertFromDto(PropertyInfo bindingProperty, PropertyInfo dtoProperty, DtoFieldMappingAttribute mapping, object rawValue)
        {
            if (bindingProperty.PropertyType == dtoProperty.PropertyType)
            {
                return rawValue;
            }

            if (mapping.ConverterType != null)
            {
                return CreateConverter(mapping.ConverterType).ConvertFromDtoAttribute(rawValue);
            }

            if (dtoProperty.PropertyType == typeof(string) && rawValue != null)
            {
                return rawValue.ToString();
            }

            return null;
        }

        private static IDtoAttributeConverter CreateConverter(Type converterType)
            => (IDtoAttributeConverter)converterType.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());
    }
}
