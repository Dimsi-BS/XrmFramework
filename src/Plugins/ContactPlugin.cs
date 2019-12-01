using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Model;

namespace Plugins
{
    public class ContactPlugin : Plugin
    {
        protected override void AddSteps()
        {
            AddStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, ContactDefinition.EntityName, nameof(FormatLastName));
        }

        public ContactPlugin(string unsecuredConfig, string securedConfig) : base(unsecuredConfig, securedConfig)
        {
        }
    }
}
