// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using NUnit.Framework;

namespace XrmFramework.Tests.Context
{
    [TestFixture]
    public class FixedDateTimeProviderTests
    {
        // ────────────────────────────────────────────────────────────
        //  Construction with a UTC date
        // ────────────────────────────────────────────────────────────

        [Test]
        public void Constructor_UtcDate_UtcNowReturnsExactSameDate()
        {
            var utcDate = new DateTime(2024, 6, 15, 12, 30, 0, DateTimeKind.Utc);
            var provider = new FixedDateTimeProvider(utcDate);

            Assert.AreEqual(utcDate, provider.UtcNow);
            Assert.AreEqual(DateTimeKind.Utc, provider.UtcNow.Kind);
        }

        [Test]
        public void Constructor_UtcDate_NowReturnsLocalTime()
        {
            var utcDate = new DateTime(2024, 6, 15, 12, 30, 0, DateTimeKind.Utc);
            var provider = new FixedDateTimeProvider(utcDate);

            var expected = utcDate.ToLocalTime();
            Assert.AreEqual(expected, provider.Now);
        }

        // ────────────────────────────────────────────────────────────
        //  Construction with a local date
        // ────────────────────────────────────────────────────────────

        [Test]
        public void Constructor_LocalDate_UtcNowReturnsUTCEquivalent()
        {
            var localDate = new DateTime(2024, 6, 15, 12, 30, 0, DateTimeKind.Local);
            var provider  = new FixedDateTimeProvider(localDate);

            var expectedUtc = localDate.ToUniversalTime();
            Assert.AreEqual(expectedUtc, provider.UtcNow);
            Assert.AreEqual(DateTimeKind.Utc, provider.UtcNow.Kind);
        }

        // ────────────────────────────────────────────────────────────
        //  UtcNow always returns exactly the same (fixed) date
        // ────────────────────────────────────────────────────────────

        [Test]
        public void UtcNow_CalledMultipleTimes_ReturnsSameValue()
        {
            var utcDate  = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var provider = new FixedDateTimeProvider(utcDate);

            var first  = provider.UtcNow;
            var second = provider.UtcNow;
            Assert.AreEqual(first, second);
        }

        [Test]
        public void Now_CalledMultipleTimes_ReturnsSameValue()
        {
            var utcDate  = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var provider = new FixedDateTimeProvider(utcDate);

            var first  = provider.Now;
            var second = provider.Now;
            Assert.AreEqual(first, second);
        }

        // ────────────────────────────────────────────────────────────
        //  Today — local day of the fixed date, at midnight
        // ────────────────────────────────────────────────────────────

        [Test]
        public void Today_ReturnsLocalDateWithoutTimeComponent()
        {
            var utcDate  = new DateTime(2024, 6, 15, 12, 30, 45, DateTimeKind.Utc);
            var provider = new FixedDateTimeProvider(utcDate);

            Assert.AreEqual(utcDate.ToLocalTime().Date, provider.Today);
            Assert.AreEqual(TimeSpan.Zero, provider.Today.TimeOfDay);
        }

        [Test]
        public void Today_MatchesTheDateOfNow()
        {
            var utcDate  = new DateTime(2024, 6, 15, 12, 30, 45, DateTimeKind.Utc);
            var provider = new FixedDateTimeProvider(utcDate);

            Assert.AreEqual(provider.Now.Date, provider.Today);
        }

        [Test]
        public void Today_CalledMultipleTimes_ReturnsSameValue()
        {
            var utcDate  = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var provider = new FixedDateTimeProvider(utcDate);

            var first  = provider.Today;
            var second = provider.Today;
            Assert.AreEqual(first, second);
        }

        // ────────────────────────────────────────────────────────────
        //  Use for deterministic relative date calculations
        // ────────────────────────────────────────────────────────────

        [Test]
        public void UtcNow_PlusDays_ProducesExpectedFutureDate()
        {
            var utcDate  = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);
            var provider = new FixedDateTimeProvider(utcDate);

            var expiry = provider.UtcNow.AddDays(30);

            Assert.AreEqual(new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Utc), expiry);
        }

        // ────────────────────────────────────────────────────────────
        //  Implements IDateTimeProvider
        // ────────────────────────────────────────────────────────────

        [Test]
        public void FixedDateTimeProvider_ImplementsIDateTimeProvider()
        {
            var utcDate  = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc);
            IDateTimeProvider provider = new FixedDateTimeProvider(utcDate);

            Assert.AreEqual(utcDate, provider.UtcNow);
        }

        [Test]
        public void FixedDateTimeProvider_ImplementsIXrmFrameworkService()
        {
            var provider = new FixedDateTimeProvider(DateTime.UtcNow);
            Assert.That(provider, Is.InstanceOf<IXrmFrameworkService>());
        }

        // ────────────────────────────────────────────────────────────
        //  Boundary dates
        // ────────────────────────────────────────────────────────────

        [Test]
        public void Constructor_MinDateUtc_Works()
        {
            var minDate  = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
            var provider = new FixedDateTimeProvider(minDate);
            Assert.AreEqual(minDate, provider.UtcNow);
        }

        [Test]
        public void Constructor_MaxDateUtc_Works()
        {
            var maxDate  = DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
            var provider = new FixedDateTimeProvider(maxDate);
            Assert.AreEqual(maxDate, provider.UtcNow);
        }
    }
}
