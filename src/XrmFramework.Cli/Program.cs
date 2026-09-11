// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using Spectre.Console.Cli;
using XrmFramework.Cli.Commands;
using XrmFramework.DeployUtils.CommandOptions;

// Entry point of the XrmFramework CLI.
//   xrmframework tables list            [--prefix <prefix>] [--filter <text>] [--custom-only]
//   xrmframework tables pull            [--table <name>] [--prefix <prefix>] [--tables-dir <directory>] [--noprompt]
//   xrmframework tables columns list    [--table <name>] [--prefix <prefix>] [--filter <text>] [--unselected-only]
//   xrmframework tables columns add     --table <name> --column <name> | --all [--noprompt]
//   xrmframework tables columns set     --table <name> --column <name> [--name <newname>] [--select|--deselect]
//   xrmframework tables edit            [--tables-dir <directory>]   (full-screen interactive editor)
//   xrmframework tables optionsets list [--option <logicalname>] [--filter <text>] [--global-only]
//   xrmframework tables optionsets set  --option <logicalname> [--name <newname>] [--value <n> --value-name <newname>]
//   xrmframework deploy plugins         --dll <path.dll> --project <name> [--on-premise] [--noprompt]
//   xrmframework deploy webresources    --project <name> [--path <directory>] [--noprompt]
//   xrmframework migrate sync-tables    --dll <path.dll> --tables-dir <directory> [--clean]   (2.* -> 3.1+ migration)
//   xrmframework migrate sync-models    --dll <path.dll> --models-dir <directory>   (.model files from hand-written IBindingModel classes)
//   xrmframework new solution           <NAME> [--output <directory>]   (scaffolds a new XrmFramework solution)
//   xrmframework new plugin             <NAME> [--solution-dir <dir>] [--solution-unique-name <name>]
//   xrmframework new console            <NAME> [--solution-dir <dir>]
//   xrmframework new azurefunction      <NAME> [--solution-dir <dir>]

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

        tables.AddBranch("columns", columns =>
        {
            columns.SetDescription("Local edits to .table files: activate or adjust columns without going through the environment or an assembly.");

            columns.AddCommand<TableColumnsListCommand>("list")
                   .WithDescription("Lists the columns already tracked in a .table file, selected or not.")
                   .WithExample("tables", "columns", "list", "--table", "ftp_contrat");

            columns.AddCommand<TableColumnsAddCommand>("add")
                   .WithDescription("Activates columns (Select: true) in one or more .table files.")
                   .WithExample("tables", "columns", "add", "--table", "ftp_contrat", "--column", "ftp_datedebut,ftp_datefin");

            columns.AddCommand<TableColumnsSetCommand>("set")
                   .WithDescription("Renames a column's C# name and/or toggles its selection in a .table file.")
                   .WithExample("tables", "columns", "set", "--table", "ftp_contrat", "--column", "ftp_datedebut", "--name", "DateDebut");
        });

        tables.AddCommand<TableEditCommand>("edit")
              .WithDescription("Full-screen interactive editor: browse tracked tables, toggle columns, rename them. Same edits as 'columns add/set', without needing table/column names on the command line.")
              .WithExample("tables", "edit");

        tables.AddBranch("optionsets", optionsets =>
        {
            optionsets.SetDescription("Local edits to .table files: rename an option set and/or its members without going through the environment or an assembly.");

            optionsets.AddCommand<TableOptionSetsListCommand>("list")
                      .WithDescription("Lists the option sets tracked locally, or the members of one given via --option.")
                      .WithExample("tables", "optionsets", "list")
                      .WithExample("tables", "optionsets", "list", "--option", "ftp_contrat_statut");

            optionsets.AddCommand<TableOptionSetsSetCommand>("set")
                      .WithDescription("Renames an option set's C# name and/or one of its member's name, in every .table file that declares it.")
                      .WithExample("tables", "optionsets", "set", "--option", "ftp_contrat_statut", "--name", "StatutContrat")
                      .WithExample("tables", "optionsets", "set", "--option", "ftp_contrat_statut", "--value", "1", "--value-name", "EnCours");
        });
    });

    config.AddBranch("deploy", deploy =>
    {
        deploy.SetDescription("Deployment of components to the selected environment (xrmFramework.config).");

        deploy.AddCommand<DeployPluginsCommand>("plugins")
              .WithDescription("Deploys an assembly (plugins, custom APIs, workflows) to the selected environment.")
              .WithExample("deploy", "plugins", "--dll", "bin/net8.0/MyProject.Plugins.dll", "--project", "Plugins");

        deploy.AddCommand<DeployWebResourcesCommand>("webresources")
              .WithDescription("Publishes a project's web resources (html, css, js, images...) to the selected environment.")
              .WithExample("deploy", "webresources", "--project", "Webresources")
              .WithExample("deploy", "webresources", "--project", "Webresources", "--path", "Webresources");
    });

    // sync-tables is a one-shot version upgrade; sync-models is the odd one out here — it can be
    // run once to bulk-convert a project's binding models, or repeatedly as more of them migrate.
    config.AddBranch("migrate", migrate =>
    {
        migrate.SetDescription("Migrations of a project's sources from what an assembly's classes declare.");

        migrate.AddCommand<MigrateSyncTablesCommand>("sync-tables")
               .WithDescription("Migrates definitions from XrmFramework 2.* to 3.1+: updates the .table files from a 2.* assembly, then cleans up the *Definition.cs files. Run once.")
               .WithExample("migrate", "sync-tables", "--dll", "bin/MyProject.dll", "--tables-dir", "Definitions");

        migrate.AddCommand<MigrateSyncModelsCommand>("sync-models")
               .WithDescription("Generates .model files from the hand-written IBindingModel classes ([[CrmEntity]] + [[CrmMapping]]/[[CrmLookup]]/[[ExtendBindingModel]]/[[ChildRelationship]]) found in a compiled assembly, for ModelSourceFileGenerator to reproduce them at compile time. Safe to re-run as more classes migrate.")
               .WithExample("migrate", "sync-models", "--dll", "bin/MyProject.Core.dll", "--models-dir", "Model");
    });

    config.AddBranch("new", @new =>
    {
        @new.SetDescription("Scaffolds a new XrmFramework solution or project, in-process — no dotnet-new template, no external script.");

        @new.AddCommand<NewSolutionCommand>("solution")
            .WithDescription("Creates a new XrmFramework solution: Core, Plugins, Utils (DefinitionManager, RemoteDebugger, Deploy.*), Webresources.")
            .WithExample("new", "solution", "Contoso")
            .WithExample("new", "solution", "Contoso", "--output", "C:\\Projects");

        @new.AddCommand<NewPluginCommand>("plugin")
            .WithDescription("Adds a plugin project (and its Deploy.* companion) to an existing solution: dotnet sln add, RemoteDebugger reference, and xrmFramework.config registration.")
            .WithExample("new", "plugin", "Contoso.Warehouse")
            .WithExample("new", "plugin", "Contoso.Warehouse", "--solution-unique-name", "ContosoPlugins");

        @new.AddCommand<NewConsoleCommand>("console")
            .WithDescription("Adds a console app project to an existing solution.")
            .WithExample("new", "console", "Contoso.Scripts");

        @new.AddCommand<NewAzureFunctionCommand>("azurefunction")
            .WithDescription("Adds an isolated-worker Azure Function project to an existing solution.")
            .WithExample("new", "azurefunction", "Contoso.Api");
    });
});

// Backward compatibility: existing deployment scripts and pipelines pass -NoPrompt, but Spectre reads
// a single dash as a group of one-letter switches (-N -o -P ...) and refuses to declare a short option
// longer than one character, so the token is translated before parsing rather than declared.
return app.Run(CommandLineAliases.NormalizeNoPrompt(args));
