// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.PluginInventory.Tests;

/// <summary>
///     Loads a .NET Framework plugin assembly on this runtime and RUNS a plugin in it, against stub
///     Dataverse services - the shape a .NET-modern remote debugger host needs.
/// </summary>
/// <remarks>
///     Deliberately in process, which is the opposite of what <see cref="InventoryHost" /> does, and
///     for a reason worth spelling out: the inventory goes out of process to keep this assembly's
///     .NET <c>Microsoft.Xrm.Sdk</c> (through DeployUtils -> Dataverse.Client) away from the plugin's
///     net4x one. Executing a plugin needs the exact opposite. The plugin has to receive an
///     <c>IPluginExecutionContext</c> the HOST implements, so host and plugin must agree on one
///     single <c>Microsoft.Xrm.Sdk</c> - and they do, because the assembly Dataverse.Client ships for
///     net8.0/net10.0 carries the same identity as the net462 one: <c>Version=9.0.0.0</c>,
///     <c>PublicKeyToken=31bf3856ad364e35</c>. That is what
///     <see cref="CrossTargetFrameworkExecutionTests" /> asserts first, because everything else rests
///     on it.
/// </remarks>
internal static class PluginExecutionHost
{
    /// <summary>What a plugin did while it ran: what it traced, and what it asked Dataverse for.</summary>
    internal sealed record Run(IReadOnlyList<string> Trace, IReadOnlyList<string> OrganizationServiceCalls)
    {
        internal string TraceText => string.Join(Environment.NewLine, Trace);
    }

    /// <summary>
    ///     Loads <paramref name="pluginAssemblyPath" /> and executes the type named
    ///     <paramref name="pluginTypeName" /> as an <see cref="IPlugin" />.
    /// </summary>
    internal static Run Execute(string pluginAssemblyPath, string pluginTypeName)
    {
        var pluginType = Load(pluginAssemblyPath).GetType(pluginTypeName, throwOnError: true)!;

        // The (unsecure, secure) configuration convention Dataverse itself uses, and the one the
        // remote debugger probes for at RemoteDebugger.cs:331-336.
        var plugin = (IPlugin)Activator.CreateInstance(pluginType, new object[] { null, null })!;

        var provider = new StubServiceProvider();
        plugin.Execute(provider);

        return new Run(provider.Tracing.Lines, provider.Service.Calls);
    }

    /// <summary>Loads a plugin assembly built for .NET Framework into its own load context.</summary>
    internal static Assembly Load(string pluginAssemblyPath)
        => new PluginLoadContext(pluginAssemblyPath, InventoryHost.ShimDll)
            .LoadFromAssemblyPath(Path.GetFullPath(pluginAssemblyPath));

    /// <summary>
    ///     Resolves a net4x plugin assembly's dependencies the way a debugger host has to.
    /// </summary>
    /// <remarks>
    ///     A load context per plugin assembly, rather than the default one plus an
    ///     <c>AssemblyResolve</c> handler (what <c>PluginInventoryEngine</c> uses, and what a real
    ///     runner can keep using so that <c>Type.GetType</c> still finds the plugin): the net462 and
    ///     net48 fixtures share one assembly identity, so loading both into the default context would
    ///     silently hand the second test case the first flavour's assembly.
    /// </remarks>
    private sealed class PluginLoadContext : AssemblyLoadContext
    {
        /// <summary>
        ///     Assemblies the HOST owns, never to be loaded from beside the plugin.
        /// </summary>
        /// <remarks>
        ///     The plugin's output directory holds a copy-local net462 <c>Microsoft.Xrm.Sdk.dll</c>.
        ///     Loading it here would put two assemblies of that name in play and break the type
        ///     identity the whole approach depends on, in the least obvious way possible: the plugin
        ///     would reject the host's own <c>IPluginExecutionContext</c>. Returning null defers to
        ///     the default context, which already has the .NET build of the very same identity.
        ///     <para>
        ///         Note what is NOT in this set: <c>Microsoft.Xrm.Sdk.Workflow</c>. Dataverse.Client
        ///         ships no .NET build of it, and the net462 one - real IL, unlike the
        ///         <c>System.Activities</c> reference assembly - loads here perfectly well.
        ///     </para>
        /// </remarks>
        private static readonly HashSet<string> HostOwned = new(StringComparer.OrdinalIgnoreCase)
        {
            "Microsoft.Xrm.Sdk",
            "Microsoft.Crm.Sdk.Proxy",
        };

        private readonly string _probeDirectory;
        private readonly string _systemActivitiesShim;

        internal PluginLoadContext(string pluginAssemblyPath, string systemActivitiesShim)
            : base($"plugin:{Path.GetFileName(Path.GetDirectoryName(Path.GetFullPath(pluginAssemblyPath)))}")
        {
            _probeDirectory = Path.GetDirectoryName(Path.GetFullPath(pluginAssemblyPath))!;
            _systemActivitiesShim = systemActivitiesShim;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var name = assemblyName.Name;
            if (name == null || HostOwned.Contains(name))
                return null;

            // System.Activities does not exist on .NET 5+, and the copy beside the plugin is a
            // reference assembly. The stand-in is the only loadable answer.
            if (string.Equals(name, "System.Activities", StringComparison.OrdinalIgnoreCase))
                return File.Exists(_systemActivitiesShim) ? LoadFromAssemblyPath(_systemActivitiesShim) : null;

            var candidate = Path.Combine(_probeDirectory, name + ".dll");
            if (!File.Exists(candidate))
                return null;

            try
            {
                return LoadFromAssemblyPath(candidate);
            }
            catch (BadImageFormatException)
            {
                // A copy-local reference assembly: unloadable by design. Let the default context try.
                return null;
            }
        }
    }

