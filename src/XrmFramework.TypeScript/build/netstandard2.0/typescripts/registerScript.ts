import {FormScriptBase, RibbonScriptBase} from "./formScript";

export function registerFormScript<TFormScript extends FormScriptBase>(value: TFormScript): void {
    
    const module = {
        onLoad: (context: Xrm.Events.LoadEventContext) => {
            value.onLoad(context);
        }
    }
    
    registerScript(value.getName(), module);
}

export function registerRibbonScript<TRibbonScript extends RibbonScriptBase>(value: TRibbonScript): void {

        registerScript(value.getName(), value);
}

function registerScript(moduleName: string, value: unknown): void {

    const parts = moduleName.split(".");
    let current: any = window;

    for (let i = 0; i < parts.length - 1; i++) {
        const part = parts[i];
        current[part] = current[part] || {};
        current = current[part];
    }

    current[parts[parts.length - 1]] = value;
}
