using XrmFramework.RemoteDebugger;

namespace XrmFramework.DeployUtils.Model.Record;

public interface IRecordedPluginExecution : IMessageAddedEventProvider<RemoteDebuggerMessage, IRecordedCorrelation>
{
    string Name { get; }
}