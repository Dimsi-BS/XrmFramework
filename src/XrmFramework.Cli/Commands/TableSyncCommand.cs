// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;
using XrmFramework.DeployUtils;

namespace XrmFramework.Cli.Commands;

/// <summary>
/// Commande <c>xrmframework tables sync</c> : synchronise les fichiers <c>.table</c>
/// d'un répertoire à partir des classes <c>[EntityDefinition]</c> d'un assembly.
/// La logique réelle vit dans <see cref="TableSyncHelper.Sync(string, string, bool)"/>.
/// </summary>
public sealed class TableSyncCommand : Command<TableSyncCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        // Attributs qualifiés complètement : un global using MSTest (transitive via
        // DeployUtils) rend [Description] ambigu avec UnitTesting.DescriptionAttribute.
        [CommandOption("--dll <PATH>")]
        [System.ComponentModel.Description("Chemin vers le DLL à analyser (contient des classes *Definition avec [[EntityDefinition]]).")]
        public string? DllPath { get; init; }

        [CommandOption("--tables-dir <DIRECTORY>")]
        [System.ComponentModel.Description("Répertoire contenant les fichiers .table à mettre à jour ou créer.")]
        public string? TablesDirectory { get; init; }

        [CommandOption("--clean")]
        [System.ComponentModel.Description("Met Select=false sur les colonnes orphelines et supprime les .table entièrement générés sans donnée CRM.")]
        public bool Clean { get; init; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(DllPath))
                return ValidationResult.Error("L'option --dll est obligatoire.");

            if (string.IsNullOrWhiteSpace(TablesDirectory))
                return ValidationResult.Error("L'option --tables-dir est obligatoire.");

            return ValidationResult.Success();
        }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        => TableSyncHelper.Sync(settings.DllPath!, settings.TablesDirectory!, settings.Clean);
}
