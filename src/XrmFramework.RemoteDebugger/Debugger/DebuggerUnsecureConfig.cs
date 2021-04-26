
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace XrmFramework.RemoteDebugger
{
    [JsonObject]
    class DebuggerUnsecureConfig
    {
        [JsonProperty("debugSessions")]
        public List<Guid> DebugSessionIds { get; set; }
    }
}
