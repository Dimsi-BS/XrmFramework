// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Microsoft.Xrm.Sdk;
using XrmFramework.BindingModel.Tests.Fakes;

namespace XrmFramework.BindingModel.Tests
{
    /// <summary>
    /// Unit tests for the Entity -> IBindingModel mapping path
    /// (exercised through <see cref="BindingModelHelper.ToBindingModel{T}(Entity)"/>).
    /// </summary>
    [TestFixture]
    public class EntityToBindingModelMapperTests
    {
        // ------------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------------

        private static Entity CreateContact(Guid? id = null)
        {
            var e = new Entity(ContactDefinition.EntityName)
            {
                Id = id ?? Guid.NewGuid()
            };
            return e;
        }

        // ------------------------------------------------------------------
        // Null / wrong entity
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_NullEntity_ReturnsNull()
        {
            Entity? nullEntity = null;
            var result = nullEntity!.ToBindingModel<ContactModel>();

            Assert.IsNull(result);
        }

        [Test]
        public void ToBindingModel_WrongLogicalName_ReturnsNull()
        {
            var entity = new Entity("lead") { Id = Guid.NewGuid() };
            var result = entity.ToBindingModel<ContactModel>();

            Assert.IsNull(result);
        }

        // ------------------------------------------------------------------
        // Id mapping
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_EntityId_IsMappedToModelId()
        {
            var expectedId = Guid.NewGuid();
            var entity = CreateContact(expectedId);

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedId, model.Id);
        }

        // ------------------------------------------------------------------
        // String attribute
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_StringAttribute_MapsCorrectly()
        {
            var entity = CreateContact();
            entity[ContactDefinition.Columns.FullName] = "John Doe";

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual("John Doe", model.FullName);
        }

        [Test]
        public void ToBindingModel_StringAttribute_AbsentFromEntity_RemainsDefault()
        {
            var entity = CreateContact();
            // FullName is intentionally not set.

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.IsNull(model.FullName);
        }

        // ------------------------------------------------------------------
        // Boolean attribute
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_BooleanAttribute_True_MapsCorrectly()
        {
            var entity = CreateContact();
            entity[ContactDefinition.Columns.IsActive] = true;

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(true, model.IsActive);
        }

        [Test]
        public void ToBindingModel_BooleanAttribute_False_MapsCorrectly()
        {
            var entity = CreateContact();
            entity[ContactDefinition.Columns.IsActive] = false;

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(false, model.IsActive);
        }

