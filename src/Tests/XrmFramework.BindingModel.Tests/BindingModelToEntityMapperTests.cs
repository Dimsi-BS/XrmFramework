// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using NUnit.Framework;
using Microsoft.Xrm.Sdk;
using XrmFramework.BindingModel.Tests.Fakes;

namespace XrmFramework.BindingModel.Tests
{
    /// <summary>
    /// Unit tests for the IBindingModel -> Entity mapping path
    /// (exercised through <see cref="BindingModelHelper.ToEntity"/>).
    /// </summary>
    [TestFixture]
    public class BindingModelToEntityMapperTests
    {
        // The mapper calls IOrganizationService only for CrmLookupAttribute + AllowNotExisting
        // scenarios, which we do not exercise here.  A null service is sufficient for all tests below.
        private static readonly IOrganizationService NullService = null!;

        // ------------------------------------------------------------------
        // Entity name
        // ------------------------------------------------------------------

        [Test]
        public void ToEntity_SetsCorrectEntityLogicalName()
        {
            var model = new ContactModel { Id = Guid.NewGuid() };
            var entity = model.ToEntity(NullService);

            Assert.AreEqual(ContactDefinition.EntityName, entity.LogicalName);
        }

        // ------------------------------------------------------------------
        // Id mapping
        // ------------------------------------------------------------------

        [Test]
        public void ToEntity_SetsEntityId()
        {
            var id = Guid.NewGuid();
            var model = new ContactModel { Id = id };
            var entity = model.ToEntity(NullService);

            Assert.AreEqual(id, entity.Id);
        }

        // ------------------------------------------------------------------
        // String attribute
        // ------------------------------------------------------------------

        [Test]
        public void ToEntity_StringAttribute_IsWrittenToEntity()
        {
            var model = new ContactModel
            {
                Id = Guid.NewGuid(),
                FullName = "John Doe"
            };

            var entity = model.ToEntity(NullService);

            Assert.AreEqual("John Doe", entity.GetAttributeValue<string>(ContactDefinition.Columns.FullName));
        }

        [Test]
        public void ToEntity_NullStringAttribute_WritesNull()
        {
            var model = new ContactModel { Id = Guid.NewGuid(), FullName = null };
            var entity = model.ToEntity(NullService);

            Assert.IsTrue(entity.Contains(ContactDefinition.Columns.FullName));
            Assert.IsNull(entity.GetAttributeValue<string>(ContactDefinition.Columns.FullName));
        }

        // ------------------------------------------------------------------
        // Picklist (OptionSet)
        // ------------------------------------------------------------------

        [Test]
        public void ToEntity_PicklistEnum_WritesOptionSetValue()
        {
            var model = new ContactModel
            {
                Id = Guid.NewGuid(),
                StatusCode = ContactStatus.Active
            };

            var entity = model.ToEntity(NullService);

            var optionSet = entity.GetAttributeValue<OptionSetValue>(ContactDefinition.Columns.StatusCode);
            Assert.IsNotNull(optionSet);
            Assert.AreEqual(1, optionSet.Value);
        }

        [Test]
        public void ToEntity_PicklistEnum_NullValueEnum_WritesNull()
        {
            // ContactStatus.Null has integer value 0 and name "Null" -> should produce null OptionSetValue.
            var model = new ContactModel
            {
                Id = Guid.NewGuid(),
                StatusCode = ContactStatus.Null
            };

            var entity = model.ToEntity(NullService);

            var optionSet = entity.GetAttributeValue<OptionSetValue>(ContactDefinition.Columns.StatusCode);
            Assert.IsNull(optionSet);
        }

        // ------------------------------------------------------------------
        // Money attribute
        // ------------------------------------------------------------------

        [Test]
        public void ToEntity_MoneyAttribute_WritesMoneyValue()
        {
            var model = new ContactModel
            {
                Id = Guid.NewGuid(),
                Revenue = 9999.99m
            };

            var entity = model.ToEntity(NullService);

            var money = entity.GetAttributeValue<Money>(ContactDefinition.Columns.Revenue);
            Assert.IsNotNull(money);
            Assert.AreEqual(9999.99m, money.Value);
        }

        [Test]
        public void ToEntity_MoneyAttribute_NullDecimal_WritesNull()
        {
            var model = new ContactModel { Id = Guid.NewGuid(), Revenue = null };
            var entity = model.ToEntity(NullService);

            Assert.IsTrue(entity.Contains(ContactDefinition.Columns.Revenue));
            Assert.IsNull(entity.GetAttributeValue<Money>(ContactDefinition.Columns.Revenue));
        }

        // ------------------------------------------------------------------
        // Boolean attribute
        // ------------------------------------------------------------------

