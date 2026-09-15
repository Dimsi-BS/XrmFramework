# XrmFramework TypeScript

`XrmFramework.TypeScript` is the client-side counterpart of the framework: it turns a `Webresources`
project into a typed TypeScript codebase for Dataverse form and ribbon scripting, built with webpack
and kept in sync with the same `.table` definitions used on the server side (see
[Generate model definitions](../README.md#generate-model-definitions)).

- [XrmFramework TypeScript](#xrmframework-typescript)
  - [Project layout \& build pipeline](#project-layout--build-pipeline)
  - [Declaring a form script](#declaring-a-form-script)
  - [Declaring a ribbon script](#declaring-a-ribbon-script)
  - [Wiring scripts in Dataverse](#wiring-scripts-in-dataverse)
  - [Type-safe table definitions](#type-safe-table-definitions)
  - [The Utils API (`UtilsApi<TableDefinition>`)](#the-utils-api-utilsapitabledefinition)
    - [Reading and writing values](#reading-and-writing-values)
    - [Visibility, requirement \& focus](#visibility-requirement--focus)
    - [Disabling controls](#disabling-controls)
    - [Tabs \& sections](#tabs--sections)
    - [Controls \& attributes](#controls--attributes)
    - [Change \& save events](#change--save-events)
    - [Saving \& refreshing](#saving--refreshing)
    - [Notifications \& dialogs](#notifications--dialogs)
    - [Business process flow](#business-process-flow)
    - [Record info \& navigation](#record-info--navigation)
    - [Filtering lookups](#filtering-lookups)
    - [Misc helpers](#misc-helpers)
  - [Typed Web API](#typed-web-api)

---

## Project layout & build pipeline

A scaffolded `Webresources` project (`xrmframework new solution` generates one, see the
[CLI documentation](../src/XrmFramework.Cli/README.md)) looks like this:

```
Webresources/
├── Webresources.csproj      # references the XrmFramework.TypeScript NuGet package
├── package.json             # webpack, webpack-cli, ts-loader, copy-webpack-plugin
├── tsconfig.json
├── webpack.config.js
├── scripts/
│   ├── Forms/
│   │   └── AccountForm.ts
│   └── Ribbons/
│       └── AccountRibbon.ts
├── xrmFramework/            # generated + copied at build time — do not edit, add to .gitignore
│   ├── Utils.ts
│   ├── formScript.ts
│   ├── registerScript.ts
│   ├── tables.d.ts
│   ├── enums.ts
│   └── definitions/
│       └── AccountDefinition.d.ts
└── dist/                    # webpack output, deployed as web resources
    └── scripts/
        ├── Forms/AccountForm.js
        └── Ribbons/AccountRibbon.js
```

`Webresources.csproj` only needs a `PackageReference` to `XrmFramework.TypeScript`. That package
carries an MSBuild `.targets` file that runs, in order, on every build:

1. **`CopyXrmFrameworkTypeScriptFiles`** — copies the framework's runtime `.ts`/`.d.ts` files
   (`Utils.ts`, `formScript.ts`, `registerScript.ts`, `xrm.d.ts`, `table.d.ts`, `utilityTypes.d.ts`,
   `index.d.ts`, `xrmFramework.d.ts`) into `xrmFramework/`. Files are always overwritten to stay in
   sync with the installed package version.
2. **`TransformFiles`** — reads every `.table` file from the sibling `.Core` project's
   `Definitions/` folder (the same files the `XrmFramework.Analyzers` source generator uses to build
   the C# `*Definition` classes) and generates, into `xrmFramework/`:
   - `tables.d.ts` — the `Tables` map (`{ account: AccountDefinition, contact: ContactDefinition, ... }`);
   - `definitions/<Name>Definition.d.ts` — one `<Name>Definition` type per table, listing its columns
     (`LogName`, `Type`, ...);
   - `enums.ts` — a `const enum` for every option set actually used by a selected column, plus a
     global `EnumMap` interface that ties each enum to its option-set name.
3. **`RunWebresourcesAfterBuild`** — runs `npx webpack --mode=production` (Release) or
   `--mode=development` (any other configuration).

`webpack.config.js` walks the `scripts/` folder recursively and turns **every `.ts` file it finds
into its own entry point**, named after its path relative to `scripts/`. There is nothing to register
manually: `scripts/Forms/AccountForm.ts` is automatically compiled to `dist/scripts/Forms/AccountForm.js`,
`scripts/Ribbons/AccountRibbon.ts` to `dist/scripts/Ribbons/AccountRibbon.js`, and so on for any
subfolder you create. `assets/`, `svg/`, `js/` and `html/` folders (if present) are copied as-is into
`dist/`.

The resulting `dist/` tree is what gets deployed as web resources (`xrmframework deploy webresources`
in the CLI — see [XrmFramework CLI](../src/XrmFramework.Cli/README.md)).

---

## Declaring a form script

A form script is a class extending `FormScript<TTable>`, where `TTable` is the generated definition
type for the entity the form is bound to (e.g. `AccountDefinition`, generated from `Account.table`):

```typescript
/// <reference path="../../xrmFramework/index.d.ts" />

import { FormScript } from "../../xrmFramework/formScript";
import { registerFormScript } from "../../xrmFramework/registerScript";

class AccountForm extends FormScript<AccountDefinition> {

  public getName(): string {
    return "Forms.AccountForm";
  }

  protected internalOnLoad(utils: UtilsApi<AccountDefinition>): void {
    utils.addOnChangeAndExecute("name", this.manageNameChange);
  }

  private manageNameChange(utils: UtilsApi<AccountDefinition>): void {
    const name = utils.getValue("name");
  }
}

registerFormScript(new AccountForm());
```

- `getName()` returns the dotted name the script will be exposed under on `window` (see
  [Wiring scripts in Dataverse](#wiring-scripts-in-dataverse)). Keep it unique across the solution.
- `internalOnLoad(utils)` is where form logic goes — `utils` is a ready-to-use
  [`UtilsApi<TTable>`](#the-utils-api-utilsapitabledefinition) built from the form's `LoadEventContext`.
- `registerFormScript(new AccountForm())` must be called once at module scope. It wraps the instance
  in a `{ onLoad }` module and exposes it on `window` at the path returned by `getName()`.

## Declaring a ribbon script

A ribbon script extends `RibbonScriptBase` (via `RibbonScript<TTable>`) and exposes plain methods —
one per Ribbon Workbench Enable Rule or Command:

```typescript
/// <reference path="../../xrmFramework/index.d.ts" />

import { RibbonScript } from "../../xrmFramework/formScript";
import { registerRibbonScript } from "../../xrmFramework/registerScript";
import FormContext = XrmFramework.FormContext;

class AccountRibbon extends RibbonScript<AccountDefinition> {

  public getName(): string {
    return "Ribbon.AccountRibbon";
  }

  // Enable Rule: in Ribbon Workbench, bind this as the button's enable rule
  // and pass "PrimaryControl" as its only CRM Parameter.
  public isSendWelcomeEmailEnabled(primaryControl: FormContext<AccountDefinition>): boolean {
    const utils = this.getUtilsApi(primaryControl);
    return !utils.isCreate();
  }

  // Command: in Ribbon Workbench, bind this as the button's action
  // and pass "PrimaryControl" as its only CRM Parameter.
  public sendWelcomeEmail(primaryControl: FormContext<AccountDefinition>): void {
    const utils = this.getUtilsApi(primaryControl);
    const name = utils.getValue("name");
    utils.alert(`Sending welcome email to ${name ?? "this account"}.`);
  }
}

registerRibbonScript(new AccountRibbon());
```

- Each public method takes the arguments configured as CRM Parameters / Custom Rule Parameters in
  Ribbon Workbench — typically `XrmFramework.FormContext<TTable>` for `PrimaryControl`.
- `this.getUtilsApi(context)` builds a [`UtilsApi<TTable>`](#the-utils-api-utilsapitabledefinition)
  from that form context, exactly like the `utils` passed to a form script's `internalOnLoad`.
- `registerRibbonScript(new AccountRibbon())` exposes the instance itself (not just `onLoad`) on
  `window` at the path returned by `getName()`, so every public method becomes callable from the
  ribbon.

## Wiring scripts in Dataverse

`registerFormScript`/`registerRibbonScript` split `getName()` on `.` and create nested objects on
`window` for every segment but the last, then assign the module/instance to the last segment. For
`"Forms.AccountForm"` this produces `window.Forms.AccountForm`, exposing `Forms.AccountForm.onLoad`.

1. **Form OnLoad event**: add the compiled bundle (`Forms/AccountForm.js`) as a web resource and a
   form library, then set the OnLoad handler's **Library** to that web resource and **Function** to
   `Forms.AccountForm.onLoad` (i.e. `<getName()>.onLoad`), with "Pass execution context as first
   parameter" checked.
2. **Ribbon Workbench**: add the compiled bundle (`Ribbon/AccountRibbon.js`) as the button's
   **Library**, and set **Function** to `Ribbon.AccountRibbon.isSendWelcomeEmailEnabled` /
   `Ribbon.AccountRibbon.sendWelcomeEmail` (i.e. `<getName()>.<methodName>`), with a `PrimaryControl`
   CRM Parameter for each `FormContext<TTable>` argument.

## Type-safe table definitions

`AccountDefinition` (and every other `<Name>Definition` type) is generated straight from
`Account.table` in the `.Core` project — the same single source of truth used to generate the C#
`AccountDefinition` class (see [Generate model definitions](../README.md#generate-model-definitions)).
Regenerating it is automatic: it happens on every `Webresources` build, so adding a column with
`xrmframework tables columns` and rebuilding is enough to make it available to scripts.

This is what makes `UtilsApi<TTable>` generic-safe end to end:

- Field names passed to `getValue`, `setValue`, `getControl`, `getAttribute`, `setVisible`,
  `setDisabled`, etc. are literal string types restricted to the table's actual columns — a typo or a
  removed column is a compile-time TypeScript error.
- Picklist columns are typed against the generated `const enum` in `enums.ts` instead of a raw
  `number`, so `getValue("accountcategorycode")` returns (and `setValue` expects) the actual enum
  type, not `number`.
- Methods that only make sense for certain control types (`getVisible`, `setFocus`, `setDisabled`,
  field notifications, ...) are restricted, at the type level, to columns whose control actually
  supports that capability (see `CanGetVisible`, `IsFocusable`, `IsStandardControl`, ... in
  `xrmFramework/index.d.ts`).

## The Utils API (`UtilsApi<TableDefinition>`)

Inside `internalOnLoad`, an `addOnChange`/`addOnChangeAndExecute` callback, or a ribbon script method
(via `this.getUtilsApi(context)`), you get a `UtilsApi<TTable>` instance wrapping the current form
context. It is the main surface you use to read and manipulate the form.

### Reading and writing values

```typescript
// Typed against the column: Lookup -> Xrm.LookupValue[], Picklist -> generated enum, else raw value
const name = utils.getValue("name");
utils.setValue("name", "Contoso");

// First selected Xrm.LookupValue of a lookup/owner column, or null
const owner = utils.getLookupValue("ownerid");

// Display text of an option-set column
const statusLabel = utils.getText("statecode");

// Set an option-set column from its display text (case-insensitive)
utils.SetOptionSet("accountcategorycode", "Preferred Customer");

// Every Xrm.OptionSetValue of a picklist column
const options = utils.getOptions("accountcategorycode");

// Xrm.Attributes.AttributeType, e.g. "string", "optionset", "lookup"...
const type = utils.getType("name");

// Whether a column exists on the current form / has unsaved changes
const present = utils.isOnForm("new_customfield");
const dirty = utils.isDirty("name"); // same as getIsDirty
```

`setValue` automatically switches the attribute's submit mode from `"never"` to `"dirty"` if needed,
so a value set from script is always saved.

### Visibility, requirement & focus

| Method | Description |
| --- | --- |
| `getVisible(fieldName)` | Whether the field's control is visible. |
| `setVisible(fieldName, isVisible, showSectionIfNeeded?)` | Shows/hides one control. Header fields (`header_*`) are toggled directly; regular fields also hide their parent section when `showSectionIfNeeded` is `false` and the section would otherwise be empty. |
| `setAllVisible(fieldName, isVisible, showSectionIfNeeded?)` | Same as `setVisible`, applied to every control bound to the attribute. |
| `setRequired(fieldName, isRequired, notRequiredLevel?)` | Sets the requirement level to `"required"`, or to `notRequiredLevel` (default `"none"`) when `isRequired` is `false`. |
| `setFocus(fieldName)` | Focuses the field's control. Restricted to focusable control types. |

### Disabling controls

| Method | Description |
| --- | --- |
| `setDisabled(fieldName, disabled)` | Disables/enables the field's control. |
| `setAllDisabled(fieldName, disabled)` | Same, applied to every control bound to the attribute. |
| `getDisabled(fieldName)` | Whether the field's control is disabled. |
| `lockForm()` | Disables every control on the form (all tabs, sections, controls). |

### Tabs & sections

```typescript
utils.setVisibleTab("tab_general", true, "expanded");
utils.setVisibleSection("tab_general", "section_address", false);

const tab = utils.getTab("tab_general");
const section = utils.getSection("tab_general", "section_address");
const allTabs = utils.getTabs();
```

### Controls & attributes

```typescript
const control = utils.getControl("name");       // typed control (StringControl, LookupControl, ...)
const attribute = utils.getAttribute("name");    // typed attribute (StringAttribute, ...)
const allAttributes = utils.getAllAttributes();
```

### Change & save events

```typescript
// Register an onChange handler on one or several columns
utils.addOnChange("name", (u) => { /* ... */ });
utils.addOnChange(["name", "accountnumber"], (u) => { /* ... */ });

// Register it and immediately invoke the callback once
utils.addOnChangeAndExecute("name", (u) => { /* ... */ });

// Standard Xrm save event
utils.addOnSave((context) => { /* ... */ });

// Force an attribute's onChange handlers to run
utils.fireOnChange("name");

// True only while inside the form's OnLoad handling (getEventArgs().getDataLoadState exists)
const loading = utils.isOnLoad();
```

Every callback receives its own `UtilsApi<TTable>` instance built from the event's context, so it can
be used exactly like the `utils` passed to `internalOnLoad`.

### Saving & refreshing

```typescript
utils.save();                    // Xrm.data.save()
utils.saveData("saveandclose");  // Xrm.data.entity.save(saveMode)
utils.refresh(true);             // Xrm.data.refresh(saveData)
utils.refreshRibbon();
utils.refreshWebRessource("WebResource_Name"); // reloads an iframe/webresource control by clearing+resetting its src
```

### Notifications & dialogs

```typescript
utils.addFieldNotification("name", "This field looks wrong.");
utils.clearFieldNotification("name");

utils.setFormNotification("Record is locked.", "WARNING", "lock-notification");
utils.clearFormNotification("lock-notification");

utils.alert("Saved successfully.", () => { /* optional callback */ });
utils.confirm(
  "Delete this record?",
  () => { /* Ok */ },
  () => { /* Cancel */ },
);
```

### Business process flow

```typescript
const processManager = utils.getProcessData();
const processControl = utils.getProcessUi();
utils.setActiveStage(stageId, () => { /* ... */ });
const activeStage = utils.getActiveStage();
```

### Record info & navigation

```typescript
const entityName = utils.getEntityName();
const recordId = utils.getRecordId();       // GUID string, braces stripped
const creating = utils.isCreate();          // form type is Create or Bulk Edit
const editable = utils.isModifiable();      // form type is Create or Update

utils.openForm({ entityName: "contact", entityId: id });
utils.openWebRessource("new_page.html", "data=1", 600, 400);
utils.closeForm(/* forceClose */ true);     // forceClose sets dirty attributes to submit mode "never" first

const clientUrl = utils.getClientUrl();
const paramValue = utils.getWRParameter("data"); // reads a querystring parameter from the webresource URL
```

### Filtering lookups

```typescript
utils.setFilter("primarycontactid", () => `<filter><condition attribute="statecode" operator="eq" value="0" /></filter>`);
// or a static FetchXML filter string
utils.setFilter("primarycontactid", '<filter><condition attribute="statecode" operator="eq" value="0" /></filter>');
```

Calling `setFilter` again on the same field replaces the previously registered pre-search filter
instead of stacking a second one.

### Misc helpers

| Method | Description |
| --- | --- |
| `getUserInfos()` | The global context's `Xrm.UserSettings`, or `null`. |
| `IsUserRecordOwner()` | Whether the current user is (one of) the record's owner(s). |
| `isMobile()` / `isTablet()` | Client/form-factor checks. |
| `isIE()` | User-agent sniffing for Internet Explorer / Trident / old Edge. |
| `isNullOrEmpty(value)` | `true` if `value` is `null`, `undefined`, or an empty string. |
| `addDays(date, days)` | Returns a new `Date` offset by `days` (can be negative). |
| `extractDomain(url)` | Extracts the host (without protocol/port) from a URL. |
| `setContext(context)` | Rebinds the `UtilsApi` instance to a different form/event context. |

`Utils.ts` also augments the global `Date` prototype with `date.yyyymmdd()`, returning the date as
`"YYYY-MM-DD"`.

## Typed Web API

`xrmFramework.d.ts` also strengthens `Xrm.WebApi` (`online` and `offline`) against the generated
`Tables` map, so CRUD calls are checked against real entity/column names and `$select`/`$expand`
strings are parsed to type the returned object:

```typescript
const response = await Xrm.WebApi.createRecord("account", { name: "Contoso" });

const account = await Xrm.WebApi.retrieveRecord("account", accountId, "$select=name,accountnumber");
// account: { name?: string; accountnumber?: string }

const accounts = await Xrm.WebApi.retrieveMultipleRecords("account", "?$select=name&$top=10");

await Xrm.WebApi.updateRecord("account", accountId, { name: "New name" });
```
