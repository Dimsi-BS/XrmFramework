// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace XrmFramework.Tests.Attributes
{
    /// <summary>
    /// Tests for all simple single-property or no-argument attributes in XrmFramework.
    /// </summary>
    [TestFixture]
    public class XrmFrameworkAttributesTests
    {
        // ─────────────────────────────────────────────────────────────
        //  AlternateKeyAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void AlternateKeyAttribute_Constructor_SetsKeyName()
        {
            var attr = new AlternateKeyAttribute("myKey");
            Assert.AreEqual("myKey", attr.KeyName);
        }

        [Test]
        public void AlternateKeyAttribute_AllowsMultiple_OnFieldAndProperty()
        {
            var usage = typeof(AlternateKeyAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.IsTrue(usage.AllowMultiple);
            Assert.IsTrue(usage.ValidOn.HasFlag(AttributeTargets.Field));
            Assert.IsTrue(usage.ValidOn.HasFlag(AttributeTargets.Property));
        }

        // ─────────────────────────────────────────────────────────────
        //  AttributeMetadataAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void AttributeMetadataAttribute_Constructor_SetsType()
        {
            var attr = new AttributeMetadataAttribute(AttributeTypeCode.String);
            Assert.AreEqual(AttributeTypeCode.String, attr.Type);
        }

        [Test]
        public void AttributeMetadataAttribute_Targets_Field()
        {
            var usage = typeof(AttributeMetadataAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Field, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  CopyFromParentAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void CopyFromParentAttribute_Constructor_SetsParentTypeAndPropertyName()
        {
            var attr = new CopyFromParentAttribute(typeof(string), "Length");
            Assert.AreEqual(typeof(string), attr.ParentType);
            Assert.AreEqual("Length", attr.ParentPropertyName);
        }

        [Test]
        public void CopyFromParentAttribute_AllowsMultiple_OnProperty()
        {
            var usage = typeof(CopyFromParentAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.IsTrue(usage.AllowMultiple);
            Assert.AreEqual(AttributeTargets.Property, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  CrmModelImplementationAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void CrmModelImplementationAttribute_Constructor_SetsImplementationType()
        {
            var attr = new CrmModelImplementationAttribute(typeof(string));
            Assert.AreEqual(typeof(string), attr.ImplementationType);
        }

        [Test]
        public void CrmModelImplementationAttribute_Targets_Property()
        {
            var usage = typeof(CrmModelImplementationAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Property, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  DateTimeBehaviorAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void DateTimeBehaviorAttribute_Constructor_SetsBehavior()
        {
            var attr = new DateTimeBehaviorAttribute(DateTimeBehavior.UserLocal);
            Assert.AreEqual(DateTimeBehavior.UserLocal, attr.Behavior);
        }

        [Test]
        public void DateTimeBehaviorAttribute_AllBehaviorValues_RoundTrip()
        {
            Assert.AreEqual(DateTimeBehavior.UserLocal,         new DateTimeBehaviorAttribute(DateTimeBehavior.UserLocal).Behavior);
            Assert.AreEqual(DateTimeBehavior.DateOnly,          new DateTimeBehaviorAttribute(DateTimeBehavior.DateOnly).Behavior);
            Assert.AreEqual(DateTimeBehavior.TimeZoneIndependent, new DateTimeBehaviorAttribute(DateTimeBehavior.TimeZoneIndependent).Behavior);
        }

        [Test]
        public void DateTimeBehaviorAttribute_Targets_Field()
        {
            var usage = typeof(DateTimeBehaviorAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Field, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  DependentAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void DependentAttribute_Constructor_SetsAttributeName()
        {
            var attr = new DependentAttribute("Email");
            Assert.AreEqual("Email", attr.AttributeName);
        }

        [Test]
        public void DependentAttribute_AllowsMultiple_OnProperty()
        {
            var usage = typeof(DependentAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.IsTrue(usage.AllowMultiple);
            Assert.AreEqual(AttributeTargets.Property, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  EntityDefinitionAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void EntityDefinitionAttribute_CanBeInstantiated()
        {
            Assert.IsNotNull(new EntityDefinitionAttribute());
        }

        [Test]
        public void EntityDefinitionAttribute_Targets_Class()
        {
            var usage = typeof(EntityDefinitionAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Class, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  EntityTemplateAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void EntityTemplateAttribute_CanBeInstantiated()
        {
            Assert.IsNotNull(new EntityTemplateAttribute());
        }

        [Test]
        public void EntityTemplateAttribute_Targets_Class()
        {
            var usage = typeof(EntityTemplateAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Class, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  EnumGenerationAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void EnumGenerationAttribute_CanBeInstantiated()
        {
            Assert.IsNotNull(new EnumGenerationAttribute());
        }

        [Test]
        public void EnumGenerationAttribute_Targets_Class()
        {
            var usage = typeof(EnumGenerationAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Class, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  ExecutionOrderAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void ExecutionOrderAttribute_Constructor_SetsOrder()
        {
            var attr = new ExecutionOrderAttribute(3);
            Assert.AreEqual(3, attr.Order);
        }

        [Test]
        public void ExecutionOrderAttribute_Targets_Method()
        {
            var usage = typeof(ExecutionOrderAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Method, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  FilteringAttributesAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void FilteringAttributesAttribute_Constructor_SetsAttributes()
        {
            var attr = new FilteringAttributesAttribute("firstname", "lastname");
            CollectionAssert.AreEquivalent(new[] { "firstname", "lastname" }, attr.Attributes);
        }

        [Test]
        public void FilteringAttributesAttribute_EmptyParams_ReturnsEmptyArray()
        {
            var attr = new FilteringAttributesAttribute();
            Assert.IsNotNull(attr.Attributes);
            Assert.AreEqual(0, attr.Attributes.Length);
        }

        [Test]
        public void FilteringAttributesAttribute_Targets_Method()
        {
            var usage = typeof(FilteringAttributesAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Method, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  ImpersonationAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void ImpersonationAttribute_Constructor_SetsUsername()
        {
            var attr = new ImpersonationAttribute("admin@contoso.com");
            Assert.AreEqual("admin@contoso.com", attr.ImpersonationUsername);
        }

        [Test]
        public void ImpersonationAttribute_Targets_Method()
        {
            var usage = typeof(ImpersonationAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Method, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  LoggerClassAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void LoggerClassAttribute_Constructor_SetsLoggerClassType()
        {
            var attr = new LoggerClassAttribute(typeof(string));
            Assert.AreEqual(typeof(string), attr.LoggerClassType);
        }

        [Test]
        public void LoggerClassAttribute_Targets_Assembly()
        {
            var usage = typeof(LoggerClassAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Assembly, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  MultiAttributesAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void MultiAttributesAttribute_Constructor_SetsStartAndEndIndex()
        {
            var attr = new MultiAttributesAttribute(2, 8);
            Assert.AreEqual(2, attr.StartIndex);
            Assert.AreEqual(8, attr.EndIndex);
        }

        [Test]
        public void MultiAttributesAttribute_Targets_Field()
        {
            var usage = typeof(MultiAttributesAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Field, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  NullableAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void NullableAttribute_CanBeInstantiated()
        {
            Assert.IsNotNull(new NullableAttribute());
        }

        [Test]
        public void NullableAttribute_Targets_Parameter()
        {
            var usage = typeof(NullableAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Parameter, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  PrimaryAttributeAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void PrimaryAttributeAttribute_Constructor_SetsType()
        {
            var attrId   = new PrimaryAttributeAttribute(PrimaryAttributeType.Id);
            var attrName = new PrimaryAttributeAttribute(PrimaryAttributeType.Name);
            var attrImg  = new PrimaryAttributeAttribute(PrimaryAttributeType.Image);

            Assert.AreEqual(PrimaryAttributeType.Id,    attrId.Type);
            Assert.AreEqual(PrimaryAttributeType.Name,  attrName.Type);
            Assert.AreEqual(PrimaryAttributeType.Image, attrImg.Type);
        }

        [Test]
        public void PrimaryAttributeAttribute_Targets_Field()
        {
            var usage = typeof(PrimaryAttributeAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;
            Assert.AreEqual(AttributeTargets.Field, usage.ValidOn);
        }

        // ─────────────────────────────────────────────────────────────
        //  SettingNameAttribute
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void SettingNameAttribute_Constructor_SetsName()
        {
            var attr = new SettingNameAttribute("MyConnectionString");
            Assert.AreEqual("MyConnectionString", attr.Name);
        }
    }
}
