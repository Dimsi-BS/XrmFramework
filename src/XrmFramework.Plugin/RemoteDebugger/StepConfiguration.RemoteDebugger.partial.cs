using Newtonsoft.Json;

namespace XrmFramework
{
    partial class StepConfiguration
    {

        [JsonProperty("debug")]
        public StepDebugInfos Debug { get; set; }
    }

    public class StepDebugInfos
    {

    }
}
