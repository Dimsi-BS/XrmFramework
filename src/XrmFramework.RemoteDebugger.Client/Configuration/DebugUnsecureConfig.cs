using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmFramework.RemoteDebugger.Client.Configuration
{
    [JsonObject]
    [Serializable]
    public class DebugUnsecureConfig
    {
        [JsonProperty("debugSessions")]
        public List<Guid> DebugSessionIds { get; set; }

        [JsonProperty("pluginName")]
        public string PluginName { get; set; }

        [JsonProperty("assemblyQualifiedName")]
        public string AssemblyQualifiedName { get; set; }

        [JsonProperty("debugUserUri")]
        public string DebugUserUri { get; set; }
    }
}
