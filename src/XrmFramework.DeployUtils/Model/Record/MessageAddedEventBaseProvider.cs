using Newtonsoft.Json;

namespace XrmFramework.DeployUtils.Model.Record;


public abstract class MessageAddedEventBaseProvider<TChildren, TParent> 
    : RemoteDebuggerBaseObject
    , IMessageAddedEventProvider<TChildren, TParent>
    where TParent : IRemoteDebuggerObject 
    where TChildren : IRemoteDebuggerObject
{
    protected readonly List<TChildren> _children = new ();

    private TParent _parent = default;

    [JsonProperty("children")]
    public IReadOnlyCollection<TChildren> Children => _children.AsReadOnly();

    public TParent Parent => _parent;

    public event MessageAddedHandler MessageAdded;

    public void AddMessage(RemoteDebugger.RemoteDebuggerMessage message)
    {
        AddMessageInternal(message);
    }

    public void SetParent(TParent parent)
    {
        _parent = parent;
    }
}
