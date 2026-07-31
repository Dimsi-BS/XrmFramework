// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Spectre.Console.Cli;
using XrmFramework.Cli.Commands;

// Entry point of the XrmFramework CLI.
//   xrmframework tables sync   --dll <path.dll> --tables-dir <directory> [--clean]   (2.* -> 3.1+ migration)
//   xrmframework tables list   [--prefix <prefix>] [--filter <text>] [--custom-only]
//   xrmframework tables pull   [--table <name>] [--prefix <prefix>] [--tables-dir <directory>] [--noprompt]
//   xrmframework deploy plugins --dll <path.dll> --project <name> [--on-premise] [--noprompt]
var app = new CommandApp();

app.Configure(config =>
{
    config.SetApplicationName("xrmframework");

    config.AddBranch("tables", tables =>
    {
        tables.SetDescription("Commands related to tables / .table files.");

        tables.AddCommand<TableSyncCommand>("sync")
              .WithDescription("Migrates definitions from XrmFramework 2.* to 3.1+: updates the .table files from a 2.* assembly, then cleans up the *Definition.cs files. Run once.")
              .WithExample("tables", "sync", "--dll", "bin/MyProject.dll", "--tables-dir", "Definitions");

        tables.AddCommand<TableListCommand>("list")
              .WithDescription("Lists the tables of the selected environment, filterable by prefix.")
              .WithExample("tables", "list", "--prefix", "ftp_");

        tables.AddCommand<TablePullCommand>("pull")
              .WithDescription("Generates or updates .table files from the environment's metadata (by default: those already present).")
              .WithExample("tables", "pull")
              .WithExample("tables", "pull", "--table", "account,ftp_contrat");
    });

    config.AddBranch("deploy", deploy =>
    {
        deploy.SetDescription("Deployment of components to the selected environment (xrmFramework.config).");

        deploy.AddCommand<DeployPluginsCommand>("plugins")
              .WithDescription("Deploys an assembly (plugins, custom APIs, workflows) to the selected environment.")
              .WithExample("deploy", "plugins", "--dll", "bin/net8.0/MyProject.Plugins.dll", "--project", "Plugins");
    });
});

return app.Run(args);
