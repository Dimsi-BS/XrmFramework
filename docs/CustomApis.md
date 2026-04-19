# XrmFramework — Custom APIs

- [XrmFramework — Custom APIs](#xrmframework--custom-apis)
  - [Overview](#overview)
  - [Anatomy of an XrmFramework Custom API](#anatomy-of-an-xrmframework-custom-api)
  - [Step 1 — Decorate the class with `CustomApiAttribute`](#step-1--decorate-the-class-with-customapiattribute)
    - [`CustomApiAttribute` properties](#customapiattribute-properties)
    - [`CustomApiBindingType` values](#customapibindingtype-values)
    - [`AllowedCustomProcessingStep` values](#allowedcustomprocessingstep-values)
  - [Step 2 — Inherit from `CustomApi` and point to the execution method](#step-2--inherit-from-customapi-and-point-to-the-execution-method)
  - [Step 3 — Declare input and output parameters](#step-3--declare-input-and-output-parameters)
    - [Supported types](#supported-types)
    - [`CustomApiInputAttribute` / `CustomApiOutputAttribute` properties](#customapiinputattribute--customapioutputattribute-properties)
  - [Step 4 — Implement the logic through `ICustomApiContext`](#step-4--implement-the-logic-through-icustomapicontext)
  - [Automatic registration in Dataverse](#automatic-registration-in-dataverse)
    - [What is created in Dataverse](#what-is-created-in-dataverse)
    - [Update behavior](#update-behavior)
    - [On-Premise specifics](#on-premise-specifics)
  - [Full examples](#full-examples)
    - [Global Custom API — `RecomputeAccountScore`](#global-custom-api--recomputeaccountscore)
    - [Entity-bound Custom API — `ApproveOpportunity`](#entity-bound-custom-api--approveopportunity)
  - [Best practices and common pitfalls](#best-practices-and-common-pitfalls)

## Overview

*Custom APIs* are the Dataverse mechanism that lets you expose a business operation as a message of the Dataverse Web API / Organization Service, on par with built-in messages (`Create`, `Update`, `qualifylead`, ...). XrmFramework manages their full lifecycle end-to-end: **you never create the `customapi`, `customapirequestparameter` and `customapiresponseproperty` records manually**; you write a decorated C# class, and the framework's deployment tool creates, updates or deletes them in the target environment.

At runtime, an XrmFramework Custom API is a plugin: it inherits from the same base class, runs inside the Dataverse sandbox, and benefits from the same facilities (service injection, rich context, extended traces, remote debugger).

## Anatomy of an XrmFramework Custom API

An XrmFramework Custom API is made of:

1. A **public class** inheriting from `XrmFramework.CustomApi` (itself derived from `XrmFramework.Plugin`).
2. A **`[CustomApi]` attribute** placed on the class, describing the API's metadata (binding, unique name, flags, privilege, ...).
3. **`CustomApiInArgument<T>` and `CustomApiOutArgument<T>` properties** decorated with `[CustomApiInput]` / `[CustomApiOutput]` that define the input and output parameters.
4. A **public method** — whose name is passed to the base constructor via `base(nameof(...))` — that carries the business logic. This method can receive, through injection, an `IPluginContext` (or `ICustomApiContext`) and any service declared in the project.

## Step 1 — Decorate the class with `CustomApiAttribute`

The `CustomApiAttribute` is **mandatory**: the `Plugin` base class throws `InvalidPluginExecutionException("... : No CustomApiAttribute found")` at runtime if it is missing, and the deployment tool refuses to publish the class.

```csharp
[CustomApi(CustomApiBindingType.Global,
    Name = "RecomputeAccountScore",
    DisplayName = "Recompute account score",
    Description = "Recompute the custom score of an account",
    IsFunction = false,
    IsPrivate = false,
    AllowedCustomProcessing = AllowedCustomProcessingStep.AsyncOnly,
    WorkflowSdkStepEnabled = true)]
public class RecomputeAccountScore : CustomApi
{
    ...
}
```

### `CustomApiAttribute` properties

| Property | Type | Description |
| --- | --- | --- |
| `BindingType` | `CustomApiBindingType` | **Mandatory constructor argument.** Defines whether the API is global, bound to a single entity, or bound to an entity collection. |
| `Name` | `string` | Logical name of the API. Defaults to the class name if not set. The `UniqueName` actually created in Dataverse is `{publisher.CustomizationPrefix}_{Name}`. |
| `DisplayName` | `string` | Display name of the API. Defaults to `Name` if not set. |
| `Description` | `string` | Description. Defaults to `Name` if not set. |
| `BoundEntityLogicalName` | `string` | Logical name of the entity (or collection) the API is bound to when `BindingType` ≠ `Global`. |
| `IsFunction` | `bool` | `true` to expose the API as an OData function (`GET` call, no side effect), `false` for an action (`POST` call). |
| `IsPrivate` | `bool` | `true` to mark the API as private (undocumented, not reachable from connectors). |
| `AllowedCustomProcessing` | `AllowedCustomProcessingStep` | Level of extensibility offered to custom steps on this API. |
| `ExecutePrivilegeName` | `string` | Security privilege required to execute the API. `null` means no specific privilege. |
| `WorkflowSdkStepEnabled` | `bool` | Exposes the API as a step usable in classic workflows / flows. *Ignored on On-Premise.* |

### `CustomApiBindingType` values

| Value | Meaning |
| --- | --- |
| `Global` | API not bound to any entity. Called directly (`POST /api/data/v9.x/<prefix>_<name>`). |
| `Entity` | API bound to a single entity. `BoundEntityLogicalName` must be set. |
| `EntityCollection` | API bound to an entity collection. `BoundEntityLogicalName` must be set. |

### `AllowedCustomProcessingStep` values

| Value | Meaning |
| --- | --- |
| `None` | No custom step allowed. |
| `AsyncOnly` | Only asynchronous steps can register on this API. |
| `SyncAndAsync` | Both synchronous and asynchronous steps are allowed. |

## Step 2 — Inherit from `CustomApi` and point to the execution method

The abstract base class `XrmFramework.CustomApi` expects **the name of the method that will implement the API** as its constructor argument:

```csharp
public class RecomputeAccountScore : CustomApi
{
    public RecomputeAccountScore() : base(nameof(MyMethod)) { }

    public void MyMethod(ICustomApiContext context, IAccountService accountService)
    {
        ...
    }
}
```

That name is read at runtime by `Plugin.SetCustomApiInfos`: a synthetic step is created in `PostOperation` / `Synchronous` and pointed at the named method. In other words, for a Custom API you **do not implement `AddSteps()`** — the `CustomApi` class already provides an empty implementation and builds the step from the method name passed to the constructor.

Constraints on the method (enforced at startup):

- It must be **public**, **non static**, and its name must exactly match the string passed to `base(...)`.
- Its parameters must be **interfaces**: `IPluginContext` or `ICustomApiContext` for the context, and any service interface declared in your project (same injection rules as for a standard plugin — see [WorkingWithServices.md](WorkingWithServices.md)).

## Step 3 — Declare input and output parameters

Each parameter is represented by a **property** of the class, typed as `CustomApiInArgument<T>` or `CustomApiOutArgument<T>`, decorated with `[CustomApiInput]` or `[CustomApiOutput]`. These properties are hydrated automatically by the base class constructor — you never instantiate them yourself.

```csharp
[CustomApiInput(Description = "Identifier of the account to recompute", IsOptional = false)]
public CustomApiInArgument<EntityReference> AccountRef { get; set; }

[CustomApiInput(IsOptional = true)]
public CustomApiInArgument<bool> ForceRefresh { get; set; }

[CustomApiOutput(Description = "New score computed for the account")]
public CustomApiOutArgument<decimal> NewScore { get; set; }
```

For each property, the framework reads the generic type `T` and infers the `CustomApiArgumentType` that will be persisted in Dataverse.

### Supported types

Mapping performed by `CustomApi` (in `XrmFramework.Plugin/CustomApi/CustomApi.cs`):

| C# type (`T`) | Dataverse `CustomApiArgumentType` |
| --- | --- |
| `bool` | `Boolean` |
| `DateTime` | `DateTime` |
| `decimal` | `Decimal` |
| `Entity` | `Entity` |
| `EntityCollection` | `EntityCollection` |
| `EntityReference` | `EntityReference` |
| `float` | `Float` |
| `int` | `Integer` |
| `Money` | `Money` |
| `OptionSetValue` / `enum` | `Picklist` |
| `string` | `String` |
| `string[]` | `StringArray` |
| `Guid` | `Guid` |
| *any other type* | `String` with automatic JSON serialization (Newtonsoft.Json) |

For a complex type (business DTO, POCO), the framework switches to "serialized string" mode: on the caller side, the parameter travels as JSON, and on the plugin side `GetArgumentValue<MyDto>` deserializes it automatically.

### `CustomApiInputAttribute` / `CustomApiOutputAttribute` properties

| Property | Type | Description |
| --- | --- | --- |
| `Name` | `string` | Logical name of the parameter. Defaults to the C# property name. |
| `DisplayName` | `string` | Display name. Defaults to `{CustomApiName}.{ParameterName}`. |
| `Description` | `string` | Description. Defaults to `{CustomApiName}.{ParameterName}`. |
| `LogicalEntityName` | `string` | Logical name of the targeted entity when the parameter is of type `Entity`, `EntityReference` or `EntityCollection`. |
| `IsOptional` | `bool` | Whether the parameter is optional. |

## Step 4 — Implement the logic through `ICustomApiContext`

At runtime, `LocalPluginContext` implements `ICustomApiContext`:

```csharp
public void Execute(ICustomApiContext context, IAccountService accountService)
{
    if (!context.HasArgument(AccountRef))
    {
        throw new InvalidPluginExecutionException("AccountRef is required");
    }

    var accountRef = context.GetArgumentValue(AccountRef);
    var force = context.GetArgumentValue(ForceRefresh);

    context.Log("Recomputing score for {0} (force={1})", accountRef.Id, force);

    var newScore = accountService.ComputeScore(accountRef, force);

    context.SetArgumentValue(NewScore, newScore);
}
```

Key points of the `ICustomApiContext` contract:

- `GetArgumentValue<T>(CustomApiInArgument<T>)` returns `default(T)` if the parameter was not provided (handy combined with `HasArgument`).
- For serialized parameters, JSON deserialization is transparent.
- `SetArgumentValue<T>(CustomApiOutArgument<T>, T)` writes the value into `OutputParameters`; for non-primitive types, the framework serializes it as JSON.
- `ObjectRef` returns an `EntityReference` built from `PrimaryEntityName` / `PrimaryEntityId` — useful for entity-bound APIs (`BindingType.Entity`).
- `Log(...)` writes to the plugin trace (same extended traces as XrmFramework plugins).
- `UserId` and `CorrelationId` are exposed directly.

## Automatic registration in Dataverse

Registration is handled by `XrmFramework.DeployUtils.RegistrationHelper`. In the deployment project template (`Utils\Deploy.<PluginProject>`), the `Program.cs` is a single line:

```csharp
using XrmFramework.DeployUtils;

RegistrationHelper.RegisterPluginsAndWorkflows<MyProject.Plugins.SomePluginOrCustomApi>("MyProject.Plugins", false, args);
```

On startup, `RegistrationHelper`:

1. Connects to the selected environment (`selectedConnection` in `xrmFramework.config`).
2. Loads the assembly of the generic type parameter and enumerates **every public, non-abstract type deriving from `XrmFramework.CustomApi`**.
3. Instantiates each Custom API (calling the `(string, string)` constructor when present, otherwise the parameterless one). This instantiation triggers the reading of `CustomApiAttribute` and the introspection of `CustomApiInArgument<T>` / `CustomApiOutArgument<T>`.
4. Converts each class into `Deploy.CustomApi` / `CustomApiRequestParameter` / `CustomApiResponseProperty` entities via `CustomApi.FromXrmFrameworkCustomApi(...)`.
5. Compares with existing records linked to the assembly and performs `Create` / `Update` / `Delete` to align Dataverse with the source code.
6. Adds each component to the target solution (`targetSolution` declared in `xrmFramework.config`).

### What is created in Dataverse

| Dataverse entity | Role | Code counterpart |
| --- | --- | --- |
| `customapi` | API metadata (unique name, binding, flags, privilege, `PluginTypeId`). | `CustomApi` class + `[CustomApi]`. |
| `customapirequestparameter` | Input parameters. `UniqueName` = argument name, `Name` = `{CustomApiName}.{argumentName}`. | `CustomApiInArgument<T>` properties + `[CustomApiInput]`. |
| `customapiresponseproperty` | Output properties (same naming rules). | `CustomApiOutArgument<T>` properties + `[CustomApiOutput]`. |
| `plugintype` | Plugin type executed by Dataverse when the API is called. | Created/updated from the .NET assembly. |

### Update behavior

When deploying again on an environment that already contains the assembly:

- Each Custom API is matched by `UniqueName` (`{customizationPrefix}_{Name}`). If it exists, it is updated; otherwise created.
- Parameters and properties are matched by `UniqueName` + `CustomApiId`. Obsolete entries (present in Dataverse but missing from the code) are **deleted** at the end of the cycle.
- If a Custom API class is removed from the code, its `customapi` record is deleted together with the associated `plugintype`.
- The assembly version is compared automatically: a `Major.Minor` bump triggers a full re-registration, not just an update.

### On-Premise specifics

The second argument of `RegisterPluginsAndWorkflows<TPlugin>(projectName, isOnPrem, args)` enables On-Premise mode. Two consequences for Custom APIs:

- `WorkflowSdkStepEnabled` is **not** pushed (the column does not exist in supported On-Premise Dataverse versions).
- The logical entity name of parameters is stored under the `entitylogicalname` column via the `EntityLogicalNameProperty` helper (see `CustomApiRequestParameter.partial.cs`), not under `LogicalEntityName` as in the cloud.

## Full examples

### Global Custom API — `RecomputeAccountScore`

```csharp
using XrmFramework;
using Microsoft.Xrm.Sdk;

[CustomApi(CustomApiBindingType.Global,
    Name = "RecomputeAccountScore",
    DisplayName = "Recompute account score",
    Description = "Recompute the custom score of an account",
    IsFunction = false,
    AllowedCustomProcessing = AllowedCustomProcessingStep.None)]
public class RecomputeAccountScore : CustomApi
{
    public RecomputeAccountScore() : base(nameof(MyMethod)) { }

    [CustomApiInput(Description = "Identifier of the account to recompute", LogicalEntityName = AccountDefinition.EntityName)]
    public CustomApiInArgument<EntityReference> AccountRef { get; set; }

    [CustomApiInput(IsOptional = true)]
    public CustomApiInArgument<bool> ForceRefresh { get; set; }

    [CustomApiOutput(Description = "New score computed for the account")]
    public CustomApiOutArgument<decimal> NewScore { get; set; }

    public void MyMethod(ICustomApiContext context, IAccountService accountService)
    {
        var accountRef = context.GetArgumentValue(AccountRef);
        var force = context.GetArgumentValue(ForceRefresh);

        var score = accountService.ComputeScore(accountRef, force);

        context.SetArgumentValue(NewScore, score);
    }
}
```

Web API call:

```
POST /api/data/v9.2/new_RecomputeAccountScore
Content-Type: application/json

{
  "AccountRef": { "@odata.id": "accounts(00000000-0000-0000-0000-000000000001)" },
  "ForceRefresh": true
}
```

### Entity-bound Custom API — `ApproveOpportunity`

```csharp
[CustomApi(CustomApiBindingType.Entity,
    Name = "ApproveOpportunity",
    BoundEntityLogicalName = OpportunityDefinition.EntityName,
    IsFunction = false,
    ExecutePrivilegeName = "prvApproveOpportunity")]
public class ApproveOpportunity : CustomApi
{
    public ApproveOpportunity() : base(nameof(MyMethod)) { }

    [CustomApiInput(IsOptional = true)]
    public CustomApiInArgument<string> Comment { get; set; }

    [CustomApiOutput]
    public CustomApiOutArgument<bool> Approved { get; set; }

    public void MyMethod(ICustomApiContext context, IOpportunityService opportunityService)
    {
        var opportunityRef = context.ObjectRef;
        var comment = context.GetArgumentValue(Comment);

        var ok = opportunityService.Approve(opportunityRef, comment);

        context.SetArgumentValue(Approved, ok);
    }
}
```

When the API is bound to an entity, the target record is identified through the resource URL (`POST /opportunities(<id>)/Microsoft.Dynamics.CRM.new_ApproveOpportunity`) and exposed in the plugin through `context.ObjectRef`.

## Best practices and common pitfalls

- **Always use `nameof(...)`** in the `base(...)` call to prevent a silent drift between the method name and the string passed to the base class.
- **Do not implement `AddSteps()`**: the `CustomApi` class already provides an empty implementation and builds the step from the method name. The resulting step is always `PostOperation` / `Synchronous`.
- **One `[CustomApiInput]` or `[CustomApiOutput]` per property**; any other property without a decoration is simply ignored by the introspection.
- **`BoundEntityLogicalName` is required** by Dataverse as soon as `BindingType` ≠ `Global`. Server-side deployment will fail if it is missing.
- **Private Custom APIs (`IsPrivate = true`)**: they are not listed by connectors and are not considered a public API; use them for internal processing.
- **Complex types in input/output**: possible (automatic JSON serialization) but hard to discover from the caller side. Prefer an explicit contract in primitive types for public APIs.
- **Always check for an optional argument** with `context.HasArgument(...)` before consuming its value to avoid relying on an ambiguous `default(T)` (e.g. `false` for a `bool`, `Guid.Empty` for a `Guid`).
- **Traces** emitted through `context.Log(...)` show up in the standard plugin trace and in the remote debugger (see [RemoteDebugger.md](RemoteDebugger.md)).
- **To replay/debug a Custom API locally**, the same mechanisms as for a standard plugin apply: `SendToRemoteDebugger` is evaluated in `Plugin.Execute` before the Custom API step is built.
