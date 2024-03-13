using XrmFramework.DeployUtils.Model.Record;

namespace XrmFramework.RemoteDebugger.Client.Recorder;

public class RecordedSession
    : MessageAddedEventBaseProvider<IRecordedCorrelation, INotAvailable>
    , IRecordedSession
{
    public RecordedSession(IConsoleService consoleService)
    {
        _consoleService = consoleService;
    }

    private readonly List<IRecordedCorrelation> _recordedCorrelations = new();
    private readonly IConsoleService _consoleService;

    public IReadOnlyCollection<IRecordedCorrelation> Correlations => _recordedCorrelations.AsReadOnly();

    public override string Name => string.Empty;

    protected override void AddMessageInternal(RemoteDebuggerMessage message)
    {
        IRecordedCorrelation recordedCorrelation = null;
        switch (message.MessageType)
        {
            case RemoteDebuggerMessageType.Context:
                {
                    var context = message.GetContext<RemoteDebugExecutionContext>();

                    recordedCorrelation = AddCorrelation(context.CorrelationId);
                    _consoleService.AddCorrelation(recordedCorrelation);

                    recordedCorrelation.AddMessage(message);
                    break;
                }
            case RemoteDebuggerMessageType.Ping:
                // Do not log Ping messages
                break;
            default:
                {
                    recordedCorrelation =
                        _recordedCorrelations.Find(c => c.Children.Any(p => p.Id == message.PluginExecutionId));
                    recordedCorrelation.AddMessage(message);
                    break;
                }
        }

        _consoleService.Refresh();
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
#pragma warning disable XRM0300
            StartDate = DateTime.Now
#pragma warning restore XRM0300
        };
        
        _recordedCorrelations.Add(correlation);

        return correlation;
    }
}