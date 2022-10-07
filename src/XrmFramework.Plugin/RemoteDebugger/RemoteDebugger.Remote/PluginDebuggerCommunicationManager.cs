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

    public PluginDebuggerCommunicationManager(LocalPluginContext context, string assemblyQualifiedName, string securedConfig,
        string unsecuredConfig) : base(context)
    {
        AssemblyQualifiedName = assemblyQualifiedName;
        _unSecuredConfig = unsecuredConfig;
        _securedConfig = securedConfig;
    }
    public override DebugSession GetDebugSession()
    {
        var query = CreateBaseDebugSessionQuery(Context.GetInitiatingUserId().ToString());

        return Context.AdminOrganizationService.RetrieveAll<DebugSession>(query).FirstOrDefault();
    }

    protected override RemoteDebugExecutionContext InitRemoteContext()
    {
        var remoteContext = Context.RemoteContext;
        remoteContext.Id = Guid.NewGuid();
        remoteContext.TypeAssemblyQualifiedName = AssemblyQualifiedName;
        remoteContext.UnsecureConfig = _unSecuredConfig;
        remoteContext.SecureConfig = _securedConfig;
        return remoteContext;
    }
}

