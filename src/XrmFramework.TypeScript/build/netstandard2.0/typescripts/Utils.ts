/// <reference path="./xrmFramework.d.ts" />



import LoadEventContext = Xrm.Events.LoadEventContext;

class UtilsClass<TableDefinition extends Table, TEventContext extends Xrm.Events.EventContext> implements UtilsApi<TableDefinition> {
    constructor(
        context: TEventContext | XrmFramework.FormContext<TableDefinition>
    ) {
        if ("getFormContext" in context) {
            this.formContext = context.getFormContext() as XrmFramework.FormContext<TableDefinition>;
            this.eventContext = context;
        } else {
            this.formContext = context;
        }
    }

    isOnLoad(): boolean {
        if (!!this.eventContext && "getEventArgs" in this.eventContext && typeof this.eventContext.getEventArgs === "function")
        {
            const args = this.eventContext.getEventArgs();
            
            return !!args && "getDataLoadState" in args && typeof args.getDataLoadState === "function";
        }
        return false;
    }

    private formContext: XrmFramework.FormContext<TableDefinition> & Xrm.FormContext;
    private eventContext: TEventContext | undefined;
    private readonly preSearchCache: Record<string, (() => void) | undefined> = {};

    public getVisible<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: CanGetVisible<TableDefinition, ColumnName> extends true ? ColumnName : never): boolean {
        const control = this.getXrm().getControl(fieldName);
       
        if (control == null) return false;

        const visibleControl = control as Xrm.Controls.UiCanGetVisibleElement;
        return visibleControl.getVisible();
    }

    public getEntityName(): string {
        return this.getXrm().data.entity.getEntityName();
    }

    public save() {
        return this.getXrm().data.save();
    }

    public saveData(saveMode?: Xrm.EntitySaveMode) {
        this.getXrm().data.entity.save(saveMode);
    }

    public lockForm(): void {
        this.getXrm().ui.tabs.forEach((tab) => {
            tab.sections.forEach((section) => {
                section.controls.forEach((control) => {
                    if ("setDisabled" in control && typeof control.setDisabled === "function") {
                        control.setDisabled(true);
                    }
                });
            });
        });
    }

    public setAllVisible<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName, isVisible: boolean, showSectionIfNeeded = true): void {
        const attribute = this.getXrm().getAttribute(fieldName);
        if (attribute != null) {
            attribute.controls.forEach((control) => {
                control.setVisible(isVisible);
            });
        }
    }

    public getTab(tabId: string) {
        return this.getXrm().ui.tabs.get(tabId);
    }

    public getTabs() {
        return this.getXrm().ui.tabs;
    }

    public refresh(saveData?: boolean) {
        return this.getXrm().data.refresh(saveData ?? false);
    }

    public refreshRibbon(): void {
        this.getXrm().ui.refreshRibbon();
    }

    public setVisible<ColumnName extends ColumnControlNames<TableDefinition>>(
        fieldName: CanSetVisible<TableDefinition, ColumnName> extends true ? ColumnName : never, 
        isVisible: boolean, 
        showSectionIfNeeded = true
    ): void {
        const control = this.getXrm().getControl(fieldName);
        if (control == null) return;

        const visibleControl = control as Xrm.Controls.UiCanSetVisibleElement & Xrm.Controls.Control;
        
        if (fieldName.startsWith("header_")) {
            visibleControl.setVisible(isVisible);
            return;
        }
        
        const section = control.getParent();
        const sectionVisibility = section.getVisible();

        visibleControl.setVisible(isVisible);

        if (!showSectionIfNeeded && !sectionVisibility) {
            section.setVisible(false);
        }
    }

    public getRecordId(): string {
        return this.getXrm().data.entity.getId().replace("{", "").replace("}", "");
    }
    
    public setVisibleTab(tabName: string, isVisible: boolean, displayState?: Xrm.DisplayState): void {
        const control = this.getXrm().ui.tabs.get(tabName);
        if (control != null) {
            control.setVisible(isVisible);
            if (isVisible && displayState != null) {
                control.setDisplayState(displayState);
            }
        }
    }

    public setVisibleSection(tabName: string, sectionName: string, isVisible: boolean): void {
        const section = this.getSection(tabName, sectionName);
        if (section != null) {
            section.setVisible(isVisible);
        }
    }

    public getSection(tabName: string, sectionName: string) {
        const tab = this.getXrm().ui.tabs.get(tabName);
        if (tab == null) {
            return null;
        }
        return tab.sections.get(sectionName);
    }

    public getClientUrl(): string | null {
        const context = this.getGlobalContext();
        return context?.getClientUrl() ?? null;
    }

    public getUserInfos() {
        return this.getGlobalContext()?.userSettings ?? null;
    }

    public setRequired(fieldName: string, isRequired: boolean, notRequiredLevel: Xrm.Attributes.RequirementLevel = "none"): void {
        const attribute = this.getXrm().getAttribute(fieldName);
        if (attribute != null) {
            attribute.setRequiredLevel(isRequired ? "required" : notRequiredLevel);
        }
    }

    public setFocus<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: IsFocusable<TableDefinition, ColumnName> extends true ? ColumnName: never): void {
        const control = this.getXrm().getControl(fieldName);
        if (control != null) {
            const focusableControl = control as Xrm.Controls.UiFocusable;
            focusableControl.setFocus();
        }
    }

    public isOnForm(fieldName: LogNameOrLogNames<TableDefinition>): boolean {
        const xrm = this.getXrm();

        if (Array.isArray(fieldName)) {
            return fieldName.every((name) => xrm.getAttribute(name) != null);
        }

        if (typeof fieldName === "string") {
            return xrm.getAttribute(fieldName) != null;
        }

        return false;
    }

    public getIsDirty(fieldName: LogNameOrLogNames<TableDefinition>): boolean {
        if (this.isOnForm(fieldName)) {
            const name = Array.isArray(fieldName) ? fieldName[0] : fieldName;
            return this.getXrm().getAttribute(name)?.getIsDirty() ?? false;
        }
        return false;
    }

    public getProcessData() {
        return this.getXrm().data.process;
    }

    public getProcessUi() {
        return this.getXrm().ui.process;
    }

    public getLookupValue<ColumnName extends LookupColumnLogNames<TableDefinition>>(fieldName: ColumnName) : Xrm.LookupValue | null {
        const value = this.getValue(fieldName);
        return value == null ? null : value[0];
    }

    
    getValue(fieldName: string) : any {
        if (fieldName !== undefined && this.isOnForm(fieldName)) {
            const attribute = this.getXrm().getAttribute(fieldName);
            const value = attribute?.getValue() ?? null;

            if (attribute?.getAttributeType() === "optionset" && value === -1) {
                return null;
            }

            return value;
        }
        return null;
    }

    public getText<OptionSetFieldName extends OptionSetColumnLogNames<TableDefinition>>(fieldName: OptionSetFieldName): string | null {
        if (!this.isOnForm(fieldName)) {
            return null;
        }

        const attribute = this.getXrm().getAttribute(fieldName);
        if (attribute == null) return null;

        if (attribute.getAttributeType() !== "optionset") {
            throw new Error(`${fieldName} is not an optionset`);
        }

        const optionSetAttribute = attribute as Xrm.Attributes.OptionSetAttribute;
        
        return optionSetAttribute.getText();
    }

    public setValue(fieldName: string, value: any): void
    {
        const attribute = this.getXrm().getAttribute(fieldName);
        if (attribute != null) {
            attribute.setValue(value);
            if (attribute.getSubmitMode() === "never") {
                attribute.setSubmitMode("dirty");
            }
        }
    }

    public setSubmitMode<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName, value: Xrm.SubmitMode): void {
        const attribute = this.getXrm().getAttribute(fieldName);
        if (attribute != null) {
            attribute.setSubmitMode(value);
        }
    }

    public setAllDisabled<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName, disabled: boolean): void {
        const attribute = this.getXrm().getAttribute(fieldName);
        if (attribute != null) {
            attribute.controls.forEach((control) => {
                control.setDisabled(disabled);
            });
        }
    }

    public setDisabled<ColumnName extends ColumnControlNames<TableDefinition>>(fieldName: CanSetDisabled<TableDefinition, ColumnName> extends true ? ColumnName : never, disabled: boolean): void {
        const control = this.getXrm().getControl(fieldName);
        if (control != null) {
            const disabledControl = control as Xrm.Controls.UiCanSetDisabledElement;
            disabledControl.setDisabled(disabled);
        }
    }

    public getDisabled<ColumnName extends ColumnControlNames<TableDefinition>>(fieldName: CanGetDisabled<TableDefinition, ColumnName> extends true ? ColumnName : never): boolean {
        const control = this.getXrm().getControl(fieldName);
        
        if (control == null) return false;
        
        const disabledControl = control as Xrm.Controls.UiCanGetDisabledElement;
        return disabledControl.getDisabled();
    }

    public isMobile(): boolean {
        return this.getGlobalContext()?.client.getClient() === "Mobile";
    }

    public isTablet(): boolean {
        return this.getGlobalContext()?.client.getFormFactor() === XrmEnum.ClientFormFactor.Tablet;
    }

    public addOnChange(
        fieldNames: LogNameOrLogNames<TableDefinition>,
        callback: (utils: UtilsApi<TableDefinition>) => void
    ): boolean {
        let isOk = true;

        if (fieldNames != null) {
            const onChangeHandler = this.onChangeHandler(callback);
            
            if (typeof fieldNames === "string") {
                isOk = this.internalAddOnChange(fieldNames, onChangeHandler);
            } else {
                for (let i = 0; i < fieldNames.length; i++) {
                    isOk = this.internalAddOnChange(fieldNames[i], onChangeHandler) && isOk;
                }
                if (!isOk) {
                    for (let j = 0; j < fieldNames.length; j++) {
                        this.internalRemoveOnChange(fieldNames[j], onChangeHandler);
                    }
                }
            }
        }
        return isOk;
    }

    public addOnChangeAndExecute(fieldNames: LogNameOrLogNames<TableDefinition>, callback: (utils: UtilsApi<TableDefinition>) => void): void {
        if (this.addOnChange(fieldNames, callback)) {
            callback(new UtilsClass(this.formContext));
        }
    }

    public addOnSave(callback: Xrm.Events.SaveEventHandler): void {
        this.getXrm().data.entity.addOnSave(callback);
    }

    public addFieldNotification<ColumnName extends ColumnControlNames<TableDefinition>>(fieldName: IsStandardControl<TableDefinition, ColumnName> extends true ? ColumnName : never, message: string): void {
        const control = this.getXrm().getControl(fieldName);
        if (control != null) {
            const standardControl = control as Xrm.Controls.StandardControl;
            standardControl.setNotification(message, fieldName);
        }
    }

    public clearFieldNotification<ColumnName extends ColumnControlNames<TableDefinition>>(fieldName: IsStandardControl<TableDefinition, ColumnName> extends true ? ColumnName : never): void {
        const control = this.getXrm().getControl(fieldName);
        if (control != null) {
            const standardControl = control as Xrm.Controls.StandardControl;
            standardControl.clearNotification(fieldName);
        }
    }

    public setFormNotification(message: string, level: XrmEnum.FormNotificationLevel, id: string): void {
        this.getXrm().ui.setFormNotification(message, level, id);
    }

    public clearFormNotification(id: string): void {
        this.getXrm().ui.clearFormNotification(id);
    }

    public addDays(origDate: Date, days: number): Date {
        return new Date(origDate.getTime() + days * 24 * 60 * 60 * 1000);
    }

    public isCreate(): boolean {
        const formTypeValue = this.getXrm().ui.getFormType();
        return formTypeValue === 1 || formTypeValue === 5;
    }

    public isModifiable(): boolean {
        const formTypeValue = this.getXrm().ui.getFormType();
        return formTypeValue === 1 || formTypeValue === 2;
    }

    public alert(message: string, callback?: () => void): void {
        const alertStrings = { confirmButtonLabel: "Ok", text: message };
        const alertOptions = { height: 200, width: 400 };

        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
            () => callback?.(),
            (error) => console.log(error.message),
        );
    }

    public confirm(message: string, functionWhenOk: () => void, functionWhenCancel: () => void): void {
        const confirmStrings = {
            text: message,
            title: "Confirmation",
            confirmButtonLabel: "Oui",
            cancelButtonLabel: "Non",
        };
        const confirmOptions = { height: 200, width: 450 };

        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then((success) => {
            if (success.confirmed) {
                functionWhenOk();
            } else {
                functionWhenCancel();
            }
        });
    }

    public closeForm(forceClose = false): void {
        if (forceClose) {
            const dirtyAttributes = this.getXrm().getAttribute((attribute) => attribute.getIsDirty());
            for (let i = 0; i < dirtyAttributes.length; i++) {
                this.setSubmitMode(dirtyAttributes[i].getName(), "never");
            }
        }

        this.getXrm().ui.close();
    }

    public getWRParameter(searchFor: string): string {
        const query = window.location.search.substring(1);
        const parms = query.split("&");

        for (let i = 0; i < parms.length; i++) {
            const pos = parms[i].indexOf("=");
            if (pos > 0 && searchFor === parms[i].substring(0, pos)) {
                return parms[i].substring(pos + 1);
            }
        }
        return "";
    }

    public fireOnChange(fieldName: string): void {
        const attribute = this.getXrm().getAttribute(fieldName);
        if (attribute != null) {
            attribute.fireOnChange();
        }
    }

    public IsUserRecordOwner(): boolean {
        const userId = this.getUserInfos()?.userId;
        const owner = this.getValue("ownerid") as Array<{ id: string }> | null;

        if (!userId || !owner?.[0]?.id) {
            return false;
        }

        return owner[0].id.indexOf(userId) >= 0;
    }

    public openForm(entityFormOptions: EntityFormOptions, parameters?: any) {
        if (typeof entityFormOptions === "string" && typeof parameters === "string") {
            entityFormOptions = { entityName: entityFormOptions, entityId: parameters };
            parameters = null;
        }

        return window.Xrm.Navigation.openForm(entityFormOptions as Xrm.Navigation.EntityFormOptions, parameters);
    }

    public isNullOrEmpty(value: string | null | undefined): boolean {
        return value == null || value.length === 0;
    }

    public openWebRessource(webResourceName: string, webResourceData: string | null, width: number, height: number): void {
        const windowOptions : Xrm.Navigation.OpenWebresourceOptions = { height, width, openInNewWindow: false };
        window.Xrm.Navigation.openWebResource(webResourceName, windowOptions, webResourceData ?? undefined);
    }

    public isDirty<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName): boolean {
        return this.isOnForm(fieldName) ? (this.getXrm().getAttribute(fieldName)?.getIsDirty() ?? false) : false;
    }

    public getControl<ControlName extends ColumnControlNames<TableDefinition>>(controlName: ControlName) : ControlTypeFromColumnName<TableDefinition, ControlName> | null {
        return this.getXrm().getControl(controlName);
    }

    public getAttribute<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName) {
        return this.getXrm().getAttribute(fieldName);
    }

    public getAllAttributes() {
        return this.getXrm().data.entity.attributes;
    }

    public refreshWebRessource(webRessourceName: string): void {
        const webResourceControl = this.getXrm().getControl<Xrm.Controls.FramedControl>(webRessourceName);
        if (webResourceControl != null) {
            const src = webResourceControl.getSrc();
            webResourceControl.setSrc("");
            webResourceControl.setSrc(src);
        }
    }

    public setFilter<ColumnName extends LookupColumnLogNames<TableDefinition>>(fieldName: ColumnName, getFilter: string | (() => string)): void {
        const attribute = this.getXrm().getAttribute(fieldName);
        if (attribute == null) {
            throw new Error(`The field ${fieldName} is not on the form`);
        }

        attribute.controls.forEach((control) => {
            const controlName = control.getName();
            const controlType = control.getControlType();

            if (controlType !== "lookup") {
                throw new Error("Utils.setFilter fieldName is not a lookup.");
            }

            const lookupControl = control as Xrm.Controls.LookupControl;
            
            if (typeof this.preSearchCache[controlName] !== "undefined") {
                lookupControl.removePreSearch(this.preSearchCache[controlName]!);
            }

            this.preSearchCache[controlName] = () => {
                const filter = typeof getFilter === "function" ? getFilter() : getFilter;
                lookupControl.addCustomFilter(filter);
            };

            lookupControl.addPreSearch(this.preSearchCache[controlName]!);
        });
    }

    public setActiveStage(stageId: string, callbackFunction: () => void): void {
        this.getXrm().data.process.setActiveStage(stageId, callbackFunction);
    }

    public getActiveStage() {
        return this.getXrm().data.process.getActiveStage();
    }

    public extractDomain(url: string): string {
        let domain: string;

        if (url.indexOf("://") > -1) {
            domain = url.split("/")[2];
        } else {
            domain = url.split("/")[0];
        }

        return domain.split(":")[0];
    }

    public getOptions<OptionSetFieldName extends OptionSetColumnLogNames<TableDefinition>>(fieldName: OptionSetFieldName) {
        const attribute = this.getXrm().getAttribute(fieldName);
        if (attribute == null) {
            return [];
        }
        const optionSetAttribute = attribute as Xrm.Attributes.OptionSetAttribute;
        
        return optionSetAttribute.getOptions();
    }

    public SetOptionSet<ColumnName extends OptionSetColumnLogNames<TableDefinition>>(fieldName: ColumnName, setText: string | null): void {
        try {
            const control = this.getXrm().getAttribute(fieldName);

            if (control?.getAttributeType() === "optionset") {
                const optionSetControl = control as Xrm.Attributes.OptionSetAttribute;
                
                if (setText === "" || setText == null) {
                    optionSetControl.setValue(null);
                } else {
                    const controlOpts = optionSetControl.getOptions();
                    for (let i = 0; i <= controlOpts.length - 1; i++) {
                        if (controlOpts[i].text.toLowerCase() === setText.toLowerCase()) {
                            optionSetControl.setValue(controlOpts[i].value);
                            return;
                        }
                    }
                }
            } else {
                alert("Invalid field type or field not found. Field type should be OptionSet");
            }
        } catch (e) {
            alert(`Error in SetOptionSet: fieldName = ${fieldName} setText = ${setText} error = ${e}`);
        }
    }

    public getType<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName): Xrm.Attributes.AttributeType | undefined {
        return this.getXrm().getAttribute(fieldName)?.getAttributeType();
    }

    public isIE(): boolean {
        const ua = window.navigator.userAgent;

        if (ua.indexOf("MSIE ") > 0 || ua.indexOf("Trident/") > 0 || ua.indexOf("Edge/") > 0) {
            return true;
        }
        return false;
    }

    public setContext(context: any): void {
        if (this.formContext != null && context == null) {
            return;
        }

        if (context != null && typeof context.getFormContext === "function") {
            this.formContext = context.getFormContext();
        } else {
            this.formContext = context;
        }
    }

    private internalAddOnChange<ColumnName extends ColumnLogNames<TableDefinition>>(fieldName: ColumnName, callback: Xrm.Events.ContextSensitiveHandler): boolean {
        const attribute = this.getXrm().getAttribute(fieldName);
        if (attribute != null) {
            attribute.addOnChange(callback);
            return true;
        }
        return false;
    }
    
    private onChangeHandler(callback: (utils: UtilsApi<TableDefinition>) => void): Xrm.Events.ContextSensitiveHandler {
        return (context) => {
            const utils = new UtilsClass<TableDefinition, Xrm.Events.EventContext>(context);
            callback(utils);
        };
    }

    private internalRemoveOnChange(fieldName: string, callback: Xrm.Events.ContextSensitiveHandler): void {
        const attribute = this.getXrm().getAttribute(fieldName);
        if (attribute != null) {
            attribute.removeOnChange(callback);
        }
    }

    private getGlobalContext() {
        if (typeof window.GetGlobalContext === "function") {
            return window.GetGlobalContext();
        }

        if (window.Xrm?.Utility?.getGlobalContext != null) {
            return window.Xrm.Utility.getGlobalContext();
        }

        if (window.parent?.Xrm?.Utility?.getGlobalContext != null) {
            return window.parent.Xrm.Utility.getGlobalContext();
        }

        if (window.opener?.Xrm?.Utility?.getGlobalContext != null) {
            return window.opener.Xrm.Utility.getGlobalContext();
        }

        return null;
    }

    private getXrm(): XrmFramework.FormContext<TableDefinition> & Xrm.FormContext {
        return this.formContext;
    }
}

export { UtilsClass };

declare global {
    interface Date {
        yyyymmdd(): string;
    }
}

Date.prototype.yyyymmdd = function (): string {
    const mm = `${this.getMonth() + 1}`;
    const dd = `${this.getDate()}`;
    return `${this.getFullYear()}-${!mm[1] ? "0" : ""}${mm}-${!dd[1] ? "0" : ""}${dd}`;
};
