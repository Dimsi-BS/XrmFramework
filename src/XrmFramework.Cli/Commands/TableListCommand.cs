// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// Commande <c>xrmframework tables list</c> : liste les tables de l'environnement sélectionné dans
/// la configuration du projet courant. La logique vit dans <see cref="CrmTableHelper.List" />.
/// </summary>
public sealed class TableListCommand : Command<TableListCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        // Attributs qualifiés complètement : un global using MSTest (transitif via DeployUtils)
        // rend [Description] ambigu avec UnitTesting.DescriptionAttribute.
        [CommandOption("--prefix <PREFIX>")]
        [System.ComponentModel.Description("Ne retenir que les tables dont le nom logique commence par ce préfixe (ex. ftp_).")]
        public string? Prefix { get; init; }

        [CommandOption("--filter <TEXT>")]
        [System.ComponentModel.Description("Ne retenir que les tables dont le nom logique ou le libellé contient ce texte.")]
        public string? Filter { get; init; }

        [CommandOption("--custom-only")]
        [System.ComponentModel.Description("Ne retenir que les tables personnalisées.")]
        public bool CustomOnly { get; init; }

        [CommandOption("--project-root <DIR>")]
        [System.ComponentModel.Description("Racine contenant le dossier Config/ (défaut : recherche en remontant depuis le dossier courant).")]
        public string? ProjectRoot { get; init; }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        => CrmTableHelper.List(settings.ProjectRoot, settings.Prefix, settings.Filter, settings.CustomOnly);
}
