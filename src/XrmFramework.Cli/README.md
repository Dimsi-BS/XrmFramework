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
**the target environment**. `migrate sync-tables`, `tables columns` and `tables optionsets`, on
the other hand, do **not** need a connection — they work solely from local files (and, for
`migrate sync-tables`, an assembly).

### Automatic configuration discovery

`tables list`, `tables pull`, `tables columns` and `tables optionsets` **walk up the directory
tree** from the current folder until they find a `Config/xrmFramework.config`: the CLI can
therefore be launched from any subdirectory of the solution (including a `bin/Debug`).
`--project-root` bypasses this search. `tables columns` and `tables optionsets` never connect to
the environment, but still need this discovery to locate the `.table` files.

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
| `-n`, `--noprompt`, `-NoPrompt` | ❌ | Silent mode: skips the confirmation (CI/CD). |

#### Default selection: already-tracked tables

Without `--table` or `--prefix`, `pull` refreshes **all the tables already described by a
`.table` file** in the target directory — a bulk update after a model change, without having
to re-enumerate the project's tables.

- The selection is read from the files, by their `LogName`: a renamed `.table` file remains
  tracked.
- `OptionSets.table` (global option sets) is excluded — it doesn't correspond to any CRM
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
`migrate sync-tables` migration, which reads it from the existing code.

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
  (`migrate sync-tables --clean` does it during a 2.\* migration).
- **Global** option sets are merged in a purely additive way in `OptionSets.table`: fetching a
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

### `xrmframework tables columns` ✅ *(available)* — local edits to `.table` files

Activates or adjusts columns already present in a `.table` file **without going through either
an assembly (`migrate sync-tables`) or the environment (`tables pull`)** — entirely offline,
reading and writing only via [`TableFileStore`](../XrmFramework.DeployUtils/TableSync/TableFileStore.cs).
Three verbs:

```
xrmframework tables columns list  [--table <names>] [--prefix <prefix>] [--filter <text>] [--unselected-only]
xrmframework tables columns add   --table <names> | --prefix <prefix>  --column <names> | --all  [-n]
xrmframework tables columns set   --table <name> --column <name> [--name <newname>] [--select | --deselect]
```

All three also accept `--tables-dir <DIRECTORY>` and `--project-root <DIR>`, same meaning as on
`tables pull`.

#### `list` — see what's already tracked

