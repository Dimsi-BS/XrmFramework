// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Model;
using Plugins;

namespace Acquereurs.Plugins
{
    public class AccountPlugin : Plugin
    {
        /// <summary>
        /// AddSteps allows to generate SdkMessageProcessing items.
        /// </summary>
        protected override void AddSteps()
        {
            AddStep(Stages.PreValidation, Messages.Create, Modes.Synchronous, AccountDefinition.EntityName, nameof(AssignContactOwnerToAccount));

            AddStep(Stages.PostOperation, Messages.Update, Modes.Synchronous, AccountDefinition.EntityName, nameof(UpdateSubContactInfos));
        }

        /// <summary>
        /// Assign the account to the parent Contact's owner 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="accountService"></param>
        /// <param name="systemUserService"></param>
        public void AssignContactOwnerToAccount(IPluginContext context, IAccountService accountService, ISystemuserService systemUserService)
        {
            #region Parameters check

            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (accountService == null)
                throw new ArgumentNullException(nameof(accountService));
            if (systemUserService == null)
                throw new ArgumentNullException(nameof(systemUserService));

            #endregion

            var account = context.GetInputParameter<Entity>(InputParameters.Target);

            var primaryContactRef = account.GetAttributeValue<EntityReference>(AccountDefinition.Columns.PrimaryContactId);

            if (primaryContactRef != null)
            {
                var contact = accountService.Retrieve(primaryContactRef, ContactDefinition.Columns.OwnerId);

                var ownerRef = contact.GetAttributeValue<EntityReference>(ContactDefinition.Columns.OwnerId);

                // Check if the user is active
                if (systemUserService.IsActiveUser(ownerRef))
                {
                    account[AccountDefinition.Columns.OwnerId] = ownerRef;
                }
                else
                {
                    throw new InvalidPluginExecutionException("The owner in disabled.");
                }
            }
        }

        [FilteringAttributes(AccountDefinition.Columns.Address1_Line1, AccountDefinition.Columns.Address1_Line2)]
        [PreImage(AccountDefinition.Columns.Address1_Line1, AccountDefinition.Columns.Address1_Line2)]
        public void UpdateSubContactInfos(IPluginContext context, IAccountService accountService)
        {
            #region Parameters check

            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (accountService == null)
                throw new ArgumentNullException(nameof(accountService));

            #endregion

            var account = context.GetInputParameter<Entity>(InputParameters.Target);

            var preImage = context.HasPreImage(PreImageName) ? context.GetPreImage(PreImageName) : null;


            var address1 = account.GetAttributeValue<string>(preImage, AccountDefinition.Columns.Address1_Line1);
            var address2 = account.GetAttributeValue<string>(preImage, AccountDefinition.Columns.Address1_Line2);

            ICollection<EntityReference> subContacts = accountService.GetSubContactRefs(account.ToEntityReference());

            foreach (var contactRef in subContacts)
            {
                var updatedContact = contactRef.ToEntity();

                updatedContact[ContactDefinition.Columns.Address1_Line1] = address1;
                updatedContact[ContactDefinition.Columns.Address1_Line2] = address2;

                accountService.Update(updatedContact, true);
            }
        }
    }
}
