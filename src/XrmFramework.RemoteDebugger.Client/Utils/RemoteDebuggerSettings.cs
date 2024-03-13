using Spectre.Console.Cli;
using System.ComponentModel;

namespace XrmFramework.RemoteDebugger.Client.Utils
{
    internal class RemoteDebuggerSettings : CommandSettings
    {
        [CommandOption("-c|--connectionString")]
        [Description("The name of the connection string")]
        public string ConnectionName { get; set; }
    }
}
