using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.IO;

namespace XrmFramework.RemoteDebugger;

[JsonObject(MemberSerialization.OptIn)]
public class RemoteDebuggerMessage
{

    public RemoteDebuggerMessage()
    {
    }

    public RemoteDebuggerMessage(RemoteDebuggerMessageType type, object content, Guid pluginExecutionId)
    {
        MessageType = type;
        PluginExecutionId = pluginExecutionId;

        Content = JsonConvert.SerializeObject(content, RemoteDebuggerSettings.JsonSerializerSettings);
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
            throw new InvalidDataException($"The message is not an {messageType} message");

        var stringContent = Content;

        try
        {
            stringContent = JsonConvert.DeserializeObject<string>(Content);
        }
        catch (JsonReaderException)
        {
            // the object is already ready to deserialize
        }

        return JsonConvert.DeserializeObject<T>(stringContent, RemoteDebuggerSettings.JsonSerializerSettings);
    }

}
