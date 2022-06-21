﻿using System;

namespace XrmFramework.RemoteDebugger.Client.Configuration
{
    /// <summary>
    /// Information on the Target DebugAssembly, Definition of constants and conventions
    /// </summary>
    public class DebugAssemblySettings
    {
        /// <summary>Name of the Remote Debugger Assembly, should be standard and pushed in all recent solutions</summary>
        public const string DebugAssemblyName = "XrmFramework.RemoteDebuggerPlugin";

        /// <summary>Name of the Remote Debugger Plugin, should be standard and pushed in all recent solutions</summary>
        public const string DebugPluginName = "XrmFramework.RemoteDebugger.RemoteDebuggerPlugin";

        /// <summary>Name of the Remote Debugger CustomApi, should be standard and pushed in all recent solutions</summary>
        public const string DebugCustomApiName = "XrmFramework.RemoteDebugger.RemoteDebuggerCustomApi";

        ///<summary>
        /// This constant defines how many characters will be added in the CustomApis prefix to make them unique <br/>
        /// The characters will be taken from the debug session id so they will be the same for a given debugee
        /// </summary>
        private const int DebugCustomPrefixNumber = 3;

        public DebugAssemblySettings(Guid debugSessionId)
        {
            DebugCustomApiPrefix = debugSessionId.ToString().Substring(0, DebugCustomPrefixNumber);
        }

        private string DebugCustomApiPrefix { get; }

        /// <summary><see cref="Guid"/> of the Debugger Assembly</summary>
        /// <remarks>As the debug Assembly is heavily manipulated,
        /// it is simpler to keep these stored in a separate object make sure they're not lost</remarks>
        public Guid AssemblyId { get; set; }

        /// <summary><see cref="Guid"/> of the Debugger PluginType</summary>
        /// <remarks>As the debug Assembly is heavily manipulated,
        /// it is simpler to keep these stored in a separate object make sure they're not lost</remarks>
        public Guid PluginId { get; set; }

        /// <summary><see cref="Guid"/> of the Debugger CustomApi PluginType</summary>
        /// <remarks>As the debug Assembly is heavily manipulated,
        /// it is simpler to keep these stored in a separate object make sure they're not lost</remarks>
        public Guid CustomApiId { get; set; }

        /// <summary>
        /// Removes the added prefix used to deploy a CustomApi so it can look like the original
        /// </summary>
        /// <param name="uniqueName"> Unique Name of the CustomApi deployed on the RemoteDebugger</param>
        /// <returns>The Unique Name of the CustomApi stripped from its artificial prefix</returns>
        public static string RemoveCustomPrefix(string uniqueName)
        {
            var index = uniqueName.IndexOf('_');
            return uniqueName.Remove(index - DebugCustomPrefixNumber, DebugCustomPrefixNumber);
        }

        /// <summary>
        /// Adds an artificial prefix to a Unique Name so it can be pushed on the CRM without conflicting the original CustomApi
        /// </summary>
        /// <param name="uniqueName"> Unique Name of the original CustomApi</param>
        /// <returns>The Unique Name of the CustomApi ready to be deployed on the RemoteDebugger</returns>
        public string AddCustomPrefix(string uniqueName)
        {
            return uniqueName.Insert(uniqueName.IndexOf('_'), DebugCustomApiPrefix);
        }
    }
}