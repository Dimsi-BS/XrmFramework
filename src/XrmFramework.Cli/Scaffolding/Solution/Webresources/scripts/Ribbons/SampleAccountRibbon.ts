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
