/// <reference path="./tables.d.ts" />
/// <reference path="./enums.ts" />
/// <reference path="./xrm.d.ts" />"
/// <reference path="./utilityTypes.d.ts" />

declare var Xrm: XrmFramework.XrmFrameworkStatic;

declare namespace XrmFramework {

    interface CreateResponse<EntityName extends keyof Tables> {
        entityType: EntityName;
        id: string;
    }


    interface XrmFrameworkWebApi extends XrmFrameworkWebApiOnline {
        offline: XrmFrameworkWebApiOffline;
        online: XrmFrameworkWebApiOnline;
    }

    interface XrmFrameworkWebApiOnline extends XrmFrameworkWebApiOffline {}

    interface XrmFrameworkStatic extends Xrm.XrmStatic {
        WebApi: XrmFrameworkWebApi & Xrm.XrmStatic["WebApi"];
    }

    interface XrmFrameworkWebApiOffline extends Xrm.WebApiOffline {
        createRecord<EntityName extends keyof Tables, TableDefinition extends Tables[EntityName]>(entityName: EntityName, obj: QueryResultTableObject<TableDefinition>): Xrm.Async.PromiseLike<CreateResponse<EntityName>>;


        deleteRecord<EntityName extends keyof Tables>(entityLogicalName: EntityName, id: string): Xrm.Async.PromiseLike<string>;

        /**
         * Retrieves an entity record.
         * @param entityLogicalName The entity logical name of the record you want to retrieve. For example: "account".
         * @param id GUID of the entity record you want to retrieve.
         * @param options (Optional) OData system query options, $select and $expand, to retrieve your data.
         * - Use the $select system query option to limit the properties returned by including a comma-separated
         *   list of property names. This is an important performance best practice. If properties aren’t
         *   specified using $select, all properties will be returned.
         * - Use the $expand system query option to control what data from related entities is returned. If you
         *   just include the name of the navigation property, you’ll receive all the properties for related
         *   records. You can limit the properties returned for related records using the $select system query
         *   option in parentheses after the navigation property name. Use this for both single-valued and
         *   collection-valued navigation properties.
         * - You can also specify multiple query options by using & to separate the query options.
         * @example <caption>options example:</caption>
         * options: $select=name&$expand=primarycontactid($select=contactid,fullname)
         * @returns On success, returns a promise containing a JSON object with the retrieved attributes and their values.
         * @see {@link https://learn.microsoft.com/en-us/power-apps/developer/model-driven-apps/clientapi/reference/xrm-webapi/retrieverecord External Link: retrieveRecord (Client API reference)}
         */
        retrieveRecord<EntityName extends keyof Tables, Query extends string,
            // compute Columns as the SelectedColumns for the query
            Columns extends (keyof QueryResultTableObject<Tables[EntityName]>)[] = SelectedColumns<Tables[EntityName], Query>,
            T = FilteredTableObject<Tables[EntityName], Columns>
        >(entityLogicalName: EntityName, id: string, options?: Query): Xrm.Async.PromiseLike<T>;

        /**
         * Retrieves a collection of entity records.
         * @param entityLogicalName The entity logical name of the records you want to retrieve. For example: "account".
         * @param options (Optional) OData system query options or FetchXML query to retrieve your data.
         * * Following system query options are supported: $select, $top, $filter, $expand, and $orderby.
         * * To specify a FetchXML query, use the fetchXml attribute to specify the query.
         * * NOTE: You must always use the $select system query option to limit the properties returned for an entity
         * record by including a comma-separated list of property names. This is an important performance best practice.
         * * If properties aren’t specified using $select, all properties will be returned.
         * * You can specify multiple system query options by using & to separate the query options.
         * @param maxPageSize (Optional) Specify a positive number that indicates the number of entity records to be returned per page.
         * * If you do not specify this parameter, the default value is passed as 5000. If the number of records being retrieved is more than the specified
         * maxPageSize value, nextLink attribute in the returned promise object will contain a link to retrieve the next set of entities.
         * @returns On success, returns a promise object containing the attributes specified earlier in the description of the successCallback parameter.
         * @see {@link https://learn.microsoft.com/en-us/power-apps/developer/model-driven-apps/clientapi/reference/xrm-webapi/retrievemultiplerecords External Link: retrieveMultipleRecords (Client API reference)}
         */
        retrieveMultipleRecords<EntityName extends keyof Tables, Query extends string,
            // compute Columns as the SelectedColumns for the query
            Columns extends (keyof QueryResultTableObject<Tables[EntityName]>)[] = SelectedColumns<Tables[EntityName], Query>,
            T = FilteredTableObject<Tables[EntityName], Columns>
        >(
            entityLogicalName: EntityName,
            options?: Query,
            maxPageSize?: number,
        ): Xrm.Async.PromiseLike<Xrm.RetrieveMultipleResult<T>>;

