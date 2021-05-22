# XrmFramework Utilities

- [XrmFramework Utilities](#xrmframework-utilities)
  - [Entity extensions](#entity-extensions)
    - [OptionSetValues](#optionsetvalues)
      - [Get Enum value from OptionSetValue](#get-enum-value-from-optionsetvalue)
      - [Set OptionSetValue from Enum](#set-optionsetvalue-from-enum)
    - [Attribute Values](#attribute-values)

## Entity extensions

### OptionSetValues

#### Get Enum value from OptionSetValue

GetOptionSetValue
```csharp

AccountType type = accountEntity.GetOptionSetValue<AccountType>(AccountDefinition.Columns.AccountType);

```

GetOptionSetValue with defaultValue
```csharp
AccountType type = accountEntity.GetOptionSetValue<AccountType>(AccountDefinition.Columns.AccountType, AccountType.Client);
```

#### Set OptionSetValue from Enum
```csharp
accountEntity.SetOptionSetValue<AccountType>(AccountDefinition.Columns.AccountType, AccountType.Corporate);
```

### Attribute Values

Get attribute value with default Value specified.

```csharp
string name = accountEntity.GetAttributeValue<string>(AccountDefinition.Columns.Name, "default value");
```

Get Attribute value from a main entity and looking to find the value in the preImage entity specified.

```csharp
string name = accountEntity.GetAttributeValue<string>(preImage, AccountDefinition.Columns.Name, "default value");
```

