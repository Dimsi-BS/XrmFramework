// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace XrmFramework.Tests.Attributes
{
    [TestFixture]
    public class ImageAttributeTests
    {
        // ─────────────────────────────────────────────────────────────
        //  PreImageAttribute — columns constructor
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void PreImageAttribute_ColumnsConstructor_SetsColumns()
        {
            var attr = new PreImageAttribute("name", "emailaddress1");

            CollectionAssert.AreEquivalent(new[] { "name", "emailaddress1" }, attr.Columns);
            Assert.IsFalse(attr.AllColumns);
        }

        [Test]
        public void PreImageAttribute_EmptyColumnsConstructor_SetsEmptyColumns()
        {
            var attr = new PreImageAttribute();

            Assert.IsNotNull(attr.Columns);
            Assert.AreEqual(0, attr.Columns.Length);
            Assert.IsFalse(attr.AllColumns);
        }

        // ─────────────────────────────────────────────────────────────
        //  PreImageAttribute — allColumns constructor
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void PreImageAttribute_AllColumnsTrue_SetsAllColumns()
        {
            var attr = new PreImageAttribute(true);

            Assert.IsTrue(attr.AllColumns);
            Assert.IsNull(attr.Columns);
        }

        [Test]
        public void PreImageAttribute_AllColumnsFalse_SetsAllColumnsFalse()
        {
            var attr = new PreImageAttribute(false);

            Assert.IsFalse(attr.AllColumns);
            Assert.IsNull(attr.Columns);
        }

        // ─────────────────────────────────────────────────────────────
        //  PostImageAttribute — columns constructor
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void PostImageAttribute_ColumnsConstructor_SetsColumns()
        {
            var attr = new PostImageAttribute("statecode", "statuscode");

            CollectionAssert.AreEquivalent(new[] { "statecode", "statuscode" }, attr.Columns);
            Assert.IsFalse(attr.AllColumns);
        }

        // ─────────────────────────────────────────────────────────────
        //  PostImageAttribute — allColumns constructor
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void PostImageAttribute_AllColumnsTrue_SetsAllColumns()
        {
            var attr = new PostImageAttribute(true);

            Assert.IsTrue(attr.AllColumns);
            Assert.IsNull(attr.Columns);
        }

        // ─────────────────────────────────────────────────────────────
        //  AttributeUsage
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void ImageAttribute_TargetsMethod()
        {
            var usage = typeof(ImageAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Method, usage.ValidOn);
        }

        [Test]
        public void PreImageAttribute_InheritsFromImageAttribute()
        {
            Assert.IsTrue(typeof(PreImageAttribute).IsSubclassOf(typeof(ImageAttribute)));
        }

        [Test]
        public void PostImageAttribute_InheritsFromImageAttribute()
        {
            Assert.IsTrue(typeof(PostImageAttribute).IsSubclassOf(typeof(ImageAttribute)));
        }
    }
}
