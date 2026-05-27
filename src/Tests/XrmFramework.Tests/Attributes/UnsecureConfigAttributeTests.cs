// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;
using NUnit.Framework;

namespace XrmFramework.Tests.Attributes
{
    [TestFixture]
    public class UnsecureConfigAttributeTests
    {
        // ─────────────────────────────────────────────────────────────
        //  Constructor(Type resourceType, string propertyName)
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void TypeConstructor_SetsResourceTypeAndPropertyName()
        {
            var attr = new UnsecureConfigAttribute(typeof(string), "MyProperty");

            Assert.AreEqual(typeof(string), attr.ResourceType);
            Assert.AreEqual("MyProperty", attr.PropertyName);
            Assert.IsNull(attr.UnsecureConfig);
        }

        // ─────────────────────────────────────────────────────────────
        //  Constructor(string unsecureConfig)
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void StringConstructor_SetsUnsecureConfig()
        {
            var attr = new UnsecureConfigAttribute("key=value;other=data");

            Assert.AreEqual("key=value;other=data", attr.UnsecureConfig);
            Assert.IsNull(attr.ResourceType);
            Assert.IsNull(attr.PropertyName);
        }

        [Test]
        public void StringConstructor_EmptyString_SetsEmptyConfig()
        {
            var attr = new UnsecureConfigAttribute(string.Empty);

            Assert.AreEqual(string.Empty, attr.UnsecureConfig);
        }

        // ─────────────────────────────────────────────────────────────
        //  AttributeUsage
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void UnsecureConfigAttribute_TargetsMethod()
        {
            var usage = typeof(UnsecureConfigAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Method, usage.ValidOn);
        }
    }
}
