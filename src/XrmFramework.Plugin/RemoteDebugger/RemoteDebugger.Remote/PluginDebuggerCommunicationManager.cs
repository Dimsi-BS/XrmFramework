using System;
using System.Linq;
using XrmFramework.BindingModel;
using XrmFramework.RemoteDebugger;

namespace XrmFramework.Remote;

internal class PluginDebuggerCommunicationManager : DebuggerCommunicationManager
{
    protected string AssemblyQualifiedName;
    private readonly string _unSecuredConfig;
    private readonly string _securedConfig;

    public PluginDebuggerCommunicationManager(string assemblyQualifiedName, string securedConfig,
        string unsecuredConfig)
    {
        AssemblyQualifiedName = assemblyQualifiedName;
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
        remoteContext.TypeAssemblyQualifiedName = AssemblyQualifiedName;
        remoteContext.UnsecureConfig = _unSecuredConfig;
        remoteContext.SecureConfig = _securedConfig;
        return remoteContext;
    }
}

