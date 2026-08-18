// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using Spectre.Console.Cli;
using XrmFramework.Cli.Commands;

// Entry point of the XrmFramework CLI.
//   xrmframework tables list          [--prefix <prefix>] [--filter <text>] [--custom-only]
//   xrmframework tables pull          [--table <name>] [--prefix <prefix>] [--tables-dir <directory>] [--noprompt]
//   xrmframework deploy plugins       --dll <path.dll> --project <name> [--on-premise] [--noprompt]
//   xrmframework migrate sync-tables  --dll <path.dll> --tables-dir <directory> [--clean]   (2.* -> 3.1+ migration)

// A Windows console still starts on a legacy code page (CP850 / CP1252). Those cover Western
// European letters, so accents survive them, but anything outside their 256 slots does not:
// arrows, box-drawing beyond the few they carry, and every Dataverse display name written in a
// script they do not cover (Greek, Polish, Turkish...) come out as "?" or mojibake. Spectre draws
// its tables and rules with box-drawing characters, so this affects the tool's own chrome too.
//
// Setting the encoding is the fix; stripping characters from the source is not, since the labels
// come from the environment. It throws when no console is attached (output redirected to a file
// or a CI log): there is no code page to set then, and the default UTF-8 stream already applies.
try
{
    Console.OutputEncoding = Encoding.UTF8;
}
catch (IOException)
{
}

var app = new CommandApp();

app.Configure(config =>
{
    config.SetApplicationName("xrmframework");

    config.AddBranch("tables", tables =>
    {
        tables.SetDescription("Commands related to tables / .table files.");

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

    // One-shot upgrades, as opposed to the day-to-day loop the other branches serve: each command
    // here rewrites the project's own sources once, and has no reason to be run again afterwards.
    config.AddBranch("migrate", migrate =>
    {
        migrate.SetDescription("One-shot migrations of a project's sources. Run once, then commit.");

        migrate.AddCommand<MigrateSyncTablesCommand>("sync-tables")
               .WithDescription("Migrates definitions from XrmFramework 2.* to 3.1+: updates the .table files from a 2.* assembly, then cleans up the *Definition.cs files. Run once.")
               .WithExample("migrate", "sync-tables", "--dll", "bin/MyProject.dll", "--tables-dir", "Definitions");
    });
});

return app.Run(args);
