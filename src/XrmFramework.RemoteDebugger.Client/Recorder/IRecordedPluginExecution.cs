using System;
using System.Collections.Generic;

namespace XrmFramework.RemoteDebugger.Client.Recorder;

public interface IRecordedPluginExecution
{
    Guid Id { get; set; }
    
    string Name { get; }
    
    IReadOnlyCollection<RemoteDebuggerMessage> Messages { get; }

    void AddMessage(RemoteDebuggerMessage message);
}