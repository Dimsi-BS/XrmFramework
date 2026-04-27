using System;
using Newtonsoft.Json;

namespace XrmFramework
{
    public partial class StepConfiguration
    {
        /// <summary>Name of the plugin</summary>
        [JsonProperty("pluginName")]
        public string PluginName { get; set; }

        [JsonProperty("assemblyFullName")] public string AssemblyName { get; set; }

        /// <summary>Name of the Assembly</summary>
        [JsonProperty("assemblyQualifiedName")]
        public string AssemblyQualifiedName { get; set; }

        /// <summary>Id of the Debug Session linked to the step if pushed in the RemoteDebuggerPlugin</summary>
        [JsonProperty("debugSessionId")]
        public Guid DebugSessionId { get; set; }
    }
}
