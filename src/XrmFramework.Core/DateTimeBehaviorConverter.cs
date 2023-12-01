using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace XrmFramework.Core;

public class DateTimeBehaviorConverter : JsonConverter<DateTimeBehavior>
{
    public override void WriteJson(JsonWriter writer, DateTimeBehavior value, JsonSerializer serializer)
    {
            writer.WriteValue((int)value);
    }

    public override DateTimeBehavior ReadJson(JsonReader reader, Type objectType, DateTimeBehavior existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {   
        if (reader.TokenType == JsonToken.Integer)
        {
            var value = (long)reader.Value;
            
            return (DateTimeBehavior)value;
        }

        if (reader.TokenType == JsonToken.String)
        {
            var value = reader.Value.ToString();
            if (Enum.TryParse(value, out DateTimeBehavior result))
            {
                return result;
            }
        }
        
        if (reader.TokenType == JsonToken.StartObject)
        {
            var value = serializer.Deserialize<TestExtractDateTimeBehavior>(reader);

            return value.Value;
        }
        
        throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing DateTimeBehavior. toto");
    }
    
    private sealed class TestExtractDateTimeBehavior
    {
        [JsonProperty("Value")]
        public DateTimeBehavior Value { get; set; }
    }
}