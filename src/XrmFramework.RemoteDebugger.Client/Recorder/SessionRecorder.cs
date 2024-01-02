using System;
using System.Collections.Generic;
using System.Linq;

namespace XrmFramework.RemoteDebugger.Client.Recorder;

public class SessionRecorder : ISessionRecorder
{
    private readonly List<IRecordedCorrelation> _recordedCorrelations = new();
    
    public IReadOnlyCollection<IRecordedCorrelation> Correlations => _recordedCorrelations.AsReadOnly();

    public void AddMessage(RemoteDebuggerMessage message)
    {
        switch (message.MessageType)
        {
            case RemoteDebuggerMessageType.Context:
            {
                var context = message.GetContext<RemoteDebugExecutionContext>();

                var recordedCorrelation = AddCorrelation(context.CorrelationId);

                recordedCorrelation.AddMessage(message);
                break;
            }
            case RemoteDebuggerMessageType.Ping:
                // Do not log Ping messages
                break;
            default:
            {
                var recordedCorrelation =
                    _recordedCorrelations.Find(c => c.PluginExecutions.Any(p => p.Id == message.PluginExecutionId));
                recordedCorrelation.AddMessage(message);
                break;
            }
        }
    }

    private IRecordedCorrelation AddCorrelation(Guid correlationId)
    {
        var correlation = _recordedCorrelations.Find(c => c.Id == correlationId);
        if (correlation != null)
        {
            return correlation;
        }
        
        correlation = new RecordedCorrelation
        {
            Id = correlationId,
            StartDate = DateTime.Now
        };
        
        _recordedCorrelations.Add(correlation);

        return correlation;
    }
}