        [Test]
        public void ToEntity_BooleanAttribute_True_WritesTrue()
        {
            var model = new ContactModel { Id = Guid.NewGuid(), IsActive = true };
            var entity = model.ToEntity(NullService);

            Assert.AreEqual(true, entity.GetAttributeValue<bool?>(ContactDefinition.Columns.IsActive));
        }

        // ------------------------------------------------------------------
        // DateTime: MinValue should be treated as null
        // ------------------------------------------------------------------

        [Test]
        public void ToEntity_DateTimeMinValue_WritesNull()
        {
            var model = new ContactModel
            {
                Id = Guid.NewGuid(),
                BirthDate = DateTime.MinValue
            };

            var entity = model.ToEntity(NullService);

            Assert.IsTrue(entity.Contains(ContactDefinition.Columns.BirthDate));
            Assert.IsNull(entity.GetAttributeValue<DateTime?>(ContactDefinition.Columns.BirthDate));
        }

        [Test]
        public void ToEntity_ValidDateTime_WritesDateTime()
        {
            var date = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var model = new ContactModel { Id = Guid.NewGuid(), BirthDate = date };
            var entity = model.ToEntity(NullService);

            Assert.AreEqual(date, entity.GetAttributeValue<DateTime?>(ContactDefinition.Columns.BirthDate));
        }

        // ------------------------------------------------------------------
        // InitializedProperties tracking (BindingModelBase)
        // ------------------------------------------------------------------

        [Test]
        public void ToEntity_WithBindingModelBase_OnlyInitializedPropertiesAreWritten()
        {
            // Only set FullName; Email and Revenue should not appear in the entity.
            var model = new ContactModelWithBase
            {
                Id = Guid.NewGuid(),
                FullName = "Alice"
            };

            var entity = model.ToEntity(NullService);

            Assert.IsTrue(entity.Contains(ContactDefinition.Columns.FullName),
                "FullName was set, it should be present.");
            Assert.IsFalse(entity.Contains(ContactDefinition.Columns.Email),
                "Email was NOT set, it should be absent.");
            Assert.IsFalse(entity.Contains(ContactDefinition.Columns.Revenue),
                "Revenue was NOT set, it should be absent.");
        }

        [Test]
        public void ToEntity_WithBindingModelBase_MultipleInitializedProperties_AllWritten()
        {
            var model = new ContactModelWithBase
            {
                Id = Guid.NewGuid(),
                FullName = "Bob",
                Email = "bob@example.com",
                Revenue = 500m
            };

            var entity = model.ToEntity(NullService);

            Assert.AreEqual("Bob", entity.GetAttributeValue<string>(ContactDefinition.Columns.FullName));
            Assert.AreEqual("bob@example.com", entity.GetAttributeValue<string>(ContactDefinition.Columns.Email));
            var money = entity.GetAttributeValue<Money>(ContactDefinition.Columns.Revenue);
            Assert.IsNotNull(money);
            Assert.AreEqual(500m, money.Value);
        }

        [Test]
        public void ToEntity_WithBindingModelBase_NothingSet_EntityHasNoAttributes()
        {
            var model = new ContactModelWithBase { Id = Guid.NewGuid() };
            var entity = model.ToEntity(NullService);

            // Only the Id-based attribute (contactid) might be present; no user-set attributes.
            Assert.IsFalse(entity.Contains(ContactDefinition.Columns.FullName));
            Assert.IsFalse(entity.Contains(ContactDefinition.Columns.Email));
            Assert.IsFalse(entity.Contains(ContactDefinition.Columns.Revenue));
        }

        // ------------------------------------------------------------------
        // Lookup (Guid typed) attribute
        // ------------------------------------------------------------------

        [Test]
        public void ToEntity_LookupGuidAttribute_WritesEntityReference()
        {
            var accountId = Guid.NewGuid();
            var model = new ContactModel { Id = Guid.NewGuid(), AccountId = accountId };
            var entity = model.ToEntity(NullService);

            var entityRef = entity.GetAttributeValue<EntityReference>(ContactDefinition.Columns.AccountId);
            Assert.IsNotNull(entityRef);
            Assert.AreEqual(accountId, entityRef.Id);
            Assert.AreEqual(AccountDefinition.EntityName, entityRef.LogicalName);
        }

        [Test]
        public void ToEntity_LookupGuidAttribute_EmptyGuid_WritesNull()
        {
            var model = new ContactModel { Id = Guid.NewGuid(), AccountId = Guid.Empty };
            var entity = model.ToEntity(NullService);

            // An empty Guid should map to a null EntityReference.
            var entityRef = entity.GetAttributeValue<EntityReference>(ContactDefinition.Columns.AccountId);
            Assert.IsNull(entityRef);
        }
    }
}
