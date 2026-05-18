using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using XrmFramework.RemoteDebugger.Converters;

namespace XrmFramework.RemoteDebugger;

public partial class RemoteDebuggerContractResolver : DefaultContractResolver
{
    private static readonly ICollection<JsonConverter> Converters = new List<JsonConverter>
    {
        new ParameterCollectionConverter(),
        new KeyAttributeCollectionConverter(),
        new FormattedValueCollectionConverter(),
        new EntityImageCollectionConverter(),
        new AttributeCollectionConverter(),
        new RelatedEntityCollectionConverter(),
        new RelationshipQueryCollectionConverter(),
        new OrganizationRequestCollectionConverter(),
        new OrganizationResponseCollectionConverter(),
        new EntityConverter(),
        new ObjectSerializationConverter(),
        new ConditionExpressionConverter(),
        new AliasedValueConverter()
    };

    partial void AddArgumentConverter(ICollection<JsonConverter> converters);
    
    /// <inheritdoc />
    protected override JsonConverter ResolveContractConverter(Type objectType)
    {
        AddArgumentConverter(Converters);
        
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
}