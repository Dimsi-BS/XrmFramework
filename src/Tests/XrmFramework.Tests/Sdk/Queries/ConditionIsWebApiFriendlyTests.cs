// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;
using XrmFramework.Sdk.Queries;
using XrmFramework.Tests.Sdk.Queries.Fakes;

namespace XrmFramework.Tests.Sdk.Queries
{
    [TestFixture]
    public class ConditionIsWebApiFriendlyTests
    {
        private static Condition GetCondition(string attribute, ConditionOperator op, string alias = null)
        {
            var query = new Query(FakeContactDefinition.EntityName);
            if (alias != null)
                query.Criteria.AddCondition(alias, attribute, op, null);
            else
                query.Criteria.AddCondition(attribute, op);
            return query.Criteria.Conditions[0];
        }

        [Test]
        public void IsWebApiFriendly_EqualOperator_ReturnsTrue()
        {
            var condition = GetCondition(FakeContactDefinition.Columns.Email, ConditionOperator.Equal);
            Assert.IsTrue(condition.IsWebApiFriendly);
        }

        [Test]
        public void IsWebApiFriendly_NotEqualOperator_ReturnsTrue()
        {
            var condition = GetCondition(FakeContactDefinition.Columns.Email, ConditionOperator.NotEqual);
            Assert.IsTrue(condition.IsWebApiFriendly);
        }

        [Test]
        public void IsWebApiFriendly_InOperator_ReturnsTrue()
        {
            var condition = GetCondition(FakeContactDefinition.Columns.Email, ConditionOperator.In);
            Assert.IsTrue(condition.IsWebApiFriendly);
        }

        [Test]
        public void IsWebApiFriendly_NullOperator_ReturnsTrue()
        {
            var condition = GetCondition(FakeContactDefinition.Columns.Email, ConditionOperator.Null);
            Assert.IsTrue(condition.IsWebApiFriendly);
        }

        [Test]
        public void IsWebApiFriendly_YesterdayOperator_ReturnsFalse()
        {
            var condition = GetCondition(FakeContactDefinition.Columns.BirthDate, ConditionOperator.Yesterday);
            Assert.IsFalse(condition.IsWebApiFriendly);
        }

        [Test]
        public void IsWebApiFriendly_LastXDaysOperator_ReturnsFalse()
        {
            var condition = GetCondition(FakeContactDefinition.Columns.BirthDate, ConditionOperator.LastXDays);
            Assert.IsFalse(condition.IsWebApiFriendly);
        }

        [Test]
        public void IsWebApiFriendly_WithEntityAlias_ReturnsFalse()
        {
            var condition = GetCondition(FakeContactDefinition.Columns.Email, ConditionOperator.Equal, "alias");
            Assert.IsFalse(condition.IsWebApiFriendly);
        }

        [Test]
        public void IsWebApiFriendly_EqualUserIdOperator_ReturnsFalse()
        {
            var condition = GetCondition(FakeContactDefinition.Columns.Email, ConditionOperator.EqualUserId);
            Assert.IsFalse(condition.IsWebApiFriendly);
        }
    }
}
