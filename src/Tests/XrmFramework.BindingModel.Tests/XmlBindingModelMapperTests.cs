// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using XrmFramework.BindingModel.Tests.Fakes;

namespace XrmFramework.BindingModel.Tests
{
    /// <summary>
    /// Unit tests for the XElement ↔ IXmlModel mapping path
    /// (exercised through <see cref="BindingModelHelper.ToBindingModel{T}(XElement)"/>
    /// and <see cref="BindingModelHelper.ToXElement{T}"/>).
    /// </summary>
    [TestFixture]
    public class XmlBindingModelMapperTests
    {
        // ------------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------------

        private static XElement BuildContactElement(
            string? fullName = null,
            string? age = null,
            string? score = null,
            string? isActive = null,
            string? birthDate = null,
            string? trackingId = null)
        {
            var el = new XElement("contact");
            if (fullName != null) el.Add(new XElement("fullname", fullName));
            if (age != null) el.Add(new XElement("age", age));
            if (score != null) el.Add(new XElement("score", score));
            if (isActive != null) el.Add(new XElement("isactive", isActive));
            if (birthDate != null) el.Add(new XElement("birthdate", birthDate));
            if (trackingId != null) el.Add(new XElement("trackingid", trackingId));
            return el;
        }

        // ------------------------------------------------------------------
        // Null element
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_NullElement_ReturnsDefault()
        {
            XElement? nullElement = null;
            var result = nullElement!.ToBindingModel<SimpleXmlModel>();

            Assert.IsNull(result);
        }

        // ------------------------------------------------------------------
        // String parsing
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_StringElement_MapsCorrectly()
        {
            var element = BuildContactElement(fullName: "Alice");
            var model = element.ToBindingModel<SimpleXmlModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual("Alice", model.FullName);
        }

        [Test]
        public void ToBindingModel_AbsentStringElement_RemainsNull()
        {
            var element = BuildContactElement(); // no fullname element
            var model = element.ToBindingModel<SimpleXmlModel>();

            Assert.IsNotNull(model);
            Assert.IsNull(model.FullName);
        }

        // ------------------------------------------------------------------
        // Int parsing
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_IntElement_MapsCorrectly()
        {
            var element = BuildContactElement(age: "42");
            var model = element.ToBindingModel<SimpleXmlModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(42, model.Age);
        }

        [Test]
        public void ToBindingModel_InvalidIntElement_MapsToNull()
        {
            var element = BuildContactElement(age: "not-a-number");
            var model = element.ToBindingModel<SimpleXmlModel>();

            Assert.IsNotNull(model);
            // int.TryParse fails → null returned and cast to int gives 0 (default).
            Assert.AreEqual(0, model.Age);
        }

        // ------------------------------------------------------------------
        // Decimal parsing — dot separator (en-US)
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_DecimalDotSeparator_MapsCorrectly()
        {
            var element = BuildContactElement(score: "12.5");
            var model = element.ToBindingModel<SimpleXmlModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(12.5m, model.Score);
        }

        // ------------------------------------------------------------------
        // Decimal parsing — comma separator (fr-FR)
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_DecimalCommaSeparator_MapsCorrectly()
        {
            var element = BuildContactElement(score: "12,5");
            var model = element.ToBindingModel<SimpleXmlModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(12.5m, model.Score);
        }

        // ------------------------------------------------------------------
        // Boolean parsing
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_BooleanTrue_MapsCorrectly()
        {
            var element = BuildContactElement(isActive: "true");
            var model = element.ToBindingModel<SimpleXmlModel>();

            Assert.IsNotNull(model);
            Assert.IsTrue(model.IsActive);
        }

        [Test]
        public void ToBindingModel_BooleanFalse_MapsCorrectly()
        {
            var element = BuildContactElement(isActive: "false");
            var model = element.ToBindingModel<SimpleXmlModel>();

            Assert.IsNotNull(model);
            Assert.IsFalse(model.IsActive);
        }

        // ------------------------------------------------------------------
        // DateTime parsing
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_DateTimeIso8601_MapsCorrectly()
        {
            var element = BuildContactElement(birthDate: "1990-06-15T00:00:00");
            var model = element.ToBindingModel<SimpleXmlModel>();

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.BirthDate);
            Assert.AreEqual(1990, model.BirthDate!.Value.Year);
            Assert.AreEqual(6, model.BirthDate.Value.Month);
            Assert.AreEqual(15, model.BirthDate.Value.Day);
        }

        [Test]
        public void ToBindingModel_EmptyDateTimeElement_MapsToNull()
        {
            var element = BuildContactElement(birthDate: "");
            var model = element.ToBindingModel<SimpleXmlModel>();

            Assert.IsNotNull(model);
            Assert.IsNull(model.BirthDate);
        }

        // ------------------------------------------------------------------
        // Guid parsing
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_ValidGuid_MapsCorrectly()
        {
            var id = Guid.NewGuid();
            var element = BuildContactElement(trackingId: id.ToString());
            var model = element.ToBindingModel<SimpleXmlModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(id, model.TrackingId);
        }

