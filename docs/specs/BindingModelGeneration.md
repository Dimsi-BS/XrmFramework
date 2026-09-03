# Spec — Generating binding models from `.model` files

**Status:** draft, for discussion · **Target:** 3.2 · **Owner:** —

Generate a binding model — the class, its `[Crm*]` attributes, its properties, **and** its
`ToEntity` / `ToBindingModel` mapping methods — from a declarative `.model` file, the way
`*Definition` classes are already generated from `.table` files.

The intent is that a `.model` becomes the single source of truth for a binding model, so that
adding a column to a model is an edit to a data file rather than three edits to hand-written C#
(the property, the `[CrmMapping]`, and the mapping code).

---

## 1. What already exists

This feature is **roughly half built and currently unreachable**. Anyone picking it up should
start here rather than from a blank page.

| Piece | Location | State |
|---|---|---|
| `.model` schema | [`XrmFramework.Core/Model.cs`](../../src/XrmFramework.Core/Model.cs), [`ModelProperty.cs`](../../src/XrmFramework.Core/ModelProperty.cs) | Exists. JSON, abbreviated names (`tName`, `ns`, `Cols`, `LogN`, `UsePropCh`). |
| Class generator | [`ModelSourceFileGenerator.cs`](../../src/XrmFramework.Analyzers/Generators/ModelSourceFileGenerator.cs) (348 lines) | Exists. Reads `.model` + `.table`, emits the partial class, `[CrmEntity]`, `[CrmMapping]`, `[CrmLookup]`, `[ChildRelationship]`, properties, backing fields, `OnPropertyChanged`. |
| Mapping generator | [`MappingSourceGenerator.cs`](../../src/XrmFramework.Analyzers/Generators/MappingSourceGenerator.cs) (737 lines) | Exists. Emits explicit, breakpoint-able `ToBindingModel(Entity)` / `ToEntity(IOrganizationService?)` for any non-abstract `partial` class with `[CrmEntity]` implementing `IBindingModel`. |
| Runtime base | [`BindingModelBase.cs`](../../src/XrmFramework/BindingModel/BindingModelBase.cs) | Exists. `IBindingModel`, `INotifyPropertyChanged`, dependent-attribute propagation. |
| Model folder in the template | `Solution/$safeprojectname$.Core/` declares `Model/` next to `Definitions/` | Exists, empty. |

### What is missing or broken

1. **`.model` is never passed to the compiler.** Every `AdditionalFiles` declaration in the repo
   globs `**/*.table` and nothing else — see [`XrmFramework.props`](../../src/XrmFramework/build/XrmFramework.props)
   and the six other projects that repeat the pattern. `ModelSourceFileGenerator` filters on
   `a.Path.EndsWith(".model")`, so today it is guaranteed to receive zero of them.
2. **No `.model` file exists anywhere**, in the repo or in the templates, and nothing produces
   one — no DefinitionManager screen, no CLI command. The format has never been exercised.
3. **No tests.** `XrmFramework.Analyzers.Tests.csproj` already carries a comment about fixtures
   "used by `TableSourceFileGeneratorTests` and `ModelSourceFileGeneratorTests`", but that second
   test class does not exist, and `Resources/` holds only `.table` fixtures.
4. **The two generators cannot see each other** — see §2. This is the blocking issue.
5. `ModelSourceFileGenerator` reports failures by emitting a source file named `Exception.txt`
   containing a commented-out stack trace. It has no diagnostic id.

---

## 2. The blocking constraint

**Roslyn source generators never see one another's output.** Every generator in a compilation
receives the same input `Compilation` and the same syntax trees; generated files are added
afterwards. Two consequences, both fatal to the naive design:

**a. The mapping generator will not see a generated class.**
`MappingSourceGenerator` discovers candidates through
`context.SyntaxProvider.CreateSyntaxProvider(CouldBeBindingModel, …)`, which walks the
compilation's *source* syntax trees. A class emitted by `ModelSourceFileGenerator` is not in
them. So a `.model` would produce a class with correct attributes and **no mapping methods** —
exactly half of what this feature is for.

**b. The mapping generator may not resolve `*Definition` constants either.**
`ExtractModelInfo` requires the attribute argument to be a resolved constant:

```csharp
if (crmEntityAttr.ConstructorArguments.FirstOrDefault().Value is not string entityName)
    return null;   // silently produces nothing
```

`[CrmEntity(AccountDefinition.EntityName)]` only resolves if `AccountDefinition` is a real type
in the compilation or a referenced assembly. In the generated solution layout, `.table` files and
binding models both live in the **`.Core` project**, so `AccountDefinition` is emitted by
`TableSourceFileGenerator` in that same pass and is *not* resolvable — which would mean
hand-written binding models in `.Core` silently get no mapping today.

