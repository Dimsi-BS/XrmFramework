// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;

namespace XrmFramework.Tests.Extensions
{
    [TestFixture]
    public class DecimalExtensionsTests
    {
        // ────────────────────────────────────────────────────────────
        //  ToMoney
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ToMoney_WithValue_ReturnsMoney()
        {
            decimal? value = 42.50m;
            var money = value.ToMoney();

            Assert.IsNotNull(money);
            Assert.AreEqual(42.50m, money!.Value);
        }

        [Test]
        public void ToMoney_WithNull_ReturnsNull()
        {
            decimal? value = null;
            Assert.IsNull(value.ToMoney());
        }

        [Test]
        public void ToMoney_WithZero_ReturnsMoney()
        {
            decimal? value = 0m;
            var money = value.ToMoney();

            Assert.IsNotNull(money);
            Assert.AreEqual(0m, money!.Value);
        }

        [Test]
        public void ToMoney_WithNegativeValue_ReturnsMoney()
        {
            decimal? value = -100.99m;
            var money = value.ToMoney();

            Assert.IsNotNull(money);
            Assert.AreEqual(-100.99m, money!.Value);
        }

        [Test]
        public void ToMoney_WithLargeValue_ReturnsMoney()
        {
            decimal? value = 1_000_000m;
            var money = value.ToMoney();

            Assert.IsNotNull(money);
            Assert.AreEqual(1_000_000m, money!.Value);
        }
    }
}
