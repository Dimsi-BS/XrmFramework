using Microsoft.Xrm.Sdk.Workflow;
using Newtonsoft.Json;

namespace XrmFramework.RemoteDebugger.Converters;


public class ArgumentsCollectionConverter : JsonConverter<ArgumentsCollection>
{
    private readonly DataCollectionConverter<string, object> _internalConverter = new DataCollectionConverter<string, object>();

    public override void WriteJson(JsonWriter writer, ArgumentsCollection value, JsonSerializer serializer)
    {
        _internalConverter.WriteJson(writer, value, serializer);
    }

    public override ArgumentsCollection ReadJson(JsonReader reader, System.Type objectType, ArgumentsCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return (ArgumentsCollection)_internalConverter.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
    }
}