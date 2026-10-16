// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using NUnit.Framework;
using XrmFramework.Model;

namespace XrmFramework.Tests.Service
{
    [TestFixture]
    public class CrmSettingsFactoryTests
    {
        private CrmSettingsFactory<CrmSettings> _factory = null!;

        private class JsonTestModel
        {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }

        private class TestSettings : CrmSettings
        {
            [SettingName("my.string.setting")]
            public string MySetting { get; set; } = string.Empty;
        }

        /// <summary>
        /// Builds a raw environmentvariabledefinition <see cref="Entity"/> as RetrieveMultiple would
        /// return it: the queried value comes back aliased through the environmentvariablevalue link,
        /// exactly like <see cref="CrmSettingsFactory{TSettings}.GetEnvironmentVariables"/> queries it.
        /// </summary>
        private static Entity MakeEnvironmentVariableEntity(string schemaName, string queriedValue)
        {
            var entity = new Entity(EnvironmentVariableDefinition.EntityName, Guid.NewGuid())
            {
                [EnvironmentVariableDefinition.Columns.SchemaName] = schemaName,
                [EnvironmentVariableDefinition.Columns.Type] = new OptionSetValue((int)EnvironmentVariableType.String),
                [$"{EnvironmentVariableValueDefinition.EntityName}.{EnvironmentVariableValueDefinition.Columns.Value}"]
                    = new AliasedValue(EnvironmentVariableValueDefinition.EntityName, EnvironmentVariableValueDefinition.Columns.Value, queriedValue)
            };

            return entity;
        }

        private static Mock<IOrganizationService> MakeMockServiceReturning(params Entity[] entities)
        {
            var mockService = new Mock<IOrganizationService>();
            mockService
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection(entities.ToList()) { MoreRecords = false });

            return mockService;
        }

        // ────────────────────────────────────────────────────────────
        //  Settings - end to end
        // ────────────────────────────────────────────────────────────

        [Test]
        public void Settings_DefaultInitSettings_QueriesCrmAndAppliesTheRetrievedValue()
        {
            var mockService = MakeMockServiceReturning(MakeEnvironmentVariableEntity("my.string.setting", "QueriedValue"));

            var factory = new CrmSettingsFactory<TestSettings>(
                mockService.Object,
                _ => Enumerable.Empty<(string, object)>());

            var settings = factory.Settings;

            Assert.AreEqual("QueriedValue", settings.MySetting);
            mockService.Verify(s => s.RetrieveMultiple(It.IsAny<QueryBase>()), Times.Once);
        }

        [Test]
        public void Settings_InitSettingsOverride_IsAppliedWithoutQueryingCrm()
        {
            var mockService = MakeMockServiceReturning();

            var factory = new CrmSettingsFactory<TestSettings>(
                mockService.Object,
                _ => new[] { ("my.string.setting", (object)"OverrideValue") });

            var settings = factory.Settings;

            Assert.AreEqual("OverrideValue", settings.MySetting);
            mockService.Verify(s => s.RetrieveMultiple(It.IsAny<QueryBase>()), Times.Never);
        }

        [Test]
        public void Settings_InitSettingsReturningNull_FallsBackToCrmWithoutThrowing()
        {
            var mockService = MakeMockServiceReturning(MakeEnvironmentVariableEntity("my.string.setting", "QueriedValue"));

            var factory = new CrmSettingsFactory<TestSettings>(
                mockService.Object,
                _ => null!);

            TestSettings settings = null!;
            Assert.DoesNotThrow(() => settings = factory.Settings);

            Assert.AreEqual("QueriedValue", settings.MySetting);
            mockService.Verify(s => s.RetrieveMultiple(It.IsAny<QueryBase>()), Times.Once);
        }

        [Test]
        public void Settings_IsComputedOnceAndCached()
        {
            var mockService = MakeMockServiceReturning(MakeEnvironmentVariableEntity("my.string.setting", "QueriedValue"));

            var factory = new CrmSettingsFactory<TestSettings>(
                mockService.Object,
                _ => Enumerable.Empty<(string, object)>());

            var first = factory.Settings;
            var second = factory.Settings;

            Assert.AreSame(first, second);
            mockService.Verify(s => s.RetrieveMultiple(It.IsAny<QueryBase>()), Times.Once);
        }

        [SetUp]
        public void SetUp()
        {
            var mockService = new Mock<IOrganizationService>();
            _factory = new CrmSettingsFactory<CrmSettings>(
                mockService.Object,
                _ => Enumerable.Empty<(string, object)>()
            );
        }

