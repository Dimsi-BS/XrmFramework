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
            var query = new Query(ContactDefinition.EntityName);

            var result = query.ToWebApiString();

            StringAssert.StartsWith("$select=", result);
            StringAssert.Contains(ContactDefinition.Columns.Id, result);
            StringAssert.Contains(ContactDefinition.Columns.FullName, result);
        }

        [Test]
        public void ToWebApiString_WithPagingCookie_ReturnsStringAfterCollectionName()
        {
            var query = new Query(ContactDefinition.EntityName);
            query.PagingInfo.PagingCookie = $"{ContactDefinition.EntityCollectionName}?$skiptoken=xyz";

            var result = query.ToWebApiString();

            Assert.AreEqual("$skiptoken=xyz", result);
        }

        [Test]
        public void ToWebApiString_WithCondition_ContainsFilterParam()
        {
            var query = new Query(ContactDefinition.EntityName);
            query.Criteria.AddCondition(ContactDefinition.Columns.Email, ConditionOperator.NotNull);

            var result = query.ToWebApiString();

            StringAssert.Contains("$filter=", result);
            StringAssert.Contains(ContactDefinition.Columns.Email, result);
            StringAssert.Contains("ne null", result);
        }

        [Test]
        public void ToWebApiString_WithOrder_ContainsOrderByParam()
        {
            var query = new Query(ContactDefinition.EntityName);
            query.AddOrder(ContactDefinition.Columns.FullName);

            var result = query.ToWebApiString();

            StringAssert.Contains("$orderby=", result);
            StringAssert.Contains(ContactDefinition.Columns.FullName, result);
            StringAssert.Contains("asc", result);
        }

        [Test]
        public void ToWebApiString_WithDescendingOrder_ContainsDesc()
        {
            var query = new Query(ContactDefinition.EntityName);
            query.AddOrder(ContactDefinition.Columns.FullName, descending: true);

            var result = query.ToWebApiString();

            StringAssert.Contains("desc", result);
        }

        [Test]
        public void ToWebApiString_WithTopCount_ContainsTopParam()
        {
            var query = new Query(ContactDefinition.EntityName) { TopCount = 10 };

            var result = query.ToWebApiString();

            StringAssert.Contains("$top=10", result);
        }

        [Test]
        public void ToWebApiString_LookupAttribute_UsesUnderscoreValueSuffix()
        {
            var query = new Query(ContactDefinition.EntityName);
            query.AddColumn(ContactDefinition.Columns.AccountId);
            query.Criteria.AddCondition(ContactDefinition.Columns.AccountId, ConditionOperator.NotNull);

            var result = query.ToWebApiString();

            StringAssert.Contains($"_{ContactDefinition.Columns.AccountId}_value", result);
        }
    }
}
