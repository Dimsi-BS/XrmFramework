// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;
using NUnit.Framework.Legacy;
using XrmFramework.Sdk.Queries;
using XrmFramework.Tests.Sdk.Queries.Fakes;

namespace XrmFramework.Tests.Sdk.Queries
{
    [TestFixture]
    public class QueryCriteriaTests
    {
        [Test]
        public void ToFetchXmlString_AndOperator_ContainsAndType()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.Criteria.AddCondition(FakeContactDefinition.Columns.Email, ConditionOperator.NotNull);

            var xml = query.Criteria.ToFetchXmlString();

            StringAssert.Contains("type=\"and\"", xml);
        }

        [Test]
        public void ToFetchXmlString_OrFilter_ContainsOrType()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            var orFilter = query.Criteria.AddFilter(LogicalOperator.Or);
            orFilter.AddCondition(FakeContactDefinition.Columns.Email, ConditionOperator.NotNull);

            var xml = query.Criteria.ToFetchXmlString();

            StringAssert.Contains("type=\"or\"", xml);
        }

        [Test]
        public void ToFetchXmlString_TwoConditions_ContainsBothConditions()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.Criteria.AddCondition(FakeContactDefinition.Columns.Email, ConditionOperator.NotNull);
            query.Criteria.AddCondition(FakeContactDefinition.Columns.FullName, ConditionOperator.NotNull);

            var xml = query.Criteria.ToFetchXmlString();

            StringAssert.Contains(FakeContactDefinition.Columns.Email, xml);
            StringAssert.Contains(FakeContactDefinition.Columns.FullName, xml);
        }

        [Test]
        public void HasConditions_WithCondition_ReturnsTrue()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.Criteria.AddCondition(FakeContactDefinition.Columns.Email, ConditionOperator.NotNull);

            Assert.IsTrue(query.Criteria.HasConditions);
        }

        [Test]
        public void HasConditions_Empty_ReturnsFalse()
        {
            var query = new Query(FakeContactDefinition.EntityName);

            Assert.IsFalse(query.Criteria.HasConditions);
        }

        [Test]
        public void HasConditions_WithEmptyNestedFilter_ReturnsFalse()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.Criteria.AddFilter(LogicalOperator.Or);

            Assert.IsFalse(query.Criteria.HasConditions);
        }

        [Test]
        public void HasConditions_WithConditionInNestedFilter_ReturnsTrue()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            var orFilter = query.Criteria.AddFilter(LogicalOperator.Or);
            orFilter.AddCondition(FakeContactDefinition.Columns.Email, ConditionOperator.NotNull);

            Assert.IsTrue(query.Criteria.HasConditions);
        }

        [Test]
        public void IsWebApiFriendly_AllFriendlyConditions_ReturnsTrue()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.Criteria.AddCondition(FakeContactDefinition.Columns.Email, ConditionOperator.NotNull);

            Assert.IsTrue(query.Criteria.IsWebApiFriendly);
        }

        [Test]
        public void IsWebApiFriendly_WithUnfriendlyCondition_ReturnsFalse()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.Criteria.AddCondition(FakeContactDefinition.Columns.BirthDate, ConditionOperator.Yesterday);

            Assert.IsFalse(query.Criteria.IsWebApiFriendly);
        }

        [Test]
        public void ToWebApiString_SingleCondition_ReturnsConditionString()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.Criteria.AddCondition(FakeContactDefinition.Columns.Email, ConditionOperator.NotNull);
            int i = 0;

            var result = query.Criteria.ToWebApiString(() => ++i);

            StringAssert.Contains("ne null", result);
        }

        [Test]
        public void ToWebApiString_TwoConditions_JoinsWithAndOperator()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.Criteria.AddCondition(FakeContactDefinition.Columns.Email, ConditionOperator.NotNull);
            query.Criteria.AddCondition(FakeContactDefinition.Columns.FullName, ConditionOperator.NotNull);
            int i = 0;

            var result = query.Criteria.ToWebApiString(() => ++i);

            StringAssert.Contains(" and ", result);
        }

        [Test]
        public void AddFilter_ReturnsNewFilterWithSameEntityName()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            var filter = query.Criteria.AddFilter(LogicalOperator.Or);

            Assert.IsNotNull(filter);
            Assert.AreEqual(LogicalOperator.Or, filter.Operator);
            Assert.AreEqual(1, query.Criteria.Filters.Count);
        }
    }
}
