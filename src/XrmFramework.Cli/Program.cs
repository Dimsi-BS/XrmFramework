// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Spectre.Console.Cli;
using XrmFramework.Cli.Commands;

// Point d'entrée du CLI XrmFramework.
//   xrmframework tables sync   --dll <chemin.dll> --tables-dir <répertoire> [--clean]
//   xrmframework tables list   [--prefix <préfixe>] [--filter <texte>] [--custom-only]
//   xrmframework tables pull   --table <nom> [--prefix <préfixe>] [--tables-dir <répertoire>] [--noprompt]
//   xrmframework deploy plugins --dll <chemin.dll> --project <nom> [--on-premise] [--noprompt]
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

        tables.AddCommand<TableListCommand>("list")
              .WithDescription("Liste les tables de l'environnement sélectionné, filtrables par préfixe.")
              .WithExample("tables", "list", "--prefix", "ftp_");

        tables.AddCommand<TablePullCommand>("pull")
              .WithDescription("Génère ou met à jour des fichiers .table depuis les métadonnées de l'environnement.")
              .WithExample("tables", "pull", "--table", "account,ftp_contrat");
    });

    config.AddBranch("deploy", deploy =>
    {
        deploy.SetDescription("Déploiement de composants vers l'environnement sélectionné (xrmFramework.config).");

        deploy.AddCommand<DeployPluginsCommand>("plugins")
              .WithDescription("Déploie une assembly (plugins, custom APIs, workflows) vers l'environnement sélectionné.")
              .WithExample("deploy", "plugins", "--dll", "bin/net8.0/MonProjet.Plugins.dll", "--project", "Plugins");
    });
});

return app.Run(args);
