// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;

namespace XrmFramework.PluginInventory
{
    /// <summary>
    /// Outil d'inventaire net462, lancé hors-process par le CLI net8 (commande deploy plugins).
    ///
    /// Usage : XrmFramework.PluginInventory.exe &lt;chemin-assembly-plugin.dll&gt;
    /// Sortie : manifeste JSON (stdout) ; diagnostics et erreurs (stderr) ; code retour 0 = succès.
    /// </summary>
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                if (args.Length < 1 || string.IsNullOrWhiteSpace(args[0]))
                {
                    Console.Error.WriteLine("Usage : XrmFramework.PluginInventory <chemin-assembly-plugin.dll>");
                    return 1;
                }

                var dllPath = Path.GetFullPath(args[0]);
                if (!File.Exists(dllPath))
                {
                    Console.Error.WriteLine($"Assembly introuvable : {dllPath}");
                    return 1;
                }

                var json = PluginInventoryEngine.BuildManifestJson(dllPath);

                // Émettre strictement le JSON sur stdout (UTF-8 sans BOM) ; tout le reste sur stderr.
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
