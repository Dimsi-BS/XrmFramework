// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace XrmFramework.RemoteDebugger.Client
{
    /// <summary>
    ///     Makes a .NET Framework plugin assembly resolvable by a .NET runner, so that the debugger's
    ///     <c>Type.GetType</c> lookup finds the plugin Dataverse asked for.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A .NET Framework runner needs none of this: it project-references the Plugins project,
    ///         so the assembly sits in its own output directory and the CLR finds it unaided. A .NET
    ///         runner cannot project-reference a net48 project at all, so its build copies the
    ///         plugin's output into a folder of its own and points this at it.
    ///     </para>
    ///     <para>
    ///         The plugin assembly itself stays net462/net48 - it is the one that gets deployed, and
    ///         debugging a different build would defeat the purpose. A .NET-modern runtime loads and
    ///         runs it because mscorlib and friends redirect to the facades, the same property the
    ///         plugin inventory host has relied on since deployment started working off Windows.
    ///     </para>
    ///     <para>
    ///         Nothing is loaded eagerly, and that is the point rather than an economy. Resolution
    ///         runs through <see cref="AppDomain.AssemblyResolve" />, which the runtime consults only
    ///         for what it could not resolve itself - so the runner's own <c>Microsoft.Xrm.Sdk</c>
    ///         always wins and the plugin's copy-local net462 one is never even offered. That matters:
    ///         two assemblies of that name in one process and the plugin would reject the
    ///         <c>IPluginExecutionContext</c> the runner hands it, a cast failure with nothing in it
    ///         to suggest why. Here it cannot arise. For the same reason a plugin directory full of
    ///         .NET Framework builds of <c>System.Memory</c>, <c>System.Text.Json</c> and the rest is
    ///         harmless: the runtime has those, so they are never asked for.
    ///     </para>
    /// </remarks>
    public static class PluginAssemblyLoader
    {
        private static readonly object Gate = new object();
        private static readonly HashSet<string> ProbeDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static string _systemActivitiesStandIn;
        private static bool _resolverInstalled;

        /// <summary>
        ///     Makes the plugin assemblies in <paramref name="directory" /> resolvable by name, along
        ///     with the dependencies sitting beside them.
        /// </summary>
        /// <param name="directory">
        ///     Directory holding a plugin assembly and its dependencies - typically the Plugins
        ///     project's build output, copied next to the runner. A relative path resolves against the
        ///     runner's own directory.
        /// </param>
        /// <param name="systemActivitiesStandIn">
        ///     Optional path to an assembly to serve in place of <c>System.Activities</c>, which does
        ///     not exist on .NET. Only needed to ENUMERATE the types of an assembly that contains
        ///     Custom Workflow Activities; resolving a plugin type never touches them, so debugging
        ///     plugins and Custom APIs needs nothing here. Running an activity is not possible on .NET
        ///     whatever is passed - see <c>WorkflowActivitySupport</c>.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> when the directory was found and registered;
        ///     <see langword="false" /> when <paramref name="directory" /> is not set or does not
        ///     exist, so that the same startup code can serve a .NET Framework runner, which does not
        ///     need this and has no such folder.
        /// </returns>
        public static bool UsePluginDirectory(string directory, string systemActivitiesStandIn = null)
        {
            if (string.IsNullOrWhiteSpace(directory))
                return false;

            var fullPath = Path.IsPathRooted(directory)
                ? directory
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? ".", directory);

            if (!Directory.Exists(fullPath))
                return false;

            lock (Gate)
            {
                ProbeDirectories.Add(fullPath);

                if (!string.IsNullOrWhiteSpace(systemActivitiesStandIn))
                    _systemActivitiesStandIn = systemActivitiesStandIn;

                if (!_resolverInstalled)
                {
                    AppDomain.CurrentDomain.AssemblyResolve += Resolve;
                    _resolverInstalled = true;
                }
            }

            return true;
        }

        /// <summary>
        ///     Resolves a plugin assembly, or one of its dependencies, from beside it.
        /// </summary>
        private static Assembly Resolve(object sender, ResolveEventArgs args)
        {
            var simpleName = new AssemblyName(args.Name).Name;
            if (string.IsNullOrEmpty(simpleName))
                return null;

            if (string.Equals(simpleName, "System.Activities", StringComparison.OrdinalIgnoreCase))
            {
                return !string.IsNullOrWhiteSpace(_systemActivitiesStandIn)
                       && File.Exists(_systemActivitiesStandIn)
                    ? Assembly.LoadFrom(_systemActivitiesStandIn)
                    : null;
            }

            string[] directories;
            lock (Gate)
            {
                directories = ProbeDirectories.ToArray();
            }

            foreach (var directory in directories)
            {
                var candidate = Path.Combine(directory, simpleName + ".dll");
                if (!File.Exists(candidate))
                    continue;

                try
                {
                    return Assembly.LoadFrom(candidate);
                }
                catch (BadImageFormatException)
                {
                    // A copy-local REFERENCE assembly - the System.Activities beside a plugin that
                    // has workflow activities is one. Unloadable by design, and not an error here.
                    return null;
                }
            }

            return null;
        }
    }
}
