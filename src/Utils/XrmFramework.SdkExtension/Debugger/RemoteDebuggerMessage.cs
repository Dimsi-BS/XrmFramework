
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using XrmFramework.Debugger.Converters;
#if INTERNAL_NEWTONSOFT
using Newtonsoft.Json.Xrm;
#else 
using Newtonsoft.Json;
#endif

namespace XrmFramework.Debugger
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RemoteDebuggerMessage
    {
        [JsonProperty("messageType")]
        public RemoteDebuggerMessageType MessageType { get; set; }

        [JsonProperty("pluginExecutionId")]
        public Guid PluginExecutionId { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("userId")]
        public Guid? UserId { get; set; }

        public RemoteDebuggerMessage() { }

        public RemoteDebuggerMessage(RemoteDebuggerMessageType type, object content, Guid pluginExecutionId)
        {
            MessageType = type;
            PluginExecutionId = pluginExecutionId;

            Content = JsonConvert.SerializeObject(content, Converters.ToArray());
        }

        public T GetContext<T>() where T : RemoteDebugExecutionContext
        {
            if (MessageType != RemoteDebuggerMessageType.Context)
            {
                throw new Exception("The message is not a context message");
            }

            var stringContent = Content;

            try
            {
                stringContent = JsonConvert.DeserializeObject<string>(Content);
            }
            catch (JsonReaderException)
            {
                // the object is already ready to deserialize
            }

            return JsonConvert.DeserializeObject<T>(stringContent, Converters.ToArray());
        }

        public OrganizationRequest GetOrganizationRequest()
        {
            if (MessageType != RemoteDebuggerMessageType.Request)
            {
                throw new Exception("The message is not a request message");
            }


            var stringContent = Content;

            try
            {
                stringContent = JsonConvert.DeserializeObject<string>(Content);
            }
            catch (JsonReaderException)
            {
                // the object is already ready to deserialize
            }

            return JsonConvert.DeserializeObject<OrganizationRequest>(stringContent, Converters.ToArray());
        }

        public OrganizationResponse GetOrganizationResponse()
        {
            if (MessageType != RemoteDebuggerMessageType.Response)
            {
                throw new Exception("The message is not a response message");
            }

            var stringContent = Content;

            try
            {
                stringContent = JsonConvert.DeserializeObject<string>(Content);
            }
            catch (JsonReaderException)
            {
                // the object is already ready to deserialize
            }

            return JsonConvert.DeserializeObject<OrganizationResponse>(stringContent, Converters.ToArray());
        }

        private static readonly ICollection<JsonConverter> Converters = new List<JsonConverter>
        {
            new ParameterCollectionConverter(),
            new KeyAttributeCollectionConverter(),
            new FormattedValueCollectionConverter(),
            new EntityImageCollectionConverter(),
            new AttributeCollectionConverter(),
            new RelatedEntityCollectionConverter(),
            new RelationshipQueryCollectionConverter(),
            new OrganizationRequestCollectionConverter(),
            new OrganizationResponseCollectionConverter(),
            new CustomKeyValuePairConverter<string, object>(),
            new CustomKeyValuePairConverter<string, Entity>(),
            new CustomKeyValuePairConverter<string, string>(),
            new CustomKeyValuePairConverter<Relationship, QueryBase>(),
            new EntityConverter(),
            new ObjectSerializationConverter(),
            new ConditionExpressionConverter()
        };

        public Exception GetException()
        {
            if (MessageType != RemoteDebuggerMessageType.Exception)
            {
                throw new Exception("The message is not an exception message");
            }


            var stringContent = Content;

            try
            {
                stringContent = JsonConvert.DeserializeObject<string>(Content);
            }
            catch (JsonReaderException)
            {
                // the object is already ready to deserialize
            }

            return JsonConvert.DeserializeObject<Exception>(stringContent, Converters.ToArray());
        }

        public override string ToString() => $"{MessageType} / {PluginExecutionId} / {Content}";
    }
}