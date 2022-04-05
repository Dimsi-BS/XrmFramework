
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace XrmFramework.RemoteDebugger
{
    [JsonObject]
    public class DebuggerUnsecureConfig
    {
        [JsonProperty("debugSessions")]
        public List<Guid> DebugSessionIds { get; set; }

        [JsonProperty("pluginName")]
        public string PluginName { get; set; }

        [JsonProperty("assemblyQualifiedName")]
        public string AssemblyQualifiedName { get; set; }

        [JsonProperty("debuggeeId")]
        public Guid DebugSessionId { get; set; }
    }
}