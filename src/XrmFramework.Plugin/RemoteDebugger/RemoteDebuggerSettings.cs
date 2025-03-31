using Newtonsoft.Json;
using XrmFramework.RemoteDebugger.Client.Infrastructure.ContractResolvers;

// ReSharper disable once CheckNamespace
namespace XrmFramework.RemoteDebugger;

public static class RemoteDebuggerSettings
{
    public static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        ContractResolver = new RemoteDebuggerContractResolver()
    };
}
