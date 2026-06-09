// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#if !NET462_OR_GREATER
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace XrmFramework.DeployUtils.Factories
{
    /// <summary>
    ///     Lance l'outil d'inventaire net462 (<c>XrmFramework.PluginInventory.exe</c>) hors-process et
    ///     récupère le manifeste JSON sur sa sortie standard.
    /// </summary>
    /// <remarks>
    ///     Indispensable depuis un process net8/net10 : instancier un plugin net462 (pour exécuter son
    ///     enregistrement de steps) exige le runtime .NET Framework. Le déploiement n'est donc possible
    ///     que sous Windows (ou via un lanceur Mono en développement, cf. variables d'environnement).
    /// </remarks>
    internal static class PluginInventoryProcessRunner
    {
        private const string ExeFileName = "XrmFramework.PluginInventory.exe";

        public static string Run(string dllPath)
        {
            var exe = LocateExe();

            var psi = new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = new UTF8Encoding(false),
                StandardErrorEncoding = new UTF8Encoding(false),
            };

            // Sous Windows, l'exe net462 s'exécute directement. En dev (macOS/Linux), un lanceur
            // (ex. "mono") peut être fourni via XRMFRAMEWORK_INVENTORY_LAUNCHER.
            var launcher = Environment.GetEnvironmentVariable("XRMFRAMEWORK_INVENTORY_LAUNCHER");
            if (!string.IsNullOrWhiteSpace(launcher))
            {
                psi.FileName = launcher;
                psi.ArgumentList.Add(exe);
            }
            else
            {
                psi.FileName = exe;
            }

            psi.ArgumentList.Add(dllPath);

            using var process = Process.Start(psi)
                ?? throw new InvalidOperationException($"Impossible de démarrer '{psi.FileName}'.");

            // Lecture asynchrone des deux flux pour éviter tout interblocage de buffer.
            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            var stdout = stdoutTask.GetAwaiter().GetResult();
            var stderr = stderrTask.GetAwaiter().GetResult();

            if (process.ExitCode != 0)
                throw new InvalidOperationException(
                    $"L'inventaire des plugins ({ExeFileName}) a échoué (code {process.ExitCode}).{Environment.NewLine}{stderr}");

            // Avertissements éventuels (ex. dépendances manquantes) sans échec.
            if (!string.IsNullOrWhiteSpace(stderr))
                Console.Error.WriteLine(stderr);

            return stdout;
        }

        private static string LocateExe()
        {
            // 1) Override explicite.
            var overridePath = Environment.GetEnvironmentVariable("XRMFRAMEWORK_INVENTORY_EXE");
            if (!string.IsNullOrWhiteSpace(overridePath) && File.Exists(overridePath))
                return overridePath;

            // 2) Embarqué à côté du tool, sous inventory/.
            var candidate = Path.Combine(AppContext.BaseDirectory, "inventory", ExeFileName);
            if (File.Exists(candidate))
                return candidate;

            // 3) Repli : à la racine du tool.
            candidate = Path.Combine(AppContext.BaseDirectory, ExeFileName);
            if (File.Exists(candidate))
                return candidate;

            throw new FileNotFoundException(
                $"Outil d'inventaire introuvable ({ExeFileName}). Attendu sous " +
                $"'{Path.Combine(AppContext.BaseDirectory, "inventory")}'. Définissez la variable " +
                "d'environnement XRMFRAMEWORK_INVENTORY_EXE pour pointer vers l'exécutable net462.");
        }
    }
}
#endif
