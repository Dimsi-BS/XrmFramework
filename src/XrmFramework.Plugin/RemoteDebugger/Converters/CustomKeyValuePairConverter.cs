
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;

namespace XrmFramework.RemoteDebugger.Converters
{
    public class CustomKeyValuePairConverter<TKey, TValue> : JsonConverter<KeyValuePair<TKey, TValue>>
    {
        private const string KeyName = "Key";
        private const string ValueName = "Value";
        private const string TypeName = "Type";


        public override void WriteJson(JsonWriter writer, KeyValuePair<TKey, TValue> value, JsonSerializer serializer)
        {
            DefaultContractResolver resolver = serializer.ContractResolver as DefaultContractResolver;

            writer.WriteStartObject();
            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(TypeName) : TypeName);
            serializer.Serialize(writer, value.Value?.GetType().AssemblyQualifiedName, typeof(string));
            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(KeyName) : KeyName);
            serializer.Serialize(writer, value.Key, typeof(TKey));
            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(ValueName) : ValueName);
            serializer.Serialize(writer, value.Value, typeof(TValue));
            writer.WriteEndObject();
        }

        public override KeyValuePair<TKey, TValue> ReadJson(JsonReader reader, Type objectType, KeyValuePair<TKey, TValue> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (!ReflectionUtils.IsNullableType(objectType))
                {
                    throw JsonSerializationException.Create(reader, "Cannot convert null value to KeyValuePair.");
                }

                return default;
            }

            TKey key = default;
            TValue value = default;
            string typeName = null;

            reader.ReadAndAssert();

            Type t = ReflectionUtils.IsNullableType(objectType)
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            JsonContract keyContract = serializer.ContractResolver.ResolveContract(typeof(TKey));
            JsonContract typeContract = serializer.ContractResolver.ResolveContract(typeof(string));

            while (reader.TokenType == JsonToken.PropertyName)
            {
                string propertyName = reader.Value.ToString();
                if (string.Equals(propertyName, TypeName, StringComparison.OrdinalIgnoreCase))
                {
                    reader.ReadForTypeAndAssert(typeContract, false);

                    typeName = (string)serializer.Deserialize(reader, typeContract.UnderlyingType);
                }
                else if (string.Equals(propertyName, KeyName, StringComparison.OrdinalIgnoreCase))
                {
                    reader.ReadForTypeAndAssert(keyContract, false);

                    key = (TKey)serializer.Deserialize(reader, keyContract.UnderlyingType);
                }
                else if (string.Equals(propertyName, ValueName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(typeName))
                    {
                        var valueType = Type.GetType(typeName);

                        JsonContract valueContract = serializer.ContractResolver.ResolveContract(valueType);

                        reader.ReadForTypeAndAssert(valueContract, false);

                        value = (TValue)serializer.Deserialize(reader, valueContract.UnderlyingType);
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
                else
                {
                    reader.Skip();
                }

                reader.ReadAndAssert();
            }

            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }
}
