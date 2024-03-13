
using Newtonsoft.Json;

namespace XrmFramework.DeployUtils.Model.Record
{
    public abstract class RemoteDebuggerBaseObject : IRemoteDebuggerObject
    {
        public Guid Id { get; set; }

        public bool Selected { get; set; }

        public abstract string Name { get; }

        protected abstract void AddMessageInternal(RemoteDebugger.RemoteDebuggerMessage message);
    }
}