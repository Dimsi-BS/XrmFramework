using XrmFramework.RemoteDebugger;

namespace XrmFramework.DeployUtils.Model.Record;

public class RecordedCorrelation
    : MessageAddedEventBaseProvider<IRecordedPluginExecution, IRecordedSession>
    , IRecordedCorrelation
{
    public DateTime StartDate { get; set; }

    public override string Name => Id.ToString();

    protected override void AddMessageInternal(RemoteDebuggerMessage message)
    {
        var pluginExecution = AddPluginExecution(message.PluginExecutionId);
        pluginExecution.SetParent(this);
        
        pluginExecution.AddMessage(message);
    }

    private IRecordedPluginExecution AddPluginExecution(Guid pluginExecutionId)
    {
        if (_children.Find(p => p.Id == pluginExecutionId) is { } pluginExecution)
        {
            return pluginExecution;
        }

        pluginExecution = new RecordedPluginExecution
        {
            Id = pluginExecutionId,
            Selected = !_children.Any()
        };

        pluginExecution.SetParent(this);

        _children.Add(pluginExecution);

        return pluginExecution;
    }
}