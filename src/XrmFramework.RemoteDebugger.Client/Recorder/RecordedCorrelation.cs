using System;
using System.Collections.Generic;

namespace XrmFramework.RemoteDebugger.Client.Recorder;

public class RecordedCorrelation : IRecordedCorrelation
{
    public Guid Id { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public void AddMessage(RemoteDebuggerMessage message)
    {
        var pluginExecution = AddPluginExecution(message.PluginExecutionId);
        
        pluginExecution.AddMessage(message);
    }
    
    private readonly List<IRecordedPluginExecution> _pluginExecutions = new();
    
    public IReadOnlyCollection<IRecordedPluginExecution> PluginExecutions => _pluginExecutions.AsReadOnly();
    
    private IRecordedPluginExecution AddPluginExecution(Guid pluginExecutionId)
    {
        if (_pluginExecutions.Find(p => p.Id == pluginExecutionId) is { } pluginExecution)
        {
            return pluginExecution;
        }

        pluginExecution = new RecordedPluginExecution
        {
            Id = pluginExecutionId
        };
        
        _pluginExecutions.Add(pluginExecution);

        return pluginExecution;
    }
}