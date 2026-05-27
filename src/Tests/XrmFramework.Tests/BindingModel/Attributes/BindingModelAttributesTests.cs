// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;
using NUnit.Framework;
using XrmFramework.BindingModel;

namespace XrmFramework.Tests.BindingModel.Attributes
{
    [TestFixture]
    public class BindingModelAttributesTests
    {
        // ─────────────────────────────────────────────────────────────
        //  ExtendBindingModelAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void ExtendBindingModelAttribute_CanBeInstantiated()
        {
            Assert.IsNotNull(new ExtendBindingModelAttribute());
        }

        [Test]
        public void ExtendBindingModelAttribute_TargetsProperty()
        {
            var usage = typeof(ExtendBindingModelAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Property, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  ModelPropertyConverterAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void ModelPropertyConverterAttribute_Constructor_SetsConverterType()
        {
            var attr = new ModelPropertyConverterAttribute(typeof(string));

            Assert.AreEqual(typeof(string), attr.ConverterType);
        }

        [Test]
        public void ModelPropertyConverterAttribute_Constructor_SetsConstructorParameters()
        {
            var attr = new ModelPropertyConverterAttribute(typeof(string), "param1", 42);

            Assert.AreEqual(2, attr.ConstructorParameters.Length);
            Assert.AreEqual("param1", attr.ConstructorParameters[0]);
            Assert.AreEqual(42, attr.ConstructorParameters[1]);
        }

        [Test]
        public void ModelPropertyConverterAttribute_NoConstructorParameters_EmptyArray()
        {
            var attr = new ModelPropertyConverterAttribute(typeof(string));

            Assert.IsNotNull(attr.ConstructorParameters);
            Assert.AreEqual(0, attr.ConstructorParameters.Length);
        }

        [Test]
        public void ModelPropertyConverterAttribute_TargetsProperty()
        {
            var usage = typeof(ModelPropertyConverterAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Property, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  UpsertBehaviourAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void UpsertBehaviourAttribute_Constructor_SetsBehaviourType()
        {
            var attr = new UpsertBehaviourAttribute(typeof(string));

            Assert.AreEqual(typeof(string), attr.BehaviourType);
        }

        [Test]
        public void UpsertBehaviourAttribute_TargetsClass()
        {
            var usage = typeof(UpsertBehaviourAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Class, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  UpsertOrderAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void UpsertOrderAttribute_Constructor_SetsOrder()
        {
            var attr = new UpsertOrderAttribute(5);

            Assert.AreEqual(5, attr.Order);
        }

        [Test]
        public void UpsertOrderAttribute_Constructor_ZeroOrder()
        {
            var attr = new UpsertOrderAttribute(0);

            Assert.AreEqual(0, attr.Order);
        }

        [Test]
        public void UpsertOrderAttribute_TargetsProperty()
        {
            var usage = typeof(UpsertOrderAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Property, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  XmlTransformAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void XmlTransformAttribute_Constructor_SetsActionType()
        {
            var attr = new XmlTransformAttribute(typeof(string));

            Assert.AreEqual(typeof(string), attr.ActionType);
        }

        [Test]
        public void XmlTransformAttribute_TargetsClass()
        {
            var usage = typeof(XmlTransformAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Class, usage.ValidOn);
        }
    }
}
