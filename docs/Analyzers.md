# XrmFramework Analyzers

XrmFramework ships a set of **Roslyn analyzers** (and code fixes) that catch common
Dataverse plugin mistakes **at build time**, in the IDE, before the code is ever
deployed. They are the framework's guard rails: a misspelled callback, a plugin that
the platform could never instantiate, or a magic string where a typed definition was
expected all surface as a build diagnostic with a one-click fix where possible.

- [How the analyzers are delivered](#how-the-analyzers-are-delivered)
- [Severity and suppression](#severity-and-suppression)
- [Diagnostics at a glance](#diagnostics-at-a-glance)
- [Plugin registration rules](#plugin-registration-rules)
  - [XRM0002](#xrm0002)
  - [XRM0003](#xrm0003)
  - [XRM0010](#xrm0010)
  - [XRM0011](#xrm0011)
  - [XRM0012](#xrm0012)
  - [XRM0013](#xrm0013)
  - [XRM0014](#xrm0014)
- [Definition rules](#definition-rules)
  - [XRM0200](#xrm0200)
- [Usage rules](#usage-rules)
  - [XRM0300](#xrm0300)
- [Source generator diagnostics](#source-generator-diagnostics)
  - [XRM1002](#xrm1002)
  - [XRM2001](#xrm2001)
- [Reserved identifiers](#reserved-identifiers)

---

## How the analyzers are delivered

The analyzers live in the **`XrmFramework.Analyzers`** NuGet package (with code fixes in
`XrmFramework.Analyzers.CodeFixes`). They are referenced automatically by the project
templates, so a solution scaffolded with `dotnet new xrmSolution` already has them
enabled. They run inside the C# compiler (`netstandard2.0` Roslyn component): you see
their diagnostics in Visual Studio / Rider / VS Code **and** on every `dotnet build`
or CI run — there is nothing to launch.

Two families of diagnostics are emitted:

| Prefix range | Source | Reported by |
|---|---|---|
| `XRM00xx` – `XRM03xx` | **Analyzers** — static analysis of your source | `DiagnosticAnalyzer` rules |
| `XRM1xxx` / `XRM2xxx` | **Source generators** — failures while emitting generated code | The generator that failed |

---

## Severity and suppression

Each rule has a default severity (`Error` blocks the build, `Warning` does not). When a
diagnostic is a deliberate, reviewed exception you can suppress it three ways:

**Per project / folder — `.editorconfig`** (preferred, reviewable, scoped):

```ini
# Downgrade XRM0011 to a suggestion for this project
dotnet_diagnostic.XRM0011.severity = suggestion

# Turn XRM0200 off entirely (e.g. for hand-written fake definitions in a test project)
dotnet_diagnostic.XRM0200.severity = none
```

**Per line — `#pragma`:**

```csharp
#pragma warning disable XRM0012 // legacy step kept as a string on purpose
AddStep(Stages.PostOperation, Messages.Update, Modes.Synchronous, AccountDefinition.EntityName, "OnPostUpdate");
#pragma warning restore XRM0012
```

**Per member — attribute:**

```csharp
[SuppressMessage("Syntax", "XRM0011:Prefer using call to ...Definition.EntityName")]
```

> The `Fake*` test definitions in this repository, for instance, suppress `XRM0200`
> because they intentionally use string literals instead of generated `…Definition`
> classes.

---

## Diagnostics at a glance

| Rule | Title | Category | Severity | Code fix |
|---|---|---|:---:|:---:|
| [XRM0002](#xrm0002) | Callback must be visible | Naming | 🔴 Error | ✅ *Make method public* |
| [XRM0003](#xrm0003) | A plugin class must be public | Syntax | 🔴 Error | ✅ *Make class public* |
| [XRM0010](#xrm0010) | Callback not found | Syntax | 🔴 Error | — |
| [XRM0011](#xrm0011) | Prefer using `…Definition.EntityName` | Syntax | 🟡 Warning | — |
| [XRM0012](#xrm0012) | Use `nameof` expression | Syntax | 🟡 Warning | ✅ *Use nameof* |
| [XRM0013](#xrm0013) | Invalid callback method | Syntax | 🔴 Error | — |
| [XRM0014](#xrm0014) | `AddStep` must be called from `AddSteps` | Syntax | 🔴 Error | — |
| [XRM0200](#xrm0200) | Use `…Definition.EntityName` in `[CrmEntity]` | Syntax | 🔴 Error | — |
| [XRM0300](#xrm0300) | Use `IDateTimeProvider` instead of `DateTime.Now` | Usage | 🔴 Error | ✅ *Inject IDateTimeProvider* |
| [XRM1002](#xrm1002) | EnumGenerator failure | XrmFramework.Generators | 🔴 Error | — |
| [XRM2001](#xrm2001) | MappingGenerator failure | XrmFramework.Generators | 🟡 Warning | — |

---

## Plugin registration rules

These rules validate the `AddStep(...)` calls used to declare plugin steps (see
[Plugins.md](Plugins.md)). `AddStep`'s signature is:

```csharp
AddStep(Stages stage, Messages message, Modes mode, string entityName, string methodName, params string[] columns);
//                                                   ▲ 4th arg            ▲ 5th arg
//                                                   entityName           callback (method name)
```

Most of the rules below inspect the **4th argument** (`entityName`) and the **5th
argument** (the callback method name).

### XRM0002

**Callback must be visible** · Category `Naming` · Severity 🔴 **Error** · Code fix: *Make method public*

A method referenced as the callback of an `AddStep(...)` call must be **public, instance
(non-static) and non-abstract** — otherwise the framework cannot invoke it at runtime.
The rule only flags methods that are actually wired up by an `AddStep` call.

```csharp
// ❌ referenced by AddStep but not public
private void OnPostUpdate(IPluginContext context) { ... }

protected override void AddSteps()
    => AddStep(Stages.PostOperation, Messages.Update, Modes.Synchronous,
               AccountDefinition.EntityName, nameof(OnPostUpdate));
```

```csharp
// ✅ public instance method
public void OnPostUpdate(IPluginContext context) { ... }
```

**Message:** `The Method '{0}.{1}' should be public, not static and not abstract`

### XRM0003

**A plugin class must be public** · Category `Syntax` · Severity 🔴 **Error** · Code fix: *Make class public*

Any class implementing `IPlugin` (directly or through `XrmFramework.Plugin` /
`XrmFramework.CustomApi`) must be `public`. Dataverse instantiates the plugin type by
name through reflection; a non-public type can never be registered.

```csharp
// ❌
class AccountPlugin : Plugin { ... }
```

```csharp
// ✅
public class AccountPlugin : Plugin { ... }
```

**Message:** `The Plugin class '{0}' should be public`

### XRM0010

**Callback not found** · Category `Syntax` · Severity 🔴 **Error** · Code fix: —

The 5th argument of `AddStep` references a method that does **not exist** on the plugin
class or any of its base types. This usually means a typo in a string literal, or a
reference to a member that lives on another type.

```csharp
// ❌ there is no method called "OnPostUdpate" (typo)
AddStep(Stages.PostOperation, Messages.Update, Modes.Synchronous,
        AccountDefinition.EntityName, "OnPostUdpate");
```

```csharp
// ✅ reference an existing method with nameof so a rename can't break it
AddStep(Stages.PostOperation, Messages.Update, Modes.Synchronous,
        AccountDefinition.EntityName, nameof(OnPostUpdate));
```

**Message:** `The method '{0}' does not exist in type '{1}' or parent types`

### XRM0011

**Prefer using call to `…Definition.EntityName`** · Category `Syntax` · Severity 🟡 **Warning** · Code fix: —

The 4th argument (`entityName`) is a raw string literal instead of the generated
`…Definition.EntityName` constant. Magic strings are not refactor-safe and silently
break if the logical name changes.

```csharp
// ❌
AddStep(Stages.PostOperation, Messages.Update, Modes.Synchronous, "account", nameof(OnPostUpdate));
```

```csharp
// ✅
AddStep(Stages.PostOperation, Messages.Update, Modes.Synchronous, AccountDefinition.EntityName, nameof(OnPostUpdate));
```

**Message:** `Prefer using call to ...Definition.EntityName`

### XRM0012

**Use `nameof` expression** · Category `Syntax` · Severity 🟡 **Warning** · Code fix: *Use nameof*

The 5th argument is a string literal that **does** match an existing method. The method
exists, so this is not an error — but a literal won't follow a rename. The code fix
rewrites it as `nameof(...)`.

```csharp
// ❌ "OnPostUpdate" is a valid method, but a rename would silently break it
AddStep(Stages.PostOperation, Messages.Update, Modes.Synchronous, AccountDefinition.EntityName, "OnPostUpdate");
```

```csharp
// ✅ one-click fix → nameof
AddStep(Stages.PostOperation, Messages.Update, Modes.Synchronous, AccountDefinition.EntityName, nameof(OnPostUpdate));
```

**Message:** `Use 'nameof' expression to reference parameter '{0}' name`

### XRM0013

**Invalid callback method** · Category `Syntax` · Severity 🔴 **Error** · Code fix: —

The 5th argument uses `nameof(...)` (or another invocation) but the referenced symbol is
**not an ordinary method** declared in the class — for example a property, a
constructor, a field or a type. The callback must resolve to a normal method.

```csharp
// ❌ Name is a property, not a method
AddStep(Stages.PostOperation, Messages.Update, Modes.Synchronous, AccountDefinition.EntityName, nameof(Name));
```

```csharp
// ✅ point at an ordinary method
AddStep(Stages.PostOperation, Messages.Update, Modes.Synchronous, AccountDefinition.EntityName, nameof(OnPostUpdate));
```

**Message:** `The referenced element '{0}' should be a method declared in the containing class`

### XRM0014

**`AddStep` must be called from `AddSteps`** · Category `Syntax` · Severity 🔴 **Error** · Code fix: —

`Plugin.AddStep(...)` is being called from a method other than the overridden
`AddSteps()`. Step registration is collected once, when the framework calls `AddSteps`;
calling `AddStep` from anywhere else registers nothing.

```csharp
// ❌
public void Configure() => AddStep(/* ... */);
```

```csharp
// ✅
protected override void AddSteps() => AddStep(/* ... */);
```

**Message:** `AddStep should be called from AddSteps method`

---

## Definition rules

### XRM0200

**Use `…Definition.EntityName` in `[CrmEntity]`** · Category `Syntax` · Severity 🔴 **Error** · Code fix: —

The first argument of a `[CrmEntity(...)]` attribute is a string literal instead of the
generated `…Definition.EntityName` constant. Binding models must reference the typed
constant so they stay consistent with the generated definition.

```csharp
// ❌
[CrmEntity("account")]
public partial class AccountModel : IBindingModel { ... }
```

```csharp
// ✅
[CrmEntity(AccountDefinition.EntityName)]
public partial class AccountModel : IBindingModel { ... }
```

**Message:** `Use Definition class .EntityName in CrmEntityAttribute declaration`

> Hand-written test/fake definitions that deliberately use string literals can silence
> this rule with `dotnet_diagnostic.XRM0200.severity = none` in the test project's
> `.editorconfig`.

---

## Usage rules

### XRM0300

**Use `IDateTimeProvider` instead of `DateTime.Now` / `DateTime.UtcNow`** · Category `Usage` · Severity 🔴 **Error** · Code fix: *Inject IDateTimeProvider*

Direct use of `DateTime.Now` or `DateTime.UtcNow` inside a class that derives from
`XrmFramework.Plugin`, implements `Microsoft.Xrm.Sdk.IPlugin`, or implements
`XrmFramework.IService`. Reading the ambient clock directly makes the code
non-deterministic: unit tests can't pin the time, and **Remote Debugger session replay**
(see [RemoteDebugger.md](RemoteDebugger.md#session-recording-and-replay)) cannot
reproduce the original execution. Inject `IDateTimeProvider` instead.

```csharp
// ❌ inside a plugin / service
var now = DateTime.UtcNow;
```

```csharp
// ✅ plugin — inject the provider as a method parameter
public void OnPostCreate(IPluginContext context, IDateTimeProvider dateTimeProvider)
{
    var now = dateTimeProvider.UtcNow;
}

// ✅ service — inject it through the constructor (the code fix wires up the field for you)
public class AccountService : DefaultService, IAccountService
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public AccountService(IServiceContext context, IDateTimeProvider dateTimeProvider) : base(context)
        => _dateTimeProvider = dateTimeProvider;

    public void Touch(EntityReference r) => /* ... */ _dateTimeProvider.UtcNow;
}
```

The code fix adapts to the target: for plugins it adds an `IDateTimeProvider` **method
parameter**; for services it injects the provider through the **constructor** (or the
**primary constructor**) and creates the backing field, then rewrites the call to
`_dateTimeProvider.Now` / `.UtcNow`.

**Message:** `Replace '{0}' with IDateTimeProvider.{1} — inject IDateTimeProvider as a method parameter`

---

## Source generator diagnostics

These are not static-analysis rules: they are emitted by the XrmFramework **source
generators** when they fail to produce generated code. They almost always point at a
malformed declaration the generator could not handle; the `{1}` placeholder carries the
underlying exception message.

### XRM1002

**EnumGenerator failure** · Category `XrmFramework.Generators` · Severity 🔴 **Error**

The smart-enum generator could not produce the `Items` collection for a type decorated
with `[EnumGeneration]`. Check the decorated enum/class for an unsupported member shape;
the message includes the failing type and the exception detail.

**Message:** `Could not generate Items collection for '{0}': {1}`

### XRM2001

**MappingGenerator failure** · Category `XrmFramework.Generators` · Severity 🟡 **Warning**

The mapping generator could not generate the mapping for a binding model. The model's
generated mapping is skipped; fix the reported type and rebuild.

**Message:** `Could not generate mapping for '{0}': {1}`

---

## Reserved identifiers

`XRM0100`, `XRM0101` and `XRM0102` are **reserved** for the binding-model analyzer
(`partial` requirement, converter generation). That analyzer is currently disabled in
source, so these diagnostics are **not emitted** today — they are documented here only so
the identifiers are not reused.

---

*See also: [Plugins](Plugins.md) · [Working with Services](WorkingWithServices.md) · [Custom APIs](CustomApis.md) · [Remote Debugger](RemoteDebugger.md)*
