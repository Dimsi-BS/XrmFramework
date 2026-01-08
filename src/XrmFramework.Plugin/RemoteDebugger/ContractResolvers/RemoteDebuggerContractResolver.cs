using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using XrmFramework.RemoteDebugger.Converters;

namespace XrmFramework.RemoteDebugger.Client.Infrastructure.ContractResolvers;

public class RemoteDebuggerContractResolver : DefaultContractResolver
{
    /// <inheritdoc />
    protected override JsonConverter ResolveContractConverter(Type objectType)
    {
        var converter = Converters.FirstOrDefault(c =>
            typeof(JsonConverter<>).MakeGenericType(objectType).IsInstanceOfType(c));

        if (converter != null) return converter;

        if (objectType.IsGenericType && objectType.GenericTypeArguments.Length == 2 &&
            typeof(KeyValuePair<,>).MakeGenericType(objectType.GenericTypeArguments).IsAssignableFrom(objectType))
        {
            var converterType =
                typeof(CustomKeyValuePairConverter<,>).MakeGenericType(objectType.GenericTypeArguments);

            return (JsonConverter)Activator.CreateInstance(converterType);
        }

        return base.ResolveContractConverter(objectType);
    }
    
    /// <inheritdoc />
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        // Filter out internal properties
        if (property.Writable 
            && member is PropertyInfo propertyInfo 
            && (propertyInfo.GetMethod.IsAssembly || propertyInfo.SetMethod.IsAssembly))
        {
            property.ShouldSerialize = _ => false;
        }

        return property;
    }

    /// <inheritdoc />
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

        // only serializer properties that start with the specified character
        properties =
            properties.Where(p => p.PropertyName != "WatsonBuckets").ToList();

        return properties;
    }
    
    private static readonly ICollection<JsonConverter> Converters = new List<JsonConverter>
    {
        new ParameterCollectionConverter(),
        new KeyAttributeCollectionConverter(),
        new FormattedValueCollectionConverter(),
        new EntityImageCollectionConverter(),
        new AttributeCollectionConverter(),
        new EndpointCollectionConverter(),
        new RelatedEntityCollectionConverter(),
        new RelationshipQueryCollectionConverter(),
        new OrganizationRequestCollectionConverter(),
        new OrganizationResponseCollectionConverter(),
        new ArgumentsCollectionConverter(),
        new EntityConverter(),
        new ObjectSerializationConverter(),
        new ConditionExpressionConverter(),
        new AliasedValueConverter()
    };
}
