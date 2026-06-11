// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;
using NUnit.Framework.Legacy;
using XrmFramework.Sdk.Queries;
using XrmFramework.Tests.Sdk.Queries.Fakes;

namespace XrmFramework.Tests.Sdk.Queries
{
    [TestFixture]
    public class ConditionToWebApiStringTests
    {
        private static string GetWebApiString(string attribute, ConditionOperator op, object value = null)
        {
            var query = new Query(FakeContactDefinition.EntityName);
            if (value != null)
                query.Criteria.AddCondition(attribute, op, value);
            else
                query.Criteria.AddCondition(attribute, op);
            int i = 0;
            return query.Criteria.Conditions[0].ToWebApiString(() => ++i);
        }

        [Test]
        public void ToWebApiString_NullOperator_ReturnsEqNull()
        {
            var result = GetWebApiString(FakeContactDefinition.Columns.Email, ConditionOperator.Null);

            Assert.AreEqual($"{FakeContactDefinition.Columns.Email} eq null", result);
        }

        [Test]
        public void ToWebApiString_NotNullOperator_ReturnsNeNull()
        {
            var result = GetWebApiString(FakeContactDefinition.Columns.Email, ConditionOperator.NotNull);

            Assert.AreEqual($"{FakeContactDefinition.Columns.Email} ne null", result);
        }

        [Test]
        public void ToWebApiString_EqualStringValue_ReturnsEqWithQuotedValue()
        {
            var result = GetWebApiString(FakeContactDefinition.Columns.Email, ConditionOperator.Equal, "test@test.com");

            Assert.AreEqual($"{FakeContactDefinition.Columns.Email} eq 'test@test.com'", result);
        }

        [Test]
        public void ToWebApiString_GreaterThanIntValue_ReturnsGtOperator()
        {
            var result = GetWebApiString(FakeContactDefinition.Columns.Age, ConditionOperator.GreaterThan, 18);

            StringAssert.Contains(" gt ", result);
            StringAssert.Contains("18", result);
        }

        [Test]
        public void ToWebApiString_LikeWithBothWildcards_ReturnsContainsFunction()
        {
            var result = GetWebApiString(FakeContactDefinition.Columns.Email, ConditionOperator.Like, "%test%");

            Assert.AreEqual($"contains({FakeContactDefinition.Columns.Email}, 'test')", result);
        }

        [Test]
        public void ToWebApiString_LikeWithLeadingWildcard_ReturnsEndswithFunction()
        {
            var result = GetWebApiString(FakeContactDefinition.Columns.Email, ConditionOperator.Like, "%test");

            Assert.AreEqual($"endswith({FakeContactDefinition.Columns.Email}, 'test')", result);
        }

        [Test]
        public void ToWebApiString_LikeWithTrailingWildcard_ReturnsStartswithFunction()
        {
            var result = GetWebApiString(FakeContactDefinition.Columns.Email, ConditionOperator.Like, "test%");

            Assert.AreEqual($"startswith({FakeContactDefinition.Columns.Email}, 'test')", result);
        }

        [Test]
        public void ToWebApiString_NotLike_ReturnsPrefixedContains()
        {
            var result = GetWebApiString(FakeContactDefinition.Columns.Email, ConditionOperator.NotLike, "%test%");

            StringAssert.StartsWith("not ", result);
            StringAssert.Contains("contains(", result);
        }

        [Test]
        public void ToWebApiString_BeginsWith_ReturnsStartswithFunction()
        {
            var result = GetWebApiString(FakeContactDefinition.Columns.Email, ConditionOperator.BeginsWith, "john");

            Assert.AreEqual($"startswith({FakeContactDefinition.Columns.Email}, 'john')", result);
        }

        [Test]
        public void ToWebApiString_DoesNotBeginWith_ReturnsPrefixedStartswith()
        {
            var result = GetWebApiString(FakeContactDefinition.Columns.Email, ConditionOperator.DoesNotBeginWith, "john");

            Assert.AreEqual($"not startswith({FakeContactDefinition.Columns.Email}, 'john')", result);
        }

        [Test]
        public void ToWebApiString_EndsWith_ReturnsEndswithFunction()
        {
            var result = GetWebApiString(FakeContactDefinition.Columns.Email, ConditionOperator.EndsWith, ".com");

            Assert.AreEqual($"endswith({FakeContactDefinition.Columns.Email}, '.com')", result);
        }

        [Test]
        public void ToWebApiString_DoesNotEndWith_ReturnsPrefixedEndswith()
        {
            var result = GetWebApiString(FakeContactDefinition.Columns.Email, ConditionOperator.DoesNotEndWith, ".com");

            Assert.AreEqual($"not endswith({FakeContactDefinition.Columns.Email}, '.com')", result);
        }

        [Test]
        public void ToWebApiString_InOperator_UsesAliasAndOrExpression()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.Criteria.AddCondition(FakeContactDefinition.Columns.Email, ConditionOperator.In,
                new object[] { "a@a.com", "b@b.com" });
            int i = 0;
            var result = query.Criteria.Conditions[0].ToWebApiString(() => ++i);

            StringAssert.Contains("@p1", result);
            StringAssert.Contains("eq", result);
            StringAssert.Contains(" or", result);
            Assert.IsTrue(query.Criteria.Conditions[0].UseAlias);
        }

        [Test]
        public void ToWebApiString_NotInOperator_UsesNegateAndAndExpression()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.Criteria.AddCondition(FakeContactDefinition.Columns.Email, ConditionOperator.NotIn,
                new object[] { "a@a.com", "b@b.com" });
            int i = 0;
            var result = query.Criteria.Conditions[0].ToWebApiString(() => ++i);

            StringAssert.StartsWith("not ", result);
            StringAssert.Contains("neq", result);
            StringAssert.Contains(" and", result);
        }
    }
}
