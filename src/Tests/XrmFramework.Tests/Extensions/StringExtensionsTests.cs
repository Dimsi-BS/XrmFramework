// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using NUnit.Framework;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        private static readonly Guid SampleGuid = new Guid("12345678-1234-1234-1234-123456789012");

        // ────────────────────────────────────────────────────────────
        //  ToGuid
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ToGuid_ValidGuidString_ReturnsGuid()
        {
            var guid = SampleGuid.ToString();
            Assert.AreEqual(SampleGuid, guid.ToGuid());
        }

        [Test]
        public void ToGuid_InvalidString_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => "not-a-guid".ToGuid());
        }

        // ────────────────────────────────────────────────────────────
        //  ShouldNotBeNull
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ShouldNotBeNull_NonEmptyString_DoesNotThrow()
        {
            // Should not throw
            "hello".ShouldNotBeNull();
        }

        [Test]
        public void ShouldNotBeNull_NullString_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((string)null!).ShouldNotBeNull());
        }

        [Test]
        public void ShouldNotBeNull_EmptyString_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => string.Empty.ShouldNotBeNull());
        }

        // ────────────────────────────────────────────────────────────
        //  IsNull
        // ────────────────────────────────────────────────────────────

        [Test]
        public void IsNull_NullString_ReturnsTrue()
        {
            Assert.IsTrue(((string)null!).IsNull());
        }

        [Test]
        public void IsNull_EmptyString_ReturnsTrue()
        {
            Assert.IsTrue(string.Empty.IsNull());
        }

        [Test]
        public void IsNull_NonEmptyString_ReturnsFalse()
        {
            Assert.IsFalse("hello".IsNull());
        }

        // ────────────────────────────────────────────────────────────
        //  ToEntityReference — by ID
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ToEntityReference_WithGuidString_ReturnsReferenceWithId()
        {
            var idString = SampleGuid.ToString();
            var reference = idString.ToEntityReference("contact");

            Assert.IsNotNull(reference);
            Assert.AreEqual("contact", reference.LogicalName);
            Assert.AreEqual(SampleGuid, reference.Id);
        }

        [Test]
        public void ToEntityReference_NullIdString_ReturnsNull()
        {
            var reference = ((string)null!).ToEntityReference("contact");
            Assert.IsNull(reference);
        }

        [Test]
        public void ToEntityReference_EmptyIdString_ReturnsNull()
        {
            var reference = string.Empty.ToEntityReference("contact");
            Assert.IsNull(reference);
        }

        [Test]
        public void ToEntityReference_NullEntityName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => SampleGuid.ToString().ToEntityReference(null!));
        }

        // ────────────────────────────────────────────────────────────
        //  ToEntityReference — by alternate key
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ToEntityReference_WithKeyAttribute_ReturnsReferenceWithKeyAttributes()
        {
            var reference = "mykey123".ToEntityReference("contact", "externalkey");

            Assert.IsNotNull(reference);
            Assert.AreEqual("contact", reference.LogicalName);
            Assert.AreEqual(Guid.Empty, reference.Id);
            Assert.IsTrue(reference.KeyAttributes.ContainsKey("externalkey"));
            Assert.AreEqual("mykey123", reference.KeyAttributes["externalkey"]);
        }

        [Test]
        public void ToEntityReference_WithKeyAttribute_NullValue_ReturnsNull()
        {
            var reference = ((string)null!).ToEntityReference("contact", "externalkey");
            Assert.IsNull(reference);
        }

        // ────────────────────────────────────────────────────────────
        //  ToRelationship
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ToRelationship_ValidName_ReturnsRelationship()
        {
            var rel = "contact_account".ToRelationship();
            Assert.IsNotNull(rel);
            Assert.AreEqual("contact_account", rel.SchemaName);
        }

        [Test]
        public void ToRelationship_NullName_ReturnsNull()
        {
            Assert.IsNull(((string)null!).ToRelationship());
        }

        [Test]
        public void ToRelationship_EmptyName_ReturnsNull()
        {
            Assert.IsNull(string.Empty.ToRelationship());
        }

        // ────────────────────────────────────────────────────────────
        //  TrimIfTooLong
        // ────────────────────────────────────────────────────────────

        [Test]
        public void TrimIfTooLong_ShorterThanMax_ReturnsOriginalString()
        {
            Assert.AreEqual("hello", "hello".TrimIfTooLong(10));
        }

        [Test]
        public void TrimIfTooLong_ExactlyMax_ReturnsOriginalString()
        {
            Assert.AreEqual("hello", "hello".TrimIfTooLong(5));
        }

        [Test]
        public void TrimIfTooLong_LongerThanMax_ReturnsTrimmedString()
        {
            Assert.AreEqual("hel", "hello".TrimIfTooLong(3));
        }

        [Test]
        public void TrimIfTooLong_MaxZero_ReturnsEmptyString()
        {
            Assert.AreEqual("", "hello".TrimIfTooLong(0));
        }
    }
}
