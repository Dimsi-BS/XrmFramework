using XrmFramework.DeployUtils.Model.Record;

namespace XrmFramework.RemoteDebugger;

partial class RemoteDebuggerMessage : MessageAddedEventBaseProvider<INotAvailable, IRecordedPluginExecution>
{
    public override string Name => $"{MessageType}";

    protected override void AddMessageInternal(RemoteDebuggerMessage message)
    {
        throw new NotImplementedException();
    }
}
