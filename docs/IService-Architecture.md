# IService Architecture

- [IService Architecture](#iservice-architecture)
  - [1. Overview](#1-overview)
  - [2. Why IService Does Not Expose `Execute`](#2-why-iservice-does-not-expose-execute)
    - [2.1 The Problem with Execute in Plugin Code](#21-the-problem-with-execute-in-plugin-code)
    - [2.2 Where Execute Lives](#22-where-execute-lives)
  - [3. The IService Interface](#3-the-iservice-interface)
    - [3.1 Core CRUD Operations](#31-core-crud-operations)
    - [3.2 Typed Model Operations](#32-typed-model-operations)
    - [3.3 Relationship & Sharing Operations](#33-relationship--sharing-operations)
    - [3.4 Team & Role Operations](#34-team--role-operations)
  - [4. Creating Domain-Specific Services](#4-creating-domain-specific-services)
    - [4.1 Define the Interface](#41-define-the-interface)
    - [4.2 Implement the Service](#42-implement-the-service)
    - [4.3 Impersonation Overloads](#43-impersonation-overloads)
  - [5. Using Services in Plugins](#5-using-services-in-plugins)
    - [5.1 Basic Injection](#51-basic-injection)
    - [5.2 Multiple Services](#52-multiple-services)
  - [6. Service Infrastructure](#6-service-infrastructure)
    - [6.1 DefaultService](#61-defaultservice)
    - [6.2 Logged Service Wrappers](#62-logged-service-wrappers)
    - [6.3 Dependency Injection Registration](#63-dependency-injection-registration)
    - [6.4 Full Inheritance Chain](#64-full-inheritance-chain)
  - [7. IOrganizationService vs IService — Comparison](#7-iorganizationservice-vs-iservice--comparison)
  - [8. Summary](#8-summary)

---

## 1. Overview

In Dynamics 365 / Dataverse development, plugins run in a sandbox and receive a raw `IOrganizationService` instance from Microsoft. This interface is powerful but low-level: it exposes a generic `Execute(OrganizationRequest)` method that accepts any request object and returns a generic response. While flexible, this API surface makes plugin code verbose, hard to read, and difficult to maintain at scale.

XrmFramework replaces direct `IOrganizationService` usage inside plugins with a richer, project-specific abstraction called **`IService`**. The goal is simple:

> **Plugin handlers should read like business requirements, not like database queries.**

---

## 2. Why IService Does Not Expose `Execute`

The most visible design choice of `IService` is the deliberate omission of the `Execute(OrganizationRequest)` method that sits at the heart of `IOrganizationService`. This section explains why.

### 2.1 The Problem with Execute in Plugin Code

Consider a plugin that must enforce a business rule: when an Account is updated, retrieve its account number.

**Without IService — raw `IOrganizationService`:**

```csharp
public void Execute(IServiceProvider serviceProvider)
{
    var context    = (IPluginExecutionContext)serviceProvider
                       .GetService(typeof(IPluginExecutionContext));
    var factory    = (IOrganizationServiceFactory)serviceProvider
                       .GetService(typeof(IOrganizationServiceFactory));
    var orgService = factory.CreateOrganizationService(context.UserId);

    // The query is written inline, inside the plugin
    var account = orgService.Retrieve(
        "account",
        context.PrimaryEntityId,
        new ColumnSet("accountnumber")
    );

    var number = account.GetAttributeValue<string>("accountnumber");

    // A reader must parse all of the above to understand what this does
}
```

**With IService — named service method:**

```csharp
public void OnPostUpdate(IPluginContext context, IAccountService accountService)
{
    var number = accountService.GetAccountNumber(
        new EntityReference(AccountDefinition.EntityName, context.PrimaryEntityId)
    );

    context.Log("Account number: {0}", number);
}
```

The second version is self-documenting. A developer reading `GetAccountNumber(...)` immediately understands the intent without tracing through query-building code. The query logic lives once, inside the service implementation — tested once and reused by every plugin that needs it.

### 2.2 Where Execute Lives

`Execute` is not gone — it is encapsulated. `DefaultService`, the base class for all concrete service implementations, holds a private bridge method:

```csharp
// Inside DefaultService — private, not part of any public interface
private TResponse Execute<TRequest, TResponse>(
    IOrganizationService service,
    TRequest request,
    bool bypassCustomPluginExecution)
    where TRequest : OrganizationRequest
    where TResponse : OrganizationResponse
{
    if (bypassCustomPluginExecution)
    {
        request["BypassCustomPluginExecution"] = true;
    }
    return (TResponse)service.Execute(request);
}
```

This private method is the **only place** in the framework where `IOrganizationService.Execute` is called directly. Everything above it exposes clean, named operations.

---

## 3. The IService Interface

`IService` is declared as a `partial interface` in XrmFramework, allowing generated code to contribute additional members. It intentionally exposes **no** `Execute` method.

### 3.1 Core CRUD Operations

```csharp
Guid           Create(Entity entity, bool useAdmin = false, bool bypassCustomPluginExecution = false);
UpsertResponse Upsert(Entity entity, bool useAdmin = false, bool bypassCustomPluginExecution = false);
void           Update(Entity entity, bool useAdmin = false, bool bypassCustomPluginExecution = false);
void           Delete(string logicalName, Guid id, bool useAdmin = false, bool bypassCustomPluginExecution = false);
void           Delete(EntityReference objectReference, bool useAdmin = false, bool bypassCustomPluginExecution = false);
Entity         Retrieve(string entityName, Guid id, params string[] columns);
Entity         Retrieve(EntityReference objectRef, params string[] columns);
```

### 3.2 Typed Model Operations

```csharp
T      GetById<T>(Guid id)                   where T : IBindingModel, new();
T      GetById<T>(EntityReference entityRef) where T : IBindingModel, new();
T      Upsert<T>(T model, bool isAdmin = false, bool bypassCustomPluginExecution = false)
                                             where T : IBindingModel, new();
Entity ToEntity<T>(T model)                  where T : IBindingModel;
```

### 3.3 Relationship & Sharing Operations

```csharp
void AssociateRecords(EntityReference objectRef, Relationship relationName,
                      params EntityReference[] entityReferences);
void AssignEntity(EntityReference objectReference, EntityReference ownerRef, ...);
void Share(EntityReference objectRef, EntityReference assignee, AccessRights accessRights, ...);
void UnShare(EntityReference objectRef, EntityReference revokee, ...);
void Merge(EntityReference target, Guid subordinate, Entity content, ...);
```

### 3.4 Team & Role Operations

```csharp
void               AddUsersToTeam(EntityReference teamRef, params EntityReference[] userRefs);
void               RemoveUsersFromTeam(EntityReference teamRef, params EntityReference[] userRefs);
bool               UserHasRole(Guid userId, Guid parentRoleId);
bool               UserHasOneRoleOf(Guid userId, params Guid[] parentRoleIds);
ICollection<Guid>  GetUserRoleIds(EntityReference userRef);
void               AddRoleToUserOrTeam(EntityReference userOrTeamRef, string parentRootRoleId, ...);
```

None of these methods require the caller to build an `OrganizationRequest` object or invoke `Execute`. Every operation is expressed as a named, typed action.

---

## 4. Creating Domain-Specific Services

The real power of `IService` comes from extending it with project-specific interfaces that name operations after business concepts rather than platform primitives.

### 4.1 Define the Interface

Create a new interface that extends `IService`. Method names should communicate business intent:

```csharp
using XrmFramework;

namespace Contoso.Core.Services
{
    public interface IAccountService : IService
    {
        /// <summary>
        /// Computes and returns the canonical account number for the given account.
        /// </summary>
        string GetAccountNumber(EntityReference accountRef);

        /// <summary>
        /// Marks the account as a premium partner and notifies the owner.
        /// </summary>
        void PromoteToPremiumPartner(EntityReference accountRef);
    }
}
```

### 4.2 Implement the Service

The implementation class extends `DefaultService` (which already implements `IService`) and provides the business logic. Queries and platform calls stay here — invisible to plugin code:

```csharp
using XrmFramework;

namespace Contoso.Core.Services
{
    public class AccountService : DefaultService, IAccountService
    {
        public AccountService(IServiceContext context) : base(context) { }

        public string GetAccountNumber(EntityReference accountRef)
        {
            // The query lives here — not in every plugin that needs the number
            var account = AdminOrganizationService.Retrieve(
                accountRef,
                new ColumnSet(AccountDefinition.Columns.AccountNumber)
            );
            return account.GetAttributeValue<string>(AccountDefinition.Columns.AccountNumber);
        }

        public void PromoteToPremiumPartner(EntityReference accountRef)
        {
            var update = new Entity(AccountDefinition.EntityName, accountRef.Id);
            update[AccountDefinition.Columns.IsPremium] = true;
            Update(update);   // calls IService.Update — no raw Execute needed

            // Notify the owner ...
        }
    }
}
```

### 4.3 Impersonation Overloads

Every `IService` method accepts either a `bool useAdmin` flag or a `Guid callerId`. Services therefore handle impersonation transparently — plugin code never needs to instantiate a separate service factory:

```csharp
// Operate as the system administrator
Create(entity, useAdmin: true);

// Operate as a specific user
Update(entity, callerId: ownerUserId);

// Bypass downstream plugins for this write
Delete(entityRef, bypassCustomPluginExecution: true);
```

---

## 5. Using Services in Plugins

XrmFramework plugins declare their service dependencies as method parameters. The framework resolves and injects the correct implementation automatically at runtime.

### 5.1 Basic Injection

```csharp
// The framework sees IAccountService and injects AccountService automatically.
// No new(), no factory calls, no service locator.
public void OnPreUpdate(IPluginContext context, IAccountService accountService)
{
    var number = accountService.GetAccountNumber(
        new EntityReference(AccountDefinition.EntityName, context.PrimaryEntityId)
    );

    context.Log("Computed account number: {0}", number);
}
```

### 5.2 Multiple Services

Multiple services can be declared in any order. The framework validates all parameters at plugin construction time:

```csharp
public void OnPostCreate(
    IPluginContext context,
    IAccountService accountService,
    IContactService contactService)
{
    accountService.PromoteToPremiumPartner(
        new EntityReference(AccountDefinition.EntityName, context.PrimaryEntityId)
    );

    contactService.SendWelcomeEmail(context.InitiatingUserId);
}
```

The handler reads as a sequence of business actions. There is no mention of `Execute`, no `OrganizationRequest`, no manual `ColumnSet` assembly. All of that complexity is encapsulated in the service layer, tested once, and reused everywhere.

---

## 6. Service Infrastructure

### 6.1 DefaultService

`DefaultService` is the ready-to-use base class for all service implementations. It receives an `IServiceContext` in its constructor and exposes:

| Member | Description |
|--------|-------------|
| `OrganizationService` | Acts as the triggering user |
| `AdminOrganizationService` | Acts as the system administrator |
| `Log(message, args)` | Writes to the Dataverse trace log |
| `BusinessUnitRef`, `UserId`, `CorrelationId` | Execution context metadata |

### 6.2 Logged Service Wrappers

The framework automatically generates a `LoggedI<ServiceName>` wrapper for every service interface. This wrapper implements the same interface and delegates every call to the real implementation, surrounding it with timing and structured log output — at zero cost to the developer:

```csharp
// Auto-generated — never written by hand
public class LoggedIAccountService : LoggedServiceBase, IAccountService
{
    public string GetAccountNumber(EntityReference accountRef)
    {
        var sw = Stopwatch.StartNew();
        Log(nameof(GetAccountNumber), "Start: accountRef = {0}", accountRef);
        var result = Service.GetAccountNumber(accountRef);
        Log(nameof(GetAccountNumber), "End: duration = {0}, result = {1}",
                                      sw.Elapsed, result);
        return result;
    }
}
```

### 6.3 Dependency Injection Registration

Services are registered through `InternalDependencyProvider`. One call wires up the full implementation and logging chain:

```csharp
RegisterService<IAccountService, AccountService, LoggedIAccountService>(container);
```

When a plugin requests `IAccountService`, the container instantiates `AccountService`, wraps it in `LoggedIAccountService`, and returns it.

### 6.4 Full Inheritance Chain

| Layer | Type / Role |
|-------|-------------|
| Interface | `IService` — marker interface for all services |
| Domain interface | `IAccountService : IService` — exposes business operations |
| Base implementation | `DefaultService : IService` — CRUD helpers, private `Execute` bridge |
| Domain implementation | `AccountService : DefaultService, IAccountService` — query logic & business rules |
| Logged wrapper | `LoggedIAccountService : IAccountService` — auto-generated timing & tracing |

---

## 7. IOrganizationService vs IService — Comparison

| Aspect | `IOrganizationService` | `IService` |
|--------|------------------------|------------|
| Provided by | Microsoft SDK | XrmFramework |
| Core API style | `Execute(OrganizationRequest)` — generic | Named, strongly-typed methods |
| CRUD | Primitive `Create` / `Retrieve` / `Update` / `Delete` | Same primitives, plus typed overloads (`IBindingModel`) |
| Query execution | Direct — caller builds FetchXml / QueryExpression | Hidden — queries stay inside service implementations |
| Plugin readability | Low — requires reading request-assembly code | High — method name expresses business intent |
| Code reuse | None — every plugin duplicates query code | Full — one service method, many callers |
| Logging / tracing | Manual | Automatic via generated `LoggedService` wrapper |
| Accessible in plugins | Via `IServiceContext` (internal use) | Via dependency injection (preferred) |

---

## 8. Summary

The `IService` abstraction is not about hiding the Dataverse platform — it is about putting knowledge in the right place. Queries, request assembly, and response parsing belong in services. Plugin handlers belong to business logic expressed in plain, readable method calls.

**Key takeaways:**

- **`Execute` is absent by design.** Plugin code must not build `OrganizationRequest` objects. That responsibility belongs to service implementations.
- **Method names are contracts.** Every `IService` method name communicates a business action, making plugin handlers self-documenting.
- **Queries are centralised.** A FetchXml or `QueryExpression` written once in a service is tested once and reused by any plugin, workflow, or custom API that needs the same data.
- **Logging is free.** The framework generates logged wrappers automatically; no boilerplate is required in service implementations.
- **Dependency injection is enforced.** Services arrive in plugin handlers via parameter injection; no manual instantiation is ever needed.

---

*See also: [Working with Services](WorkingWithServices.md) · [Plugins](Plugins.md)*
