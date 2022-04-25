using Newtonsoft.Json;
using System.Collections.Generic;

namespace XrmFramework
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class StepConfiguration
    {
        [JsonProperty("relName")]
        public string RelationshipName { get; set; }

        [JsonProperty("methods")] public List<string> MethodNames { get; set; } = new();
    }
}