        private static EnvironmentVariable MakeVariable(EnvironmentVariableType type, string defaultValue)
            => new EnvironmentVariable
            {
                Type = type,
                Entity = new Entity("environmentvariabledefinition"),
                DefaultValue = defaultValue
            };

        // ────────────────────────────────────────────────────────────
        //  Null variable
        // ────────────────────────────────────────────────────────────

        [Test]
        public void GetEnvironmentVariableValue_NullVariable_ReturnsNull()
        {
            var result = _factory.GetEnvironmentVariableValue(typeof(string), null!);
            Assert.IsNull(result);
        }

        // ────────────────────────────────────────────────────────────
        //  String
        // ────────────────────────────────────────────────────────────

        [Test]
        public void GetEnvironmentVariableValue_StringType_ReturnsStringValue()
        {
            var variable = MakeVariable(EnvironmentVariableType.String, "hello world");

            var result = _factory.GetEnvironmentVariableValue(typeof(string), variable);

            Assert.AreEqual("hello world", result);
        }

        [Test]
        public void GetEnvironmentVariableValue_StringType_WrongExpectedType_ThrowsArgumentException()
        {
            var variable = MakeVariable(EnvironmentVariableType.String, "hello");

            Assert.Throws<ArgumentException>(() =>
                _factory.GetEnvironmentVariableValue(typeof(int), variable));
        }

        // ────────────────────────────────────────────────────────────
        //  Number
        // ────────────────────────────────────────────────────────────

        [Test]
        public void GetEnvironmentVariableValue_NumberType_ReturnsParsedInt()
        {
            var variable = MakeVariable(EnvironmentVariableType.Number, "42");

            var result = _factory.GetEnvironmentVariableValue(typeof(int), variable);

            Assert.AreEqual(42, result);
        }

        [Test]
        public void GetEnvironmentVariableValue_NumberType_WrongExpectedType_ThrowsArgumentException()
        {
            var variable = MakeVariable(EnvironmentVariableType.Number, "42");

            Assert.Throws<ArgumentException>(() =>
                _factory.GetEnvironmentVariableValue(typeof(string), variable));
        }

        // ────────────────────────────────────────────────────────────
        //  Boolean
        // ────────────────────────────────────────────────────────────

        [Test]
        public void GetEnvironmentVariableValue_BooleanType_TrueValue_ReturnsBoolTrue()
        {
            var variable = MakeVariable(EnvironmentVariableType.Boolean, "true");

            var result = _factory.GetEnvironmentVariableValue(typeof(bool), variable);

            Assert.AreEqual(true, result);
        }

        [Test]
        public void GetEnvironmentVariableValue_BooleanType_FalseValue_ReturnsBoolFalse()
        {
            var variable = MakeVariable(EnvironmentVariableType.Boolean, "false");

            var result = _factory.GetEnvironmentVariableValue(typeof(bool), variable);

            Assert.AreEqual(false, result);
        }

        [Test]
        public void GetEnvironmentVariableValue_BooleanType_WrongExpectedType_ThrowsArgumentException()
        {
            var variable = MakeVariable(EnvironmentVariableType.Boolean, "true");

            Assert.Throws<ArgumentException>(() =>
                _factory.GetEnvironmentVariableValue(typeof(string), variable));
        }

        // ────────────────────────────────────────────────────────────
        //  JSON
        // ────────────────────────────────────────────────────────────

        [Test]
        public void GetEnvironmentVariableValue_JsonType_DeserializesToExpectedObject()
        {
            var variable = MakeVariable(EnvironmentVariableType.JSON,
                "{\"Name\":\"test\",\"Value\":99}");

            var result = _factory.GetEnvironmentVariableValue(typeof(JsonTestModel), variable);

            Assert.IsInstanceOf<JsonTestModel>(result);
            var model = (JsonTestModel)result!;
            Assert.AreEqual("test", model.Name);
            Assert.AreEqual(99, model.Value);
        }

        // ────────────────────────────────────────────────────────────
        //  Unknown type
        // ────────────────────────────────────────────────────────────

        [Test]
        public void GetEnvironmentVariableValue_UnknownType_ThrowsArgumentOutOfRangeException()
        {
            var variable = new EnvironmentVariable
            {
                Type = (EnvironmentVariableType)(-1),
                Entity = new Entity("environmentvariabledefinition"),
                DefaultValue = "anything"
            };

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _factory.GetEnvironmentVariableValue(typeof(string), variable));
        }
    }
}
