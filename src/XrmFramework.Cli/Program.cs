// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Spectre.Console.Cli;
using XrmFramework.Cli.Commands;

// Point d'entrée du CLI XrmFramework.
//   xrmframework tables sync --dll <chemin.dll> --tables-dir <répertoire> [--clean]
var app = new CommandApp();

app.Configure(config =>
{
    config.SetApplicationName("xrmframework");

    config.AddBranch("tables", tables =>
    {
        tables.SetDescription("Commandes liées aux tables / fichiers .table.");

        tables.AddCommand<TableSyncCommand>("sync")
              .WithDescription("Synchronise les fichiers .table depuis un assembly contenant des classes [[EntityDefinition]].")
              .WithExample("tables", "sync", "--dll", "bin/MonProjet.dll", "--tables-dir", "Definitions");
    });
});

return app.Run(args);
