// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.Tests.Extensions
{
    [TestFixture]
    public class EntityReferenceExtensionsTests
    {
        private static readonly Guid SampleId = new Guid("aaaabbbb-cccc-dddd-eeee-ffffffffffff");

        // ────────────────────────────────────────────────────────────
        //  ToEntity
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ToEntity_NullReference_ReturnsNull()
        {
            EntityReference? reference = null;
            Assert.IsNull(reference.ToEntity());
        }

        [Test]
        public void ToEntity_ReferenceWithId_ReturnsEntityWithSameLogicalNameAndId()
        {
            var reference = new EntityReference("contact", SampleId);
            var entity = reference.ToEntity();

            Assert.IsNotNull(entity);
            Assert.AreEqual("contact", entity!.LogicalName);
            Assert.AreEqual(SampleId, entity.Id);
        }

        [Test]
        public void ToEntity_ReferenceWithKeyAttributes_CopiesKeyAttributes()
        {
            var reference = new EntityReference("contact");
            reference.KeyAttributes.Add(new KeyValuePair<string, object>("alternatekey", "ABC123"));

            var entity = reference.ToEntity();

            Assert.IsNotNull(entity);
            Assert.IsTrue(entity!.KeyAttributes.ContainsKey("alternatekey"));
            Assert.AreEqual("ABC123", entity.KeyAttributes["alternatekey"]);
        }

        [Test]
        public void ToEntity_ReferenceWithIdAndKeyAttributes_CopiesBoth()
        {
            var reference = new EntityReference("account", SampleId);
            reference.KeyAttributes.Add(new KeyValuePair<string, object>("externalid", "EXT-001"));

            var entity = reference.ToEntity();

            Assert.AreEqual(SampleId, entity!.Id);
            Assert.IsTrue(entity.KeyAttributes.ContainsKey("externalid"));
        }

        [Test]
        public void ToEntity_ReferenceWithNoKeyAttributes_EntityHasEmptyKeyAttributes()
        {
            var reference = new EntityReference("lead", SampleId);
            var entity = reference.ToEntity();

            Assert.IsNotNull(entity);
            Assert.AreEqual(0, entity!.KeyAttributes.Count);
        }

        [Test]
        public void ToEntity_PreservesLogicalName()
        {
            var reference = new EntityReference("opportunity", SampleId);
            var entity = reference.ToEntity();

            Assert.AreEqual("opportunity", entity!.LogicalName);
        }
    }
}
