// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// Commande <c>xrmframework tables pull</c> : génère ou met à jour les fichiers <c>.table</c> à
/// partir des métadonnées de l'environnement sélectionné.
/// La logique vit dans <see cref="CrmTableHelper.Pull" />.
/// </summary>
public sealed class TablePullCommand : Command<TablePullCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-t|--table <NAME>")]
        [System.ComponentModel.Description("Nom logique d'une table à récupérer. Option répétable, et accepte une liste séparée par des virgules.")]
        public string[]? Tables { get; init; }

        [CommandOption("--prefix <PREFIX>")]
        [System.ComponentModel.Description("Récupère en outre toutes les tables dont le nom logique commence par ce préfixe.")]
        public string? Prefix { get; init; }

        [CommandOption("--tables-dir <DIRECTORY>")]
        [System.ComponentModel.Description("Répertoire des fichiers .table (défaut : le dossier Definitions du projet Core).")]
        public string? TablesDirectory { get; init; }

        [CommandOption("--project-root <DIR>")]
        [System.ComponentModel.Description("Racine contenant le dossier Config/ (défaut : recherche en remontant depuis le dossier courant).")]
        public string? ProjectRoot { get; init; }

        [CommandOption("-n|--noprompt")]
        [System.ComponentModel.Description("Mode silencieux : ignore la confirmation (CI/CD).")]
        public bool NoPrompt { get; init; }

        public override ValidationResult Validate()
        {
            var hasTables = Tables is { Length: > 0 };

            if (!hasTables && string.IsNullOrWhiteSpace(Prefix))
                return ValidationResult.Error(
                    "Indiquez au moins une table via --table, ou un préfixe via --prefix.");

            return ValidationResult.Success();
        }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        => CrmTableHelper.Pull(
            settings.ProjectRoot,
            settings.TablesDirectory,
            settings.Tables,
            settings.Prefix,
            settings.NoPrompt);
}