        // ------------------------------------------------------------------
        // DateTime attribute
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_DateTimeAttribute_MapsCorrectly()
        {
            var expectedDate = new DateTime(1990, 6, 15, 0, 0, 0, DateTimeKind.Utc);
            var entity = CreateContact();
            entity[ContactDefinition.Columns.BirthDate] = expectedDate;

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedDate, model.BirthDate);
        }

        // ------------------------------------------------------------------
        // Money attribute
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_MoneyAttribute_MapsToDecimal()
        {
            var entity = CreateContact();
            entity[ContactDefinition.Columns.Revenue] = new Money(1234.56m);

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(1234.56m, model.Revenue);
        }

        [Test]
        public void ToBindingModel_MoneyAttribute_NullMoney_MapsToNull()
        {
            var entity = CreateContact();
            entity[ContactDefinition.Columns.Revenue] = null;

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.IsNull(model.Revenue);
        }

        [Test]
        public void ToBindingModel_MoneyAttribute_AbsentFromEntity_RemainsNull()
        {
            var entity = CreateContact();
            // Revenue is intentionally not set.

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.IsNull(model.Revenue);
        }

        // ------------------------------------------------------------------
        // Picklist (OptionSet) attribute
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_PicklistAttribute_MapsToEnum()
        {
            var entity = CreateContact();
            entity[ContactDefinition.Columns.StatusCode] = new OptionSetValue(1);

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(ContactStatus.Active, model.StatusCode);
        }

        [Test]
        public void ToBindingModel_PicklistAttribute_NullOptionSet_LeavesDefault()
        {
            var entity = CreateContact();
            entity[ContactDefinition.Columns.StatusCode] = null;

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(ContactStatus.Null, model.StatusCode);
        }

        // ------------------------------------------------------------------
        // MultiSelectPicklist attribute
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_MultiSelectPicklist_MapsToEnumList()
        {
            var entity = CreateContact();
            entity[ContactDefinition.Columns.Interests] = new OptionSetValueCollection(
                new List<OptionSetValue>
                {
                    new(1), // Sports
                    new(3)  // Travel
                });

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.That(model.Interests, Is.EquivalentTo(new[] { ContactInterest.Sports, ContactInterest.Travel }));
        }

        [Test]
        public void ToBindingModel_MultiSelectPicklist_EmptyCollection_LeavesEmptyList()
        {
            var entity = CreateContact();
            entity[ContactDefinition.Columns.Interests] = null;

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Interests.Count);
        }

        // ------------------------------------------------------------------
        // Lookup attribute (Guid typed)
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_LookupAttribute_MapsGuidFromEntityReference()
        {
            var accountId = Guid.NewGuid();
            var entity = CreateContact();
            entity[ContactDefinition.Columns.AccountId] =
                new EntityReference(AccountDefinition.EntityName, accountId);

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(accountId, model.AccountId);
        }

        [Test]
        public void ToBindingModel_LookupAttribute_AbsentFromEntity_LeavesGuidEmpty()
        {
            // When the lookup attribute is not in the entity at all, HasValueFor returns false
            // and the property keeps its default value (Guid.Empty).
            var entity = CreateContact();
            // AccountId is intentionally not set.

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(Guid.Empty, model.AccountId);
        }

        // ------------------------------------------------------------------
        // MapMany (batch mapping with cache)
        // ------------------------------------------------------------------

        [Test]
        public void MapMany_WithMultipleEntities_ReturnsAllModels()
        {
            var entities = Enumerable.Range(0, 5)
                .Select(i =>
                {
                    var e = CreateContact();
                    e[ContactDefinition.Columns.FullName] = $"Contact {i}";
                    return e;
                })
                .ToList();

            var models = entities.ToBindingModel<ContactModel>().ToList();

            Assert.AreEqual(5, models.Count);
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual($"Contact {i}", models[i].FullName);
            }
        }

        [Test]
        public void MapMany_EmptyCollection_ReturnsEmptyEnumerable()
        {
            var result = Enumerable.Empty<Entity>().ToBindingModel<ContactModel>();
            Assert.AreEqual(0, result.Count());
        }

        // ------------------------------------------------------------------
        // Multiple scalar attributes in one entity
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_MultipleAttributes_AllMappedCorrectly()
        {
            var contactId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var birthDate = new DateTime(1985, 3, 22, 0, 0, 0, DateTimeKind.Utc);

            var entity = new Entity(ContactDefinition.EntityName) { Id = contactId };
            entity[ContactDefinition.Columns.FullName] = "Jane Smith";
            entity[ContactDefinition.Columns.Email] = "jane@example.com";
            entity[ContactDefinition.Columns.IsActive] = true;
            entity[ContactDefinition.Columns.BirthDate] = birthDate;
            entity[ContactDefinition.Columns.Revenue] = new Money(5000m);
            entity[ContactDefinition.Columns.StatusCode] = new OptionSetValue(1);
            entity[ContactDefinition.Columns.AccountId] =
                new EntityReference(AccountDefinition.EntityName, accountId);

            var model = entity.ToBindingModel<ContactModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(contactId, model.Id);
            Assert.AreEqual("Jane Smith", model.FullName);
            Assert.AreEqual("jane@example.com", model.Email);
            Assert.AreEqual(true, model.IsActive);
            Assert.AreEqual(birthDate, model.BirthDate);
            Assert.AreEqual(5000m, model.Revenue);
            Assert.AreEqual(ContactStatus.Active, model.StatusCode);
            Assert.AreEqual(accountId, model.AccountId);
        }
    }
}