        /**
         * Updates an entity record.
         * @param entityLogicalName The entity logical name of the record you want to update. For example: "account".
         * @param id GUID of the entity record you want to update.
         * @param data A JSON object containing key: value pairs, where key is the property of the entity and value is the value of the property you want update.
         * @returns On success, returns a promise object containing the attributes specified earlier in the description of the successCallback parameter.
         * @see {@link https://learn.microsoft.com/en-us/power-apps/developer/model-driven-apps/clientapi/reference/xrm-webapi/updaterecord External Link: updateRecord (Client API reference)}
         */
        updateRecord<EntityName extends keyof Tables, Query extends string,
            // compute Columns as the SelectedColumns for the query
            Columns extends (keyof QueryResultTableObject<Tables[EntityName]>)[] = SelectedColumns<Tables[EntityName], Query>,
            T extends FilteredTableObject<Tables[EntityName], Columns>
        >(entityLogicalName: EntityName, id: string, data: T): Xrm.Async.PromiseLike<Xrm.UpdateResponse>;
    }
    
    interface FormContext<TableDefinition extends Table> extends Xrm.FormContext {
        /**
         * Gets a control by name or index.
         * @param T A Control type
         * @param controlNameOrIndex Name of the control or the control index.
         * @returns The control.
         */
        getControl<ColumnName extends ColumnLogNames<TableDefinition>, T extends ControlTypeFromColumnName<TableDefinition, ColumnName>>(controlNameOrIndex: ColumnName): T | null;


        getAttribute<ColumnName extends ColumnLogNames<TableDefinition>, T extends AttributeTypeFromColumnName<TableDefinition, ColumnName>>(
            attributeNameOrIndex: ColumnName,
        ): T | null;
        getAttribute(attributeNameOrIndex: string | number) : Xrm.Attributes.Attribute | null;

    }
}

type EntityFormOptions = string | Xrm.Navigation.EntityFormOptions;

type Nullable<T> = T | null;

