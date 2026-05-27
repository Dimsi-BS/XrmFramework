// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;
using NUnit.Framework;

namespace XrmFramework.Tests.Attributes
{
    [TestFixture]
    public class OptionSetDefinitionAttributeTests
    {
        // ─────────────────────────────────────────────────────────────
        //  Constructor(string logicalName)
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void SingleArgConstructor_SetsLogicalName()
        {
            var attr = new OptionSetDefinitionAttribute("contact_statuscode");

            Assert.AreEqual("contact_statuscode", attr.LogicalName);
            Assert.IsNull(attr.EntityName);
        }

        // ─────────────────────────────────────────────────────────────
        //  Constructor(string entityName, string fieldName)
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void TwoArgConstructor_SetsEntityNameAndLogicalName()
        {
            var attr = new OptionSetDefinitionAttribute("contact", "statuscode");

            Assert.AreEqual("contact", attr.EntityName);
            Assert.AreEqual("statuscode", attr.LogicalName);
        }

        // ─────────────────────────────────────────────────────────────
        //  AttributeUsage
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void OptionSetDefinitionAttribute_TargetsClassAndEnum()
        {
            var usage = typeof(OptionSetDefinitionAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.IsTrue(usage.ValidOn.HasFlag(AttributeTargets.Class));
            Assert.IsTrue(usage.ValidOn.HasFlag(AttributeTargets.Enum));
        }
    }
}
