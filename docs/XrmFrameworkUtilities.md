# XrmFramework Utilities

- [XrmFramework Utilities](#xrmframework-utilities)
  - [Entity Extensions](#entity-extensions)
    - [GetAttributeValue — with default value](#getattributevalue--with-default-value)
    - [GetAttributeValue — from target + preImage](#getattributevalue--from-target--preimage)
    - [GetAliasedValue](#getaliasedvalue)
    - [OptionSet (single-select)](#optionset-single-select)
    - [OptionSet (multi-select)](#optionset-multi-select)
    - [Merge two Entities](#merge-two-entities)
    - [MergeWith](#mergewith)
    - [CopyField](#copyfield)
    - [EmptyIds](#emptyids)
  - [EntityReference Extensions](#entityreference-extensions)
    - [ToEntity](#toentity)
  - [QueryExpression Extensions](#queryexpression-extensions)
    - [GetRootFilterExpression](#getrootfilterexpression)
    - [GetConditionValue](#getconditionvalue)
  - [String Extensions](#string-extensions)
    - [ToGuid](#toguid)
    - [ToEntityReference](#toentityreference)
    - [ShouldNotBeNull / IsNull](#shouldnotbenull--isnull)
  - [Enum Extensions](#enum-extensions)
    - [ToInt](#toint)

---

## Entity Extensions

All methods below are extension methods on `Microsoft.Xrm.Sdk.Entity` and live in the `XrmFramework` namespace.

### GetAttributeValue — with default value

Returns the attribute value, or `defaultValue` if the attribute is absent from the entity:

```csharp
string name = accountEntity.GetAttributeValue<string>(AccountDefinition.Columns.Name, "default value");
```

### GetAttributeValue — from target + preImage

Returns the value from `newEntity` if present, falls back to `preEntity` if not, and finally falls back to the optional `defaultValue`. This pattern is ubiquitous in `Update` plugin methods:

```csharp
// Without default — returns default(T) if absent in both entities
string name = targetEntity.GetAttributeValue<string>(preImage, AccountDefinition.Columns.Name);

// With explicit default
string name = targetEntity.GetAttributeValue<string>(preImage, AccountDefinition.Columns.Name, "n/a");
```

### GetAliasedValue

Reads a value returned from a linked entity column (aliased column in a `QueryExpression` or FetchXml join):

```csharp
// Without default
string city = entity.GetAliasedValue<string>("contact1.address1_city");

// With default
string city = entity.GetAliasedValue<string>("contact1.address1_city", "Unknown");
```

---

### OptionSet (single-select)

#### Get enum value from OptionSetValue

```csharp
// Direct (no preImage)
AccountType type = accountEntity.GetOptionSetValue<AccountType>(AccountDefinition.Columns.AccountType);

// With default value
AccountType type = accountEntity.GetOptionSetValue<AccountType>(
    AccountDefinition.Columns.AccountType, AccountType.Client);

// With preImage fallback
AccountType type = targetEntity.GetOptionSetValue<AccountType>(
    preImage, AccountDefinition.Columns.AccountType);

// With preImage fallback and default value
AccountType type = targetEntity.GetOptionSetValue<AccountType>(
    preImage, AccountDefinition.Columns.AccountType, AccountType.Client);
```

#### Set OptionSetValue from enum

```csharp
accountEntity.SetOptionSetValue<AccountType>(AccountDefinition.Columns.AccountType, AccountType.Corporate);
```

When the enum value is named `Null` with integer value `0`, `SetOptionSetValue` writes `null` to the attribute instead of `new OptionSetValue(0)`.

---

### OptionSet (multi-select)

#### Get a collection of enum values from a multi-select OptionSet

```csharp
// No preImage, no default
ICollection<AccountCategory> cats = accountEntity.GetOptionSetValues<AccountCategory>(
    AccountDefinition.Columns.Categories);

// With preImage fallback
ICollection<AccountCategory> cats = targetEntity.GetOptionSetValues<AccountCategory>(
    preImage, AccountDefinition.Columns.Categories);

// With a single default value when the attribute is absent
ICollection<AccountCategory> cats = accountEntity.GetOptionSetValues<AccountCategory>(
    AccountDefinition.Columns.Categories, AccountCategory.Silver);
```

#### Set multi-select OptionSet values from an enum array or collection

```csharp
// From an array
accountEntity.SetOptionSetValues<AccountCategory>(
    AccountDefinition.Columns.Categories,
    AccountCategory.Gold, AccountCategory.Platinum);

// From a collection
ICollection<AccountCategory> selected = new[] { AccountCategory.Gold };
accountEntity.SetOptionSetValues<AccountCategory>(AccountDefinition.Columns.Categories, selected);
```

---

### Merge two Entities

Produces a **new** `Entity` that contains all attributes from both `sourceEntity` (takes precedence) and `preImage`. Useful when you need the full record state during an `Update` step without issuing an additional `Retrieve`:

```csharp
// Returns a new Entity containing all fields from target + preImage
// Fields present in target overwrite those from preImage
Entity fullRecord = targetEntity.Merge(preImage);
```

---

### MergeWith

Copies all attributes from `sourceEntity` into `targetEntity` in-place. Optionally skips attributes already present in `targetEntity`:

```csharp
// Copy all fields from source into target (overwrites existing)
targetEntity.MergeWith(sourceEntity);

// Copy only fields not already present in target
targetEntity.MergeWith(sourceEntity, copyOnlyIfFieldNotExist: true);
```

---

### CopyField

Copies a single attribute from one entity to another, with optional rename and change detection:

```csharp
// Simple copy — same column name
sourceEntity.CopyField(targetEntity, AccountDefinition.Columns.Name, AccountDefinition.Columns.Name);

// Copy with rename
sourceEntity.CopyField(targetEntity, "name", "new_legacyname");

// With preImage — skips copy if the value did not change between source and preImage
sourceEntity.CopyField(preImage, targetEntity, AccountDefinition.Columns.Name, AccountDefinition.Columns.Name);

// With useDefaultValue: true — writes null to target when source does not contain the field
sourceEntity.CopyField(targetEntity, AccountDefinition.Columns.Name, AccountDefinition.Columns.Name, useDefaultValue: true);
```

---

### EmptyIds

Clears the `Id` property and optionally removes specified attribute columns from every entity in an `EntityCollection`. Handy when reusing a query result as a template for bulk creation:

```csharp
EntityCollection templates = service.RetrieveMultiple(query);
templates.EmptyIds("new_sourceid", "createdon");
```

---

## EntityReference Extensions

Extension methods on `Microsoft.Xrm.Sdk.EntityReference`.

### ToEntity

Converts an `EntityReference` to a minimal `Entity` (same `LogicalName` and `Id`, plus any `KeyAttributes`). Useful when you need to build an `Update` payload starting from a reference:

```csharp
EntityReference accountRef = new EntityReference("account", accountId);
Entity update = accountRef.ToEntity();
update["name"] = "New name";
service.Update(update);
```

Returns `null` if the input reference is `null`.

---

## QueryExpression Extensions

Extension methods on `Microsoft.Xrm.Sdk.Query.QueryExpression` and `FilterExpression`.

### GetRootFilterExpression

Returns the first concrete `FilterExpression` that contains conditions, skipping any intermediate grouping filters. Useful when you need to add conditions to the "real" criteria of a query built by another layer:

```csharp
QueryExpression query = BuildQuery();
FilterExpression rootFilter = query.GetRootFilterExpression();
rootFilter.AddCondition("statecode", ConditionOperator.Equal, 0);
```

You can also call it directly on a `FilterExpression`:

```csharp
FilterExpression root = QueryExpressionExtensions.GetRootFilterExpression(criteria);
```

### GetConditionValue

Reads the first value of a named condition from a `FilterExpression` tree, searching recursively through nested filters:

```csharp
FilterExpression criteria = query.Criteria;
Guid ownerId = criteria.GetConditionValue<Guid>("ownerid");
```

Returns `default(T)` if no condition for the attribute is found.

---

## String Extensions

Extension methods on `System.String`.

### ToGuid

Parses a string as a `Guid`. Throws `FormatException` if the string is not a valid GUID:

```csharp
Guid id = "00000000-0000-0000-0000-000000000001".ToGuid();
```

### ToEntityReference

Constructs an `EntityReference` from a string that is either a GUID or an alternate key value:

```csharp
// From a GUID string
EntityReference ref1 = "00000000-0000-0000-0000-000000000001"
    .ToEntityReference("account");

// From an alternate key value
EntityReference ref2 = "ACC-0042"
    .ToEntityReference("account", keyAttributeName: "new_accountcode");
```

Returns `null` if the string is `null` or empty.

### ShouldNotBeNull / IsNull

Guard helpers used mainly inside service implementations:

```csharp
// Throws ArgumentNullException (with the calling member name) if value is null or empty
value.ShouldNotBeNull();

// Returns true if value is null or empty
if (value.IsNull()) { /* ... */ }
```

---

## Enum Extensions

### ToInt

Converts an enum value to its underlying `int`. Syntactic shorthand used throughout the framework:

```csharp
int stateCode = DebugSessionState.Active.ToInt();

// Equivalent to
int stateCode = (int)DebugSessionState.Active;
```

Particularly useful when building `QueryExpression` conditions:

```csharp
query.Criteria.AddCondition(
    DebugSessionDefinition.Columns.StateCode,
    ConditionOperator.Equal,
    DebugSessionState.Active.ToInt());
```
