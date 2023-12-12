using Newtonsoft.Json;

namespace XrmFramework.RemoteDebugger.Model.CrmComponentInfos
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AssemblyContextInfo
    {
        [JsonProperty("assemblyName")]
        public string AssemblyName { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("publicKeyToken")]
        public string PublicKeyToken { get; set; }
        [JsonProperty("culture")]
        public string Culture { get; set; }
        [JsonProperty("plugins")]
        public List<PluginInfo> Plugins { get; set; }
        [JsonProperty("workflows")]
        public List<WorkflowsInfo> Workflows { get; set; }
        [JsonProperty("customApis")]
        public List<CustomApiInfo> CustomApis { get; set; }
    }


    [JsonObject(MemberSerialization.OptIn)]
    public class PluginInfo
    {
        [JsonProperty("pluginName")]
        public string Name { get; set; }

        [JsonProperty("steps")]
        public List<StepInfo> Steps { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class StepInfo
    {
        [JsonProperty("description")]
        public string Description => $"{PluginTypeName} : {Stage} {Message} of {EntityName}";
        [JsonProperty("pluginTypeName")]
        public string PluginTypeName { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("stage")]
        public string Stage { get; set; }
        [JsonProperty("mode")]
        public string Mode { get; set; }
        [JsonProperty("entityName")]
        public string EntityName { get; set; }
        [JsonProperty("preImage")]
        public StepImageInfo PreImage { get; set; }
        [JsonProperty("postImage")]
        public StepImageInfo PostImage { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class StepImageInfo
    {
        [JsonProperty("isUsed")]
        public bool IsUsed { get; set; }
        [JsonProperty("allAtributes")]
        public bool AllAttributes { get; set; }
        [JsonProperty("attributes")]
        public List<string> Attributes { get; set; }


    }

    [JsonObject(MemberSerialization.OptIn)]
    public class WorkflowsInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class CustomApiInfo
    {
        [JsonProperty("uniqueName")]
        public string UniqueName { get; set; }
        [JsonProperty("requestParameters")]
        public List<CustomApiRequestParameterInfo> RequestParameters { get; set; }
        [JsonProperty("responseProperties")]
        public List<CustomApiResponsePropertyInfo> ResponseProperties { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class CustomApiRequestParameterInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("isOptional")]
        public bool IsOptional { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class CustomApiResponsePropertyInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
