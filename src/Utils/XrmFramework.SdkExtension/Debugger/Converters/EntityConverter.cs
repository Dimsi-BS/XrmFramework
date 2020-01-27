using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xrm.Sdk;
#if INTERNAL_NEWTONSOFT
using Newtonsoft.Json.Xrm;
using Newtonsoft.Json.Xrm.Linq;
#else
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace XrmFramework.RemoteDebugger.Converters
{
    public class EntityConverter : JsonConverter<Entity>
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, Entity value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override Entity ReadJson(JsonReader reader, Type objectType, Entity existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var entity = new Entity();
            
            reader.Read();
            while (reader.TokenType != JsonToken.EndObject)
            {
                var memberName = reader.Value.ToString();

                reader.Read();

                switch (memberName)
                {
                    case "LogicalName":
                        entity.LogicalName = reader.Value.ToString();
                        break;
                    case "Id":
                        entity.Id = new Guid(reader.Value.ToString());
                        break;
                    case "Attributes":
                        entity.Attributes = (AttributeCollection)serializer.Deserialize(reader, typeof(AttributeCollection));
                        break;
                    case "FormattedValues":
                        entity.FormattedValues.AddRange((FormattedValueCollection)serializer.Deserialize(reader, typeof(FormattedValueCollection)));
                        break;
                    case "RelatedEntities":
                        entity.RelatedEntities.AddRange((RelatedEntityCollection)serializer.Deserialize(reader, typeof(RelatedEntityCollection)));
                        break;
                    case "KeyAttributes":
                        entity.KeyAttributes = (KeyAttributeCollection)serializer.Deserialize(reader, typeof(KeyAttributeCollection));
                        break;
                    case "EntityState":
                        entity.EntityState = (EntityState?)serializer.Deserialize(reader, typeof(EntityState?));
                        break;
                    case "RowVersion":
                        entity.RowVersion = reader.TokenType == JsonToken.Null ? null : reader.Value.ToString();
                        break;
                }
                reader.Read();
            }

            return entity;
        }
    }
}
