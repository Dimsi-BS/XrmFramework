using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Plugins;
using Microsoft.Xrm.Sdk;

namespace Acquereurs.Plugins
{
    public class TestPlugin : Plugin
    {
        protected override void AddSteps()
        {
            AddStep(Stages.PreOperation, Messages.Update, Modes.Synchronous, ContratsDefinition.EntityName, nameof(OnPreUpdate));
        }


        public void OnPreUpdate(IPluginContext context, IContractService contractService)
        {
            #region Parameters check

            if (context == null) throw new ArgumentNullException(nameof(context));

            #endregion
            Guid contractId = new Guid("D6F3C8CA-34F9-E811-8146-5065F38A5A01");
            var contacts = contractService.GetListContacts(contractId);
        }




    }
}
