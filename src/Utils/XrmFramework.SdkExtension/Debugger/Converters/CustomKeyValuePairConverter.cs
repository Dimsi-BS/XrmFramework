using System;
using System.Collections.Generic;
#if INTERNAL_NEWTONSOFT
using Newtonsoft.Json.Xrm;
#else 
using Newtonsoft.Json;
#endif

namespace XrmFramework.Debugger.Converters
{
    public class CustomKeyValuePairConverter<TKey, TValue> : JsonConverter<KeyValuePair<TKey, TValue>>
    {
        public override void WriteJson(JsonWriter writer, KeyValuePair<TKey, TValue> value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Key");
            serializer.Serialize(writer, value.Key);
            writer.WritePropertyName("Type");
            writer.WriteValue(value.Value?.GetType().AssemblyQualifiedName);
            writer.WritePropertyName("Value");
            serializer.Serialize(writer, value.Value);
            writer.WriteEndObject();
        }

        public override KeyValuePair<TKey, TValue> ReadJson(JsonReader reader, Type objectType, KeyValuePair<TKey, TValue> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            TKey key = default;
            Type type = null;

            for (var i = 0; i < 3; i++)
            {
                reader.Read();
                var propertyName = (string)reader.Value;
                reader.Read();

                if (propertyName == "Key")
                {
                    if (reader.TokenType == JsonToken.String)
                    {
                        key = (TKey)reader.Value;
                    }
                    else
                    {
                        key = (TKey)serializer.Deserialize(reader, typeof(TKey));
                    }
                }
                else if (propertyName == "Type")
                {
                    var typeName = (string)reader.Value;

                    if (!string.IsNullOrEmpty(typeName))
                    {
                        type = Type.GetType(typeName);
                    }
                }
                else if (propertyName == "Value")
                {
                    break;
                }
                else
                {

                }

            }

            TValue value = default;

            if (type != null)
            {
                if (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.StartArray)
                {
                    value = (TValue)serializer.Deserialize(reader, type);
                }
                else
                {
                    if (type == typeof(Guid))
                    {
                        value = (TValue)(object)new Guid(reader.Value.ToString());
                    }
                    else if (reader.Value != null && reader.ValueType != typeof(TValue) && typeof(TValue) == typeof(string))
                    {
                        value = (TValue)(object)reader.Value.ToString();
                    }
                    else
                    {
                        value = (TValue)serializer.Deserialize(reader, type);
                    }
                }
            }

            reader.Read();

            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }
}
