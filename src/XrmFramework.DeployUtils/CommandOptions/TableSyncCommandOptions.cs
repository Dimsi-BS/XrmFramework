// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CommandLine;

namespace XrmFramework.DeployUtils.CommandOptions;

public class TableSyncCommandOptions
{
    [Option("dll", Required = true,
        HelpText = "Chemin vers le DLL à analyser (doit contenir des classes *Definition avec [EntityDefinition]).")]
    public string DllPath { get; set; } = string.Empty;

    [Option("tables-dir", Required = true,
        HelpText = "Répertoire contenant les fichiers .table à mettre à jour ou créer.")]
    public string TablesDirectory { get; set; } = string.Empty;

    [Option("clean", Required = false, Default = false,
        HelpText = "Met Select=false sur les colonnes absentes de toute Definition, et supprime les .table entièrement générés par l'outil sans donnée CRM.")]
    public bool Clean { get; set; }
}