    // ── Stub Dataverse services ─────────────────────────────────────────────────────────

    /// <summary>
    ///     The <see cref="IServiceProvider" /> Dataverse would hand the plugin.
    /// </summary>
    /// <remarks>
    ///     Throwing for unserved types is behaviour, not laziness: the framework asks for
    ///     <c>IPluginExecutionContext5</c> first and falls back to <c>IPluginExecutionContext</c> in a
    ///     catch (<c>XrmFramework/Context/LocalContext.cs:59-66</c>), so a provider that returned null
    ///     would hand it a null context instead. The real debugger throws too
    ///     (<c>LocalServiceProvider.cs:58</c>).
    /// </remarks>
    private sealed class StubServiceProvider : IServiceProvider
    {
        internal readonly StubTracingService Tracing = new();
        internal readonly StubOrganizationService Service = new();
        private readonly StubPluginExecutionContext _context = new();

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ITracingService)) return Tracing;
            if (serviceType == typeof(IPluginExecutionContext)) return _context;
            if (serviceType == typeof(IExecutionContext)) return _context;
            if (serviceType == typeof(IOrganizationServiceFactory)) return new StubOrganizationServiceFactory(Service);
            if (serviceType == typeof(IOrganizationService)) return Service;

            throw new NotImplementedException(serviceType.FullName);
        }
    }

    private sealed class StubOrganizationServiceFactory : IOrganizationServiceFactory
    {
        private readonly IOrganizationService _service;

        internal StubOrganizationServiceFactory(IOrganizationService service) => _service = service;

        public IOrganizationService CreateOrganizationService(Guid? userId) => _service;
    }

    private sealed class StubTracingService : ITracingService
    {
        internal List<string> Lines { get; } = new();

        public void Trace(string format, params object[] args)
            => Lines.Add(args is null || args.Length == 0 ? format : string.Format(format, args));
    }

    /// <summary>Answers everything with an empty result, which is all a plugin needs to run.</summary>
    private sealed class StubOrganizationService : IOrganizationService
    {
        internal List<string> Calls { get; } = new();

        public Guid Create(Entity entity)
        {
            Calls.Add($"Create {entity.LogicalName}");
            return Guid.NewGuid();
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            Calls.Add($"Retrieve {entityName}");
            return new Entity(entityName, id);
        }

        public void Update(Entity entity) => Calls.Add($"Update {entity.LogicalName}");

        public void Delete(string entityName, Guid id) => Calls.Add($"Delete {entityName}");

        public void Associate(string entityName, Guid id, Microsoft.Xrm.Sdk.Relationship relationship,
            EntityReferenceCollection related)
            => Calls.Add($"Associate {entityName}");

        public void Disassociate(string entityName, Guid id, Microsoft.Xrm.Sdk.Relationship relationship,
            EntityReferenceCollection related)
            => Calls.Add($"Disassociate {entityName}");

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            var entityName = query is QueryExpression expression ? expression.EntityName : query.GetType().Name;
            Calls.Add($"RetrieveMultiple {entityName}");
            return new EntityCollection(new List<Entity>()) { EntityName = entityName };
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            Calls.Add($"Execute {request.RequestName}");
            return new OrganizationResponse();
        }
    }

    /// <summary>
    ///     A context matching the fixture's PreOperation / Create / Synchronous / account step, so
    ///     that executing <c>AccountPlugin</c> dispatches to <c>OnCreate</c> and nothing else.
    /// </summary>
    private sealed class StubPluginExecutionContext : IPluginExecutionContext
    {
        public int Stage => 20; // PreOperation
        public IPluginExecutionContext ParentContext => null!;

        public int Mode => 0; // Synchronous
        public int IsolationMode => 2; // Sandbox
        public int Depth => 1;
        public string MessageName => "Create";
        public string PrimaryEntityName => "account";
        public Guid? RequestId { get; set; } = Guid.NewGuid();
        public string SecondaryEntityName => null!;
        public ParameterCollection InputParameters { get; } = new();
        public ParameterCollection OutputParameters { get; } = new();
        public ParameterCollection SharedVariables { get; } = new();
        public Guid UserId { get; } = Guid.NewGuid();
        public Guid InitiatingUserId { get; } = Guid.NewGuid();
        public Guid BusinessUnitId { get; } = Guid.NewGuid();
        public Guid OrganizationId { get; } = Guid.NewGuid();
        public string OrganizationName => "stub";
        public Guid PrimaryEntityId { get; } = Guid.NewGuid();
        public EntityImageCollection PreEntityImages { get; } = new();
        public EntityImageCollection PostEntityImages { get; } = new();
        public EntityReference OwningExtension { get; } = new EntityReference("pluginassembly", Guid.NewGuid());
        public Guid CorrelationId { get; } = Guid.NewGuid();
        public bool IsExecutingOffline => false;
        public bool IsOfflinePlayback => false;
        public bool IsInTransaction => true;
        public Guid OperationId { get; } = Guid.NewGuid();
        public DateTime OperationCreatedOn { get; } = DateTime.UtcNow;
    }
}
