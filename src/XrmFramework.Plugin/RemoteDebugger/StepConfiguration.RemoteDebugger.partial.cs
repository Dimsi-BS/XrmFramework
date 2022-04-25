using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace XrmFramework
{
    public partial class StepConfiguration
    {
        [JsonProperty("debugSessions")] public List<Guid> DebugSessionIds { get; set; } = new();

        [JsonProperty("pluginName")]
        public string PluginName { get; set; }

        [JsonProperty("assemblyQualifiedName")]
        public string AssemblyQualifiedName { get; set; }

        [JsonProperty("debugSessionId")]
        public Guid DebugSessionId { get; set; }
    }
}
