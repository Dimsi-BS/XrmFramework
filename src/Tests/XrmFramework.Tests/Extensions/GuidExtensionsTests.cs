// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using NUnit.Framework;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.Tests.Extensions
{
    [TestFixture]
    public class GuidExtensionsTests
    {
        private static readonly Guid SampleGuid = new Guid("12345678-1234-1234-1234-123456789012");

        [Test]
        public void ToEntityReference_ValidGuid_ReturnsEntityReferenceWithCorrectValues()
        {
            var reference = SampleGuid.ToEntityReference("contact");

            Assert.IsNotNull(reference);
            Assert.AreEqual("contact", reference.LogicalName);
            Assert.AreEqual(SampleGuid, reference.Id);
        }

        [Test]
        public void ToEntityReference_EmptyGuid_ReturnsEntityReferenceWithEmptyId()
        {
            var reference = Guid.Empty.ToEntityReference("account");

            Assert.IsNotNull(reference);
            Assert.AreEqual("account", reference.LogicalName);
            Assert.AreEqual(Guid.Empty, reference.Id);
        }

        [Test]
        public void ToEntityReference_NullEntityName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => SampleGuid.ToEntityReference(null!));
        }

        [Test]
        public void ToEntityReference_EmptyEntityName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => SampleGuid.ToEntityReference(string.Empty));
        }
    }
}
