# XrmFramework Plugins

- [XrmFramework Plugins](#xrmframework-plugins)
  - [Overview](#overview)
  - [Creating a Plugin](#creating-a-plugin)
  - [Registering Steps](#registering-steps)
    - [Stages](#stages)
    - [Messages](#messages)
    - [Modes](#modes)
  - [Method Attributes](#method-attributes)
    - [PreImage](#preimage)
    - [PostImage](#postimage)
    - [FilteringAttributes](#filteringattributes)
    - [ExecutionOrder](#executionorder)
    - [Impersonation](#impersonation)
    - [UnsecureConfig](#unsecureconfig)
  - [Method Dependency Injection](#method-dependency-injection)
    - [IPluginContext](#iplugincontext)
    - [IService](#iservice)
    - [Custom services of the consuming project](#custom-services-of-the-consuming-project)
  - [Working with Images](#working-with-images)
  - [Plugin Configuration](#plugin-configuration)
  - [Using a Third-Party Library](#using-a-third-party-library)
  - [Execution Flow](#execution-flow)
  - [Complete Example](#complete-example)

---

## Overview

XrmFramework plugins are the primary mechanism for running custom server-side logic in Dynamics 365 / Dataverse. They react to platform events (Create, Update, Delete, Associate, …) on specific entities, at specific pipeline stages, and in synchronous or asynchronous mode.

The framework wraps the raw Dataverse `IPlugin` interface behind a clean base class that handles:

- **Declarative step registration** — steps are described in code using `AddStep` calls and method attributes; the deploy tool reads them and registers the correct plugin steps, pre/post images, and filtering attributes in Dataverse automatically.
- **Dependency injection** — step methods receive a typed `IPluginContext` and any `IService` implementations they need; the framework resolves and injects them at runtime.
- **Rich tracing** — every step execution is logged (entity name, message, stage, mode, user IDs, input/shared parameters dump) without any boilerplate in your code.
- **Remote debugger support** — the plugin can be transparently redirected to a local debugger session; see [RemoteDebugger](RemoteDebugger.md).

---

## Creating a Plugin

Inherit from `Plugin` and implement `AddSteps`. The constructor signature must match the one expected by Dataverse (an unsecured config string and a secured config string).

```csharp
using XrmFramework;
using Contoso.Core;

public class AccountPlugin : Plugin
{
    public AccountPlugin(string unsecuredConfig, string securedConfig)
        : base(unsecuredConfig, securedConfig)
    {
    }

    protected override void AddSteps()
    {
        // Step registrations go here
    }
}
```

> **Important:** Never store per-execution state in instance fields. Dataverse caches plugin instances across executions; all execution state must be read from the `IPluginContext` injected into each step method.

---

## Registering Steps

Inside `AddSteps`, call `AddStep` for every event–entity–method combination you want to handle:

```csharp
protected override void AddSteps()
{
    AddStep(Stages.PreValidation, Messages.Create,  Modes.Synchronous,  AccountDefinition.EntityName, nameof(OnPreValidateCreate));
    AddStep(Stages.PreOperation,  Messages.Update,  Modes.Synchronous,  AccountDefinition.EntityName, nameof(OnPreUpdate));
    AddStep(Stages.PostOperation, Messages.Delete,  Modes.Asynchronous, AccountDefinition.EntityName, nameof(OnPostDelete));
}
```

The signature of `AddStep` is:

```csharp
protected void AddStep(
    Stages  stage,
    Messages message,
    Modes   mode,
    string  entityName,
    string  methodName,
    params string[] columns   // optional fallback columns for filtering / images
);
```

The framework validates at construction time that the referenced method exists, is public, non-static, and that all its parameters are either `IPluginContext` or an `IService` interface.

### Stages

| Value | Constant | Description |
|-------|----------|-------------|
| 10 | `Stages.PreValidation` | Before database transaction — use to throw early validation errors |
| 20 | `Stages.PreOperation` | Inside the transaction, before the core operation |
| 40 | `Stages.PostOperation` | After the core operation (inside or outside the transaction depending on mode) |

### Messages

`Messages` is a typed constant class with a static member for every built-in Dataverse message: `Messages.Create`, `Messages.Update`, `Messages.Delete`, `Messages.Associate`, `Messages.Disassociate`, `Messages.Assign`, `Messages.SetState`, and many more.

For custom messages / Custom APIs, use `Messages.From("your_message_name")` or rely on the `[CustomApi]` attribute system described in [CustomApis.md](CustomApis.md).

### Modes

| Constant | Description |
|----------|-------------|
| `Modes.Synchronous` | Runs inside the platform transaction; can roll back on exception |
| `Modes.Asynchronous` | Runs in a background job after the operation completes |

---

## Method Attributes

Attributes placed on a step method enrich the plugin step registration and control its runtime behaviour. All attributes target `AttributeTargets.Method`.

### PreImage

Requests a snapshot of the entity **before** the operation. Only useful for `Update`, `Delete`, and `Merge` messages.

```csharp
// Request specific columns
[PreImage(AccountDefinition.Columns.Name, AccountDefinition.Columns.AccountNumber)]
public void OnPreUpdate(IPluginContext context) { ... }

// Request all columns
[PreImage(true)]
public void OnPreUpdate(IPluginContext context) { ... }
```

The image is accessible at runtime via `context.GetPreImage("PreImage")`.

### PostImage

Requests a snapshot of the entity **after** the operation. Only available in `PostOperation` stage for `Create` and `Update` messages.

```csharp
[PostImage(AccountDefinition.Columns.Name, AccountDefinition.Columns.Revenue)]
public void OnPostCreate(IPluginContext context) { ... }

// Request all columns
[PostImage(true)]
public void OnPostCreate(IPluginContext context) { ... }
```

The image is accessible at runtime via `context.GetPostImage("PostImage")`.

### FilteringAttributes

Restricts the `Update` step so it only fires when at least one of the listed attributes has changed. This avoids unnecessary executions and is strongly recommended on `Update` steps.

```csharp
[FilteringAttributes(AccountDefinition.Columns.Name, AccountDefinition.Columns.AccountNumber)]
public void OnPreUpdate(IPluginContext context) { ... }
```

> If no `[FilteringAttributes]` attribute is present, the `columns` parameter passed to `AddStep` is used as the fallback filtering list.

### ExecutionOrder

Sets the execution order of the step when multiple plugins are registered on the same event. Lower values execute first (default is `1`).

```csharp
[ExecutionOrder(100)]
public void OnPreUpdate(IPluginContext context) { ... }
```

### Impersonation

Runs the step as a specific Dataverse user instead of the initiating user.

```csharp
[Impersonation("service-account@contoso.onmicrosoft.com")]
public void OnPostCreate(IPluginContext context) { ... }
```

### UnsecureConfig

Attaches a per-step unsecured configuration string to the step registration. Useful when the same plugin handles multiple steps that need different configuration.

```csharp
// Inline value
[UnsecureConfig("{\"mode\":\"strict\"}")]
public void OnPreUpdate(IPluginContext context) { ... }

// Value from a resource / static property
[UnsecureConfig(typeof(PluginResources), nameof(PluginResources.StepConfig))]
public void OnPreValidateCreate(IPluginContext context) { ... }
```

The unsecured config is available at runtime from `context.UnsecureConfig` (whole plugin level) or, when using the per-step attribute, it overrides the plugin-level value for that specific step.

---

## Method Dependency Injection

Step methods can declare any combination of the following parameter types. The framework resolves and injects all of them automatically.

**The dependency container is the only authority.** Any interface it can provide is injectable — no
marker interface to implement, no list to extend:

| Parameter type | Provided by |
| --- | --- |
| `IPluginContext` | The execution context built by the framework |
| An interface extending `IService` | The generated `RegisterServices` |
| `IDateTimeProvider` and other framework services | `InternalDependencyProvider.RegisterDefaults` |
| Any interface of the consuming project | `InternalDependencyProvider.RegisterCustomService` |

Validation happens in two steps:

1. **At plugin instantiation**, a parameter that is not an interface is rejected — the container
   would otherwise silently construct an arbitrary concrete type nobody registered:

   ```text
   MyPlugin.OnPreUpdate parameter : myParam. Only interfaces registered in the dependency container are allowed as parameters
   ```

2. **At step invocation**, an interface the container holds no registration for — directly, or
   anywhere down the constructor chain of the service being built — names the member at fault:

   ```text
   MyPlugin.OnPreUpdate parameter : pricing. IPricingCalculator is not registered in the dependency
   container. Register it in InternalDependencyProvider.RegisterCustomService, or declare it as an
   IService. Container details : Interface cannot be resolved: IPricingCalculator
   ```

### IPluginContext

Provides access to the full execution context.

```csharp
public void OnPreUpdate(IPluginContext context)
{
    var entityId   = context.PrimaryEntityId;
    var entityName = context.PrimaryEntityName;
    var userId     = context.UserId;
    var initiator  = context.InitiatingUserId;

    bool isPreValidation = context.IsPreValidation();
    bool isPreOperation  = context.IsPreOperation();
    bool isPostOperation = context.IsPostOperation();

    // Read input parameter (e.g. the entity being created/updated)
    var target = context.GetInputParameter<Entity>(InputParameters.Target);

    // Read / write shared variables (cross-plugin communication)
    context.SetSharedVariable("myKey", 42);
    int val = context.GetSharedVariable<int>("myKey");

    // Pre / post images (only present if registered via attributes)
    if (context.HasPreImage("PreImage"))
    {
        var pre = context.GetPreImage("PreImage");
    }

    // Log a message to the Dataverse trace log
    context.Log("Processing account {0}", entityId);
}
```

Key members of `IPluginContext`:

| Member | Description |
|--------|-------------|
| `PrimaryEntityId` | GUID of the record being operated on |
| `PrimaryEntityName` | Logical name of the entity |
| `UserId` | ID of the user the plugin runs as |
| `InitiatingUserId` | ID of the user who originally triggered the event |
| `OrganizationId` | ID of the current organisation |
| `CorrelationId` | Correlation ID shared across related operations |
| `ParentContext` | Parent plugin context (for nested plugin calls) |
| `IsPreValidation()` | `true` when running at stage 10 |
| `IsPreOperation()` | `true` when running at stage 20 |
| `IsPostOperation()` | `true` when running at stage 40 |
| `IsStage(Stages)` | Generic stage check |
| `IsCreate()` / `IsUpdate()` / `IsMessage(Messages)` | Message checks |
| `IsSynchronous()` / `IsAsynchronous()` | Mode checks |
| `GetInputParameter<T>(InputParameters)` | Read a pipeline input parameter |
| `SetInputParameter<T>(InputParameters, T)` | Modify a pipeline input parameter |
| `GetOutputParameter<T>(OutputParameters)` | Read a pipeline output parameter |
| `SetOutputParameter<T>(OutputParameters, T)` | Write a pipeline output parameter |
| `GetPreImage(name)` / `GetPostImage(name)` | Access registered entity snapshots |
| `GetSharedVariable<T>(key)` / `SetSharedVariable<T>(key, value)` | Cross-plugin shared variables |
| `Log(message, args)` | Write to the Dataverse trace log |
| `IsMultiplePrePostOperation` | `true` when the parent operation modified more attributes than the current stage target |

### IService

Any interface that extends `IService` (see [Working with Services](WorkingWithServices.md)) can be declared as a parameter and will be injected automatically.

```csharp
public void OnPreUpdate(IPluginContext context, IAccountService accountService)
{
    var number = accountService.GetAccountNumber(
        new EntityReference(AccountDefinition.EntityName, context.PrimaryEntityId)
    );

    context.Log("Account number: {0}", number);
}
```

Multiple services can be declared in any order:

```csharp
public void OnPostCreate(IPluginContext context, IAccountService accountService, IContactService contactService)
{
    // ...
}
```

### Custom services of the consuming project

A consuming project registers its own services by completing the `RegisterCustomService` partial
method of `InternalDependencyProvider`. Registering is all it takes: the service becomes injectable
into a plugin method, a Custom Api method or a custom workflow activity action, exactly like a
built-in one, and it needs no base class and no marker interface.

```csharp
// 1. A plain interface and its implementation
public interface IPricingCalculator
{
    decimal GetDiscountRate(Guid accountId);
}

public class PricingCalculator : IPricingCalculator
{
    private readonly IServiceContext _context;

    // Constructor parameters are resolved by the container as well
    public PricingCalculator(IServiceContext context) => _context = context;

    public decimal GetDiscountRate(Guid accountId) => 0.1m;
}
```

```csharp
// 2. The service is registered once, in the plugin project
using BoDi;

namespace XrmFramework
{
    public static partial class InternalDependencyProvider
    {
        static partial void RegisterCustomService(IObjectContainer container)
        {
            container.RegisterTypeAs<PricingCalculator, IPricingCalculator>();
        }
    }
}
```

```csharp
// 3. The service is injected like any built-in one
public void OnPreCreate(IPluginContext context, IPricingCalculator pricing, IDateTimeProvider clock)
{
    var rate = pricing.GetDiscountRate(context.PrimaryEntityId);
    context.Log("Rate {0} computed at {1}", rate, clock.UtcNow);
}
```

The container resolves the constructor parameters of `PricingCalculator` the same way, so a custom
service can itself depend on `IServiceContext`, on an `IService`, or on another custom service. A
missing registration anywhere in that chain throws at invocation time, with the faulty plugin method
parameter named.

> The `RegisterCustomService` partial method exists in two flavours: the `IObjectContainer` one shown
> above for plugin / Custom Api / workflow projects, and an `IServiceCollection` one in the core
> project for console applications and Azure Functions
> (`XrmFrameworkServiceCollectionExtension.RegisterCustomService`). Register the service in both when
> it is used on both sides.

`RegisterCustomService` runs after the framework defaults, so registering an interface the framework
already provides — `IDateTimeProvider` for instance — replaces the default implementation.

Services registered this way are **not** wrapped in a generated `Logged…` decorator — only interfaces
extending `IService` are. Use `IService` when you want the framework's automatic call logging, a plain
interface plus `RegisterCustomService` for everything else.

---

## Working with Images

Images let you compare the entity state before and after an operation.

```csharp
[PreImage(AccountDefinition.Columns.Name, AccountDefinition.Columns.Revenue)]
[PostImage(AccountDefinition.Columns.Name, AccountDefinition.Columns.Revenue)]
[FilteringAttributes(AccountDefinition.Columns.Name, AccountDefinition.Columns.Revenue)]
public void OnPreUpdate(IPluginContext context)
{
    var target = context.GetInputParameter<Entity>(InputParameters.Target);

    // The pre-image contains the record state before the update
    var pre  = context.GetPreImage(Plugin.PreImageName);

    var oldName = pre.GetAttributeValue<string>(AccountDefinition.Columns.Name);
    var newName = target.GetAttributeValue<string>(AccountDefinition.Columns.Name);

    if (oldName != newName)
    {
        context.Log("Account name changed from '{0}' to '{1}'", oldName, newName);
    }
}
```

Constants `Plugin.PreImageName` (`"PreImage"`) and `Plugin.PostImageName` (`"PostImage"`) match the names registered by the deploy tool.

---

## Plugin Configuration

Dataverse allows each plugin step to carry an **unsecured** (visible to administrators) and a **secured** (encrypted) configuration string.

The framework exposes both via `IPluginContext`:

```csharp
public void OnPreValidateCreate(IPluginContext context)
{
    // Whole-plugin unsecured config (or the step-level override if [UnsecureConfig] was used)
    string config = context.UnsecureConfig;

    // Secured config (plugin-level only)
    string secret = context.SecureConfig;
}
```

When the unsecured config is a JSON object, the framework deserialises it into a `StepConfiguration` automatically. The `Configuration` property of that object is surfaced as `UnsecureConfig` on the context, allowing you to embed arbitrary JSON in the config while the framework strips away the internal `relName` / `configuration` envelope used for Associate / Disassociate steps.

---

## Using a Third-Party Library

A plugin assembly registered on its own cannot bring a dependency with it: Dataverse loads that
single assembly into the sandbox, and anything it references that the sandbox does not already
provide fails at runtime with a `FileNotFoundException`. Microsoft does not support ILMerge as a
way around it.

The supported answer is a **plugin package**: a NuGet package holding the plugin assembly *and*
its dependencies, stored in the `pluginpackage` table and unpacked by Dataverse into the sandbox.
Since 3.2, XrmFramework builds and deploys one for you. Two things to set, once:

```xml
<!-- Contoso.Plugins.csproj -->
<PropertyGroup>
  <XrmFrameworkPluginPackage>true</XrmFrameworkPluginPackage>
</PropertyGroup>

<!-- Conditioned on the same property, so it follows it: see "Newtonsoft.Json" below.
     The scaffolded plugin project already contains this - there is nothing to edit. -->
<ItemGroup Condition="'$(XrmFrameworkPluginPackage)' == 'true'">
  <PackageReference Include="Newtonsoft.Json" />
</ItemGroup>
```

```xml
<!-- Config/xrmFramework.config -->
<add name="Contoso.Plugins" targetSolution="PluginsSolutionUniqueName" type="PluginsWorkflows" deployment="Package" />
```

Then reference the library as you would anywhere else, and keep your usual workflow — build the
solution, run the `Deploy.*` project. Publishing and packaging are hooked onto the build, so there
is no extra command and nothing changes in how you deploy.

```xml
<ItemGroup>
  <PackageReference Include="ClosedXML" Version="0.104.2" />
</ItemGroup>
```

What the framework does differently in this mode, and why it matters to you:

- **Assemblies the sandbox already provides are left out of the package** — the Dataverse SDK,
  `System.Text.Json` and the rest of the `Microsoft.CrmSdk.CoreAssemblies` closure. Shipping them
  risks loading a different version than the server's. To ship one deliberately, remove it from
  the list: `<XrmFrameworkSandboxProvidedAssemblyName Remove="System.Text.Json" />`.
- **Signing is turned off.** It is not required in a package, and a signed assembly cannot load an
  unsigned dependency — which is exactly what you are packaging. The scaffolded solution signs
  every project, hence the override.
- **Dataverse creates the `pluginassembly` and `plugintype` rows itself** from the package. The
  deployment pushes the package first, waits for the platform to unpack it, then registers steps,
  images and Custom APIs against what the platform produced.
- **Set `XrmFrameworkPackageIncludePdb` to `true`** to also ship the `.pdb`. The sandbox extracts
  the whole package, so server-side stack traces then carry line numbers — something a classic
  assembly registration cannot offer.

### Newtonsoft.Json

A plugin assembly registered on its own cannot carry Newtonsoft.Json either, and the copy living
in the sandbox is not a version anyone controls — so `XrmFramework.Plugin` ships Newtonsoft's
**source** and compiles it into the plugin assembly. That is why a plugin project gets a
`Newtonsoft.Json` namespace without ever referencing the package.

A plugin package lifts that constraint, so in package mode the framework stops compiling the
vendored source and expects the real assembly instead — hence the `PackageReference` above. The
two are mutually exclusive: both at once would declare every Newtonsoft type twice.

That reference is conditioned on `XrmFrameworkPluginPackage`, which is why switching a project
over is a single edit: the property is the only lever, and the reference follows it. A scaffolded
plugin project ships both lines, so there is nothing to add — and because the condition is in the
project itself, the ordinary restore that `dotnet build` performs picks the change up.

For a project written by hand, each half of the mistake gets its own build error naming the fix
(`XRMFW1001` for a package without the reference, `XRMFW1002` for a reference without the
package), because the compiler's own output — hundreds of `CS0246` or `CS0433` — says nothing
useful about the cause.

Two welcome side effects in package mode: the plugin assembly shrinks by roughly 750 KB, and the
Newtonsoft version becomes yours to pin in `Directory.Packages.props` rather than whatever
XrmFramework vendored.

Two platform limitations, which the deployment refuses up front rather than failing halfway:

- **Custom workflow activities are not supported** in a package. If you have any, keep them in a
  separate, signed assembly project left on `deployment="Assembly"` — a plugin project and a
  workflow project can target the same solution.
- **On-premise environments are not supported.**

One more consequence worth knowing before you switch an existing project: a package creates its
**own** `pluginassembly` row, and Microsoft documents no in-place conversion. Moving a project
that is already deployed means unregistering the old assembly — which takes its types and steps
with it — then pushing the package and re-registering. Steps are declared in code so rebuilding
them costs nothing, but their GUIDs change (in a managed solution that reads as a component
removal plus an addition, not an update) and there is a window where the plugins do not run. Plan
it as a maintenance operation; new projects have nothing to migrate.

---

## Execution Flow

When Dataverse fires a plugin step the following happens inside `Plugin.Execute`:

1. A `LocalPluginContext` is created, wrapping the raw `IServiceProvider`. It initialises the tracing service, organisation service factory, and execution context.
2. The framework logs the entity name, message, stage, mode, user IDs, and a dump of input and shared variables.
3. If a **remote debugger session** is active for the current user, execution is redirected to the local machine; see [RemoteDebugger.md](RemoteDebugger.md).
4. The list of registered `Step` objects is filtered to those matching the current stage, message, mode, and entity. For `Update` steps, the `FilteringAttributes` check is applied against the attributes present in the `Target` entity.
5. Each matching step method is called with its injected parameters.
6. If an `InvalidPluginExecutionException` is thrown it propagates directly to the platform (shows an error to the user). Any other exception is wrapped in an `InvalidPluginExecutionException`.
7. Regardless of success or failure the trace log is flushed.

---

## Complete Example

```csharp
using Microsoft.Xrm.Sdk;
using XrmFramework;
using Contoso.Core;
using Contoso.Core.Services;

namespace Contoso.Plugins
{
    public class AccountPlugin : Plugin
    {
        public AccountPlugin(string unsecuredConfig, string securedConfig)
            : base(unsecuredConfig, securedConfig)
        {
        }

        protected override void AddSteps()
        {
            // Validate before creation
            AddStep(Stages.PreValidation, Messages.Create, Modes.Synchronous,
                AccountDefinition.EntityName, nameof(OnPreValidateCreate));

            // React to name or account number changes
            AddStep(Stages.PreOperation, Messages.Update, Modes.Synchronous,
                AccountDefinition.EntityName, nameof(OnPreUpdate));

            // Post-creation async work
            AddStep(Stages.PostOperation, Messages.Create, Modes.Asynchronous,
                AccountDefinition.EntityName, nameof(OnPostCreate));
        }

        // ── PreValidation ─────────────────────────────────────────────────────

        public void OnPreValidateCreate(IPluginContext context, IAccountService accountService)
        {
            var target = context.GetInputParameter<Entity>(InputParameters.Target);
            var name   = target.GetAttributeValue<string>(AccountDefinition.Columns.Name);

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidPluginExecutionException("Account name is required.");
            }
        }

        // ── PreOperation ──────────────────────────────────────────────────────

        [PreImage(AccountDefinition.Columns.Name, AccountDefinition.Columns.AccountNumber)]
        [FilteringAttributes(AccountDefinition.Columns.Name, AccountDefinition.Columns.AccountNumber)]
        [ExecutionOrder(10)]
        public void OnPreUpdate(IPluginContext context)
        {
            var target  = context.GetInputParameter<Entity>(InputParameters.Target);
            var preImage = context.GetPreImage(PreImageName);

            var oldName = preImage.GetAttributeValue<string>(AccountDefinition.Columns.Name);
            var newName = target.GetAttributeValue<string>(AccountDefinition.Columns.Name);

            if (oldName != newName)
            {
                context.Log("Account name changed from '{0}' to '{1}'", oldName, newName);
            }
        }

        // ── PostOperation ─────────────────────────────────────────────────────

        [PostImage(AccountDefinition.Columns.Name, AccountDefinition.Columns.Revenue)]
        public void OnPostCreate(IPluginContext context, IContactService contactService)
        {
            var postImage = context.GetPostImage(PostImageName);
            var name      = postImage.GetAttributeValue<string>(AccountDefinition.Columns.Name);

            context.Log("Account '{0}' created, running async follow-up.", name);

            // Call a service method — logged automatically by the framework
            contactService.CreateDefaultContact(context.PrimaryEntityId);
        }
    }
}
```
