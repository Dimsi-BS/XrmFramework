/// <reference path="./table.d.ts" />
/// <reference path="./enums.ts" />
/// <reference path="./xrm.d.ts" />

/*** Column Helpers ***/
type ColumnFromLogName<TableDefinition extends Table, LogName extends ColumnControlNames<TableDefinition>> = Extract<ColDef<TableDefinition>, { LogName: BaseColumnName<TableDefinition, LogName> }>;

type EnumFromEnumName<TableDefinition extends Table, EnumName extends TableDefinition["Enums"][number]["LogName"]> = Extract<TableDefinition["Enums"][number], { "LogName": EnumName }>;

        type ColumnControlNames<TableDefinition extends Table> =
    | ColumnLogNames<TableDefinition>
    | `header_${ColumnLogNames<TableDefinition>}`
    | `header_process_${ColumnLogNames<TableDefinition>}`
    | string;

type BaseColumnName<
    TableDefinition extends Table,
    Name extends ColumnControlNames<TableDefinition>
> =
    Name extends `header_process_${infer Column}`
        ? Column extends ColumnLogNames<TableDefinition>
            ? Column
            : never
        : Name extends `header_${infer Column}`
            ? Column extends ColumnLogNames<TableDefinition>
                ? Column
                : never
            : Name extends ColumnLogNames<TableDefinition>
                ? Name
                : never;

type LookupColumnLogNames<TableDefinition extends Table> = Extract<ColDef<TableDefinition>, { Type: "Lookup" | "Owner" }>["LogName"];

type PicklistColumnLogNames<TableDefinition extends Table> = Extract<ColDef<TableDefinition>, { Type: "Picklist" | "State" | "Status" }>["LogName"];

/*** Attributes Helpers ***/

type ColTypeToAttributeType = {
    Uniqueidentifier: Xrm.Attributes.StringAttribute;
    String: Xrm.Attributes.StringAttribute;
    Lookup: Xrm.Attributes.LookupAttribute;
    Owner: Xrm.Attributes.LookupAttribute;
    Picklist: Xrm.Attributes.OptionSetAttribute;
    Money: Xrm.Attributes.NumberAttribute;
    Number: Xrm.Attributes.NumberAttribute;
    State: Xrm.Attributes.OptionSetAttribute;
    Status: Xrm.Attributes.OptionSetAttribute;
    Decimal: Xrm.Attributes.NumberAttribute;
    Boolean: Xrm.Attributes.BooleanAttribute;
    Double: Xrm.Attributes.NumberAttribute;
    Memo: Xrm.Attributes.StringAttribute;
    Integer: Xrm.Attributes.NumberAttribute;
    DateTime: Xrm.Attributes.DateAttribute;
    BigInt: never;
};

type AttributeTypeFromCol<
    TableDefinition extends Table,
    C extends ColDef<TableDefinition> | undefined
> = C extends { Type: "Picklist"; IsMultiSelect: true }
    ? Xrm.Attributes.MultiSelectOptionSetAttribute
    : C extends { Type: infer T }
        ? T extends keyof ColTypeToAttributeType
            ? ColTypeToAttributeType[T]
            : never
        : never;

type AttributeTypeFromColumnName<
    TableDefinition extends Table,
    ColumnName extends ColumnLogNames<TableDefinition>
> = AttributeTypeFromCol<TableDefinition, ColumnFromLogName<TableDefinition, ColumnName>>;


type QueryResultTableObject<TableDefinition extends Table> = Partial<QueryResultObjectFromCols<TableDefinition>>;

/* Generates an object type with keys = LogName and mapped values */
type QueryResultObjectFromCols<TableDefinition extends Table, C = ColDef<TableDefinition>> =
    { [K in C as NameFromCol<TableDefinition, K["LogName"], K["Type"]>]: (QueryTypeFromCol<TableDefinition, K>) };

type ColDef<TableDefinition extends Table> = TableDefinition["Cols"][number];

type NameFromCol<TableDefinition extends Table, N extends ColumnLogNames<TableDefinition>, T extends ColDef<TableDefinition>["Type"]> =
    T extends "Lookup" ? `_${Extract<N, string>}_value` : N;

type ColumnLogNames<TableDefinition extends Table> = ColDef<TableDefinition>["LogName"];

type ColumnWithValueLogNames<TableDefinition extends Table> =
    Extract<
        ColDef<TableDefinition>,
        { Type: Exclude<ColumnTypes, "Uniqueidentifier" | "BigInt"> }
    >["LogName"];

