using Newtonsoft.Json;

namespace XrmFramework
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class StepConfiguration
    {
        [JsonProperty("relName")]
        public string RelationshipName { get; set; }
    }
}
