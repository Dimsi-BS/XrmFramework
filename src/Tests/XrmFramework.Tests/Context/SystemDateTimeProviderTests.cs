// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using NUnit.Framework;

namespace XrmFramework.Tests.Context
{
    [TestFixture]
    public class SystemDateTimeProviderTests
    {
        // ────────────────────────────────────────────────────────────
        //  Singleton
        // ────────────────────────────────────────────────────────────

        [Test]
        public void Instance_IsNotNull()
        {
            Assert.IsNotNull(SystemDateTimeProvider.Instance);
        }

        [Test]
        public void Instance_ReturnsSameReference()
        {
            var a = SystemDateTimeProvider.Instance;
            var b = SystemDateTimeProvider.Instance;
            Assert.AreSame(a, b);
        }

        // ────────────────────────────────────────────────────────────
        //  UtcNow
        // ────────────────────────────────────────────────────────────

        [Test]
        public void UtcNow_HasUtcKind()
        {
            Assert.AreEqual(DateTimeKind.Utc, SystemDateTimeProvider.Instance.UtcNow.Kind);
        }

        [Test]
        public void UtcNow_IsCloseToRealUtcNow()
        {
            var before = DateTime.UtcNow.AddSeconds(-1);
            var value  = SystemDateTimeProvider.Instance.UtcNow;
            var after  = DateTime.UtcNow.AddSeconds(1);

            Assert.IsTrue(value >= before && value <= after,
                $"Expected UtcNow between {before:O} and {after:O}, got {value:O}");
        }

        // ────────────────────────────────────────────────────────────
        //  Now
        // ────────────────────────────────────────────────────────────

        [Test]
        public void Now_HasLocalOrUnspecifiedKind()
        {
            var kind = SystemDateTimeProvider.Instance.Now.Kind;
            // DateTime.Now returns Local
            Assert.AreEqual(DateTimeKind.Local, kind);
        }

        [Test]
        public void Now_IsCloseToRealNow()
        {
            var before = DateTime.Now.AddSeconds(-1);
            var value  = SystemDateTimeProvider.Instance.Now;
            var after  = DateTime.Now.AddSeconds(1);

            Assert.IsTrue(value >= before && value <= after,
                $"Expected Now between {before:O} and {after:O}, got {value:O}");
        }

        // ────────────────────────────────────────────────────────────
        //  Implémente IDateTimeProvider
        // ────────────────────────────────────────────────────────────

        [Test]
        public void SystemDateTimeProvider_ImplementsIDateTimeProvider()
        {
            IDateTimeProvider provider = SystemDateTimeProvider.Instance;
            Assert.IsNotNull(provider.UtcNow);
            Assert.IsNotNull(provider.Now);
        }

        [Test]
        public void SystemDateTimeProvider_ImplementsIXrmFrameworkService()
        {
            Assert.That(SystemDateTimeProvider.Instance, Is.InstanceOf<IXrmFrameworkService>());
        }
    }
}
