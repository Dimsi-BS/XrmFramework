using Newtonsoft.Json;

namespace XrmFramework
{
    /// <summary>Object meant to be serialized and pushed into the unsecured configuration of a step to insert custom parameters</summary>
    [JsonObject(MemberSerialization.OptIn)]
    public partial class StepConfiguration
    {
        [JsonProperty("relName")]
        public string RelationshipName { get; set; }

        /// <summary>The List of methods that the Crm knows are linked with the step</summary>
        [JsonProperty("registeredMethods")] public HashSet<string> RegisteredMethods { get; set; } = new();

        /// <summary>Allows for custom bypass of a given method without redeploying</summary>
        [JsonProperty("bannedMethods")] public List<string> BannedMethods { get; set; } = new();
    }
}