> **To verify before designing around it.** `MappingSourceGeneratorTests` hand-writes its
> `AccountDefinition` and `ContactDefinition` as ordinary source, so it never exercises the
> generated-definition case. First task of this work: add a test that feeds a `.table` **and** a
> hand-written binding model referencing the generated `…Definition.EntityName`, and assert
> whether mapping is emitted. The answer decides §3.

### Options

| Option | Sketch | Trade-off |
|---|---|---|
| **A. One generator** *(recommended)* | Merge model emission into a single generator that, for each `.model`, emits the class **and** its mapping in one file, reusing `MappingSourceGenerator`'s emitter. `MappingSourceGenerator` keeps handling hand-written models. | The emitter has to be refactored to accept a model description that comes from a `.model` as well as from a symbol. Cleanest result: one `.model` in, one complete `.cs` out. |
| **B. Two projects** | `.model` files live in `.Core`; the generated classes are consumed by `.Plugins`, where the mapping generator can see them as referenced types. | Does not work: the mapping methods must live *on* the model class, in the assembly that declares it. |
| **C. Post-init sources** | Emit the model classes from `RegisterPostInitializationOutput`. | Post-init output is not visible to `SyntaxProvider` either, and cannot depend on `AdditionalFiles`. |
| **D. Check in the generated class** | The `.model` generates a `.cs` written to disk, committed, then treated as ordinary source. | Re-introduces exactly the duplication that `.table` moved away from in 3.1. Rejected unless A proves impractical. |

Option A also settles §2b: reading the column name from the `.table` rather than from a resolved
C# constant means nothing has to be semantically resolvable at generation time. The generated
code still *references* `AccountDefinition.Columns.Name` textually, which compiles fine because
both generated files land in the same final compilation — this is already how
`MappingSourceGenerator` emits column references (it copies the argument's syntax text, see
`GetArgText`).

---

## 3. Proposed design

### 3.1 Pipeline

```
Contoso.Core/
  Definitions/Account.table   ─┐
  Definitions/OptionSets.table ├─→ TableSourceFileGenerator  → AccountDefinition, enums
  Model/AccountModel.model    ─┘
            │
            └────────────────────→ ModelSourceFileGenerator  → AccountModel.model.g.cs
                                     (class + attributes + properties + ToEntity/ToBindingModel)
```

The model generator needs the `.table` of the model's table (it already collects them) to resolve
column names, types, lookup targets and option sets. That dependency exists in the current code
and is why both file kinds are read by the same generator.

### 3.2 MSBuild wiring

`.model` must be added everywhere `.table` is declared today, with the same exclusions:

```xml
<AdditionalFiles Include="**/*.model" Exclude="bin/**;obj/**" />
```

Files to touch: `XrmFramework/build/XrmFramework.props`, `XrmFramework.Plugin/build/net462/XrmFramework.Plugin.props`,
and the four project files that repeat the glob. **Open question:** should the framework package
ship `.model` files of its own the way it ships `.table` files? Probably not — the framework
defines tables, applications define models.

### 3.3 Generated output contract

For a `.model` naming table `account` and property `Name` on column `name`:

```csharp
[GeneratedCode("XrmFramework", "<real version>")]
[ExcludeFromCodeCoverage]
[CrmEntity(AccountDefinition.EntityName)]
[JsonObject(MemberSerialization.OptIn)]
public partial class AccountModel : BindingModelBase
{
    [CrmMapping(AccountDefinition.Columns.Name)]
    public string Name { get => _name; set { … OnPropertyChanged(); } }

    // + ToBindingModel(Entity) / ToEntity(IOrganizationService?) in the same file
}
```

