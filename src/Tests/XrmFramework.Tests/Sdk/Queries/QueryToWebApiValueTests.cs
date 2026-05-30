// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using NUnit.Framework;
using XrmFramework.Sdk.Queries;

namespace XrmFramework.Tests.Sdk.Queries
{
    [TestFixture]
    public class QueryToWebApiValueTests
    {
        [Test]
        public void ToWebApiValue_Null_ReturnsNullString()
        {
            var result = Query.ToWebApiValue(AttributeTypeCode.String, null);
            Assert.AreEqual("null", result);
        }

        [Test]
        public void ToWebApiValue_Boolean_True_ReturnsLowercase()
        {
            var result = Query.ToWebApiValue(AttributeTypeCode.Boolean, true);
            Assert.AreEqual("true", result);
        }

        [Test]
        public void ToWebApiValue_Boolean_False_ReturnsLowercase()
        {
            var result = Query.ToWebApiValue(AttributeTypeCode.Boolean, false);
            Assert.AreEqual("false", result);
        }

        [Test]
        public void ToWebApiValue_Integer_ReturnsStringRepresentation()
        {
            var result = Query.ToWebApiValue(AttributeTypeCode.Integer, 42);
            Assert.AreEqual("42", result);
        }

        [Test]
        public void ToWebApiValue_Decimal_ReturnsStringRepresentation()
        {
            var value = 314m;
            var result = Query.ToWebApiValue(AttributeTypeCode.Decimal, value);
            Assert.AreEqual(value.ToString(), result);
        }

        [Test]
        public void ToWebApiValue_Money_ReturnsStringRepresentation()
        {
            var value = 1000m;
            var result = Query.ToWebApiValue(AttributeTypeCode.Money, value);
            Assert.AreEqual(value.ToString(), result);
        }

        [Test]
        public void ToWebApiValue_Uniqueidentifier_ReturnsStringRepresentation()
        {
            var guid = new Guid("12345678-1234-1234-1234-123456789012");
            var result = Query.ToWebApiValue(AttributeTypeCode.Uniqueidentifier, guid);
            Assert.AreEqual(guid.ToString(), result);
        }

        [Test]
        public void ToWebApiValue_Picklist_ReturnsIntValue()
        {
            var result = Query.ToWebApiValue(AttributeTypeCode.Picklist, 3);
            Assert.AreEqual("3", result);
        }

        [Test]
        public void ToWebApiValue_Status_ReturnsIntValue()
        {
            var result = Query.ToWebApiValue(AttributeTypeCode.Status, 1);
            Assert.AreEqual("1", result);
        }

        [Test]
        public void ToWebApiValue_State_ReturnsIntValue()
        {
            var result = Query.ToWebApiValue(AttributeTypeCode.State, 0);
            Assert.AreEqual("0", result);
        }

        [Test]
        public void ToWebApiValue_String_ReturnsQuotedValue()
        {
            var result = Query.ToWebApiValue(AttributeTypeCode.String, "hello");
            Assert.AreEqual("'hello'", result);
        }

        [Test]
        public void ToWebApiValue_Memo_ReturnsQuotedValue()
        {
            var result = Query.ToWebApiValue(AttributeTypeCode.Memo, "some text");
            Assert.AreEqual("'some text'", result);
        }

        [Test]
        public void ToWebApiValue_DateTime_ReturnsFormattedDate()
        {
            var date = new DateTime(2024, 6, 15, 10, 30, 0);
            var result = Query.ToWebApiValue(AttributeTypeCode.DateTime, date);
            Assert.AreEqual("2024/06/15T10:30:00", result);
        }

        [Test]
        public void ToWebApiValue_Lookup_ReturnsStringRepresentation()
        {
            var guid = new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
            var result = Query.ToWebApiValue(AttributeTypeCode.Lookup, guid);
            Assert.AreEqual(guid.ToString(), result);
        }
    }
}
