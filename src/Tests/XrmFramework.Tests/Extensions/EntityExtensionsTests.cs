// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.Tests.Extensions
{
    [TestFixture]
    public class EntityExtensionsTests
    {
        private enum Status
        {
            Null   = 0,
            Active = 1,
            Inactive = 2,
            Pending  = 3
        }

        // ────────────────────────────────────────────────────────────
        //  GetAttributeValue<T> — (newEntity, preEntity, field)
        // ────────────────────────────────────────────────────────────

        [Test]
        public void GetAttributeValue_FieldInNewEntity_ReturnsNewEntityValue()
        {
            var newEntity = new Entity("contact") { ["name"] = "New Name" };
            var preEntity = new Entity("contact") { ["name"] = "Old Name" };

            var result = newEntity.GetAttributeValue<string>(preEntity, "name");
            Assert.AreEqual("New Name", result);
        }

        [Test]
        public void GetAttributeValue_FieldOnlyInPreEntity_ReturnsPreEntityValue()
        {
            var newEntity = new Entity("contact");
            var preEntity = new Entity("contact") { ["name"] = "Pre Name" };

            var result = newEntity.GetAttributeValue<string>(preEntity, "name");
            Assert.AreEqual("Pre Name", result);
        }

        [Test]
        public void GetAttributeValue_FieldInNeither_ReturnsDefaultValue()
        {
            var newEntity = new Entity("contact");
            var result    = newEntity.GetAttributeValue<string>((Entity)null!, "name");
            Assert.IsNull(result);
        }

        [Test]
        public void GetAttributeValue_WithExplicitDefault_ReturnsDefaultWhenMissing()
        {
            var entity = new Entity("contact");
            var result = entity.GetAttributeValue<string>("name", "DefaultValue");
            Assert.AreEqual("DefaultValue", result);
        }

        [Test]
        public void GetAttributeValue_SingleEntity_FieldPresent_ReturnsValue()
        {
            var entity = new Entity("contact") { ["age"] = 42 };
            Assert.AreEqual(42, entity.GetAttributeValue<int>("age", 0));
        }

        // ────────────────────────────────────────────────────────────
        //  GetAliasedValue<T>
        // ────────────────────────────────────────────────────────────

        [Test]
        public void GetAliasedValue_FieldPresent_ReturnsUnwrappedValue()
        {
            var entity = new Entity("contact")
            {
                ["alias.name"] = new AliasedValue("contact", "name", "John Doe")
            };

            var result = entity.GetAliasedValue<string>("alias.name");
            Assert.AreEqual("John Doe", result);
        }

        [Test]
        public void GetAliasedValue_FieldMissing_ReturnsDefault()
        {
            var entity = new Entity("contact");
            var result = entity.GetAliasedValue<string>("alias.name");
            Assert.IsNull(result);
        }

        [Test]
        public void GetAliasedValue_WithExplicitDefault_ReturnDefaultWhenMissing()
        {
            var entity  = new Entity("contact");
            var result  = entity.GetAliasedValue<string>("alias.name", "fallback");
            Assert.AreEqual("fallback", result);
        }

        // ────────────────────────────────────────────────────────────
        //  GetOptionSetValue<T> / SetOptionSetValue<T>
        // ────────────────────────────────────────────────────────────

        [Test]
        public void GetOptionSetValue_FieldPresent_ReturnsCorrectEnumValue()
        {
            var entity = new Entity("contact");
            entity["statuscode"] = new OptionSetValue(1);

            var result = entity.GetOptionSetValue<Status>("statuscode");
            Assert.AreEqual(Status.Active, result);
        }

        [Test]
        public void GetOptionSetValue_FieldMissing_ReturnsDefault()
        {
            var entity = new Entity("contact");
            var result = entity.GetOptionSetValue<Status>("statuscode");
            Assert.AreEqual(default(Status), result);
        }

        [Test]
        public void GetOptionSetValue_WithDefaultParam_ReturnsDefaultWhenMissing()
        {
            var entity = new Entity("contact");
            var result = entity.GetOptionSetValue<Status>("statuscode", Status.Pending);
            Assert.AreEqual(Status.Pending, result);
        }

        [Test]
        public void SetOptionSetValue_NonNullEnum_SetsOptionSetValue()
        {
            var entity = new Entity("contact");
            entity.SetOptionSetValue("statuscode", Status.Active);

            var stored = entity.GetAttributeValue<OptionSetValue>("statuscode");
            Assert.IsNotNull(stored);
            Assert.AreEqual(1, stored!.Value);
        }

        [Test]
        public void SetOptionSetValue_NullEnum_SetsNullOptionSetValue()
        {
            var entity = new Entity("contact");
            entity.SetOptionSetValue("statuscode", Status.Null);

            var stored = entity.GetAttributeValue<OptionSetValue>("statuscode");
            Assert.IsNull(stored);
        }

        [Test]
        public void GetOptionSetValue_FromPreEntity_WhenNotInNewEntity()
        {
            var newEntity = new Entity("contact");
            var preEntity = new Entity("contact");
            preEntity["statuscode"] = new OptionSetValue(2);

            var result = newEntity.GetOptionSetValue<Status>(preEntity, "statuscode");
            Assert.AreEqual(Status.Inactive, result);
        }

        // ────────────────────────────────────────────────────────────
        //  GetOptionSetValues<T> / SetOptionSetValues<T>
        // ────────────────────────────────────────────────────────────

        [Test]
        public void SetOptionSetValues_MultipleValues_StoresCollection()
        {
            var entity = new Entity("contact");
            entity.SetOptionSetValues("categories", Status.Active, Status.Inactive);

            var stored = entity.GetAttributeValue<OptionSetValueCollection>("categories");
            Assert.IsNotNull(stored);
            Assert.AreEqual(2, stored!.Count);
            Assert.IsTrue(stored.Any(o => o.Value == 1));
            Assert.IsTrue(stored.Any(o => o.Value == 2));
        }

        [Test]
        public void SetOptionSetValues_EmptyArray_StoresNull()
        {
            var entity = new Entity("contact");
            entity.SetOptionSetValues<Status>("categories");

            Assert.IsNull(entity["categories"]);
        }

        [Test]
        public void GetOptionSetValues_FieldPresent_ReturnsEnumList()
        {
            var entity = new Entity("contact");
            entity.SetOptionSetValues("categories", Status.Active, Status.Pending);

            var result = entity.GetOptionSetValues<Status>("categories");
            Assert.AreEqual(2, result.Count);
            Assert.That(result.ToList(), Does.Contain(Status.Active));
            Assert.That(result.ToList(), Does.Contain(Status.Pending));
        }

        [Test]
        public void GetOptionSetValues_FieldMissing_ReturnsEmptyList()
        {
            var entity = new Entity("contact");
            var result = entity.GetOptionSetValues<Status>("categories");
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void SetOptionSetValues_FromICollection_Works()
        {
            var entity = new Entity("contact");
            ICollection<Status> values = new List<Status> { Status.Active, Status.Inactive };
            entity.SetOptionSetValues("categories", values);

            var stored = entity.GetAttributeValue<OptionSetValueCollection>("categories");
            Assert.IsNotNull(stored);
            Assert.AreEqual(2, stored!.Count);
        }

        // ────────────────────────────────────────────────────────────
        //  EmptyIds
        // ────────────────────────────────────────────────────────────

        [Test]
        public void EmptyIds_ClearsEntityIds()
        {
            var collection = new EntityCollection();
            collection.Entities.Add(new Entity("contact") { Id = Guid.NewGuid() });
            collection.Entities.Add(new Entity("contact") { Id = Guid.NewGuid() });

            collection.EmptyIds();

            foreach (var e in collection.Entities)
            {
                Assert.AreEqual(Guid.Empty, e.Id);
            }
        }

        [Test]
        public void EmptyIds_RemovesSpecifiedFields()
        {
            var collection = new EntityCollection();
            var entity = new Entity("contact") { Id = Guid.NewGuid() };
            entity["name"] = "Test";
            entity["age"]  = 30;
            collection.Entities.Add(entity);

            collection.EmptyIds("name");

            Assert.IsFalse(collection.Entities[0].Contains("name"));
            Assert.IsTrue(collection.Entities[0].Contains("age"));
        }

        // ────────────────────────────────────────────────────────────
        //  MergeWith
        // ────────────────────────────────────────────────────────────

        [Test]
        public void MergeWith_CopiesAllFieldsFromSource()
        {
            var target = new Entity("contact");
            var source = new Entity("contact")
            {
                ["name"]  = "Alice",
                ["email"] = "alice@example.com"
            };

            target.MergeWith(source);

            Assert.AreEqual("Alice", target["name"]);
            Assert.AreEqual("alice@example.com", target["email"]);
        }

        [Test]
        public void MergeWith_CopyOnlyIfNotExist_DoesNotOverwriteExistingFields()
        {
            var target = new Entity("contact") { ["name"] = "Existing" };
            var source = new Entity("contact") { ["name"] = "New", ["email"] = "new@example.com" };

            target.MergeWith(source, copyOnlyIfFieldNotExist: true);

            Assert.AreEqual("Existing", target["name"]);
            Assert.AreEqual("new@example.com", target["email"]);
        }

        [Test]
        public void MergeWith_DefaultBehavior_OverwritesExistingFields()
        {
            var target = new Entity("contact") { ["name"] = "Old" };
            var source = new Entity("contact") { ["name"] = "New" };

            target.MergeWith(source);

            Assert.AreEqual("New", target["name"]);
        }

        // ────────────────────────────────────────────────────────────
        //  CopyField
        // ────────────────────────────────────────────────────────────

        [Test]
        public void CopyField_FieldInSource_CopiesToTarget()
        {
            var source = new Entity("contact") { ["name"] = "Alice" };
            var target = new Entity("contact");

            source.CopyField(target, "name", "fullname");

            Assert.AreEqual("Alice", target["fullname"]);
        }

        [Test]
        public void CopyField_FieldNotInSource_DoesNotCopy()
        {
            var source = new Entity("contact");
            var target = new Entity("contact");

            source.CopyField(target, "name", "fullname");

            Assert.IsFalse(target.Contains("fullname"));
        }

        [Test]
        public void CopyField_FieldNotInSource_UseDefaultValue_SetsNull()
        {
            var source = new Entity("contact");
            var target = new Entity("contact") { ["fullname"] = "Existing" };

            source.CopyField(target, "name", "fullname", useDefaultValue: true);

            Assert.IsNull(target["fullname"]);
        }

        [Test]
        public void CopyField_WithPreImage_SameValueInBoth_DoesNotCopy()
        {
            var source   = new Entity("contact") { ["name"] = "Alice" };
            var preImage = new Entity("contact") { ["name"] = "Alice" };
            var target   = new Entity("contact");

            source.CopyField(preImage, target, "name", "name");

            Assert.IsFalse(target.Contains("name"));
        }

        [Test]
        public void CopyField_WithPreImage_DifferentValue_Copies()
        {
            var source   = new Entity("contact") { ["name"] = "New Alice" };
            var preImage = new Entity("contact") { ["name"] = "Old Alice" };
            var target   = new Entity("contact");

            source.CopyField(preImage, target, "name", "name");

            Assert.AreEqual("New Alice", target["name"]);
        }

        // ────────────────────────────────────────────────────────────
        //  Merge
        // ────────────────────────────────────────────────────────────

        [Test]
        public void Merge_BothEntities_CombinesAttributes()
        {
            var source   = new Entity("contact", Guid.NewGuid()) { ["name"] = "Alice" };
            var preImage = new Entity("contact") { ["email"] = "alice@example.com" };

            var merged = source.Merge(preImage);

            Assert.AreEqual("Alice", merged["name"]);
            Assert.AreEqual("alice@example.com", merged["email"]);
        }

        [Test]
        public void Merge_SourceOverridesPreImage()
        {
            var id       = Guid.NewGuid();
            var source   = new Entity("contact", id) { ["name"] = "New Name" };
            var preImage = new Entity("contact") { ["name"] = "Old Name", ["email"] = "x@y.com" };

            var merged = source.Merge(preImage);

            Assert.AreEqual("New Name", merged["name"]);
            Assert.AreEqual("x@y.com", merged["email"]);
        }

        [Test]
        public void Merge_NullSource_ReturnsPreImage()
        {
            Entity? source   = null;
            var    preImage = new Entity("contact") { ["email"] = "alice@example.com" };

            var merged = source.Merge(preImage);

            Assert.AreEqual("alice@example.com", merged["email"]);
        }

        [Test]
        public void Merge_NullPreImage_ReturnsSourceBasedEntity()
        {
            var id     = Guid.NewGuid();
            var source = new Entity("contact", id) { ["name"] = "Alice" };

            var merged = source.Merge(null);

            Assert.AreEqual("Alice", merged["name"]);
        }
    }
}
