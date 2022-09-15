using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.RemoteDebugger.Converters;

namespace XrmFramework.RemoteDebugger;

[JsonObject(MemberSerialization.OptIn)]
public class RemoteDebuggerMessage
{
	private static readonly JsonSerializerSettings SerializerSettings = new()
	{
		ContractResolver = new RemoteDebuggerContractResolver()
	};

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
		new ArgumentsCollectionConverter(),
		new EntityConverter(),
		new ObjectSerializationConverter(),
		new ConditionExpressionConverter(),
		new AliasedValueConverter()
	};

	public RemoteDebuggerMessage()
	{
	}

	public RemoteDebuggerMessage(RemoteDebuggerMessageType type, object content, Guid pluginExecutionId)
	{
		MessageType = type;
		PluginExecutionId = pluginExecutionId;

		Content = JsonConvert.SerializeObject(content, SerializerSettings);
	}

	[JsonProperty("messageType")]
	public RemoteDebuggerMessageType MessageType { get; set; }

	[JsonProperty("pluginExecutionId")]
	public Guid PluginExecutionId { get; set; }

	[JsonProperty("content")]
	public string Content { get; set; }

	[JsonProperty("userId")]
	public Guid? UserId { get; set; }

	public T GetContext<T>() where T : RemoteDebugExecutionContext
		=> Deserialize<T>(RemoteDebuggerMessageType.Context);

	public OrganizationRequest GetOrganizationRequest()
		=> Deserialize<OrganizationRequest>(RemoteDebuggerMessageType.Request);

	public OrganizationResponse GetOrganizationResponse()
		=> Deserialize<OrganizationResponse>(RemoteDebuggerMessageType.Response);

	public Exception GetException()
		=> Deserialize<Exception>(RemoteDebuggerMessageType.Exception);

	public override string ToString() => $"{MessageType} / {PluginExecutionId} / {Content}";

	private T Deserialize<T>(RemoteDebuggerMessageType messageType)
	{
		if (MessageType != messageType)
			throw new Exception($"The message is not an {messageType} message");

		var stringContent = Content;

		try
		{
			stringContent = JsonConvert.DeserializeObject<string>(Content);
		}
		catch (JsonReaderException)
		{
			// the object is already ready to deserialize
		}

		return JsonConvert.DeserializeObject<T>(stringContent, SerializerSettings);
	}

	private class RemoteDebuggerContractResolver : DefaultContractResolver
	{
		/// <inheritdoc />
		protected override JsonConverter ResolveContractConverter(Type objectType)
		{
			var converter = Converters.FirstOrDefault(c =>
				typeof(JsonConverter<>).MakeGenericType(objectType).IsInstanceOfType(c));

			if (converter != null) return converter;

			if (objectType.IsGenericType && objectType.GenericTypeArguments.Length == 2 &&
				typeof(KeyValuePair<,>).MakeGenericType(objectType.GenericTypeArguments).IsAssignableFrom(objectType))
			{
				var converterType =
					typeof(CustomKeyValuePairConverter<,>).MakeGenericType(objectType.GenericTypeArguments);

				return (JsonConverter)Activator.CreateInstance(converterType);
			}

			return base.ResolveContractConverter(objectType);
		}
		
		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

			// only serializer properties that start with the specified character
			properties =
				properties.Where(p => p.PropertyName != "WatsonBuckets").ToList();

			return properties;
		}
	}
}