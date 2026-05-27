// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;
using NUnit.Framework;
using XrmFramework.BindingModel;

namespace XrmFramework.Tests.BindingModel.Attributes
{
    [TestFixture]
    public class CrmMappingAttributeTests
    {
        // ─────────────────────────────────────────────────────────────
        //  Constructor
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Constructor_SetsAttributeName()
        {
            var attr = new CrmMappingAttribute("firstname");

            Assert.AreEqual("firstname", attr.AttributeName);
        }

        // ─────────────────────────────────────────────────────────────
        //  Default property values
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void FollowLink_DefaultIsFalse()
        {
            var attr = new CrmMappingAttribute("firstname");

            Assert.IsFalse(attr.FollowLink);
        }

        [Test]
        public void IsValidForUpdate_DefaultIsTrue()
        {
            var attr = new CrmMappingAttribute("firstname");

            Assert.IsTrue(attr.IsValidForUpdate);
        }

        [Test]
        public void LookupInfo_DefaultIsNone()
        {
            var attr = new CrmMappingAttribute("parentaccountid");

            Assert.AreEqual(LookupAttributeInfo.None, attr.LookupInfo);
        }

        [Test]
        public void DiffStringComparisonBehavior_DefaultIsInvariantCulture()
        {
            var attr = new CrmMappingAttribute("firstname");

            Assert.AreEqual(StringComparison.InvariantCulture, attr.DiffStringComparisonBehavior);
        }

        // ─────────────────────────────────────────────────────────────
        //  Mutable properties
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void FollowLink_CanBeSetToTrue()
        {
            var attr = new CrmMappingAttribute("parentaccountid") { FollowLink = true };

            Assert.IsTrue(attr.FollowLink);
        }

        [Test]
        public void IsValidForUpdate_CanBeSetToFalse()
        {
            var attr = new CrmMappingAttribute("createdon") { IsValidForUpdate = false };

            Assert.IsFalse(attr.IsValidForUpdate);
        }

        [Test]
        public void LookupInfo_CanBeSetToId()
        {
            var attr = new CrmMappingAttribute("parentaccountid") { LookupInfo = LookupAttributeInfo.Id };

            Assert.AreEqual(LookupAttributeInfo.Id, attr.LookupInfo);
        }

        [Test]
        public void LookupInfo_CanBeSetToName()
        {
            var attr = new CrmMappingAttribute("parentaccountidname") { LookupInfo = LookupAttributeInfo.Name };

            Assert.AreEqual(LookupAttributeInfo.Name, attr.LookupInfo);
        }

        [Test]
        public void DiffStringComparisonBehavior_CanBeChanged()
        {
            var attr = new CrmMappingAttribute("firstname")
            {
                DiffStringComparisonBehavior = StringComparison.OrdinalIgnoreCase
            };

            Assert.AreEqual(StringComparison.OrdinalIgnoreCase, attr.DiffStringComparisonBehavior);
        }

        // ─────────────────────────────────────────────────────────────
        //  AttributeUsage
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void CrmMappingAttribute_TargetsProperty()
        {
            var usage = typeof(CrmMappingAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Property, usage.ValidOn);
        }
    }
}
