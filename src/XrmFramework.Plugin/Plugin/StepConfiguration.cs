using Newtonsoft.Json;

namespace XrmFramework
{
    [JsonObject(MemberSerialization.OptIn)]
    internal partial class StepConfiguration
    {
        [JsonProperty("relName")]
        public string RelationshipName { get; set; }

        [JsonProperty("configuration")]
        public string Configuration { get; set; }
    }
}
