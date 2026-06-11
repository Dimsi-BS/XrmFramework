// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using NUnit.Framework;

namespace XrmFramework.Tests.Attributes
{
    [TestFixture]
    public class CrmEntityAttributeTests
    {
        // ────────────────────────────────────────────────────────────
        //  Constructeur et propriétés
        // ────────────────────────────────────────────────────────────

        [Test]
        public void Constructor_SetsEntityName()
        {
            var attr = new CrmEntityAttribute("contact");
            Assert.AreEqual("contact", attr.EntityName);
        }

        [Test]
        public void ValidForCreate_DefaultIsTrue()
        {
            var attr = new CrmEntityAttribute("account");
            Assert.IsTrue(attr.ValidForCreate);
        }

        [Test]
        public void AllowDeactivation_DefaultIsTrue()
        {
            var attr = new CrmEntityAttribute("account");
            Assert.IsTrue(attr.AllowDeactivation);
        }

        [Test]
        public void ValidForCreate_CanBeSetToFalse()
        {
            var attr = new CrmEntityAttribute("task") { ValidForCreate = false };
            Assert.IsFalse(attr.ValidForCreate);
        }

        [Test]
        public void AllowDeactivation_CanBeSetToFalse()
        {
            var attr = new CrmEntityAttribute("task") { AllowDeactivation = false };
            Assert.IsFalse(attr.AllowDeactivation);
        }

        // ────────────────────────────────────────────────────────────
        //  Réflexion — attribut appliqué sur une classe
        // ────────────────────────────────────────────────────────────
        #pragma warning disable XRM0200
        [CrmEntity("contact", ValidForCreate = true, AllowDeactivation = false)]
        #pragma warning restore XRM0200
        private class ContactDefinitionStub { }

        [Test]
        public void Attribute_RetrievableViaReflection()
        {
            var attr = typeof(ContactDefinitionStub)
                .GetCustomAttribute<CrmEntityAttribute>();

            Assert.IsNotNull(attr);
            Assert.AreEqual("contact", attr!.EntityName);
            Assert.IsTrue(attr.ValidForCreate);
            Assert.IsFalse(attr.AllowDeactivation);
        }

        // ────────────────────────────────────────────────────────────
        //  AttributeUsage
        // ────────────────────────────────────────────────────────────

        [Test]
        public void CrmEntityAttribute_TargetsClassAndInterface()
        {
            var usage = typeof(CrmEntityAttribute)
                .GetCustomAttribute<System.AttributeUsageAttribute>();

            Assert.IsNotNull(usage);
            Assert.IsTrue(usage!.ValidOn.HasFlag(System.AttributeTargets.Class));
            Assert.IsTrue(usage.ValidOn.HasFlag(System.AttributeTargets.Interface));
        }
    }
}
