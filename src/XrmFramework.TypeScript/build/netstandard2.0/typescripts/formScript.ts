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
     * Point d'entrée à appeler dans l'événement OnLoad du formulaire.
     */
    public onLoad(context: Xrm.Events.LoadEventContext): void {
        const utils = new UtilsClass<TTable, Xrm.Events.LoadEventContext>(context);
        this.internalOnLoad(utils);
    }

    /**
     * Permet aux classes filles d'initialiser leur logique spécifique.
     * Exemples : masquer/afficher des champs, mettre à jour des règles, etc.
     */
    protected abstract internalOnLoad(utils: UtilsApi<TTable>): void;

}

