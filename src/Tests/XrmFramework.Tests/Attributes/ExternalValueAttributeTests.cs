// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using NUnit.Framework;

namespace XrmFramework.Tests.Attributes
{
    [TestFixture]
    public class ExternalValueAttributeTests
    {
        // ────────────────────────────────────────────────────────────
        //  Constructeur et propriétés
        // ────────────────────────────────────────────────────────────

        [Test]
        public void Constructor_SetsExternalValue()
        {
            var attr = new ExternalValueAttribute("my_external_value");
            Assert.AreEqual("my_external_value", attr.ExternalValue);
        }

        [Test]
        public void Constructor_EmptyString_SetsEmptyExternalValue()
        {
            var attr = new ExternalValueAttribute(string.Empty);
            Assert.AreEqual(string.Empty, attr.ExternalValue);
        }

        // ────────────────────────────────────────────────────────────
        //  Réflexion — attribut appliqué sur un champ d'enum
        // ────────────────────────────────────────────────────────────

        private enum PaymentMethod
        {
            [ExternalValue("card")]  CreditCard = 1,
            [ExternalValue("wire")]  BankWire   = 2,
            Cash                               = 3   // No attribute
        }

        [Test]
        public void Attribute_RetrievableFromEnumField()
        {
            var field = typeof(PaymentMethod).GetField(nameof(PaymentMethod.CreditCard));
            var attr  = field!.GetCustomAttribute<ExternalValueAttribute>();

            Assert.IsNotNull(attr);
            Assert.AreEqual("card", attr!.ExternalValue);
        }

        [Test]
        public void Attribute_MissingOnField_ReturnsNull()
        {
            var field = typeof(PaymentMethod).GetField(nameof(PaymentMethod.Cash));
            var attr  = field!.GetCustomAttribute<ExternalValueAttribute>();

            Assert.IsNull(attr);
        }

        [Test]
        public void Attribute_AllMarkedFields_HaveCorrectValues()
        {
            Assert.AreEqual("card", typeof(PaymentMethod).GetField(nameof(PaymentMethod.CreditCard))
                !.GetCustomAttribute<ExternalValueAttribute>()!.ExternalValue);
            Assert.AreEqual("wire", typeof(PaymentMethod).GetField(nameof(PaymentMethod.BankWire))
                !.GetCustomAttribute<ExternalValueAttribute>()!.ExternalValue);
        }

        // ────────────────────────────────────────────────────────────
        //  AttributeUsage — targets Field only
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ExternalValueAttribute_TargetsField()
        {
            var usage = typeof(ExternalValueAttribute)
                .GetCustomAttribute<System.AttributeUsageAttribute>();

            Assert.IsNotNull(usage);
            Assert.IsTrue(usage!.ValidOn.HasFlag(System.AttributeTargets.Field));
        }
    }
}
