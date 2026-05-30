// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using XrmFramework.Sdk.Queries;
using XrmFramework.Tests.Sdk.Queries.Fakes;

namespace XrmFramework.Tests.Sdk.Queries
{
    [TestFixture]
    public class ConditionToFetchXmlTests
    {
        private static string GetConditionFetchXml(string attribute, ConditionOperator op, object value = null)
        {
            var query = new Query(ContactDefinition.EntityName);
            if (value != null)
                query.Criteria.AddCondition(attribute, op, value);
            else
                query.Criteria.AddCondition(attribute, op);
            return query.Criteria.Conditions[0].ToFetchXmlString();
        }

        [Test]
        public void ToFetchXmlString_NoValue_SelfClosingTag()
        {
            var xml = GetConditionFetchXml(ContactDefinition.Columns.Email, ConditionOperator.NotNull);

            StringAssert.Contains($"attribute=\"{ContactDefinition.Columns.Email}\"", xml);
            StringAssert.Contains("operator=\"not-null\"", xml);
            StringAssert.EndsWith("/>", xml);
        }

        [Test]
        public void ToFetchXmlString_StringValue_ContainsValueAttribute()
        {
            var xml = GetConditionFetchXml(ContactDefinition.Columns.Email, ConditionOperator.Equal, "test@test.com");

            StringAssert.Contains("value=\"test@test.com\"", xml);
            StringAssert.EndsWith("/>", xml);
        }

        [Test]
        public void ToFetchXmlString_BooleanTrue_UsesOneAsValue()
        {
            var xml = GetConditionFetchXml(ContactDefinition.Columns.IsActive, ConditionOperator.Equal, true);

            StringAssert.Contains("value=\"1\"", xml);
        }

        [Test]
        public void ToFetchXmlString_BooleanFalse_UsesZeroAsValue()
        {
            var xml = GetConditionFetchXml(ContactDefinition.Columns.IsActive, ConditionOperator.Equal, false);

            StringAssert.Contains("value=\"0\"", xml);
        }

        [Test]
        public void ToFetchXmlString_DateTimeValue_UsesRoundTripFormat()
        {
            var date = new DateTime(2024, 3, 15, 12, 0, 0, DateTimeKind.Utc);
            var xml = GetConditionFetchXml(ContactDefinition.Columns.BirthDate, ConditionOperator.Equal, date);

            StringAssert.Contains($"value=\"{date:o}\"", xml);
        }

        [Test]
        public void ToFetchXmlString_MultipleValues_UsesChildValueElements()
        {
            var query = new Query(ContactDefinition.EntityName);
            query.Criteria.AddCondition(ContactDefinition.Columns.Email, ConditionOperator.In,
                new object[] { "a@a.com", "b@b.com" });
            var xml = query.Criteria.Conditions[0].ToFetchXmlString();

            StringAssert.Contains("<value>a@a.com</value>", xml);
            StringAssert.Contains("<value>b@b.com</value>", xml);
            StringAssert.Contains("</condition>", xml);
        }

        [Test]
        public void ToFetchXmlString_WithEntityAlias_ContainsEntitynameAttribute()
        {
            var query = new Query(ContactDefinition.EntityName);
            query.Criteria.AddCondition("acc", ContactDefinition.Columns.Email, ConditionOperator.NotNull, null);
            var xml = query.Criteria.Conditions[0].ToFetchXmlString();

            StringAssert.Contains("entityname=\"acc\"", xml);
        }
    }
}
