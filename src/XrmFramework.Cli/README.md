# XrmFramework.Cli

The XrmFramework CLI, distributed as a **.NET tool**. It brings together the Dynamics 365 /
Dataverse development and deployment utilities behind a single command:
**`xrmframework`**.

The business logic lives in the [`XrmFramework.DeployUtils`](../XrmFramework.DeployUtils) library;
this project is only its command-line front end (based on
[Spectre.Console.Cli](https://spectreconsole.net/cli/)).

---

## Installation

### As a global tool

```bash
dotnet tool install --global XrmFramework.Cli
xrmframework --help
```

### As a local tool (recommended per repository/solution)

```bash
# at the root of the consuming repository
dotnet new tool-manifest          # if .config/dotnet-tools.json doesn't exist yet
dotnet tool install XrmFramework.Cli
dotnet xrmframework --help        # or: dotnet tool run xrmframework -- --help
```

The local tool is pinned in `.config/dotnet-tools.json` (checked into version control),
which ensures that the whole team and CI use the same version.

### From source (development)

```bash
dotnet run --project src/XrmFramework.Cli -- <command> [options]
```

---

## Environment configuration

The **connected** commands (`deploy`, `tables list`, `tables pull`) target the environment
*selected* in the project configuration, via two files (an existing XrmFramework mechanism)
read from the **`Config/`** folder at the project root:

| File | Role |
|---|---|
| `Config/xrmFramework.config` | Declares the projects and the active connection (`selectedConnection`). |
| `Config/connectionStrings.config` | Defines the named connection strings (Dataverse / On-Premises). |

`selectedConnection` points to an entry in `connectionStrings.config`: this is
**the target environment**. The `tables sync` migration, on the other hand, does **not** need
a connection (it works solely from a local assembly and the files on disk).

### Automatic configuration discovery

`tables list` and `tables pull` **walk up the directory tree** from the current folder until
they find a `Config/xrmFramework.config`: the CLI can therefore be launched from any
subdirectory of the solution (including a `bin/Debug`). `--project-root` bypasses this search.

At the root thus found, the CLI reads `Directory.Build.props` to extract
`XrmFrameworkCoreProjectName`, which gives it the default `.table` directory:
`<root>/<CoreProject>/Definitions`. This is the same resolution that MSBuild injects into the
DefinitionManager. Failing that, `--tables-dir` becomes mandatory.

> Discovery only checks for `xrmFramework.config`: `connectionStrings.config` carries secrets
> and is gitignored in generated solutions, so it is absent from a fresh clone. Its absence is
> reported precisely at connection time, rather than disguised as "configuration not found".

> The CLI loads these two files explicitly (without relying on an application `App.config`)
> — see [`ConfigHelper.UseProjectConfig`](../XrmFramework.DeployUtils/Configuration/ConfigHelper.cs)
> and [`ProjectConfigLocator`](../XrmFramework.DeployUtils/TableSync/ProjectConfigLocator.cs).

---

## Commands

### `xrmframework tables sync` ✅ *(available)* — migration from 2.\* to 3.1+

**This is a migration tool, meant to be run once**, when upgrading a project from XrmFramework
2.\* to 3.1 or above. It is not a routine command: afterwards, `tables pull` and the source
generator take over.

#### What changed between 2.\* and 3.1

Under 2.\*, the DefinitionManager wrote **two files per table** into the Core project's
`Definitions` folder: the `.table` and its `*Definition.cs`. Both were checked in, and the `.cs`
was a real compiled source file.

From 3.1 on, **the `.table` is the single source of truth**: the
[`TableSourceFileGenerator`](../XrmFramework.Analyzers/Generators/TableSourceFileGenerator.cs)
Roslyn generator emits the `*Definition` class at compile time from the `.table` alone. The
checked-in `.cs` is no longer a source — it is a duplicate of generated code, and the project
does not build until it is dealt with.

`tables sync` performs that hand-over in one pass:

1. it reflects over the **assembly last compiled under 2.\***, whose `*Definition` classes record
   which columns the project's code actually uses, and under which name each option set is
   compiled;
2. it brings the `.table` files in line — creating what is missing, setting `Select: true` on
   every column the code references, and naming the option sets;
3. it cleans up the `*Definition.cs` files sitting next to them.

What travels in step 2 is precisely what the CRM cannot tell you: **C# identifiers the compiled
code depends on.** Everything else is metadata `tables pull` can fetch back at any time.

```bash
xrmframework tables sync --dll <path.dll> --tables-dir <directory> [--clean]
```

| Option | Required | Description |
|---|:---:|---|
| `--dll <PATH>` | ✅ | Assembly compiled under 2.\* (contains `*Definition` classes decorated with `[EntityDefinition]` that expose a static `EntityName` field). |
| `--tables-dir <DIRECTORY>` | ✅ | Directory holding the `.table` and `*Definition.cs` files — usually `<CoreProject>/Definitions`. |
| `--clean` | ❌ | Sets `Select=false` on orphaned columns and deletes `.table` files entirely generated by the tool with no CRM data. |

**Example**

```bash
xrmframework tables sync --dll bin/Release/net8.0/MyProject.Plugins.dll \
                         --tables-dir ../MyProject.Core/Definitions \
                         --clean
```

> ⚠️ The command **deletes and renames source files** in `--tables-dir` (see below). Run it on a
> clean working tree so the whole migration shows up as a single reviewable diff.

#### Naming the option sets

A `.table` records an option set's logical name, which comes from the CRM; the `Name` under which
it is compiled is a project decision — teams rename `workflow_runas` into `UtilisateurExecutant`
and their code depends on it. Under 2.\* that name lived in the generated `.cs`; from 3.1 on the
generator reads it from the `.table`.

The migration recovers it from `[OptionSet(typeof(SomeEnum))]` carried by the column constants,
and applies it to the option set the column points at (matched on the column's `EnumName`) —
**in every file that records it**:

- in the table's own `Enums`;
- and in `OptionSet.table`, where shared option sets live — that file is loaded once and rewritten
  only if a name actually changed.

Both, not the first one found. The 2.\* DefinitionManager kept in a table's `Enums` every option
set one of its columns referenced, *globals included*, while also writing the globals to
`OptionSet.table`. The generator unions the two, so a rename applied to only one copy would be
contradicted by the other.

The existing values, logical name and `IsGlobal` flag are untouched: only `Name` moves. Two cases
are left alone — an option set marked `"Locked": true` (shipped by the framework, its name belongs
to the package's generated code), and a column whose `.table` entry carries no `EnumName` yet,
which happens for a column the migration itself just created. Both are reported.

#### Cleaning up the `*Definition.cs` files

Once the `.table` files are up to date, every `*Definition.cs` in the directory is stripped of
what the generator now emits:

- the `EntityName` and `EntityCollectionName` constants;
- the nested `Columns`, `AlternateKeyNames`, `ManyToManyRelationships`, `ManyToOneRelationships`
  and `OneToManyRelationships` classes, together with their attributes;
- the namespace-level option set `enum`s — **only those the generator will actually re-emit**,
  i.e. declared in a `.table` *and* referenced by a selected column. An option set no column uses
  is not regenerated, so its `enum` is kept.

Then, depending on what is left:

| What remains in the Definition class | Outcome |
|---|---|
| Nothing (and nothing else in the file) | the file is **deleted** — the generated part covers it entirely |
| Members added by hand (constants, nested classes, properties, methods) | the file becomes **`*Definition.partial.cs`**, holding only those members |

A file whose Definition class ends up empty but which still carries hand-written `enum`s keeps
its `.partial.cs`, minus the now-pointless class declaration.

#### Realigning the surviving partial

A file that survives is also realigned on what the generator emits, otherwise it would collide
with it instead of merging:

- the `partial` modifier is added if missing (without it, C# sees two distinct types);
- the namespace becomes **`XrmFramework`** — the only namespace the generator emits into;
- `[GeneratedCode]`, `[EntityDefinition]` and `[ExcludeFromCodeCoverage]` are dropped from the
  class, since none of them allows multiple use and the generated part already carries them
  (CS0579). Any other attribute, such as `[DefinitionManagerIgnore]`, is kept.

> The namespace change may leave a hand-written member referring to a type that used to be found
> in the project's own namespace. Those are reported by the compiler as unresolved names — add
> the missing `using` and move on.

#### What the migration refuses to touch

Deleting source files calls for a conservative tool. A `*Definition.cs` is **left exactly as it
is**, and reported, when:

- **no `.table` in the directory declares the matching table.** The generator would produce no
  replacement, so removing the file would drop the definition altogether. Matching is done on the
  `Name` declared *inside* the `.table`, not on its file name (`Systemuser.table` declares
  `SystemUser`).
- **the file cannot be read reliably** — unbalanced braces, a construct the scanner does not
  bracket confidently, or no class matching the file name. That last match ignores casing
  (`contactdefinition.cs` finds `ContactDefinition`), an exact match winning whenever one exists;
  the declaration itself is never re-cased, C# being case-sensitive.
- **a `*Definition.partial.cs` already exists** next to it. Rather than clobbering it, the tool
  steps back and asks for a manual merge.

An already-migrated `*Definition.partial.cs` is never taken as input, so re-running the command
is harmless. The exit code stays `0` when files are skipped, but the summary line says how many —
review them by hand.

> **Not covered:** `OptionSetDefinitions.cs`, the separate file the 2.\* DefinitionManager wrote
> for global option sets. Its name does not end in `Definition.cs`, so the migration leaves it
> alone; delete it by hand once the generator emits its enums.

Implementation: [`DefinitionFileMigrator`](../XrmFramework.DeployUtils/TableSync/DefinitionFileMigrator.cs)
+ [`DefinitionSourceRewriter`](../XrmFramework.DeployUtils/TableSync/DefinitionSourceRewriter.cs)
+ [`CSharpMemberReader`](../XrmFramework.DeployUtils/TableSync/CSharpMemberReader.cs).

#### Tables shipped by the framework

The `.table` files from the XrmFramework package (`SystemUser`, `Role`, `Team`, `SdkMessage`, …)
are compiled into the consuming project: their `*Definition` classes therefore appear in the
analyzed DLL, just like those of the project itself. The command **does not create them** in
the target directory — that would duplicate a file already provided by the package — and
simply reports how many were skipped.

However, if the project **already tracks its own copy** of one of these tables (a file present
in the target directory, typically to declare additional columns alongside the framework's
own, marked `"Locked": true`), it is synchronized like any other: missing columns are added,
columns referenced by the code are activated, and orphaned columns are deselected under
`--clean`. The `Locked` marker is never modified.

The inventory lives in
[`FrameworkTableCatalog`](../XrmFramework.DeployUtils/TableSync/FrameworkTableCatalog.cs); a
test verifies that it matches exactly the `.table` files in `src/XrmFramework/Definitions`.

`OptionSet.table` is a case of its own: it describes no entity, so no `*Definition` class ever
claims it, and it holds no column. Under `--clean` both orphan heuristics used to condemn it — it
is now recognized by its `globalEnums` logical name and left alone. A genuine table that happens
to be *named* `OptionSet` is still processed like any other.

**Exit codes**

| Code | Meaning |
|:---:|---|
| `0` | Success — including "no definition found", and including files left untouched (they are reported in the summary). |
| `2` | DLL or directory not found. |
| `3` | Unexpected error (the stack trace is displayed). |
| `1` / `-1` | Argument parsing / validation error (Spectre). |

Implementation: [`TableSyncHelper.Sync`](../XrmFramework.DeployUtils/TableSyncHelper.cs)
-> [`DefinitionAnalyzer`](../XrmFramework.DeployUtils/TableSync/DefinitionAnalyzer.cs)
+ [`TableFileSyncer`](../XrmFramework.DeployUtils/TableSync/TableFileSyncer.cs)
+ [`DefinitionFileMigrator`](../XrmFramework.DeployUtils/TableSync/DefinitionFileMigrator.cs).

### `xrmframework tables list` ✅ *(available)*

Lists the tables of the selected environment. The `.table` column indicates which ones are
already tracked in the project — the information most needed when deciding what to fetch.

```bash
xrmframework tables list [--prefix <prefix>] [--filter <text>] [--custom-only] [--project-root <dir>]
```

| Option | Required | Description |
|---|:---:|---|
| `--prefix <PREFIX>` | ❌ | Only keeps tables whose **logical name** starts with this prefix (e.g. `ftp_`). |
| `--filter <TEXT>` | ❌ | Only keeps tables whose logical name **or display name** contains this text. |
| `--custom-only` | ❌ | Only keeps custom tables. |
| `--project-root <DIR>` | ❌ | Root containing `Config/` (default: search upward from the current folder). |

**Example**

```bash
xrmframework tables list --prefix ftp_
```

Metadata is retrieved without attributes (`EntityFilters.Entity`), which makes the command
noticeably faster than a full retrieval.

### `xrmframework tables pull` ✅ *(available)*

Generates or updates `.table` files from the environment's metadata: types, localized labels,
capabilities, bounds, relationships, alternate keys, and option sets. This is the headless
equivalent of the **DefinitionManager** (WinForms `net462`), usable in CI.

```bash
xrmframework tables pull [--table <names>] [--prefix <prefix>] [--tables-dir <dir>] [--project-root <dir>] [-n]
```

| Option | Required | Description |
|---|:---:|---|
| `-t`, `--table <NAME>` | ❌ | Logical name of a table. **Repeatable** option that also accepts a comma-separated list. |
| `--prefix <PREFIX>` | ❌ | Additionally fetches all tables whose logical name starts with this prefix. |
| `--tables-dir <DIRECTORY>` | ❌ | Target directory (default: the Core project's `Definitions` folder, inferred from the configuration). |
| `--project-root <DIR>` | ❌ | Root containing `Config/` (default: search upward from the current folder). |
| `-n`, `--noprompt` | ❌ | Silent mode: skips the confirmation (CI/CD). |

#### Default selection: already-tracked tables

Without `--table` or `--prefix`, `pull` refreshes **all the tables already described by a
`.table` file** in the target directory — a bulk update after a model change, without having
to re-enumerate the project's tables.

- The selection is read from the files, by their `LogName`: a renamed `.table` file remains
  tracked.
- `OptionSet.table` (global option sets) is excluded — it doesn't correspond to any CRM
  entity, but is still populated by the tables fetched.
- A `.table` file whose entity no longer exists in the environment is **reported and skipped**,
  without interrupting the others; the file is not deleted.
- If the directory contains no `.table` files, the command stops with code `1` **before
  connecting**: there's nothing to fetch, so there's no point authenticating for nothing.

**Examples**

```bash
xrmframework tables pull --noprompt
```

```bash
xrmframework tables pull --table account,ftp_contrat --noprompt
```

#### Column selection

When a `.table` is **created**, only the directly usable columns are activated
(`Select: true`):

- the primary key, the name column, and the image column (`PrimaryType`);
- columns participating in an alternate key;
- `createdon`, `modifiedon`, `statecode`, `statuscode`.

All other columns are indeed **written with their full metadata**, but remain inactive — this
avoids generating thousands of useless constants. Activating one is a deliberate act: set
`Select: true` in the `.table` (via the DefinitionManager, by hand, or with the upcoming
`tables columns`). On a project coming from 2.\*, the initial activation is done in bulk by the
`tables sync` migration, which reads it from the existing code.

#### Merge rules for an existing file

> **What becomes a C# identifier belongs to the file; what describes the table belongs to the
> CRM.**

| Element | Source of truth |
|---|---|
| `Name` (table, column, key, option set and its members) | **the file** — manually renamed, compiled code depends on it |
| `Select` | **the file** — never downgraded |
| `Locked` | **the file** — local marker, absent from the CRM |
| `Type`, `PrimaryType`, `Capa`, `Labels`, `StrLen`, `MinRange`, `MaxRange`, `DatBehav`, `IsMultiSelect`, `EnumName`, relationships | **the CRM** |

Other guarantees:

- **A column already selected stays selected.** `pull` never downgrades a `Select: true`, and
  never re-activates a column that was deliberately deactivated — including `createdon` and
  the like, even though they are activated by default on creation. This guarantee is verified
  end-to-end (metadata -> merge -> write -> re-read) by `TablePullPersistenceTests`.
- The target file is located by its **`LogName`**, not by its file name: a table whose `Name`
  was manually renamed (`Contract.table` -> `ContractLocation.table`) is correctly updated
  instead of being duplicated — the selection also survives this renaming.
- A column present in the file but **absent from the environment** is kept and reported.
  `pull` refreshes, it does not destroy; deselecting orphaned columns is a separate decision
  (`tables sync --clean` does it during a 2.\* migration).
- **Global** option sets are merged in a purely additive way in `OptionSet.table`: fetching a
  single table never removes the ones referenced by others.
- The operation is **idempotent**: a second `pull` on the same table produces an empty diff.

**Exit codes** (common to `list` and `pull`)

| Code | Meaning |
|:---:|---|
| `0` | Success (including cancellation at the confirmation prompt). |
| `1` | No table matches the criteria. |
| `2` | Configuration or directory not found. |
| `3` | Unexpected error, or at least one table failed. |
| `-1` | Argument validation error (Spectre). |

Implementation: [`CrmTableHelper`](../XrmFramework.DeployUtils/TableSync/CrmTableHelper.cs)
-> [`ProjectConfigLocator`](../XrmFramework.DeployUtils/TableSync/ProjectConfigLocator.cs)
+ [`MetadataTableFactory`](../XrmFramework.DeployUtils/TableSync/MetadataTableFactory.cs)
+ [`TableMerger`](../XrmFramework.DeployUtils/TableSync/TableMerger.cs)
+ [`TableFileStore`](../XrmFramework.DeployUtils/TableSync/TableFileStore.cs).

### `xrmframework deploy plugins` ✅ *(available)*

Deploys an XrmFramework assembly — **plugins, custom APIs, and workflows** — to the
environment selected in `Config/xrmFramework.config`.

```bash
xrmframework deploy plugins --dll <path.dll> --project <name> [--project-root <dir>] [--on-premise] [--noprompt]
```

| Option | Required | Description |
|---|:---:|---|
| `--dll <PATH>` | ✅ | Plugin project assembly (`net462`, the one registered in Dataverse). |
| `--project <NAME>` | ✅ | Project name as declared in `xrmFramework.config` (e.g. `Plugins`). |
| `--project-root <DIR>` | ❌ | Root containing the `Config/` folder (default: current folder). |
| `--on-premise` | ❌ | Targets an On-Premises CRM (default: Dataverse Online). |
| `-n`, `--noprompt` | ❌ | Silent mode: skips the connection confirmation (CI/CD). |

> **How it works — inventory via actual code execution.** A plugin is `net462`, this tool is
> `net10.0`: it therefore cannot instantiate the plugin's types itself. It delegates to the
> **`XrmFramework.PluginInventory`** tool (a `net462` executable, embedded under `inventory/`),
> which loads the assembly, **executes the constructors (`AddSteps`)**, and reflects over the
> types, then returns the JSON manifest (plugins / steps / workflows / custom APIs) on its
> standard output.
>
> Consequences:
> - Step registration is **entirely free-form**: loops, conditions, computed values,
>   configuration… since the real code runs (no static analysis constraints).
> - Deployment requires the **.NET Framework runtime** (Windows). For cross-platform
>   development, a launcher can be provided via the `XRMFRAMEWORK_INVENTORY_LAUNCHER`
>   environment variable (e.g. `mono`); `XRMFRAMEWORK_INVENTORY_EXE` allows pointing to an
>   alternative inventory executable.

**Example**

```bash
xrmframework deploy plugins --dll bin/Release/net462/MyProject.Plugins.dll \
                            --project MyProject.Plugins \
                            --noprompt
```

**Exit codes**

| Code | Meaning |
|:---:|---|
| `0` | Success (or cancellation at the confirmation prompt). |
| `1` | Project missing from `xrmFramework.config`. |
| `3` | Unexpected error (inventory, connection, deployment…). |
| `255` | Argument validation error (Spectre). |

Implementation: [`RegistrationHelper.RegisterPluginsAndWorkflows`](../XrmFramework.DeployUtils/RegistrationHelper.cs)
-> inventory [`XrmFramework.PluginInventory`](../XrmFramework.PluginInventory/PluginInventoryEngine.cs)
-> [`PluginInventoryReader`](../XrmFramework.DeployUtils/Factories/PluginInventoryReader.cs)
+ [`ConfigHelper.UseProjectConfig`](../XrmFramework.DeployUtils/Configuration/ConfigHelper.cs).

---

## Roadmap

Target command tree (✅ exist, 🚧 are upcoming):

```
xrmframework
├── tables
│   ├── sync           ✅  migration 2.* -> 3.1+, run once         (offline)
│   ├── list           ✅  lists the tables of the environment    (connected)
│   ├── pull           ✅  .table ← Dataverse metadata            (connected)
│   └── columns        🚧  adds / modifies columns of one or more tables
└── deploy
    ├── plugins        ✅  deploys a plugins / custom API / workflow assembly
    └── webresources   🚧  deploys the webresources
```

`sync` stands apart: it is the one-shot upgrade path from 2.\*, not part of the day-to-day loop.
Routine work is `pull` (rich metadata from the environment) plus column selection in the
`.table` — which `tables columns` will make scriptable.

> `deploy plugins` inventories the `net462` plugin assembly by **executing its registration
> code** via the `XrmFramework.PluginInventory` tool (embedded `net462` executable) — step
> registration therefore remains completely free-form (loops, conditions…). Requires the .NET
> Framework runtime (Windows).

### 🚧 `tables columns` — add / modify columns

Manual editing of `.table` files to activate or adjust columns without going through either an
assembly (`tables sync`) or the environment (`tables pull`). Anticipated verbs (to be
finalized): `tables columns add` / `tables columns set`. Will reuse
[`TableFileStore`](../XrmFramework.DeployUtils/TableSync/TableFileStore.cs) for reading and
writing.


### 🚧 `deploy webresources` — deploy the webresources

Deploys the webresources from a project folder to the `SelectedConnection` environment. Will
rely on [`WebResourceHelper.SyncWebResources`](../XrmFramework.DeployUtils/WebResourceHelper.cs)
(existing options: `-p/--path`, `-n/--noprompt`).

---

## Architecture & adding a command

The CLI follows the Spectre.Console.Cli model:

- **`Program.cs`** configures the `CommandApp` and the command tree (branches `tables`,
  `deploy`, …).
- **`Commands/`** contains one class per command: `Command<TSettings>` with a `Settings` class
  (`[CommandOption]` options + `Validate()`), and an `Execute(...)` that **delegates to a
  helper in `XrmFramework.DeployUtils`** (the CLI contains no business logic).

To add a command:

1. Create `Commands/MyCommand.cs` (`Command<Settings>`), `Execute` calls the helper.
2. Register it in `Program.cs` (`AddCommand` / `AddBranch`).
3. If the logic doesn't already exist in `DeployUtils`, add it there as a **parameterized API
   returning an `int`** (exit code), following the pattern of `TableSyncHelper.Sync(...)` — no
   `Environment.Exit` in the helpers.

> ⚠️ Spectre.Console.Cli 0.55: `Command<T>.Execute` is
> `protected override int Execute(CommandContext, T, CancellationToken)`.
> In help/description text, escape literal brackets (`[` -> `[[`,
> `]` -> `]]`), otherwise they are interpreted as style markup.

---

## Development

```bash
# build
dotnet build src/XrmFramework.Cli -c Release

# run without packaging
dotnet run --project src/XrmFramework.Cli -- tables sync --dll <dll> --tables-dir <dir>

# package the tool locally and inspect it
dotnet pack src/XrmFramework.Cli -c Release -o ./nupkg
unzip -p ./nupkg/XrmFramework.Cli.*.nupkg "*.nuspec"     # <packageType name="DotnetTool" />
```

> The package embeds the entire dependency *closure* under `tools/net10.0/any/`
> (including `XrmFramework.DeployUtils` and the Dataverse client): this is large but
> necessary for a self-contained tool.

The version comes from **Nerdbank.GitVersioning** (no version number to maintain by hand).
