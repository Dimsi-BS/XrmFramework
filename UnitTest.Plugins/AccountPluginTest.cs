using Acquereurs.Plugins;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Model;
using Plugins;
using Plugins.Tests;
using System;
using System.Collections.Generic;
using UnitTest.Plugins.Utils;
using Xunit;
using Xunit.Abstractions;

namespace UnitTest.Plugins
{
    /// <summary>
    /// Unit tests for "AccountPlugin"
    /// </summary>
    public class AccountPluginTest : PluginTest<AccountPlugin>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="output"></param>
        public AccountPluginTest(ITestOutputHelper output) : base(output)
        {
        }

        /// <summary>
        /// Unit test for the following case: 
        /// Create a new Account with an active Contact Owner
        /// </summary>
        [Fact]
        [PluginContext(Stages.PreValidation, Messages.Create, Modes.Synchronous, AccountDefinition.EntityName)]
        public void CreateAccountWithactiveContactOwner()
        {
            // Define the Contact Owner
            var contactOwner = new Entity()
            {
                Id = Guid.NewGuid(),
                LogicalName = SystemUserDefinition.EntityName,
                Attributes = { { SystemUserDefinition.Columns.IsDisabled, false } }
            };

            // Define the Account's Creator
            var accountCreator = new Entity()
            {
                Id = Guid.NewGuid(),
                LogicalName = SystemUserDefinition.EntityName,
                Attributes = { { SystemUserDefinition.Columns.IsDisabled, false } }
            };

            // Define the Principal Contact
            var principalContact = new Entity()
            {
                Id = Guid.NewGuid(),
                LogicalName = ContactDefinition.EntityName,
                Attributes = { { ContactDefinition.Columns.OwnerId, contactOwner.ToEntityReference() } }
            };

            // Define Account
            var accountEnitity = new Entity()
            {
                Id = Guid.NewGuid(),
                LogicalName = AccountDefinition.EntityName,
                Attributes =
                {
                    { AccountDefinition.Columns.OwnerId, accountCreator.ToEntityReference() },
                    { AccountDefinition.Columns.PrimaryContactId, principalContact.ToEntityReference() }
                }
            };
            
            // Create the plugin context
            InputParameters.Add(global::Plugins.InputParameters.Target, accountEnitity);
            Context.PrimaryEntityId = accountEnitity.Id;
            
            // Initialise the records so that they can be retrieved from the database
            FakedContext.Initialize(new List<Entity> { accountEnitity, accountCreator, principalContact, contactOwner });

            // Execute the plugin
            ExecutePlugin();

            var newOwnerRef = accountEnitity.GetAttributeValue<EntityReference>(AccountDefinition.Columns.OwnerId);
            // Assert that the target contains a new attribute
            Assert.True(newOwnerRef.Id == contactOwner.Id);
        }

        /// <summary>
        /// Unit test for the following case: 
        /// Create a new Account with an inactive Contact Owner
        /// </summary>
        [Fact]
        [PluginContext(Stages.PreValidation, Messages.Create, Modes.Synchronous, AccountDefinition.EntityName)]
        public void CreateAccountWithInactiveContactOwner()
        {   
            // Define the Contact Owner
            var contactOwner = new Entity()
            {
                Id = Guid.NewGuid(),
                LogicalName = SystemUserDefinition.EntityName,
                Attributes = { { SystemUserDefinition.Columns.IsDisabled, true } }
            };

            // Define the Account's Creator
            var accountCreator = new Entity()
            {
                Id = Guid.NewGuid(),
                LogicalName = SystemUserDefinition.EntityName,
                Attributes = { { SystemUserDefinition.Columns.IsDisabled, false } }
            };

            // Define the Principal Contact
            var principalContact = new Entity()
            {
                Id = Guid.NewGuid(),
                LogicalName = ContactDefinition.EntityName,
                Attributes = { { ContactDefinition.Columns.OwnerId, contactOwner.ToEntityReference() }, }
            };

            // Define Account
            var accountEntity = new Entity()
            {
                Id = Guid.NewGuid(),
                LogicalName = AccountDefinition.EntityName,
                Attributes =
                {
                    { AccountDefinition.Columns.OwnerId, accountCreator.ToEntityReference() },
                    { AccountDefinition.Columns.PrimaryContactId, principalContact.ToEntityReference() }
                }
            };

            // Create the plugin context
            InputParameters.Add(global::Plugins.InputParameters.Target, accountEntity);
            Context.PrimaryEntityId = accountEntity.Id;

            // Initialise the records so that they can be retrieved from the database
            FakedContext.Initialize(new List<Entity> { accountEntity, accountCreator, principalContact, contactOwner });
            try
            {
                // Execute the plugin
                ExecutePlugin();

                // This test should go to catch either it should raise en error
                Assert.True(1 == 2);
            }
            catch (InvalidPluginExecutionException ex)
            {
                Assert.True(ex.Message == "Le propriétaire du contact principal est désactivé. Veuillez contacter votre administrateur.");
            }       
        }
    }
}
