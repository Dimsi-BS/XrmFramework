/// <reference path="../../xrmFramework/index.d.ts" />

import { FormScript } from "../../xrmFramework/formScript";
import { registerFormScript } from "../../xrmFramework/registerScript";

class AccountForm extends FormScript<AccountDefinition> {

  public getName(): string {
    return "Forms.AccountForm";
  }

  protected internalOnLoad(utils: UtilsApi<AccountDefinition>): void {
    utils.addOnChangeAndExecute("name", this.manageNameChange)
  }

  private manageNameChange(utils: UtilsApi<AccountDefinition>): void {
    const name = utils.getValue("name");

  }
}

registerFormScript(new AccountForm());