type AttributeValueFromColumnName<
    TableDefinition extends Table,
    ColumnName extends ColumnLogNames<TableDefinition>
> =
    AttributeTypeFromColumnName<TableDefinition, ColumnName> extends Xrm.Attributes.Attribute<infer TValue>
        ? TValue
        : never;

type EnumTypeFromColumnName<
    TableDefinition extends Table,
    ColumnName extends PicklistColumnLogNames<TableDefinition>
> = ColumnFromLogName<TableDefinition, ColumnName> extends { EnumName: infer EnumName extends string }
    ? EnumName extends `${string}|${string}`
        ? EnumMap[EnumFromEnumName<TableDefinition, EnumName>["Name"]]
        : EnumMap[EnumFromEnumName<OptionSetsDefinition, EnumName>["Name"]]
    : never;

type ApiColTypeToTsType = {
    Uniqueidentifier: string;
    String: string;
    Lookup: string;
    Owner: string;
    Picklist: number;
    Money: number;
    Number: number;
    State: number;
    Status: number;
    Decimal: number;
    Boolean: boolean;
    Double: number;
    Memo: string;
    Integer: number;
    DateTime: Date;
    BigInt: never;
};


type QueryTypeFromCol<
    TableDefinition extends Table,
    C extends ColDef<TableDefinition> | undefined
> = C extends { Type: "Picklist"; IsMultiSelect: true }
    ? number[]
    : C extends { Type: infer T }
        ? T extends keyof ApiColTypeToTsType
            ? ApiColTypeToTsType[T]
            : never
        : never;

/***  QueryHelper ***/

type SelectedColumns<
    TableDefinition extends Table,
    Query extends string,
    QuerySelected = QuerySelect<Query>
> =
    QuerySelected extends ""
        ? []
        : QuerySelected extends `${infer Column1},${infer Rest}`
            ? Column1 extends keyof QueryResultTableObject<TableDefinition>
                ? [Column1, ...SelectedColumns<TableDefinition, Query, Rest>]
                : never
            : QuerySelected extends keyof QueryResultTableObject<TableDefinition>
                ? [QuerySelected]
                : never;

type QuerySelect<Path> = Path extends `${infer Start}$select=${infer Select}` ? RemoveAfterSelect<Select> : never

type RemoveAfterSelect<Part> = Part extends `${infer Select}&${infer AfterSelect}` ? Select : Part


/*** FilteredTableObject ***/

type FilteredTableObject<TableDefinition extends Table, Columns extends (keyof QueryResultTableObject<TableDefinition>)[], FullObject = QueryResultTableObject<TableDefinition>, ColumnsList = Columns[keyof Columns]> = {
    [key in keyof FullObject as key extends ColumnsList ? key : never]: FullObject[key]
}

/*** Control Helpers ***/

type ColTypeToControlType = {
    Uniqueidentifier: never;
    String: Xrm.Controls.StringControl;
    Lookup: Xrm.Controls.LookupControl;
    Owner: Xrm.Controls.LookupControl;
    Picklist: Xrm.Controls.OptionSetControl;
    Money: Xrm.Controls.NumberControl;
    Number: Xrm.Controls.NumberControl;
    State: Xrm.Controls.OptionSetControl;
    Status: Xrm.Controls.OptionSetControl;
    Decimal: Xrm.Controls.NumberControl;
    Boolean: Xrm.Controls.BooleanControl;
    Double: Xrm.Controls.NumberControl;
    Memo: Xrm.Controls.StringControl;
    Integer: Xrm.Controls.NumberControl;
    DateTime: Xrm.Controls.DateControl;
    BigInt: never;
};

type ControlTypeFromCol<
    TableDefinition extends Table,
    C extends ColDef<TableDefinition> | undefined
> = C extends { Type: "Picklist"; IsMultiSelect: true }
    ? Xrm.Controls.MultiSelectOptionSetControl
    : C extends { Type: infer T }
        ? T extends keyof ColTypeToControlType
            ? ColTypeToControlType[T]
            : never
        : never;

type ControlTypeFromColumnName<
    TableDefinition extends Table,
    ColumnName extends ColumnLogNames<TableDefinition>
> = ControlTypeFromCol<TableDefinition, ColumnFromLogName<TableDefinition, ColumnName>>;

type EventCallback<TableDefinition extends Table> =
    (utils: UtilsApi<TableDefinition>) => void
        | ((utils: UtilsApi<TableDefinition>) => Promise<void>)
