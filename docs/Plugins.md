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
  - [Working with Images](#working-with-images)
  - [Plugin Configuration](#plugin-configuration)
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
