// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using XrmFramework.Utils;

namespace XrmFramework.Tests.Utils
{
    [TestFixture]
    public class JsonSerializerTests
    {
        // ────────────────────────────────────────────────────────────
        //  Types de support
        // ────────────────────────────────────────────────────────────

        private class SimpleModel
        {
            public string? Name    { get; set; }
            public int     Age     { get; set; }
            public bool    Active  { get; set; }
        }

        private class DateModel
        {
            public DateTime? Date { get; set; }
        }

        private class NestedModel
        {
            public string?      Label    { get; set; }
            public SimpleModel? Child    { get; set; }
            public List<int>?   Numbers  { get; set; }
        }

        // ────────────────────────────────────────────────────────────
        //  Serialize<TM>
        // ────────────────────────────────────────────────────────────

        [Test]
        public void Serialize_SimpleObject_ReturnsJson()
        {
            var model = new SimpleModel { Name = "Alice", Age = 30, Active = true };
            var json  = JsonSerializer.Serialize(model);

            Assert.IsNotNull(json);
            Assert.That(json, Does.Contain("Alice"));
            Assert.That(json, Does.Contain("30"));
        }

        [Test]
        public void Serialize_NullObject_ReturnsNull()
        {
            var result = JsonSerializer.Serialize<SimpleModel>(null!);
            Assert.IsNull(result);
        }

        [Test]
        public void Serialize_NestedObject_IncludesNestedJson()
        {
            var model = new NestedModel
            {
                Label   = "root",
                Child   = new SimpleModel { Name = "child" },
                Numbers = new List<int> { 1, 2, 3 }
            };

            var json = JsonSerializer.Serialize(model);
            Assert.That(json, Does.Contain("root"));
            Assert.That(json, Does.Contain("child"));
        }

        // ────────────────────────────────────────────────────────────
        //  Deserialize<M>
        // ────────────────────────────────────────────────────────────

        [Test]
        public void Deserialize_ValidJson_ReturnsObject()
        {
            var json   = "{\"Name\":\"Bob\",\"Age\":25,\"Active\":false}";
            var result = JsonSerializer.Deserialize<SimpleModel>(json);

            Assert.IsNotNull(result);
            Assert.AreEqual("Bob", result!.Name);
            Assert.AreEqual(25,    result.Age);
            Assert.IsFalse(result.Active);
        }

        [Test]
        public void Deserialize_NullInput_ReturnsDefault()
        {
            var result = JsonSerializer.Deserialize<SimpleModel>(null);
            Assert.IsNull(result);
        }

        [Test]
        public void Deserialize_EmptyString_ReturnsDefault()
        {
            var result = JsonSerializer.Deserialize<SimpleModel>(string.Empty);
            Assert.IsNull(result);
        }

        [Test]
        public void Serialize_ThenDeserialize_RoundTrip()
        {
            var original = new SimpleModel { Name = "Charlie", Age = 42, Active = true };
            var json     = JsonSerializer.Serialize(original);
            var result   = JsonSerializer.Deserialize<SimpleModel>(json);

            Assert.IsNotNull(result);
            Assert.AreEqual(original.Name,   result!.Name);
            Assert.AreEqual(original.Age,    result.Age);
            Assert.AreEqual(original.Active, result.Active);
        }

        [Test]
        public void Deserialize_InvalidJson_ThrowsException()
        {
            Assert.Throws<Microsoft.Xrm.Sdk.InvalidPluginExecutionException>(
                () => JsonSerializer.Deserialize<SimpleModel>("{ not valid json !!"));
        }

        // ────────────────────────────────────────────────────────────
        //  TryDeserialize<M>
        // ────────────────────────────────────────────────────────────

        [Test]
        public void TryDeserialize_ValidJson_ReturnsTrueAndObject()
        {
            var json = "{\"Name\":\"Dave\",\"Age\":35,\"Active\":true}";
            var ok   = JsonSerializer.TryDeserialize<SimpleModel>(json, out var result, out var error);

            Assert.IsTrue(ok);
            Assert.IsNotNull(result);
            Assert.AreEqual("Dave", result!.Name);
            Assert.IsNull(error);
        }

        [Test]
        public void TryDeserialize_InvalidJson_ReturnsFalseWithErrorMessage()
        {
            var ok = JsonSerializer.TryDeserialize<SimpleModel>(
                "{ bad json !!!",
                out var result,
                out var error);

            Assert.IsFalse(ok);
            Assert.IsNotNull(error);
        }

        [Test]
        public void TryDeserialize_NullInput_ReturnsTrueWithDefaultResult()
        {
            var ok = JsonSerializer.TryDeserialize<SimpleModel>(
                null,
                out var result,
                out var error);

            Assert.IsTrue(ok);
            Assert.IsNull(result);
            Assert.IsNull(error);
        }

        // ────────────────────────────────────────────────────────────
        //  TrySerialize<M>
        // ────────────────────────────────────────────────────────────

        [Test]
        public void TrySerialize_ValidObject_ReturnsTrueAndJson()
        {
            var model = new SimpleModel { Name = "Eve", Age = 28 };
            var ok    = JsonSerializer.TrySerialize(model, out var json, out var error);

            Assert.IsTrue(ok);
            Assert.IsNotNull(json);
            Assert.That(json, Does.Contain("Eve"));
            Assert.IsNull(error);
        }

        [Test]
        public void TrySerialize_NullObject_ReturnsTrueWithNullJson()
        {
            var ok = JsonSerializer.TrySerialize<SimpleModel>(null!, out var json, out var error);

            Assert.IsTrue(ok);
            Assert.IsNull(json);
            Assert.IsNull(error);
        }

        // ────────────────────────────────────────────────────────────
        //  Gestion du format de date
        // ────────────────────────────────────────────────────────────

        [Test]
        public void Serialize_WithDateTimeFormat_UsesCustomFormat()
        {
            var model = new DateModel { Date = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc) };
            var json  = JsonSerializer.Serialize(model, dateTimeFormat: "yyyy-MM-dd");

            Assert.That(json, Does.Contain("2024-06-15"));
        }

        [Test]
        public void Deserialize_WithDateTimeFormat_ParsesCorrectly()
        {
            var json   = "{\"Date\":\"2024-06-15\"}";
            var result = JsonSerializer.Deserialize<DateModel>(json, dateTimeFormat: "yyyy-MM-dd");

            Assert.IsNotNull(result?.Date);
            Assert.AreEqual(2024, result!.Date!.Value.Year);
            Assert.AreEqual(6,    result.Date.Value.Month);
            Assert.AreEqual(15,   result.Date.Value.Day);
        }
    }
}
