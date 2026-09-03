# Working with XrmFramework Services

- [Working with XrmFramework Services](#working-with-xrmframework-services)
  - [Design Considerations](#design-considerations)
  - [Create a New Service](#create-a-new-service)
    - [Interface Definition](#interface-definition)
    - [Implementation](#implementation)
  - [Inject Services into Plugins](#inject-services-into-plugins)
    - [Single Service](#single-service)
    - [Multiple Services](#multiple-services)
  - [Inject Services into Custom APIs](#inject-services-into-custom-apis)
  - [Using the Base Service Members](#using-the-base-service-members)
    - [OrganizationService vs AdminOrganizationService](#organizationservice-vs-adminorganizationservice)
    - [Logging](#logging)
    - [Impersonation Overloads](#impersonation-overloads)
  - [Automatic Logging Wrapper](#automatic-logging-wrapper)
  - [Registering Services](#registering-services)
  - [Testing Services](#testing-services)
  - [See Also](#see-also)

---

## Design Considerations

In large Dynamics 365 / Dataverse projects, code reuse and testability are essential. The XrmFramework service layer is the answer to both: all data-access logic lives in typed service classes that are injected into plugins, Custom APIs, or any other consumer — never written inline.

A XrmFramework service **must implement `XrmFramework.IService`**. This marker interface signals to the framework's dependency-injection container that the type can be resolved and injected. Direct use of `IOrganizationService.Execute` is intentionally kept out of the public API surface; see [IService Architecture](IService-Architecture.md) for the reasoning.

---

## Create a New Service

### Interface Definition

Define a new interface that extends `IService`. Name methods after business operations, not platform primitives:

```csharp
using XrmFramework;
using Microsoft.Xrm.Sdk;
using System;

namespace Contoso.Core.Services
{
    public interface IAccountService : IService
    {
        /// <summary>Returns the canonical account number for the given account.</summary>
        string GetAccountNumber(EntityReference accountRef);

        /// <summary>Marks the account as a premium partner.</summary>
        void PromoteToPremiumPartner(EntityReference accountRef);

        /// <summary>Creates a default contact linked to the account.</summary>
        void CreateDefaultContact(Guid accountId);
    }
}
```

### Implementation

The implementation extends `DefaultService` (which already satisfies `IService`) and provides the query and business logic. All Dataverse calls stay here — invisible to plugin code:

```csharp
using XrmFramework;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Contoso.Core.Services
{
    public class AccountService : DefaultService, IAccountService
    {
        public AccountService(IServiceContext context) : base(context) { }

        public string GetAccountNumber(EntityReference accountRef)
        {
            // The query is written once here — never duplicated in plugins.
            var account = AdminOrganizationService.Retrieve(
                accountRef,
                new ColumnSet(AccountDefinition.Columns.AccountNumber));

            return account.GetAttributeValue<string>(AccountDefinition.Columns.AccountNumber);
        }

        public void PromoteToPremiumPartner(EntityReference accountRef)
        {
            var update = new Entity(AccountDefinition.EntityName, accountRef.Id);
            update[AccountDefinition.Columns.IsPremium] = true;
            Update(update);  // IService.Update — no raw Execute needed
        }

        public void CreateDefaultContact(Guid accountId)
        {
            var contact = new Entity("contact");
            contact["parentcustomerid"] = new EntityReference(AccountDefinition.EntityName, accountId);
            contact["lastname"] = "Default";
            Create(contact);
        }
    }
}
```

> **Note:** The `public` access modifier is required on each method that satisfies the interface contract. Without it the method silently fails to implement the interface.

---

## Inject Services into Plugins

The framework automatically resolves any service interface declared as a method parameter. No factory, no `new()`, no service locator.

### Single Service

```csharp
public void OnPreUpdate(IPluginContext context, IAccountService accountService)
{
    var number = accountService.GetAccountNumber(
        new EntityReference(AccountDefinition.EntityName, context.PrimaryEntityId));

    context.Log("Account number: {0}", number);
}
```

### Multiple Services

Services can be declared in any order, after `IPluginContext`:

```csharp
public void OnPostCreate(
    IPluginContext context,
    IAccountService accountService,
    IContactService contactService)
{
    accountService.PromoteToPremiumPartner(
        new EntityReference(AccountDefinition.EntityName, context.PrimaryEntityId));

    contactService.SendWelcomeEmail(context.InitiatingUserId);
}
```

The framework validates all parameters at plugin construction time, so a misconfigured service type (e.g. a missing implementation) fails fast at startup rather than at runtime.

---

## Inject Services into Custom APIs

Service injection works identically for Custom API execution methods:

```csharp
public class RecomputeAccountScore : CustomApi
{
    public RecomputeAccountScore() : base(nameof(Execute)) { }

    [CustomApiInput(IsOptional = false)]
    public CustomApiInArgument<EntityReference> AccountRef { get; set; }

    [CustomApiOutput]
    public CustomApiOutArgument<decimal> NewScore { get; set; }

    public void Execute(ICustomApiContext context, IAccountService accountService)
    {
        var accountRef = context.GetArgumentValue(AccountRef);
        var score = accountService.ComputeScore(accountRef, force: false);
        context.SetArgumentValue(NewScore, score);
    }
}
```

---

## Using the Base Service Members

`DefaultService` exposes several members that your implementation can use directly.

### OrganizationService vs AdminOrganizationService

| Member | Acts as |
|--------|---------|
| `OrganizationService` | The triggering user (respects security roles) |
| `AdminOrganizationService` | The system administrator (bypasses security) |

Use `AdminOrganizationService` when you need to read data that the triggering user might not have direct access to (e.g. audit tables, configuration records). Use `OrganizationService` when you want to enforce row-level security on the result.

### Logging

`Log` is a `LogServiceMethod` delegate — it takes the **calling method's name first**, then the message and its format arguments:

```csharp
public string GetAccountNumber(EntityReference accountRef)
{
    Log(nameof(GetAccountNumber), "called for {0}", accountRef.Id);

    var account = AdminOrganizationService.Retrieve(
        accountRef, new ColumnSet(AccountDefinition.Columns.AccountNumber));

    var number = account.GetAttributeValue<string>(AccountDefinition.Columns.AccountNumber);
    Log(nameof(GetAccountNumber), "result: {0}", number);
    return number;
}
```

That first argument is what lets the trace attribute each line to a method without the service having to prefix its own messages.

`Log` writes to the Dataverse plugin trace. When using the auto-generated `LoggedService` wrapper, every method call is already timed and logged automatically — you only need explicit `Log` calls for additional internal detail.

### Impersonation Overloads

Every `IService` CRUD method accepts optional impersonation flags:

```csharp
// Write as the system administrator
Create(entity, useAdmin: true);

// Write as a specific user
Update(entity, callerId: ownerUserId);

// Skip downstream plugin execution for this write
Delete(entityRef, bypassCustomPluginExecution: true);
```

---

## Automatic Logging Wrapper

The framework source generator emits a `LoggedI<ServiceName>` class for every service interface. This wrapper surrounds every method call with timing and structured trace output — at zero cost to the developer:

```csharp
// Auto-generated — never written by hand
public class LoggedIAccountService : LoggedServiceBase, IAccountService
{
    public string GetAccountNumber(EntityReference accountRef)
    {
        var sw = Stopwatch.StartNew();
        Log(nameof(GetAccountNumber), "Start: accountRef={0}", accountRef);
        var result = Service.GetAccountNumber(accountRef);
        Log(nameof(GetAccountNumber), "End: duration={0} result={1}", sw.Elapsed, result);
        return result;
    }
    // ...
}
```

When a plugin requests `IAccountService`, the DI container returns a `LoggedIAccountService` that wraps the real `AccountService`. The plugin code never sees the difference.

---

## Registering Services

Services are registered in the project's `InternalDependencyProvider`. One line wires up the implementation and the logging wrapper:

```csharp
// In your project's InternalDependencyProvider
RegisterService<IAccountService, AccountService, LoggedIAccountService>(container);
```

The source generator creates both `LoggedIAccountService` and the `InternalDependencyProvider` registration automatically when it detects a class that inherits from `DefaultService`.

---

## Testing Services

Because service classes receive an `IServiceContext` through their constructor, you can supply a test double at unit-test time without touching the plugin pipeline:

`IServiceContext` is an interface, so any mocking library will do — this is the pattern the framework uses for its own tests:

```csharp
var orgService = new Mock<IOrganizationService>();
var adminOrgService = new Mock<IOrganizationService>();

var context = new Mock<IServiceContext>();
context.Setup(c => c.OrganizationService).Returns(orgService.Object);
context.Setup(c => c.AdminOrganizationService).Returns(adminOrgService.Object);
context.Setup(c => c.LogServiceMethod).Returns((string _, string __, object[] ___) => { });

var account = new Entity("account", Guid.NewGuid());
account["accountnumber"] = "ACC-001";

adminOrgService
    .Setup(s => s.Retrieve("account", account.Id, It.IsAny<ColumnSet>()))
    .Returns(account);

var service = new AccountService(context.Object);

var result = service.GetAccountNumber(account.ToEntityReference());

Assert.AreEqual("ACC-001", result);
```

Setting up `LogServiceMethod` matters: `DefaultService.Log` goes through it, so a service that
traces would otherwise fail on a null delegate rather than on the behaviour under test.

For regression testing, use the **Remote Debugger session replay** mechanism to run a service method against a recorded real context — see [RemoteDebugger](RemoteDebugger.md#session-recording-and-replay).

---

## See Also

- [IService Architecture](IService-Architecture.md) — Deep dive into the design rationale and the full `IService` API surface.
- [Plugins](Plugins.md) — How to declare step methods and use service injection in plugins.
- [Custom APIs](CustomApis.md) — Service injection in Custom API execution methods.
- [Remote Debugger](RemoteDebugger.md) — Debugging service calls live against a real Dataverse environment.
