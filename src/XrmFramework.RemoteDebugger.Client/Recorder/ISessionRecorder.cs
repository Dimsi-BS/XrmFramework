using System.Collections.Generic;

namespace XrmFramework.RemoteDebugger.Client.Recorder;

public interface ISessionRecorder
{
    void AddMessage(RemoteDebuggerMessage message);
    
    IReadOnlyCollection<IRecordedCorrelation> Correlations { get; }
}