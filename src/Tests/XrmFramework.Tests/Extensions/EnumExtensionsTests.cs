// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.Tests.Extensions
{
    [TestFixture]
    public class EnumExtensionsTests
    {
        // ────────────────────────────────────────────────────────────
        //  Enums de support
        // ────────────────────────────────────────────────────────────

        private enum SimpleEnum
        {
            Null  = 0,
            First = 1,
            Second = 2,
            Third  = 3
        }

        private enum DescribedEnum
        {
            [Description("Description One")]
            ValueOne = 1,
            [Description("Description Two")]
            ValueTwo = 2,
            NoDescription = 3
        }

        private enum ExternalEnum
        {
            [ExternalValue("ext_one")]
            ValueOne = 1,
            [ExternalValue("ext_two")]
            ValueTwo = 2,
            NoExternal = 3
        }

        // ────────────────────────────────────────────────────────────
        //  ToEnum<T>(int)
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ToEnum_FromInt_ReturnsCorrectEnumValue()
        {
            Assert.AreEqual(SimpleEnum.First,  1.ToEnum<SimpleEnum>());
            Assert.AreEqual(SimpleEnum.Second, 2.ToEnum<SimpleEnum>());
            Assert.AreEqual(SimpleEnum.Third,  3.ToEnum<SimpleEnum>());
        }

        [Test]
        public void ToEnum_FromInt_Zero_ReturnsNullValue()
        {
            Assert.AreEqual(SimpleEnum.Null, 0.ToEnum<SimpleEnum>());
        }

        // ────────────────────────────────────────────────────────────
        //  ToEnum<T>(OptionSetValue)
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ToEnum_FromOptionSetValue_ReturnsCorrectEnumValue()
        {
            var osv = new OptionSetValue(2);
            Assert.AreEqual(SimpleEnum.Second, osv.ToEnum<SimpleEnum>());
        }

        [Test]
        public void ToEnum_FromNullOptionSetValue_ReturnsDefault()
        {
            OptionSetValue? osv = null;
            Assert.AreEqual(default(SimpleEnum), osv.ToEnum<SimpleEnum>());
        }

        // ────────────────────────────────────────────────────────────
        //  ToEnum<T>(string)
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ToEnum_FromString_ReturnsCorrectEnumValue()
        {
            Assert.AreEqual(SimpleEnum.First,  "First".ToEnum<SimpleEnum>());
            Assert.AreEqual(SimpleEnum.Second, "Second".ToEnum<SimpleEnum>());
        }

        [Test]
        public void ToEnum_FromNullString_ReturnsDefault()
        {
            Assert.AreEqual(default(SimpleEnum), ((string)null!).ToEnum<SimpleEnum>());
        }

        [Test]
        public void ToEnum_FromEmptyString_ReturnsDefault()
        {
            Assert.AreEqual(default(SimpleEnum), string.Empty.ToEnum<SimpleEnum>());
        }

        // ────────────────────────────────────────────────────────────
        //  ParseDescription<T>
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ParseDescription_MatchingDescription_ReturnsCorrectValue()
        {
            Assert.AreEqual(DescribedEnum.ValueOne, "Description One".ParseDescription<DescribedEnum>());
            Assert.AreEqual(DescribedEnum.ValueTwo, "Description Two".ParseDescription<DescribedEnum>());
        }

        [Test]
        public void ParseDescription_NoMatch_ReturnsDefault()
        {
            Assert.AreEqual(default(DescribedEnum), "Unknown Description".ParseDescription<DescribedEnum>());
        }

        [Test]
        public void ParseDescription_NullInput_ReturnsDefault()
        {
            Assert.AreEqual(default(DescribedEnum), ((string)null!).ParseDescription<DescribedEnum>());
        }

        [Test]
        public void ParseDescription_EmptyInput_ReturnsDefault()
        {
            Assert.AreEqual(default(DescribedEnum), "".ParseDescription<DescribedEnum>());
        }

        // ────────────────────────────────────────────────────────────
        //  ParseDescriptions<T>
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ParseDescriptions_MultipleMatches_ReturnsAllValues()
        {
            var result = "Description One;Description Two".ParseDescriptions<DescribedEnum>();
            Assert.AreEqual(2, result.Count);
            Assert.That(result.ToList(), Does.Contain(DescribedEnum.ValueOne));
            Assert.That(result.ToList(), Does.Contain(DescribedEnum.ValueTwo));
        }

        [Test]
        public void ParseDescriptions_SingleMatch_ReturnsSingleValue()
        {
            var result = "Description One".ParseDescriptions<DescribedEnum>();
            Assert.AreEqual(1, result.Count);
            Assert.That(result.ToList(), Does.Contain(DescribedEnum.ValueOne));
        }

        [Test]
        public void ParseDescriptions_CustomSeparator_Works()
        {
            var result = "Description One|Description Two".ParseDescriptions<DescribedEnum>('|');
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void ParseDescriptions_NullInput_ReturnsEmptyCollection()
        {
            var result = ((string)null!).ParseDescriptions<DescribedEnum>();
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ParseDescriptions_PartialMatch_ReturnsOnlyMatched()
        {
            var result = "Description One;Unknown".ParseDescriptions<DescribedEnum>();
            Assert.AreEqual(1, result.Count);
            Assert.That(result.ToList(), Does.Contain(DescribedEnum.ValueOne));
        }

        // ────────────────────────────────────────────────────────────
        //  ParseExternalValue<T>
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ParseExternalValue_MatchingValue_ReturnsCorrectEnum()
        {
            Assert.AreEqual(ExternalEnum.ValueOne, "ext_one".ParseExternalValue<ExternalEnum>());
            Assert.AreEqual(ExternalEnum.ValueTwo, "ext_two".ParseExternalValue<ExternalEnum>());
        }

        [Test]
        public void ParseExternalValue_NoMatch_ReturnsDefault()
        {
            Assert.AreEqual(default(ExternalEnum), "ext_unknown".ParseExternalValue<ExternalEnum>());
        }

        [Test]
        public void ParseExternalValue_NullInput_ReturnsDefault()
        {
            Assert.AreEqual(default(ExternalEnum), ((string)null!).ParseExternalValue<ExternalEnum>());
        }

        // ────────────────────────────────────────────────────────────
        //  GetDescription
        // ────────────────────────────────────────────────────────────

        [Test]
        public void GetDescription_WithDescriptionAttribute_ReturnsDescription()
        {
            Assert.AreEqual("Description One", DescribedEnum.ValueOne.GetDescription());
            Assert.AreEqual("Description Two", DescribedEnum.ValueTwo.GetDescription());
        }

        [Test]
        public void GetDescription_WithoutDescriptionAttribute_ReturnsNull()
        {
            Assert.IsNull(DescribedEnum.NoDescription.GetDescription());
        }

        // ────────────────────────────────────────────────────────────
        //  GetExternalValue
        // ────────────────────────────────────────────────────────────

        [Test]
        public void GetExternalValue_WithExternalValueAttribute_ReturnsValue()
        {
            Assert.AreEqual("ext_one", ExternalEnum.ValueOne.GetExternalValue());
            Assert.AreEqual("ext_two", ExternalEnum.ValueTwo.GetExternalValue());
        }

        [Test]
        public void GetExternalValue_WithoutExternalValueAttribute_ReturnsNull()
        {
            Assert.IsNull(ExternalEnum.NoExternal.GetExternalValue());
        }

        // ────────────────────────────────────────────────────────────
        //  ToInt
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ToInt_ReturnsUnderlyingIntValue()
        {
            Assert.AreEqual(1, SimpleEnum.First.ToInt());
            Assert.AreEqual(2, SimpleEnum.Second.ToInt());
            Assert.AreEqual(0, SimpleEnum.Null.ToInt());
        }

        // ────────────────────────────────────────────────────────────
        //  ToOptionSetValue
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ToOptionSetValue_NonNullEnum_ReturnsOptionSetValue()
        {
            var osv = SimpleEnum.First.ToOptionSetValue();
            Assert.IsNotNull(osv);
            Assert.AreEqual(1, osv!.Value);
        }

        [Test]
        public void ToOptionSetValue_EnumNamedNullWithZeroValue_ReturnsNull()
        {
            // "Null" with value 0 is the convention for "no value"
            var osv = SimpleEnum.Null.ToOptionSetValue();
            Assert.IsNull(osv);
        }

        [Test]
        public void ToOptionSetValue_ValueTwoReturnsCorrectOptionSetValue()
        {
            var osv = SimpleEnum.Second.ToOptionSetValue();
            Assert.IsNotNull(osv);
            Assert.AreEqual(2, osv!.Value);
        }

        // ────────────────────────────────────────────────────────────
        //  ToOptionSetValueCollection<T>
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ToOptionSetValueCollection_MultipleValues_ReturnsCollection()
        {
            var values = new List<SimpleEnum> { SimpleEnum.First, SimpleEnum.Second };
            var result = values.ToOptionSetValueCollection();
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(o => o.Value == 1));
            Assert.IsTrue(result.Any(o => o.Value == 2));
        }

        [Test]
        public void ToOptionSetValueCollection_ContainsNullEnum_ExcludesNullEntry()
        {
            var values = new List<SimpleEnum> { SimpleEnum.Null, SimpleEnum.First };
            var result = values.ToOptionSetValueCollection();
            // SimpleEnum.Null (value=0, name="Null") → ToOptionSetValue returns null → excluded
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Value);
        }

        [Test]
        public void ToOptionSetValueCollection_EmptyList_ReturnsEmptyCollection()
        {
            var result = new List<SimpleEnum>().ToOptionSetValueCollection();
            Assert.AreEqual(0, result.Count);
        }

        // ────────────────────────────────────────────────────────────
        //  ToEnumCollection
        // ────────────────────────────────────────────────────────────

        [Test]
        public void ToEnumCollection_ReturnsCorrectEnumObjects()
        {
            var osvs = new List<OptionSetValue>
            {
                new OptionSetValue(1),
                new OptionSetValue(2)
            };

            var result = osvs.ToEnumCollection(typeof(SimpleEnum));

            Assert.AreEqual(2, result.Count);
            Assert.That(result.ToList(), Does.Contain(SimpleEnum.First));
            Assert.That(result.ToList(), Does.Contain(SimpleEnum.Second));
        }

        [Test]
        public void ToEnumCollection_EmptyList_ReturnsEmptyCollection()
        {
            var result = new List<OptionSetValue>().ToEnumCollection(typeof(SimpleEnum));
            Assert.AreEqual(0, result.Count);
        }
    }
}
