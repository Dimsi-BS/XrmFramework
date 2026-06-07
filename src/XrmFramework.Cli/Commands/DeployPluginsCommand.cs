// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO;
using System.Reflection;
using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils;
using XrmFramework.DeployUtils.Configuration;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// Commande <c>xrmframework deploy plugins</c> : déploie une assembly XrmFramework
/// (plugins, custom APIs, workflows) vers l'environnement sélectionné dans
/// <c>Config/xrmFramework.config</c>. Délègue à
/// <see cref="RegistrationHelper.RegisterPluginsAndWorkflows(Assembly, string, bool, bool)" />.
/// </summary>
public sealed class DeployPluginsCommand : Command<DeployPluginsCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("--dll <PATH>")]
        [System.ComponentModel.Description("Assembly net8.0 du projet plugin à déployer (plugins, custom APIs, workflows).")]
        public string? DllPath { get; init; }

        [CommandOption("--project <NAME>")]
        [System.ComponentModel.Description("Nom du projet tel que déclaré dans xrmFramework.config (ex. Plugins).")]
        public string? Project { get; init; }

        [CommandOption("--project-root <DIR>")]
        [System.ComponentModel.Description("Racine du projet contenant le dossier Config/ (défaut : dossier courant).")]
        public string ProjectRoot { get; init; } = ".";

        [CommandOption("--on-premise")]
        [System.ComponentModel.Description("Cible un CRM On-Premises (défaut : Dataverse Online).")]
        public bool OnPremise { get; init; }

        [CommandOption("-n|--noprompt")]
        [System.ComponentModel.Description("Mode silencieux : ignore la confirmation de connexion (CI/CD).")]
        public bool NoPrompt { get; init; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(DllPath))
                return ValidationResult.Error("L'option --dll est obligatoire.");

            if (!File.Exists(DllPath))
                return ValidationResult.Error($"Assembly introuvable : {DllPath}");

            if (string.IsNullOrWhiteSpace(Project))
                return ValidationResult.Error("L'option --project est obligatoire.");

            return ValidationResult.Success();
        }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        // 1. Pointe la config vers Config/ de la racine projet.
        ConfigHelper.UseProjectConfig(Path.GetFullPath(settings.ProjectRoot));

        // 2. Charge l'assembly à déployer (net8.0 → chargement natif).
        var assembly = Assembly.LoadFrom(Path.GetFullPath(settings.DllPath!));

        // 3. Déploie.
        return RegistrationHelper.RegisterPluginsAndWorkflows(
            assembly, settings.Project!, settings.OnPremise, settings.NoPrompt);
    }
}
