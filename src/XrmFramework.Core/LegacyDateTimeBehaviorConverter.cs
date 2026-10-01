using System;
using Newtonsoft.Json;

namespace XrmFramework.Core
{
    /// <summary>
    /// Reads <see cref="Column.DateTimeBehavior"/> in both its current shape (the plain enum, as an
    /// int or its name) and the legacy shape some 2.* .table files still carry: the raw Dataverse SDK
    /// <c>DateTimeBehavior</c> object, serialized as <c>{ "Value": "UserLocal" }</c>. Always writes
    /// back the plain enum, so a legacy file self-heals the next time it is saved.
    /// </summary>
    public sealed class LegacyDateTimeBehaviorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(DateTimeBehavior) || objectType == typeof(DateTimeBehavior?);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.Integer)
                return (DateTimeBehavior)Convert.ToInt32(reader.Value);

            if (reader.TokenType == JsonToken.String)
                return ParseName((string)reader.Value);

            if (reader.TokenType == JsonToken.StartObject)
            {
                string value = null;

                while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                {
                    if (reader.TokenType == JsonToken.PropertyName
                        && string.Equals((string)reader.Value, "Value", StringComparison.OrdinalIgnoreCase))
                        value = reader.ReadAsString();
                    else
                        reader.Skip();
                }

                return value == null ? null : (object)ParseName(value);
            }

            throw new JsonSerializationException(
                $"Unexpected token {reader.TokenType} while reading DatBehav.");
        }

        private static DateTimeBehavior ParseName(string value)
            => (DateTimeBehavior)Enum.Parse(typeof(DateTimeBehavior), value, ignoreCase: true);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue((int)value);
        }
    }
}
