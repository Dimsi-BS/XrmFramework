using XrmFramework.RemoteDebugger;

namespace XrmFramework.DeployUtils.Model.Record;

public class RecordedPluginExecution 
    : MessageAddedEventBaseProvider<RemoteDebuggerMessage, IRecordedCorrelation>
    , IRecordedPluginExecution
{
    public override string Name => _children.FirstOrDefault()?.GetContext<RemoteDebugExecutionContext>()?.ToPrettyString() ?? "Unknown";

    protected override void AddMessageInternal(RemoteDebuggerMessage message)
    {
        message.SetParent(this);
        message.Selected = !_children.Any();
        _children.Add(message);
    }
}