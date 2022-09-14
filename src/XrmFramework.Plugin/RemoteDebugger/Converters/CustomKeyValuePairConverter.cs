
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
                if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    throw new JsonSerializationException("Cannot convert null value to KeyValuePair.");
                }

                return default;
            }

            TKey key = default;
            TValue value = default;
            string typeName = null;

            ReadAndAssert(reader);

            while (reader.TokenType == JsonToken.PropertyName)
            {
                string propertyName = reader.Value.ToString();
                if (string.Equals(propertyName, TypeName, StringComparison.OrdinalIgnoreCase))
                {
                    typeName = serializer.Deserialize<string>(reader);
                }
                else if (string.Equals(propertyName, KeyName, StringComparison.OrdinalIgnoreCase))
                {
                    key = serializer.Deserialize<TKey>(reader);
                }
                else if (string.Equals(propertyName, ValueName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(typeName))
                    {
                        var valueType = Type.GetType(typeName);

                        value = (TValue)serializer.Deserialize(reader, valueType);
                    }
                    else
                    {
                        ReadAndAssert(reader);
                    }
                }
                else
                {
                    reader.Skip();
                }

                ReadAndAssert(reader);
            }
            return new KeyValuePair<TKey, TValue>(key, value);
        }


        private void ReadAndAssert(JsonReader reader)
        {
            if (!reader.Read())
            {
                throw new JsonSerializationException("Cannot convert null value to KeyValuePair.");
            }
        }
    }
}