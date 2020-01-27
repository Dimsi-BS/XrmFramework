using System;
using System.Collections.Generic;
#if INTERNAL_NEWTONSOFT
using Newtonsoft.Json.Xrm;
#else 
using Newtonsoft.Json;
#endif

namespace XrmFramework.RemoteDebugger.Converters
{
    public class ObjectSerializationConverter : JsonConverter<ObjectSerialization>
    {
        public override void WriteJson(JsonWriter writer, ObjectSerialization value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(value.Value?.GetType().AssemblyQualifiedName);
            writer.WritePropertyName("Value");
            serializer.Serialize(writer, value.Value);
            writer.WriteEndObject();
        }

        public override ObjectSerialization ReadJson(JsonReader reader, Type objectType, ObjectSerialization existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            Type type = null;

            reader.Read();
            var propertyName = (string)reader.Value;
            reader.Read();

            var typeName = (string)reader.Value;

            if (!string.IsNullOrEmpty(typeName))
            {
                type = Type.GetType(typeName);
            }

            reader.Read();
            reader.Read();
            object value = default;

            if (type != null)
            {
                value = serializer.Deserialize(reader, type);
            }

            reader.Read();

            return new ObjectSerialization(value);
        }
    }
}
