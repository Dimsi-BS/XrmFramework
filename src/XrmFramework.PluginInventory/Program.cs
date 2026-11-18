// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XrmFramework.PluginInventory
{
    /// <summary>
    /// Inventory host, launched out-of-process by the CLI (deploy plugins command).
    ///
    /// Usage: dotnet XrmFramework.PluginInventory.dll &lt;plugin-assembly-path.dll&gt;
    /// Output: JSON manifest (stdout); diagnostics and errors (stderr); return code 0 = success.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        ///     What this runtime cannot provide on its own and the host therefore ships beside itself.
        /// </summary>
        /// <remarks>
        ///     <c>System.Activities</c> has no .NET 5+ equivalent, so a plugin assembly's
        ///     <c>CodeActivity</c>-derived types would fail to load and its Custom Workflow Activities
        ///     would be missing from the manifest. The stand-in beside us satisfies the reference; it
        ///     is only ever loaded to DESCRIBE activities, never to run them.
        ///     <para>
        ///         Resolved before the plugin's own directory on purpose: that directory always holds a
        ///         copy-local <c>System.Activities.dll</c>, and it is always a reference assembly that a
        ///         modern runtime cannot load.
        ///     </para>
        /// </remarks>
        private static IReadOnlyDictionary<string, string> HostSuppliedAssemblies()
            => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["System.Activities"] = Path.Combine(AppContext.BaseDirectory, "System.Activities.dll"),
            };

        private static int Main(string[] args)
        {
            try
            {
                if (args.Length < 1 || string.IsNullOrWhiteSpace(args[0]))
                {
                    Console.Error.WriteLine("Usage: dotnet XrmFramework.PluginInventory.dll <plugin-assembly-path.dll>");
                    return 1;
                }

                var dllPath = Path.GetFullPath(args[0]);
                if (!File.Exists(dllPath))
                {
                    Console.Error.WriteLine($"Assembly not found: {dllPath}");
                    return 1;
                }

                var json = PluginInventoryEngine.BuildManifestJson(dllPath, HostSuppliedAssemblies());

                // Emit strictly the JSON on stdout (UTF-8 without BOM); everything else on stderr.
                var stdout = Console.OpenStandardOutput();
                var bytes = new UTF8Encoding(false).GetBytes(json);
                stdout.Write(bytes, 0, bytes.Length);
                stdout.Flush();
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(PluginInventoryEngine.Flatten(ex));
                return 2;
            }
        }
    }
}
