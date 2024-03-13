namespace XrmFramework.DeployUtils.Model.Record;

public class MessageAddedEventArgs: EventArgs
{
    public Guid CorrelationId { get; set; }
}