using System;
using System.Collections.Generic;

namespace XrmFramework.RemoteDebugger.Client.Recorder;

public interface IRecordedCorrelation
{
    Guid Id { get; set; }
    DateTime StartDate { get; set; }
    
    IReadOnlyCollection<IRecordedPluginExecution> PluginExecutions { get; }
    
    void AddMessage(RemoteDebuggerMessage message);
}