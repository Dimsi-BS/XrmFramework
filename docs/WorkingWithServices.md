# Working with XrmFramework Services

- [Working with XrmFramework Services](#working-with-xrmframework-services)
  - [Design considerations](#design-considerations)
  - [Create a new Service](#create-a-new-service)
    - [Interface definition](#interface-definition)
    - [Implementation](#implementation)


## Design considerations

In large Dynamics 365 / Dataverse projects, the mutualisation and testing of code is primordial.

In the XrmFramework paradigm, the center part of the data access is the Services classes.

A XrmFramework service must implement ``XrmFramework.IService`` interface.

## Create a new Service

### Interface definition

```csharp
using XrmFramework;

namespace Contoso.Core.Services
{
  public interface IAccountService : IService
  {
    string GetAccountNumber(EntityReference accountRef);
  }
}

```

### Implementation

```csharp
using XrmFramework;

namespace Contoso.Core.Services
{
  public class AccountService : DefaultService, IAccountService
  {

    #region .ctor

    public AccountService(IServiceContext context) : base(context)
    {
    }

    #endregion

    string GetAccountNumber(EntityReference accountRef)
    {
      var account = AdminOrganizationService.Retrieve(accountRef, new ColumnSet(AccountDefinition.Columns.AccountNumber));

      return account.GetAttributeValue<string>(AccountDefinition.Columns.AccountNumber);
    }
  }
}

```
