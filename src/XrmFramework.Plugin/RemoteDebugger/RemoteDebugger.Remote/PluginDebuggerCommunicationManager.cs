using System;
using System.Linq;
using XrmFramework.BindingModel;
using XrmFramework.RemoteDebugger;

namespace XrmFramework.Remote;

internal class PluginDebuggerCommunicationManager : DebuggerCommunicationManager
{
    private readonly string _assemblyQualifiedName;
    private readonly string _unSecuredConfig;
    private readonly string _securedConfig;

    public PluginDebuggerCommunicationManager(string assemblyQualifiedName, string securedConfig,
        string unsecuredConfig)
    {
        _assemblyQualifiedName = assemblyQualifiedName;
        _unSecuredConfig = unsecuredConfig;
        _securedConfig = securedConfig;
    }
    public override DebugSession GetDebugSession(LocalPluginContext localContext)
    {
        var query = CreateBaseDebugSessionQuery(localContext.GetInitiatingUserId().ToString());

        return localContext.AdminOrganizationService.RetrieveAll<DebugSession>(query).FirstOrDefault();
    }

    protected override RemoteDebugExecutionContext InitRemoteContext(LocalPluginContext localContext)
    {
        var remoteContext = localContext.RemoteContext;
        remoteContext.Id = Guid.NewGuid();
        remoteContext.TypeAssemblyQualifiedName = _assemblyQualifiedName;
        remoteContext.UnsecureConfig = _unSecuredConfig;
        remoteContext.SecureConfig = _securedConfig;
        return remoteContext;
    }
}

