
type ColumnTypes =
    "Uniqueidentifier"
    | "String"
    | "Picklist"
    | "Boolean"
    | "BigInt"
    | "Integer"
    | "Double"
    | "Money"
    | "Memo"
    | "DateTime"
    | "Owner"
    | "Lookup"
    | "Customer"
    | "Decimal"
    | "State"
    | "Status"
    | "PartyList"
    | "Virtual"
    | "MultiSelectPicklist"

type Label = {
    Label: string,
    LangId: number
}

type Column = {
    LogName: string,
    Name: string,
    Type: ColumnTypes,
    IsMultiSelect?: boolean,
    PrimaryType?: "Id" | "Name",
    Capa: number,
    Labels: Array<Label>
}

/*
 * A column holding several options at once, in any of the three shapes a .table carries it in: the
 * CRM reports a multi-select choice as a Virtual attribute, so an older pull wrote Type "Virtual",
 * the current one writes "Picklist", and a file edited by hand says it through the type alone.
 * Matching only the "Picklist" shape typed the other two as never -- a generated definition is
 * serialized straight out of the .table, so which shape reaches here is not the script's to choose.
 */
type MultiSelectColumn = { IsMultiSelect: true } | { Type: "MultiSelectPicklist" }

type OptionSet = {
    LogName: string,
    Name: string,
    Values: Array<OptionSetValue>
}

type OptionSetValue = {
    Value: number,
    Name: string,
    Labels: Array<Label>
}

type Table = {
    LogName: string,
    Name: string,
    CollName: string,
    Cols: Array<Column>
}
