using Newtonsoft.Json;
using XrmFramework.DeployUtils.Context;

namespace XrmFramework.RemoteDebugger.Client.Model
{
    public partial class DebugSession
    {
        public string AssemblyDebugInfo { get; set; }

        public IAssemblyContext AssemblyDiff => JsonConvert.DeserializeObject<IAssemblyContext>(AssemblyDebugInfo);
    }
}
