using XrmFramework.RemoteDebugger;

namespace XrmFramework.DeployUtils.Model.Record;

public delegate void MessageAddedHandler(RemoteDebuggerMessage message);

public interface IMessageAddedEventProvider<TChildren, TParent> : IRemoteDebuggerObject where TParent : IRemoteDebuggerObject
{
    event MessageAddedHandler MessageAdded;

    IReadOnlyCollection<TChildren> Children { get; }

    TParent Parent { get; }

    void SetParent(TParent parent);

    void AddMessage(RemoteDebuggerMessage message);
}
