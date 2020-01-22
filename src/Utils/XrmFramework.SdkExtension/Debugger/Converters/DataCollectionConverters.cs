using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
#if INTERNAL_NEWTONSOFT
using Newtonsoft.Json.Xrm;
#else
using Newtonsoft.Json;
#endif

namespace XrmFramework.Debugger.Converters
{
    public class DataCollectionConverter<TKey, TValue> : JsonConverter<DataCollection<TKey, TValue>>
    {
        public override void WriteJson(JsonWriter writer, DataCollection<TKey, TValue> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (var keyValuePair in value)
            {
                serializer.Serialize(writer, keyValuePair);
            }

            writer.WriteEndArray();
        }

        public override DataCollection<TKey, TValue> ReadJson(JsonReader reader, Type objectType, DataCollection<TKey, TValue> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var retour = (DataCollection<TKey, TValue>)Activator.CreateInstance(objectType);

            var list = serializer.Deserialize<List<KeyValuePair<TKey, TValue>>>(reader);

            retour.AddRange(list);

            return retour;
        }
    }
    public class DataCollectionConverter<TValue> : JsonConverter<DataCollection<TValue>>
    {
        public override void WriteJson(JsonWriter writer, DataCollection<TValue> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (var valueTemp in value)
            {
                serializer.Serialize(writer, new ObjectSerialization(valueTemp));
            }

            writer.WriteEndArray();
        }

        public override DataCollection<TValue> ReadJson(JsonReader reader, Type objectType, DataCollection<TValue> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var retour = (DataCollection<TValue>)Activator.CreateInstance(objectType);

            var list = serializer.Deserialize<List<ObjectSerialization>>(reader);

            foreach (var o in list)
            {
                retour.Add((TValue)o.Value);
            }

            return retour;
        }
    }

    public class RelationshipQueryCollectionConverter : JsonConverter<RelationshipQueryCollection>
    {
        private readonly DataCollectionConverter<Relationship, QueryBase> _internalConverter = new DataCollectionConverter<Relationship, QueryBase>();

        public override void WriteJson(JsonWriter writer, RelationshipQueryCollection value, JsonSerializer serializer)
        {
            _internalConverter.WriteJson(writer, value, serializer);
        }

        public override RelationshipQueryCollection ReadJson(JsonReader reader, Type objectType, RelationshipQueryCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return (RelationshipQueryCollection)_internalConverter.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
        }
    }

    public class ParameterCollectionConverter : JsonConverter<ParameterCollection>
    {
        private readonly DataCollectionConverter<string, object> _internalConverter = new DataCollectionConverter<string, object>();

        public override void WriteJson(JsonWriter writer, ParameterCollection value, JsonSerializer serializer)
        {
            _internalConverter.WriteJson(writer, value, serializer);
        }

        public override ParameterCollection ReadJson(JsonReader reader, Type objectType, ParameterCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return (ParameterCollection)_internalConverter.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
        }
    }

    public class RelatedEntityCollectionConverter : JsonConverter<RelatedEntityCollection>
    {
        private readonly DataCollectionConverter<Relationship, EntityCollection> _internalConverter = new DataCollectionConverter<Relationship, EntityCollection>();

        public override void WriteJson(JsonWriter writer, RelatedEntityCollection value, JsonSerializer serializer)
        {
            _internalConverter.WriteJson(writer, value, serializer);
        }

        public override RelatedEntityCollection ReadJson(JsonReader reader, Type objectType, RelatedEntityCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return (RelatedEntityCollection)_internalConverter.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
        }
    }

    public class AttributeCollectionConverter : JsonConverter<AttributeCollection>
    {
        private readonly DataCollectionConverter<string, object> _internalConverter = new DataCollectionConverter<string, object>();

        public override void WriteJson(JsonWriter writer, AttributeCollection value, JsonSerializer serializer)
        {
            _internalConverter.WriteJson(writer, value, serializer);
        }