`partial` is mandatory — it is what lets a project add hand-written members, and it is what
[XRM0100](../Analyzers.md#reserved-identifiers) exists to enforce. Reviving that rule and shipping
this feature belong together.

### 3.4 Schema changes to `Model` / `ModelProperty`

The current schema carries fields the generator ignores entirely:
`LookupTargetModel`, `LookupTargetTableLogicalName`, `LookupTargetColumnLogicalName`,
`JsonConverterType`, `JsonConverterConstructorArguments`, `JsonIgnore`, `ModelConverterType`,
`ModelConverterConstructorArguments`, and `Model.JsonMemberSerializationStrategy`.

Decide per field: implement it, or drop it before the format has any users. Dropping is nearly
free today and impossible once `.model` files exist in customer repositories — **this is the one
decision that must be made before the first release, not after.**

Also worth settling now:

- `Model.Properties` has no setter, only a populated collection. It deserializes, but it makes
  the DTO awkward to build in tooling; align it with how `Table` does it.
- Abbreviated JSON names (`tName`, `ns`, `Cols`, `LogN`, `UsePropCh`) were presumably chosen for
  file size. `.table` files are far larger and are not abbreviated. Consider aligning on
  readability, since these files are reviewed in pull requests.
- The generated class currently defaults to namespace `ProjectModels` when `ns` is empty, and is
  stamped `[GeneratedCode("XrmFramework", "2.0")]`. Both should follow the project
  (`XrmFrameworkCoreProjectName`, real assembly version) the way the table generator does.

### 3.5 Behaviour to fix while porting

Current `ModelSourceFileGenerator` behaviours that should not survive into a shipped feature:

- Throws a bare `Exception` when `OptionSets.table` is absent (`"global enums is null for some reason"`).
- **Silently skips** a property whose `LogN` matches no column in the table (`continue`) — the
  most likely authoring mistake produces a class quietly missing a property.
- Throws when a lookup column has no matching many-to-one relationship, and when the model's
  table is not found — while also having an earlier branch that emits a placeholder source file
  for that same case.
- Emits `[ChildRelationship(...)]` for one-to-many properties, an attribute
  `MappingSourceGenerator` does not handle at all. Either the mapping generator learns it, or
  child collections are declared out of scope for the first version.

### 3.6 Diagnostics

Failures must be diagnostics, not an `Exception.txt` source file. Reserve a contiguous block in
the generator range (`XRM1005`–`XRM1008` are free; `XRM1001`–`XRM1004` are the table generator's,
`XRM2001` the mapping generator's) for at least:

| Id | Condition |
|---|---|
| `XRM1005` | `.model` references a table no `.table` declares |
| `XRM1006` | Property references a column absent from the table, or not `Select: true` |
| `XRM1007` | Lookup property whose column has no many-to-one relationship in the table |
| `XRM1008` | Malformed `.model` (JSON parse failure), carrying the parser message |

Add them to `AnalyzerReleases.Unshipped.md` and document them in `docs/Analyzers.md` in the same
change — that pairing was missed for `XRM1001`/`1003`/`1004` and had to be fixed after the fact
in #47.

---

## 4. Authoring `.model` files

A format nothing can produce will not get used. `.table` has two producers (the DefinitionManager
UI and `xrmframework tables pull/columns/optionsets`); `.model` needs at least one.

Cheapest credible path, symmetric with the existing CLI:

```
xrmframework models list                     # models tracked in the project
xrmframework models add <name> --table <t>   # create a .model from a table
xrmframework models columns add <name> -c <col>...   # add properties from selected columns
```

All three are offline, operating on local files, and would reuse `ProjectConfigLocator` and a
`ModelFileStore` mirroring [`TableFileStore`](../../src/XrmFramework.DeployUtils/TableSync/TableFileStore.cs).
`models columns add` should refuse a column that is not `Select: true` in the `.table` — a
property mapped to a column the definition does not expose cannot compile.

DefinitionManager support is desirable but is a WinForms `net462` project; treat it as a later
increment.

---

## 5. Testing

Mirror `TableSourceFileGeneratorTests`: snapshot tests (Verify) over fixture files in
`Resources/`, feeding `.model` + `.table` as `AdditionalText` and asserting the full generated
source. Cases that must exist before this ships:

- Scalar columns of each `AttributeTypeCode`, including option sets and multi-select.
- Lookup property, with and without the target table tracked in the project.
- A property whose column is absent, or present but not selected → diagnostic, not silence.
- Missing `OptionSets.table` → diagnostic, not exception.
- Idempotence: same inputs, byte-identical output.
- **The §2 verification test**, which decides the whole architecture.

---

## 6. Out of scope for a first version

- Child collections / `[ChildRelationship]`, unless §3.5 resolves cheaply.
- Round-tripping: generating a `.model` *from* an existing hand-written binding model (a
  migration akin to `migrate sync-tables`). Worth doing later; not needed to ship.
- DefinitionManager UI.
- Any change to `BindingModelBase` or to the `IService` typed-model methods
  (`GetById<T>`, `Upsert<T>`), which already work against `IBindingModel`.

---

## 7. Open questions

1. Does `MappingSourceGenerator` actually work today for a hand-written binding model in a
   project whose `*Definition` classes are generated? (§2b — first thing to measure.)
2. Which `ModelProperty` fields survive? (§3.4 — must be answered before first release.)
3. Do child collections make the first version?
4. Does the framework package ship `.model` files, or is the format application-only?
5. Is the abbreviated JSON naming kept for compatibility with something, or free to change?
