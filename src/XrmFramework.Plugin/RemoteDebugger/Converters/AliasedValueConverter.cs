
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;

namespace XrmFramework.RemoteDebugger.Converters
{
    public class AliasedValueConverter : JsonConverter<AliasedValue>
    {
        public override void WriteJson(JsonWriter writer, AliasedValue value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("attributeLogicalName");
            writer.WriteValue(value.AttributeLogicalName);
            writer.WritePropertyName("entityLogicalName");
            writer.WriteValue(value.EntityLogicalName);
            writer.WritePropertyName("value");

            serializer.Serialize(writer, new ObjectSerialization(value.Value));

            writer.WriteEndObject();
        }

        public override AliasedValue ReadJson(JsonReader reader, Type objectType, AliasedValue existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            reader.Read();
            var propertyName = (string)reader.Value;
            reader.Read();

            var attributeLogicalName = (string)reader.Value;

            reader.Read();
            propertyName = (string)reader.Value;

            reader.Read();

            var entityLogicalName = (string)reader.Value;

            reader.Read();
            propertyName = (string)reader.Value;
            reader.Read();

            var serializedValue = serializer.Deserialize<ObjectSerialization>(reader);

            reader.Read();

            var aliasedValue = new AliasedValue(entityLogicalName, attributeLogicalName, serializedValue.Value);

            return aliasedValue;
        }
    }
}
