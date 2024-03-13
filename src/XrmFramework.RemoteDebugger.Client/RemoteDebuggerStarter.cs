using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Spectre.Console;
using XrmFramework.RemoteDebugger.Client;
using XrmFramework.RemoteDebugger.Client.Configuration;
using Spectre.Console.Cli;
using XrmFramework.RemoteDebugger.Client.Utils;
using XrmFramework.RemoteDebugger.Client.Commands;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace XrmFramework.RemoteDebugger.Common;


[SuppressMessage("ReSharper", "UnusedType.Global")]
public sealed class RemoteDebuggerStarter<TProgram, TMessageManager> where TMessageManager : class, IRemoteDebuggerMessageManager
{
    /// <summary>
    /// Entrypoint for debugging all referenced projects
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public async Task RunAsync(string[] args)
    {
        var assembly = Assembly.GetCallingAssembly();

        AnsiConsole.Write(new FigletText("XrmFramework").Centered().Color(Color.Blue));
        AnsiConsole.Write(new FigletText("Remote Debugger").RightJustified().Color(Color.Green));
        AnsiConsole.WriteLine();

        var serviceCollection = new DebuggerServiceCollectionFactory().CreateServiceCollection<TMessageManager>();

        var registrar = new TypeRegistrar(serviceCollection);
        var commandApp = new CommandApp(registrar);

        commandApp.SetDefaultCommand<RemoteDebuggerCommand<TProgram>>();

        await commandApp.RunAsync(args);
    }

}