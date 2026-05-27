// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using NUnit.Framework;
using System;

namespace XrmFramework.Tests.Attributes
{
    [TestFixture]
    public class CrmLookupAttributeTests
    {
        // ─────────────────────────────────────────────────────────────
        //  Constructor(string entityName, string attributeName)
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void CrmLookupAttribute_StringConstructor_SetsTargetEntityNameAndAttributeName()
        {
            var attr = new CrmLookupAttribute("contact", "parentaccountid");

            Assert.AreEqual("contact", attr.TargetEntityName);
            Assert.AreEqual("parentaccountid", attr.AttributeName);
            Assert.IsFalse(attr.AllowNotExisting);
        }

        [Test]
        public void CrmLookupAttribute_StringConstructor_AllowNotExisting_True()
        {
            var attr = new CrmLookupAttribute("contact", "parentaccountid", allowNotExisting: true);

            Assert.IsTrue(attr.AllowNotExisting);
        }

        [Test]
        public void CrmLookupAttribute_StringConstructor_AllowNotExisting_Default_False()
        {
            var attr = new CrmLookupAttribute("contact", "parentaccountid");
            Assert.IsFalse(attr.AllowNotExisting);
        }

        // ─────────────────────────────────────────────────────────────
        //  Constructor(Type definitionType, string attributeName)
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void CrmLookupAttribute_TypeConstructor_SetsAttributeName()
        {
            var attr = new CrmLookupAttribute(typeof(string), "parentaccountid");

            Assert.AreEqual("parentaccountid", attr.AttributeName);
            Assert.IsFalse(attr.AllowNotExisting);
        }

        [Test]
        public void CrmLookupAttribute_TypeConstructor_AllowNotExisting_True()
        {
            var attr = new CrmLookupAttribute(typeof(string), "parentaccountid", allowNotExisting: true);

            Assert.IsTrue(attr.AllowNotExisting);
        }

        [Test]
        public void CrmLookupAttribute_TypeConstructor_TargetEntityName_IsNull()
        {
            var attr = new CrmLookupAttribute(typeof(string), "parentaccountid");
            // TargetEntityName comes from the string constructor only
            Assert.IsNull(attr.TargetEntityName);
        }

        // ─────────────────────────────────────────────────────────────
        //  RelationshipName
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void CrmLookupAttribute_RelationshipName_CanBeSet()
        {
            var attr = new CrmLookupAttribute("contact", "parentaccountid")
            {
                RelationshipName = "contact_parent_account"
            };

            Assert.AreEqual("contact_parent_account", attr.RelationshipName);
        }

        [Test]
        public void CrmLookupAttribute_RelationshipName_DefaultIsNull()
        {
            var attr = new CrmLookupAttribute("contact", "parentaccountid");
            Assert.IsNull(attr.RelationshipName);
        }

        // ─────────────────────────────────────────────────────────────
        //  AttributeUsage
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void CrmLookupAttribute_AllowsMultiple_OnPropertyAndField()
        {
            var usage = typeof(CrmLookupAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.IsTrue(usage.AllowMultiple);
            Assert.IsTrue(usage.ValidOn.HasFlag(AttributeTargets.Property));
            Assert.IsTrue(usage.ValidOn.HasFlag(AttributeTargets.Field));
        }
    }
}
