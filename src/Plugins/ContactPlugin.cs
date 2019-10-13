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

        public void FormatLastName(IPluginContext context, IService service)
        {
            #region Parameters Check

            if (context == null) throw new ArgumentNullException(nameof(context));
            if (service == null) throw new ArgumentNullException(nameof(service));

            #endregion

            var contact = context.GetInputParameter<Entity>(InputParameters.Target);

            var firstName = contact.GetAttributeValue<string>(ContactDefinition.Columns.FirstName);
            var lastName = contact.GetAttributeValue<string>(ContactDefinition.Columns.LastName)?.ToUpperInvariant();

            contact[ContactDefinition.Columns.LastName] = lastName;
            contact[ContactDefinition.Columns.FullName] = string.IsNullOrEmpty(firstName) ? lastName : $"{firstName} {lastName}";


        }

        public ContactPlugin(string unsecuredConfig, string securedConfig) : base(unsecuredConfig, securedConfig)
        {
        }
    }
}
