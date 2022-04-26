using Newtonsoft.Json;
using System.Collections.Generic;

namespace XrmFramework
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class StepConfiguration
    {
        [JsonProperty("relName")]
        public string RelationshipName { get; set; }

        [JsonProperty("registeredMethods")] public List<string> RegisteredMethods { get; set; } = new();

        [JsonProperty("bannedMethods")] public List<string> BannedMethods { get; set; } = new();
    }
}