        public override AttributeCollection ReadJson(JsonReader reader, Type objectType, AttributeCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return (AttributeCollection)_internalConverter.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
        }
    }

    public class KeyAttributeCollectionConverter : JsonConverter<KeyAttributeCollection>
    {
        private readonly DataCollectionConverter<string, object> _internalConverter = new DataCollectionConverter<string, object>();

        public override void WriteJson(JsonWriter writer, KeyAttributeCollection value, JsonSerializer serializer)
        {
            _internalConverter.WriteJson(writer, value, serializer);
        }

        public override KeyAttributeCollection ReadJson(JsonReader reader, Type objectType, KeyAttributeCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return (KeyAttributeCollection)_internalConverter.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
        }
    }

    public class FormattedValueCollectionConverter : JsonConverter<FormattedValueCollection>
    {
        private readonly DataCollectionConverter<string, string> _internalConverter = new DataCollectionConverter<string, string>();

        public override void WriteJson(JsonWriter writer, FormattedValueCollection value, JsonSerializer serializer)
        {
            _internalConverter.WriteJson(writer, value, serializer);
        }

        public override FormattedValueCollection ReadJson(JsonReader reader, Type objectType, FormattedValueCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return (FormattedValueCollection)_internalConverter.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
        }
    }

    public class EntityImageCollectionConverter : JsonConverter<EntityImageCollection>
    {
        private readonly DataCollectionConverter<string, Entity> _internalConverter = new DataCollectionConverter<string, Entity>();

        public override void WriteJson(JsonWriter writer, EntityImageCollection value, JsonSerializer serializer)
        {
            _internalConverter.WriteJson(writer, value, serializer);
        }

        public override EntityImageCollection ReadJson(JsonReader reader, Type objectType, EntityImageCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return (EntityImageCollection)_internalConverter.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
        }
    }

    public class OrganizationRequestCollectionConverter : JsonConverter<OrganizationRequestCollection>
    {
        private readonly DataCollectionConverter<OrganizationRequest> _internalConverter = new DataCollectionConverter<OrganizationRequest>();

        public override void WriteJson(JsonWriter writer, OrganizationRequestCollection value, JsonSerializer serializer)
        {
            _internalConverter.WriteJson(writer, value, serializer);
        }

        public override OrganizationRequestCollection ReadJson(JsonReader reader, Type objectType, OrganizationRequestCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return (OrganizationRequestCollection)_internalConverter.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
        }
    }

    public class OrganizationResponseCollectionConverter : JsonConverter<OrganizationResponseCollection>
    {
        private readonly DataCollectionConverter<OrganizationResponse> _internalConverter = new DataCollectionConverter<OrganizationResponse>();

        public override void WriteJson(JsonWriter writer, OrganizationResponseCollection value, JsonSerializer serializer)
        {
            _internalConverter.WriteJson(writer, value, serializer);
        }

        public override OrganizationResponseCollection ReadJson(JsonReader reader, Type objectType, OrganizationResponseCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return (OrganizationResponseCollection)_internalConverter.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
        }
    }

    public class ArgumentsCollectionConverter : JsonConverter<ArgumentsCollection>
    {
        private readonly DataCollectionConverter<string, object> _internalConverter = new DataCollectionConverter<string, object>();

        public override void WriteJson(JsonWriter writer, ArgumentsCollection value, JsonSerializer serializer)
        {
            _internalConverter.WriteJson(writer, value, serializer);
        }

        public override ArgumentsCollection ReadJson(JsonReader reader, Type objectType, ArgumentsCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return (ArgumentsCollection)_internalConverter.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
        }
    }

    [JsonObject]
    public class ObjectSerialization
    {
        public ObjectSerialization(object objectToSerialize)
        {
            Value = objectToSerialize;
            Type = objectToSerialize.GetType().AssemblyQualifiedName;
        }

        [JsonProperty]
        public string Type { get; set; }

        [JsonProperty]
        public object Value { get; set; }
    }
}
