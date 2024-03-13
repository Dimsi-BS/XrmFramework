using XrmFramework.RemoteDebugger.Client.Recorder;

namespace XrmFramework.DeployUtils.Model.Record;

public interface IRecordedCorrelation : IMessageAddedEventProvider<IRecordedPluginExecution, IRecordedSession>
{
    DateTime StartDate { get; set; }

}

