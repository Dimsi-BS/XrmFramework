
using Newtonsoft.Json;

namespace XrmFramework.DeployUtils.Model.Record
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IRemoteDebuggerObject
    {
        [JsonProperty("name")]
        Guid Id { get; }

        [JsonProperty("name")]
        string Name { get; }

        bool Selected { get; set; }
    }
}
