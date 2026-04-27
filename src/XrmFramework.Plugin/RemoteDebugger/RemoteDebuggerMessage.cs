using System;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;

namespace XrmFramework.RemoteDebugger;

[JsonObject(MemberSerialization.OptIn)]
public partial class RemoteDebuggerMessage
{
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        ContractResolver = new RemoteDebuggerContractResolver()
    };

    public RemoteDebuggerMessage()
    {
    }

    public RemoteDebuggerMessage(RemoteDebuggerMessageType type, object? content, Guid pluginExecutionId)
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
    public string Content { get; set; } = null!;

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
            throw new TypeMismatchException($"The message is not an {messageType} message");

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

}

public class TypeMismatchException : Exception
{
    public TypeMismatchException(string message) : base(message) { }
}