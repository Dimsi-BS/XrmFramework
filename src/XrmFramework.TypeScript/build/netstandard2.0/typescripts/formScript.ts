/// <reference path="./xrmFramework.d.ts" />
/// <reference path="./Utils.ts" />

import {UtilsClass} from "./Utils";

abstract class ScriptBase {
    public abstract getName(): string;
}

export abstract class FormScriptBase extends ScriptBase {    
    public abstract onLoad(context: Xrm.Events.LoadEventContext): void;
}

export abstract class RibbonScriptBase extends ScriptBase {}

export abstract class RibbonScript<TTable extends Table> extends RibbonScriptBase {
    protected getUtilsApi(context: XrmFramework.FormContext<TTable>): UtilsApi<TTable> {
        return new UtilsClass(context);
    }
}

export abstract class FormScript<TTable extends Table> extends FormScriptBase {

    /**
     * Entry point to call from the form's OnLoad event.
     */
    public onLoad(context: Xrm.Events.LoadEventContext): void {
        const utils = new UtilsClass<TTable, Xrm.Events.LoadEventContext>(context);
        this.internalOnLoad(utils);
    }

    /**
     * Allows derived classes to initialize their specific logic.
     * Examples: hiding/showing fields, updating rules, etc.
     */
    protected abstract internalOnLoad(utils: UtilsApi<TTable>): void;

}

