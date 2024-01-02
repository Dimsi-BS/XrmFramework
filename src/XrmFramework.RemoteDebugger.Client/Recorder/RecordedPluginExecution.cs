using System;
using System.Collections.Generic;

namespace XrmFramework.RemoteDebugger.Client.Recorder;

public class RecordedPluginExecution : IRecordedPluginExecution
{
    private readonly List<RemoteDebuggerMessage> _messages = new();
    
    public Guid Id { get; set; }
    
    public IReadOnlyCollection<RemoteDebuggerMessage> Messages => _messages.AsReadOnly();
    
    public void AddMessage(RemoteDebuggerMessage message) 
        => _messages.Add(message);
}