        [Test]
        public void ToBindingModel_EmptyGuidString_MapsToGuidEmpty()
        {
            var element = BuildContactElement(trackingId: "");
            var model = element.ToBindingModel<SimpleXmlModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(Guid.Empty, model.TrackingId);
        }

        // ------------------------------------------------------------------
        // Collection of child elements
        // ------------------------------------------------------------------

        [Test]
        public void ToBindingModel_ChildCollection_MapsAllElements()
        {
            var element = new XElement("contact",
                new XElement("children",
                    new XElement("child",
                        new XElement("name", "Child A"),
                        new XElement("value", "10")),
                    new XElement("child",
                        new XElement("name", "Child B"),
                        new XElement("value", "20"))));

            var model = element.ToBindingModel<SimpleXmlModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Children.Count);
            Assert.AreEqual("Child A", model.Children[0].Name);
            Assert.AreEqual(10, model.Children[0].Value);
            Assert.AreEqual("Child B", model.Children[1].Name);
            Assert.AreEqual(20, model.Children[1].Value);
        }

        [Test]
        public void ToBindingModel_EmptyChildCollection_LeavesEmptyList()
        {
            var element = new XElement("contact",
                new XElement("children")); // no child elements

            var model = element.ToBindingModel<SimpleXmlModel>();

            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Children.Count);
        }

        // ------------------------------------------------------------------
        // Serialization: ToXElement
        // ------------------------------------------------------------------

        [Test]
        public void ToXElement_StringProperty_IsPresent()
        {
            var model = new SimpleXmlModel { FullName = "Bob" };
            var element = model.ToXElement();

            Assert.IsNotNull(element);
            Assert.AreEqual("Bob", element.Element("fullname")?.Value);
        }

        [Test]
        public void ToXElement_IntProperty_IsPresent()
        {
            var model = new SimpleXmlModel { Age = 33 };
            var element = model.ToXElement();

            Assert.IsNotNull(element);
            Assert.AreEqual("33", element.Element("age")?.Value);
        }

        [Test]
        public void ToXElement_DecimalProperty_UsesInvariantCulture()
        {
            var model = new SimpleXmlModel { Score = 7.5m };
            var element = model.ToXElement();

            Assert.IsNotNull(element);
            // InvariantCulture uses '.' as decimal separator.
            Assert.AreEqual("7.5", element.Element("score")?.Value);
        }

        [Test]
        public void ToXElement_DateTimeProperty_UsesIso8601()
        {
            var model = new SimpleXmlModel
            {
                BirthDate = new DateTime(1990, 6, 15, 12, 30, 0)
            };
            var element = model.ToXElement();

            Assert.IsNotNull(element);
            // Expect ISO-8601 sortable format.
            Assert.That(element.Element("birthdate")?.Value, Does.StartWith("1990-06-15T12:30:00"));
        }

        // ------------------------------------------------------------------
        // Roundtrip: FromXElement → ToXElement
        // ------------------------------------------------------------------

        [Test]
        public void Roundtrip_FromAndToXElement_PreservesAllScalars()
        {
            var trackingId = Guid.NewGuid();
            var source = new XElement("contact",
                new XElement("fullname", "Roundtrip"),
                new XElement("age", "28"),
                new XElement("score", "3.14"),
                new XElement("isactive", "true"),
                new XElement("birthdate", "1995-07-04T00:00:00"),
                new XElement("trackingid", trackingId.ToString()));

            // Deserialize then re-serialize.
            var model = source.ToBindingModel<SimpleXmlModel>();
            var result = model!.ToXElement();

            Assert.IsNotNull(result);
            Assert.AreEqual("Roundtrip", result.Element("fullname")?.Value);
            Assert.AreEqual("28", result.Element("age")?.Value);
            // Decimal may come back as "3.14" (invariant).
            Assert.AreEqual("3.14", result.Element("score")?.Value);
            Assert.AreEqual("True", result.Element("isactive")?.Value);
            Assert.AreEqual(trackingId.ToString(), result.Element("trackingid")?.Value);
        }

        [Test]
        public void Roundtrip_FromAndToXElement_PreservesChildCollection()
        {
            var source = new XElement("contact",
                new XElement("children",
                    new XElement("child",
                        new XElement("name", "Alpha"),
                        new XElement("value", "1")),
                    new XElement("child",
                        new XElement("name", "Beta"),
                        new XElement("value", "2"))));

            var model = source.ToBindingModel<SimpleXmlModel>();
            var result = model!.ToXElement();

            Assert.IsNotNull(result);
            var children = result.Element("children");
            Assert.IsNotNull(children);
            var childList = children.Elements("child").ToList();
            Assert.AreEqual(2, childList.Count);
            Assert.AreEqual("Alpha", childList[0].Element("name")?.Value);
            Assert.AreEqual("Beta", childList[1].Element("name")?.Value);
        }

        // ------------------------------------------------------------------
        // Null model → ToXElement returns null
        // ------------------------------------------------------------------

        [Test]
        public void ToXElement_NullModel_ReturnsNull()
        {
            SimpleXmlModel? nullModel = null;
            var result = nullModel.ToXElement();

            Assert.IsNull(result);
        }
    }
}
