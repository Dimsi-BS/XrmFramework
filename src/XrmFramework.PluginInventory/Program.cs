// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;

namespace XrmFramework.PluginInventory
{
    /// <summary>
    /// net462 inventory tool, launched out-of-process by the net8 CLI (deploy plugins command).
    ///
    /// Usage: XrmFramework.PluginInventory.exe &lt;plugin-assembly-path.dll&gt;
    /// Output: JSON manifest (stdout); diagnostics and errors (stderr); return code 0 = success.
    /// </summary>
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                if (args.Length < 1 || string.IsNullOrWhiteSpace(args[0]))
                {
                    Console.Error.WriteLine("Usage: XrmFramework.PluginInventory <plugin-assembly-path.dll>");
                    return 1;
                }

                var dllPath = Path.GetFullPath(args[0]);
                if (!File.Exists(dllPath))
                {
                    Console.Error.WriteLine($"Assembly not found: {dllPath}");
                    return 1;
                }

                var json = PluginInventoryEngine.BuildManifestJson(dllPath);

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
