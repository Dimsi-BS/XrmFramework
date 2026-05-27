// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;
using NUnit.Framework;

namespace XrmFramework.Tests.Attributes
{
    [TestFixture]
    public class OptionSetAttributeTests
    {
        private enum SampleState { Active = 0, Inactive = 1 }

        // ─────────────────────────────────────────────────────────────
        //  Constructor
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Constructor_WithEnumType_SetsEnumType()
        {
            var attr = new OptionSetAttribute(typeof(SampleState));

            Assert.AreEqual(typeof(SampleState), attr.EnumType);
        }

        [Test]
        public void Constructor_WithNonEnumType_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new OptionSetAttribute(typeof(string)));
        }

        [Test]
        public void Constructor_WithIntType_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new OptionSetAttribute(typeof(int)));
        }

        // ─────────────────────────────────────────────────────────────
        //  AttributeUsage
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void OptionSetAttribute_TargetsField()
        {
            var usage = typeof(OptionSetAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Field, usage.ValidOn);
        }
    }
}
