using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
#if INTERNAL_NEWTONSOFT
using Newtonsoft.Json.Xrm;
using Newtonsoft.Json.Xrm.Linq;
#else
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace XrmFramework.Debugger.Converters
{
    public class ConditionExpressionConverter : JsonConverter<ConditionExpression>
    {
        public override void WriteJson(JsonWriter writer, ConditionExpression value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("attributeName");
            writer.WriteValue(value.AttributeName);
            writer.WritePropertyName("operator");
            writer.WriteValue(value.Operator);
            writer.WritePropertyName("values");
            writer.WriteValue(value.Values.Select(v => new ObjectSerialization(v)).ToList());

            writer.WritePropertyName("entityName");
            writer.WriteValue(value.EntityName);

            writer.WriteEndObject();
        }

        public override ConditionExpression ReadJson(JsonReader reader, Type objectType, ConditionExpression existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // AttributeName, Operator, Values

            var condition = new ConditionExpression();

            reader.Read();
            var propertyName = (string)reader.Value;
            reader.Read();

            condition.AttributeName = (string)reader.Value;

            reader.Read();
            propertyName = (string)reader.Value;

            reader.Read();

            condition.Operator = (ConditionOperator)serializer.Deserialize(reader, typeof(ConditionOperator));

            reader.Read();
            propertyName = (string)reader.Value;
            reader.Read();

            var serializedValues = serializer.Deserialize<List<ObjectSerialization>>(reader);

            serializedValues.ForEach(o => condition.Values.Add(o.Value));

            reader.Read();
            propertyName = (string)reader.Value;
            reader.Read();

            reader.Read();

            return condition;
        }
    }
}
