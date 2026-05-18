/// <reference path="./xrmFramework.d.ts" />


type IsOptionSetColumn<TableDefinition extends Table, ColumnName extends ColumnLogNames<TableDefinition>> = ColumnFromLogName<TableDefinition, ColumnName> extends { Type: "Picklist" } ? true : false;

type OptionSetColumnLogNames<TableDefinition extends Table> = Extract<ColDef<TableDefinition>, { Type: "Picklist" }>["LogName"];





type FormColTypeToTsType = {
    Uniqueidentifier: string;
    String: string;
    Lookup: Xrm.LookupValue[];
    Owner: Xrm.LookupValue[];
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


type FormAttributeValueTypeFromCol<
    TableDefinition extends Table,
    C extends ColDef<TableDefinition> | undefined
> = C extends { Type: "Picklist"; IsMultiSelect: true }
    ? number[]
    : C extends { Type: infer T }
        ? T extends keyof FormColTypeToTsType
            ? FormColTypeToTsType[T]
            : never
        : never;

type FormAttributeValueFromColumnName<
    TableDefinition extends Table,
    ColumnName extends ColumnLogNames<TableDefinition>
> = FormAttributeValueTypeFromCol<TableDefinition, ColumnFromLogName<TableDefinition, ColumnName>>;








type SelectedColumns2<
    TableDefinition extends Table,
    Query extends string,
    QuerySelected = QuerySelect<Query>
> =
    QuerySelected extends `${infer Column1},${infer Rest}`
        ? Column1 extends keyof QueryResultTableObject<TableDefinition>
            ? [Column1, ...SelectedColumns2<TableDefinition, Query, Rest>]
            : SelectedColumns2<TableDefinition, Query, Rest> // skip unknown column here
        : QuerySelected extends keyof QueryResultTableObject<TableDefinition>
            ? [QuerySelected]
            : [];


type CallbackFn<EntityName extends keyof Tables, Columns extends (keyof QueryResultTableObject<Tables[EntityName]>)[]> = (req: { params: FilteredTableObject<Tables[EntityName], Columns> }) => void;

type IsFocusable<TableDefinition extends Table, ColumnName extends ColumnLogNames<TableDefinition>> = ControlTypeFromCol<TableDefinition, ColumnFromLogName<TableDefinition, ColumnName>> extends Xrm.Controls.UiFocusable ? true : false;

type CanGetVisible<TableDefinition extends Table, ColumnName extends ColumnControlNames<TableDefinition>>
    = ControlTypeFromCol<TableDefinition, ColumnFromLogName<TableDefinition, ColumnName>> extends Xrm.Controls.UiCanGetVisibleElement ? true : false;

type CanSetVisible<TableDefinition extends Table, ColumnName extends ColumnControlNames<TableDefinition>>
    = ControlTypeFromCol<TableDefinition, ColumnFromLogName<TableDefinition, ColumnName>> extends Xrm.Controls.UiCanSetVisibleElement ? true : false;

type CanGetDisabled<TableDefinition extends Table, ColumnName extends ColumnControlNames<TableDefinition>>
    = ControlTypeFromCol<TableDefinition, ColumnFromLogName<TableDefinition, ColumnName>> extends Xrm.Controls.UiCanGetDisabledElement ? true : false;

type CanSetDisabled<TableDefinition extends Table, ColumnName extends ColumnControlNames<TableDefinition>>
    = ControlTypeFromCol<TableDefinition, ColumnFromLogName<TableDefinition, ColumnName>> extends Xrm.Controls.UiCanSetDisabledElement ? true : false;

type IsStandardControl<TableDefinition extends Table, ColumnName extends ColumnControlNames<TableDefinition>>
    = ControlTypeFromCol<TableDefinition, ColumnFromLogName<TableDefinition, ColumnName>> extends Xrm.Controls.StandardControl ? true : false;

type LogNameOrLogNames<TableDefinition extends Table> =
    | ColumnLogNames<TableDefinition>
    | ColumnLogNames<TableDefinition>[];

type AttributeKey<TableDefinition extends Table> = keyof QueryResultTableObject<TableDefinition>;

    
            
type AttributeFromKey<
    TableDefinition extends Table,
    K extends AttributeKey<TableDefinition>
> = K extends `_${infer LogName}_value`
    ? Extract<ColDef<TableDefinition>, { LogName: LogName; Type: "Lookup" }>
    : Extract<ColDef<TableDefinition>, { LogName: K }>;


