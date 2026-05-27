// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using XrmFramework.BindingModel;

namespace XrmFramework.Tests.BindingModel.Attributes
{
    [TestFixture]
    public class CrmRelationshipAttributeTests
    {
        // ─────────────────────────────────────────────────────────────
        //  ChildRelationshipAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void ChildRelationshipAttribute_GetRelationship_ReturnsExpectedSchemaName()
        {
            var attr = new ChildRelationshipAttribute("account_contact");

            var relationship = attr.GetRelationship();

            Assert.AreEqual("account_contact", relationship.SchemaName);
        }

        [Test]
        public void ChildRelationshipAttribute_GetRelationship_RoleIsReferenced()
        {
            var attr = new ChildRelationshipAttribute("account_contact");

            var relationship = attr.GetRelationship();

            Assert.AreEqual(EntityRole.Referenced, relationship.PrimaryEntityRole);
        }

        [Test]
        public void ChildRelationshipAttribute_IsValidForUpdate_DefaultIsTrue()
        {
            var attr = new ChildRelationshipAttribute("account_contact");

            Assert.IsTrue(attr.IsValidForUpdate);
        }

        [Test]
        public void ChildRelationshipAttribute_IsValidForUpdate_CanBeSetToFalse()
        {
            var attr = new ChildRelationshipAttribute("account_contact") { IsValidForUpdate = false };

            Assert.IsFalse(attr.IsValidForUpdate);
        }

        // ─────────────────────────────────────────────────────────────
        //  ManyToManyRelationshipAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void ManyToManyRelationshipAttribute_GetRelationship_ReturnsExpectedSchemaName()
        {
            var attr = new ManyToManyRelationshipAttribute("systemuser_teams");

            var relationship = attr.GetRelationship();

            Assert.AreEqual("systemuser_teams", relationship.SchemaName);
        }

        [Test]
        public void ManyToManyRelationshipAttribute_GetRelationship_RoleIsReferenced()
        {
            var attr = new ManyToManyRelationshipAttribute("systemuser_teams");

            var relationship = attr.GetRelationship();

            Assert.AreEqual(EntityRole.Referenced, relationship.PrimaryEntityRole);
        }

        [Test]
        public void ManyToManyRelationshipAttribute_UpdateStrategy_DefaultIsNone()
        {
            var attr = new ManyToManyRelationshipAttribute("systemuser_teams");

            Assert.AreEqual(UpdateStrategy.None, attr.UpdateStrategy);
        }

        [Test]
        public void ManyToManyRelationshipAttribute_UpdateStrategy_CanBeSetToAdd()
        {
            var attr = new ManyToManyRelationshipAttribute("systemuser_teams")
            {
                UpdateStrategy = UpdateStrategy.Add
            };

            Assert.AreEqual(UpdateStrategy.Add, attr.UpdateStrategy);
        }

        [Test]
        public void ManyToManyRelationshipAttribute_UpdateStrategy_CanBeSetToReplace()
        {
            var attr = new ManyToManyRelationshipAttribute("systemuser_teams")
            {
                UpdateStrategy = UpdateStrategy.Replace
            };

            Assert.AreEqual(UpdateStrategy.Replace, attr.UpdateStrategy);
        }

        // ─────────────────────────────────────────────────────────────
        //  GetRelationship returns a new instance each call
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void GetRelationship_ReturnsNewInstanceEachCall()
        {
            var attr = new ChildRelationshipAttribute("account_contact");

            var r1 = attr.GetRelationship();
            var r2 = attr.GetRelationship();

            Assert.AreNotSame(r1, r2);
        }

        // ─────────────────────────────────────────────────────────────
        //  AttributeUsage
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void CrmRelationshipAttribute_TargetsPropertyAndField()
        {
            var usage = typeof(CrmRelationshipAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.IsTrue(usage.ValidOn.HasFlag(AttributeTargets.Property));
            Assert.IsTrue(usage.ValidOn.HasFlag(AttributeTargets.Field));
        }
    }
}