Prints, per table, every column the `.table` file already knows about (`tables pull` writes
the full metadata for **all** columns, selected or not — see
[Column selection](#column-selection)). This is how you find the logical names to pass to `add`.

| Option | Required | Description |
|---|:---:|---|
| `-t`, `--table <NAME>` | ❌ | Table to inspect. Repeatable, comma-separated. Default: every table already tracked (having a `.table` file). |
| `--prefix <PREFIX>` | ❌ | Also inspects every tracked table whose logical name starts with this prefix. |
| `--filter <TEXT>` | ❌ | Only keeps columns whose logical name or C# name contains this text. |
| `--unselected-only` | ❌ | Only keeps columns not yet activated — the candidates for `add`. |

```bash
xrmframework tables columns list --table ftp_contrat --unselected-only
```

#### `add` — activate columns

Sets `Select: true` on the requested columns. Unlike `list`, it **mutates files**, so it never
defaults to the whole project: `--table` or `--prefix` is required, and so is `--column` or
`--all`.

| Option | Required | Description |
|---|:---:|---|
| `-t`, `--table <NAME>` | ⚠️ | Table(s) to edit. Repeatable, comma-separated. Required unless `--prefix` is given. |
| `--prefix <PREFIX>` | ⚠️ | Also edits every tracked table whose logical name starts with this prefix. |
| `-c`, `--column <NAME>` | ⚠️ | Column(s) to activate. Repeatable, comma-separated. Required unless `--all` is given. |
| `--all` | ⚠️ | Activates every column not yet selected, instead of an explicit `--column` list. |
| `-n`, `--noprompt` | ❌ | Silent mode: skips the confirmation (CI/CD). |

A column already selected is left untouched (no-op, not an error); a requested column absent
from the file is reported and the others still proceed.

```bash
xrmframework tables columns add --table ftp_contrat --column ftp_datedebut,ftp_datefin --noprompt
```

```bash
# Activate the same audit column across every ftp_-prefixed table already tracked.
xrmframework tables columns add --prefix ftp_ --column createdby
```

#### `set` — rename or toggle a single column

Renames a column's C# `Name` and/or flips its `Select` flag. `--table` accepts either the
logical name or the file's C# `Name`, for convenience.

| Option | Required | Description |
|---|:---:|---|
| `-t`, `--table <NAME>` | ✅ | Table to edit (logical name or C# name). |
| `-c`, `--column <NAME>` | ✅ | Logical name of the column to edit. |
| `--name <NEWNAME>` | ❌ | Renames the column's C# name. Rejected if another column in the same table already has that name. |
| `--select` | ❌ | Activates the column. Mutually exclusive with `--deselect`. |
| `--deselect` | ❌ | Deactivates the column. Mutually exclusive with `--select`. |

At least one of `--name`, `--select` or `--deselect` is required.

```bash
xrmframework tables columns set --table ftp_contrat --column ftp_datefin --name DateFinContrat
```

**Exit codes** (all three verbs)

| Code | Meaning |
|:---:|---|
| `0` | Success (including "nothing to do": already in the requested state). |
| `1` | No table or column matches the criteria. |
| `2` | Configuration or `.table` directory not found. |
| `3` | Unexpected error, or `set --name` collides with another column's C# name. |
| `-1` / `255` | Argument validation error (Spectre). |

Implementation: [`ColumnHelper`](../XrmFramework.DeployUtils/TableSync/ColumnHelper.cs)
-> [`TableFileStore`](../XrmFramework.DeployUtils/TableSync/TableFileStore.cs)
+ [`ProjectConfigLocator`](../XrmFramework.DeployUtils/TableSync/ProjectConfigLocator.cs).

### `xrmframework tables edit` ✅ *(available)* — full-screen interactive editor

The interactive counterpart of `tables columns add`/`set`/`pull`: a full-screen, keyboard-driven
console UI ([Terminal.Gui](https://github.com/gui-cs/Terminal.Gui)) over the locally tracked
`.table` files, for when you'd rather browse than remember exact table/column logical names.
Editing is entirely offline, same [`TableFileStore`](../XrmFramework.DeployUtils/TableSync/TableFileStore.cs)
as every other `tables` command underneath; pulling (`P`) is the one thing that talks to the
environment, and does so exactly like `tables pull` on the command line.

```
xrmframework tables edit [--tables-dir <DIRECTORY>] [--project-root <DIR>]
```

Tables tracked locally on the left, the columns of whichever one is selected on the right — the
left pane starts empty on a brand-new project, `P` is all it takes to populate it:

| Key | Action |
|---|---|
| `↑` / `↓`, `Tab` | Navigate / switch pane |
| `Space`, `Enter` | Toggle the selected column's `Select` flag |
| `R` | Rename the selected column's C# name |
| `O` | Edit the option set the selected column is tied to (Picklist, State, Status...) |
| `P` | Pull from the environment — update tracked tables, or import new ones |
| `Esc`, `Q` | Quit |

`P` asks which of the two: **update tracked** re-pulls every table already tracked (`tables pull`
with no criteria), **import new** shows every table in the environment (flagging what's already
tracked, like `tables list`) and prompts for the logical name(s) to add. Either way, the screen
exits for the duration of the pull — it is a network call with its own confirmation prompt and
progress output, already built on the normal scrolling console in
[`CrmTableHelper`](../XrmFramework.DeployUtils/TableSync/CrmTableHelper.cs) — then reopens
afterward over whatever landed on disk.

Every toggle or rename is validated (same duplicate-name rule as `columns set --name`) and saved
to disk immediately — there is no separate save step.

`O` on a column with no option set, or one never pulled locally (no matching entry under any
tracked `.table`'s `Enums`), reports why instead of opening anything. Otherwise it opens a second
screen — the interactive counterpart of `tables optionsets set` — over that option set's members:

| Key | Action |
|---|---|
| `↑` / `↓` | Navigate members |
| `Enter`, `R` | Rename the selected member's C# name |
| `N` | Rename the option set's own C# name |
| `Esc`, `Q` | Close, back to the columns screen |

A global option set can be declared in several `.table` files at once; a rename here reaches
every copy the same way `tables optionsets set` does, skipping (and reporting) any copy marked
`Locked` — its name belongs to the framework package's own generated code.

Requires a real terminal (not redirected output); on an unsupported terminal, prefer the
non-interactive `tables columns`/`tables optionsets` commands.

Implementation: [`TableEditorApp` / `TableEditorWindow` / `OptionSetEditorWindow`](Tui)
-> [`TableFileStore`](../XrmFramework.DeployUtils/TableSync/TableFileStore.cs)
+ [`CrmTableHelper`](../XrmFramework.DeployUtils/TableSync/CrmTableHelper.cs) (`P`)
+ [`ProjectConfigLocator`](../XrmFramework.DeployUtils/TableSync/ProjectConfigLocator.cs).

### `xrmframework tables optionsets` ✅ *(available)* — rename option sets and their members

The companion of `tables columns` for option sets: renames an option set's C# name and/or one
of its member's name in a `.table` file — entirely offline, same as `tables columns`. Two verbs:

```
xrmframework tables optionsets list [--option <logicalname>] [--filter <text>] [--global-only]
xrmframework tables optionsets set  --option <logicalname> [--name <newname>] [--value <n> --value-name <newname>]
```

Both also accept `--tables-dir <DIRECTORY>` and `--project-root <DIR>`, same meaning as on
`tables pull`.

#### Why a rename must reach every copy

An option set's logical name is unique, but its **declaration** is not: the historical
DefinitionManager kept in a table's own `Enums` every option set one of its columns
referenced — global ones included — while also writing the globals to `OptionSets.table` (see
[Merge rules for an existing file](#merge-rules-for-an-existing-file)). A global option set
shared by several tables is therefore typically declared **several times over**. `set` looks it
up by logical name across every local `.table` file (`OptionSets.table` included) and renames
**every** copy it finds in one pass — the same reconciliation `migrate sync-tables` performs
when recovering names from a 2.\* assembly (see
[`TableFileSyncer.ApplyOptionSetName`](../XrmFramework.DeployUtils/TableSync/TableFileSyncer.cs)).
Renaming only the first copy found would leave the others to disagree, and `tables optionsets
list`'s overview flags exactly that drift as a `(mismatch)`.

A copy marked `"Locked": true` — the framework package's own option sets — is left untouched and
reported instead: its name belongs to the package's generated code.

#### `list` — see what's tracked

Without `--option`: one row per distinct option set found locally (logical name, C# name,
whether it's global, whether it's locked, member count, and which `.table` file(s) declare it).
With `--option <logicalname>`: the members of that one option set (value, C# name, external
value), plus the C# name as recorded by each declaring file.

| Option | Required | Description |
|---|:---:|---|
| `-o`, `--option <LOGICALNAME>` | ❌ | Drills into that option set's members instead of the overview. |
| `--filter <TEXT>` | ❌ | Overview only: keeps option sets whose logical name or C# name contains this text. |
| `--global-only` | ❌ | Overview only: keeps global option sets. |

```bash
xrmframework tables optionsets list --option ftp_contrat_statut
```

#### `set` — rename the option set and/or one member

| Option | Required | Description |
|---|:---:|---|
| `-o`, `--option <LOGICALNAME>` | ✅ | Option set to edit. |
| `--name <NEWNAME>` | ❌ | Renames the option set's C# name, in every declaring file. |
| `--value <NUMBER>` | ⚠️ | Numeric value of the member to rename. Requires `--value-name`. |
| `--value-name <NEWNAME>` | ⚠️ | New C# name for the member designated by `--value`. |

At least one of `--name` or the `--value`/`--value-name` pair is required.

```bash
xrmframework tables optionsets set --option ftp_contrat_statut --name StatutContrat
```

```bash
xrmframework tables optionsets set --option ftp_contrat_statut --value 1 --value-name EnCours
```

**Exit codes** (both verbs)

| Code | Meaning |
|:---:|---|
| `0` | Success (including "nothing to do": already in the requested state, or a member not found — reported, not fatal). |
| `1` | The option set is not declared in any local `.table` file. |
| `2` | Configuration or `.table` directory not found. |
| `3` | Unexpected error. |
| `-1` / `255` | Argument validation error (Spectre). |

Implementation: [`OptionSetHelper`](../XrmFramework.DeployUtils/TableSync/OptionSetHelper.cs)
-> [`TableFileStore`](../XrmFramework.DeployUtils/TableSync/TableFileStore.cs)
+ [`ColumnHelper`](../XrmFramework.DeployUtils/TableSync/ColumnHelper.cs) (shared local file
resolution).

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
| `-n`, `--noprompt`, `-NoPrompt` | ❌ | Silent mode: skips the connection confirmation (CI/CD). `-NoPrompt` (any casing) is kept for backward compatibility with the deployment scripts written against that spelling. |

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

### `xrmframework migrate sync-tables` ✅ *(available)* — migration from 2.\* to 3.1+

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

`migrate sync-tables` performs that hand-over in one pass:

1. it reflects over the **assembly last compiled under 2.\***, whose `*Definition` classes record
   which columns the project's code actually uses, and under which name each option set is
   compiled;
2. it brings the `.table` files in line — creating what is missing, setting `Select: true` on
   every column the code references, and naming the option sets;
3. it cleans up the `*Definition.cs` files sitting next to them.

What travels in step 2 is precisely what the CRM cannot tell you: **C# identifiers the compiled
code depends on.** Everything else is metadata `tables pull` can fetch back at any time.

```bash
xrmframework migrate sync-tables --dll <path.dll> --tables-dir <directory> [--clean]
```

| Option | Required | Description |
|---|:---:|---|
| `--dll <PATH>` | ✅ | Assembly compiled under 2.\* (contains `*Definition` classes decorated with `[EntityDefinition]` that expose a static `EntityName` field). |
| `--tables-dir <DIRECTORY>` | ✅ | Directory holding the `.table` and `*Definition.cs` files — usually `<CoreProject>/Definitions`. |
| `--clean` | ❌ | Sets `Select=false` on orphaned columns and deletes `.table` files entirely generated by the tool with no CRM data. |

**Example**

```bash
xrmframework migrate sync-tables --dll bin/Release/net8.0/MyProject.Plugins.dll \
                                 --tables-dir ../MyProject.Core/Definitions \
                                 --clean
```

> ⚠️ The command **deletes and renames source files** in `--tables-dir` (see below). Run it on a
> clean working tree so the whole migration shows up as a single reviewable diff.

#### Naming the option sets and their members

A `.table` records an option set's logical name, which comes from the CRM; the `Name` under which
it is compiled is a project decision — teams rename `workflow_runas` into `RunAsUser`
and their code depends on it. The same holds one level down: the generator derives each member's
name from its CRM label and strips the diacritics (`Modèle` becomes `Modele`), but those get
renamed too, and every `MyEnum.EnCours` in the project compiles against the result. Under 2.\*
both lived in the generated `.cs`; from 3.1 on the generator reads them from the `.table`.

The migration recovers them from `[OptionSet(typeof(SomeEnum))]` carried by the column constants —
the enum's name, and its members read off the type itself — and applies them to the option set the
column points at (matched on the column's `EnumName`) — **in every file that records it**:

- in the table's own `Enums`;
- and in `OptionSets.table`, where shared option sets live — that file is loaded once and rewritten
  only if a name actually changed.

Both, not the first one found. The 2.\* DefinitionManager kept in a table's `Enums` every option
set one of its columns referenced, *globals included*, while also writing the globals to
`OptionSets.table`. The generator unions the two, so a rename applied to only one copy would be
contradicted by the other.

Members are matched on their **numeric value**, which is the stable CRM key — never on their
position. Labels, `ExtVal`, logical name and the `IsGlobal` flag are untouched: only `Name` moves.

Four cases are deliberately left alone:

| Case | Why |
|---|---|
| Option set marked `"Locked": true` | Shipped by the framework — its names belong to the package's generated code, members included. |
| Column whose `.table` entry carries no `EnumName` | Nothing links it to an option set. Happens for a column the migration itself just created; a `tables pull` fills the metadata in. |
| A member the assembly declares twice for one value | C# allows aliases, so there is no way to tell which name the `.table` should carry. |
| A value the assembly declares no member for | The code never referenced it; the `.table` keeps the name it already had. |

One subtlety the migration handles for you: when an option set allows an empty value
(`HasNullValue`), the generator prepends a synthetic `Null = 0` member. It mirrors the flag rather
than any CRM option, so it is skipped — the real option numbered `0`, if there is one, keeps its
own name.

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

#### `OptionSetDefinitions.cs`

The 2.\* DefinitionManager gathered every option set enum into a single file of its own. It holds
no Definition class, so it goes through a pass of its own, on the same rule as the enums found
inside a `*Definition.cs`: an enum the generator re-emits is dropped, one that no **selected**
column references is kept.

| What remains in the file | Outcome |
|---|---|
| Nothing — every enum is regenerated | the file is **deleted** |
| Enums the generator does not emit | the file is **trimmed in place**, keeping only those |

A trimmed file stays in the project's own namespace: what survives is precisely what the
generator does *not* emit, so moving it to `XrmFramework` would only break the references to it.

The file is **left alone**, and reported, when none of its enums is regenerated — the signature of
a wrong `--tables-dir`, or of `.table` files declaring no selected option set column.

An enum only counts as regenerated once the `.table` files **name** the option set behind it: a
nameless option set produces no enum. This is why the `.table` synchronization runs first — on a
directory it has not been through, this pass keeps enums the generator will later emit, and the
project ends up with the same type declared twice.

What the generator emits is not decided here: both read
[`OptionSetSelection`](../XrmFramework.Core/OptionSetSelection.cs), so this pass cannot delete an
enum the generator then declines to emit.

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

> Both copies reach the generator — `XrmFramework.props` declares the package's `.table` files as
> `AdditionalFiles` before the project's own — and it folds them into a **single** `*Definition`
> class. That merge is additive: it takes the union of the columns and of the option sets, so a
> column selected only in the project's copy keeps the `enum` it references. On a conflict the
> file loaded first wins, which is the package's; renaming an option set both files declare
> therefore has to be done in both.

`OptionSets.table` is a case of its own: it describes no entity, so no `*Definition` class ever
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

---

## Roadmap

Target command tree (✅ exist, 🚧 are upcoming):

```
xrmframework
├── tables
│   ├── list           ✅  lists the tables of the environment    (connected)
│   ├── pull           ✅  .table ← Dataverse metadata            (connected)
│   ├── columns
│   │   ├── list       ✅  lists the columns already tracked      (offline)
│   │   ├── add        ✅  activates columns                     (offline)
│   │   └── set        ✅  renames a column / toggles selection   (offline)
│   └── optionsets
│       ├── list       ✅  lists option sets / their members      (offline)
│       └── set        ✅  renames an option set / a member       (offline)
├── deploy
│   ├── plugins        ✅  deploys a plugins / custom API / workflow assembly
│   └── webresources   🚧  deploys the webresources
└── migrate
    └── sync-tables    ✅  migration 2.* -> 3.1+, run once         (offline)
```

`migrate` stands apart from `tables` and `deploy`: its command rewrites the project's own
sources once and is not part of the day-to-day loop — `sync-tables` is the upgrade path
from 2.\*. Routine work is `pull` (rich metadata from the environment) plus the local edits
`tables columns` and `tables optionsets` make scriptable: column selection and C# naming in the
`.table`.

> `deploy plugins` inventories the `net462` plugin assembly by **executing its registration
> code** via the `XrmFramework.PluginInventory` tool (embedded `net462` executable) — step
> registration therefore remains completely free-form (loops, conditions…). Requires the .NET
> Framework runtime (Windows).

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
dotnet run --project src/XrmFramework.Cli -- migrate sync-tables --dll <dll> --tables-dir <dir>

# package the tool locally and inspect it
dotnet pack src/XrmFramework.Cli -c Release -o ./nupkg
unzip -p ./nupkg/XrmFramework.Cli.*.nupkg "*.nuspec"     # <packageType name="DotnetTool" />
```

> The package embeds the entire dependency *closure* under `tools/net10.0/any/`
> (including `XrmFramework.DeployUtils` and the Dataverse client): this is large but
> necessary for a self-contained tool.

The version comes from **Nerdbank.GitVersioning** (no version number to maintain by hand).