interface UtilsApi<TableDefinition extends Table> {
    getVisible(fieldName: string): boolean;
    getEntityName(): string;
    save(): Xrm.Async.PromiseLike<any>;
    saveData(saveMode?: Xrm.EntitySaveMode): void;
    lockForm(): void;
    setAllVisible<ColumnName extends TableDefinition["Cols"][number]["LogName"]>(fieldName: ColumnName, isVisible: boolean, showSectionIfNeeded?: boolean): void;
    getTab(tabId: string): Nullable<Xrm.Controls.Tab>;
    getTabs(): Xrm.Collection.ItemCollection<Xrm.Controls.Tab>;
    refresh(saveData?: boolean): Xrm.Async.PromiseLike<any>;
    refreshRibbon(): void;
    setVisible<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName, isVisible: boolean, showSectionIfNeeded?: boolean): void;
    getRecordId(): string;
    setVisibleTab(tabName: string, isVisible: boolean, displayState?: Xrm.DisplayState): void;
    setVisibleSection(tabName: string, sectionName: string, isVisible: boolean): void;
    getSection(tabName: string, sectionName: string): Nullable<Xrm.Controls.Section>;
    getClientUrl(): string | null;
    getUserInfos(): Xrm.UserSettings | null;
    setRequired<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName, isRequired: boolean, notRequiredLevel?: Xrm.Attributes.RequirementLevel): void;
    setFocus<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName): void;
    isOnForm<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName | ColDef<TableDefinition>["LogName"]): boolean;
    getIsDirty<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName | ColDef<TableDefinition>["LogName"]): boolean;
    getProcessData(): Xrm.ProcessFlow.ProcessManager | null;
    getProcessUi(): Xrm.Controls.ProcessControl | null;
    getLookupValue<ColumnName extends LookupColumnLogNames<TableDefinition>>(fieldName: ColumnName): any | null;

    getValue<ColumnName extends LookupColumnLogNames<TableDefinition>>(
        fieldName: ColumnName
    ) : Xrm.LookupValue[] | null
    getValue<ColumnName extends PicklistColumnLogNames<TableDefinition>>(
        fieldName: ColumnName
    ) : EnumTypeFromColumnName<TableDefinition, ColumnName> | null;
    getValue<ColumnName extends ColumnWithValueLogNames<TableDefinition>>(
        fieldName: ColumnName
    ): AttributeValueFromColumnName<TableDefinition, ColumnName> | null;
    getText<OptionSetFieldName extends OptionSetColumnLogNames<TableDefinition>>(fieldName: IsOptionSetColumn<TableDefinition, OptionSetFieldName> extends true ? OptionSetFieldName : never): string | null;
    setValue<ColumnName extends LookupColumnLogNames<TableDefinition>>(
        fieldName: ColumnName,
        value: Xrm.LookupValue[] | null
    ): void;
    setValue<ColumnName extends PicklistColumnLogNames<TableDefinition>>(
        fieldName: ColumnName,
        value: EnumTypeFromColumnName<TableDefinition, ColumnName> | null
    ): void;
    setValue<ColumnName extends ColumnLogNames<TableDefinition>>(
        fieldName: ColumnName,
        value: AttributeValueFromColumnName<TableDefinition, ColumnName> | null
    ): void;
    setSubmitMode<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName, value: Xrm.SubmitMode): void;
    setAllDisabled<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName, disabled: boolean): void;
    setDisabled<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName, disabled: boolean): void;
    setDisabled<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName, disabled: boolean): void;
    getDisabled<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName): boolean;
    isMobile(): boolean;
    isTablet(): boolean;
    addOnChange(
        fieldNames: LogNameOrLogNames<TableDefinition>,
        callback: (utils: UtilsApi<TableDefinition>) => void
    ): boolean;
    addOnChangeAndExecute(fieldNames: LogNameOrLogNames<TableDefinition>, callback: EventCallBack<TableDefinition>): void;
    addOnSave(callback: Xrm.Events.SaveEventHandler): void;
    addFieldNotification(fieldName: string, message: string): void;
    clearFieldNotification(fieldName: string): void;
    setFormNotification(message: string, level: XrmEnum.FormNotificationLevel, id: string): void;
    clearFormNotification(id: string): void;
    addDays(origDate: Date, days: number): Date;
    isCreate(): boolean;
    isModifiable(): boolean;
    alert(message: string, callback?: () => void): void;
    confirm(message: string, functionWhenOk: () => void, functionWhenCancel: () => void): void;
    closeForm(forceClose?: boolean): void;
    getWRParameter(searchFor: string): string;
    fireOnChange(fieldName: string): void;
    IsUserRecordOwner(): boolean;
    openForm(entityFormOptions: EntityFormOptions, parameters?: any): ReturnType<Xrm.Navigation["openForm"]>;
    isNullOrEmpty(value: string | null | undefined): boolean;
    openWebRessource(webResourceName: string, webResourceData: string | null, width: number, height: number): void;
    isDirty<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName): boolean;
    getControl<ControlName extends ColumnControlNames<TableDefinition>>(controlName: ControlName) : ControlTypeFromColumnName<TableDefinition, ControlName> | null;
    getAttribute<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName): Nullable<AttributeTypeFromColumnName<TableDefinition, ColumnName>>;
    getAllAttributes(): Xrm.Collection.ItemCollection<Xrm.Attributes.Attribute>;
    refreshWebRessource(webRessourceName: string): void;
    setFilter<ColumnName extends LookupColumnLogNames<TableDefinition>>(fieldName: ColumnName, getFilter: string | (() => string)): void;
    setActiveStage(stageId: string, callbackFunction: () => void): void;
    getActiveStage(): Xrm.ProcessFlow.Stage | null;
    extractDomain(url: string): string;
    getOptions<OptionSetFieldName extends OptionSetColumnLogNames<TableDefinition>>(fieldName: OptionSetFieldName): Xrm.OptionSetValue[];
    SetOptionSet<ColumnName extends OptionSetColumnLogNames<TableDefinition>>(fieldName: ColumnName, setText: string | null): void;
    getType<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName): Xrm.Attributes.AttributeType | undefined;
    isIE(): boolean;
    setContext(context: XrmFramework.FormContext<TableDefinition>): void;

    isOnLoad(): boolean;
}

interface Window {
    Xrm: XrmFramework.XrmFrameworkStatic;
    GetGlobalContext(): Xrm.GlobalContext;
}


