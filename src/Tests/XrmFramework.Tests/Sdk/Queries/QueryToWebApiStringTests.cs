// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;
using NUnit.Framework.Legacy;
using XrmFramework.Sdk.Queries;
using XrmFramework.Tests.Sdk.Queries.Fakes;

namespace XrmFramework.Tests.Sdk.Queries
{
    [TestFixture]
    public class QueryToWebApiStringTests
    {
        [Test]
        public void ToWebApiString_BasicQuery_StartsWithSelectAndPrimaryColumns()
        {
            var query = new Query(FakeContactDefinition.EntityName);

            var result = query.ToWebApiString();

            StringAssert.StartsWith("$select=", result);
            StringAssert.Contains(FakeContactDefinition.Columns.Id, result);
            StringAssert.Contains(FakeContactDefinition.Columns.FullName, result);
        }

        [Test]
        public void ToWebApiString_WithPagingCookie_ReturnsStringAfterCollectionName()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.PagingInfo.PagingCookie = $"{FakeContactDefinition.EntityCollectionName}?$skiptoken=xyz";

            var result = query.ToWebApiString();

            Assert.AreEqual("$skiptoken=xyz", result);
        }

        [Test]
        public void ToWebApiString_WithCondition_ContainsFilterParam()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.Criteria.AddCondition(FakeContactDefinition.Columns.Email, ConditionOperator.NotNull);

            var result = query.ToWebApiString();

            StringAssert.Contains("$filter=", result);
            StringAssert.Contains(FakeContactDefinition.Columns.Email, result);
            StringAssert.Contains("ne null", result);
        }

        [Test]
        public void ToWebApiString_WithOrder_ContainsOrderByParam()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.AddOrder(FakeContactDefinition.Columns.FullName);

            var result = query.ToWebApiString();

            StringAssert.Contains("$orderby=", result);
            StringAssert.Contains(FakeContactDefinition.Columns.FullName, result);
            StringAssert.Contains("asc", result);
        }

        [Test]
        public void ToWebApiString_WithDescendingOrder_ContainsDesc()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.AddOrder(FakeContactDefinition.Columns.FullName, descending: true);

            var result = query.ToWebApiString();

            StringAssert.Contains("desc", result);
        }

        [Test]
        public void ToWebApiString_WithTopCount_ContainsTopParam()
        {
            var query = new Query(FakeContactDefinition.EntityName) { TopCount = 10 };

            var result = query.ToWebApiString();

            StringAssert.Contains("$top=10", result);
        }

        [Test]
        public void ToWebApiString_LookupAttribute_UsesUnderscoreValueSuffix()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.AddColumn(FakeContactDefinition.Columns.FakeAccountId);
            query.Criteria.AddCondition(FakeContactDefinition.Columns.FakeAccountId, ConditionOperator.NotNull);

            var result = query.ToWebApiString();

            StringAssert.Contains($"_{FakeContactDefinition.Columns.FakeAccountId}_value", result);
        }
    }
}
