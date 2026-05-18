
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
    | "Decimal" 
    | "State" 
    | "Status"
    | "PartyList"

type Label = {
    Label: string,
    LangId: number
}

type Column = {
    LogName: string,
    Name: string,
    Type: ColumnTypes,
    PrimaryType?: "Id" | "Name",
    Capa: number,
    Labels: Array<Label>
}

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
