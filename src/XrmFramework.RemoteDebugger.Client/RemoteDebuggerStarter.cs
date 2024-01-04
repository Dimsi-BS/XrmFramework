using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XrmFramework.DeployUtils;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.RemoteDebugger.Client;
using XrmFramework.RemoteDebugger.Client.Configuration;
using XrmFramework.RemoteDebugger.Client.Recorder;

// ReSharper disable once CheckNamespace
namespace XrmFramework.RemoteDebugger.Common;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public sealed class RemoteDebuggerStarter<T> where T : class, IRemoteDebuggerMessageManager
{
    private static readonly Lazy<IServiceProvider> LazyServiceProvider = new(DebuggerServiceCollectionHelper.ConfigureForRemoteDebug<T>, true);

    private IServiceProvider ServiceProvider => LazyServiceProvider.Value;

    /// <summary>
    /// Entrypoint for debugging all referenced projects
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public async Task RunAsync()
    {
        Console.WriteLine(@"Welcome to XrmFramework Remote Debugger");

        var assembliesToDebug = Assembly.GetCallingAssembly().GetReferencedAssemblies()
            .Select(Assembly.Load)
            .Where(a => a.GetType("XrmFramework.Plugin") != null
                        || a.GetType("XrmFramework.CustomApi") != null
                        || a.GetType("XrmFramework.Workflow.CustomWorkflowActivity") != null
            )
            .ToList();

        if (!assembliesToDebug.Any())
        {
            throw new ArgumentException(
                "No project containing components to debug were found, please check that they are referenced");
        }

        var solutionContext = ServiceProvider.GetRequiredService<ISolutionContext>();

        var remoteDebuggerHelper = ServiceProvider.GetRequiredService<RegistrationHelper>();

        assembliesToDebug.ForEach(assembly =>
        {
            var targetSolutionName = ServiceCollectionHelper.GetTargetSolutionName(assembly.GetName().Name);
            solutionContext.InitSolutionContext(targetSolutionName);
            remoteDebuggerHelper.UpdateDebugger(assembly);
        });

        using var manager = ServiceProvider.GetRequiredService<IRemoteDebuggerMessageManager>();

        await manager.RunAsync();
    }
}