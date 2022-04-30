using System;

namespace XrmFramework.RemoteDebugger.Client.Configuration
{
    public class DebugAssemblySettings
    {
        public const string DebugAssemblyName = "XrmFramework.RemoteDebuggerPlugin";
        public const string DebugPluginName = "XrmFramework.RemoteDebugger.RemoteDebuggerPlugin";
        public const string DebugCustomApiName = "XrmFramework.RemoteDebugger.RemoteDebuggerCustomApi";

        // This constant defines how many characters will be added in the CustomApis prefix to make them unique
        // The characters will be taken from the debug session id so they will be the same for a given debugee
        public const int DebugCustomPrefixNumber = 3;

        public string DebugCustomPrefix { get; set; }

        public Guid AssemblyId { get; set; }
        public Guid PluginId { get; set; }
        public Guid CustomApiId { get; set; }

        public static string RemoveCustomPrefix(string uniqueName)
        {
            var index = uniqueName.IndexOf('_');
            return uniqueName.Remove(index - DebugCustomPrefixNumber, DebugCustomPrefixNumber);
        }

        public string AddCustomPrefix(string uniqueName)
        {
            return uniqueName.Insert(uniqueName.IndexOf('_'), DebugCustomPrefix);
        }
    }
}
