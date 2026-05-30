// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;
using NUnit.Framework.Legacy;
using XrmFramework.Sdk.Queries;
using XrmFramework.Tests.Sdk.Queries.Fakes;

namespace XrmFramework.Tests.Sdk.Queries
{
    [TestFixture]
    public class QueryToFetchXmlTests
    {
        [Test]
        public void ToFetchXmlString_BasicQuery_ContainsEntityNameAndPrimaryColumns()
        {
            var query = new Query(ContactDefinition.EntityName);

            var xml = query.ToFetchXmlString();

            StringAssert.Contains($"<entity name=\"{ContactDefinition.EntityName}\">", xml);
            StringAssert.Contains($"<attribute name=\"{ContactDefinition.Columns.Id}\"", xml);
            StringAssert.Contains($"<attribute name=\"{ContactDefinition.Columns.FullName}\"", xml);
        }

        [Test]
        public void ToFetchXmlString_WithTopCount_ContainsCountAttribute()
        {
            var query = new Query(ContactDefinition.EntityName) { TopCount = 50 };

            var xml = query.ToFetchXmlString();

            StringAssert.Contains("count=\"50\"", xml);
        }

        [Test]
        public void ToFetchXmlString_WithPaging_ContainsPageAndCount()
        {
            var query = new Query(ContactDefinition.EntityName);
            query.PagingInfo.PageSize = 25;
            query.PagingInfo.PageNumber = 2;

            var xml = query.ToFetchXmlString();

            StringAssert.Contains("count=\"25\"", xml);
            StringAssert.Contains("page=\"2\"", xml);
        }

        [Test]
        public void ToFetchXmlString_WithPagingCookie_ContainsCookieAttribute()
        {
            var query = new Query(ContactDefinition.EntityName);
            query.PagingInfo.PageSize = 10;
            query.PagingInfo.PageNumber = 3;
            query.PagingInfo.PagingCookie = "encodedcookie==";

            var xml = query.ToFetchXmlString();

            StringAssert.Contains("paging-cookie=\"encodedcookie==\"", xml);
        }

        [Test]
        public void ToFetchXmlString_WithAdditionalColumn_ContainsAttribute()
        {
            var query = new Query(ContactDefinition.EntityName);
            query.AddColumn(ContactDefinition.Columns.Email);

            var xml = query.ToFetchXmlString();

            StringAssert.Contains($"<attribute name=\"{ContactDefinition.Columns.Email}\"", xml);
        }

        [Test]
        public void ToFetchXmlString_WithOrder_ContainsOrderElement()
        {
            var query = new Query(ContactDefinition.EntityName);
            query.AddOrder(ContactDefinition.Columns.FullName, descending: false);

            var xml = query.ToFetchXmlString();

            StringAssert.Contains($"<order attribute=\"{ContactDefinition.Columns.FullName}\" descending=\"false\"", xml);
        }

        [Test]
        public void ToFetchXmlString_WithDescendingOrder_ContainsDescendingTrue()
        {
            var query = new Query(ContactDefinition.EntityName);
            query.AddOrder(ContactDefinition.Columns.FullName, descending: true);

            var xml = query.ToFetchXmlString();

            StringAssert.Contains("descending=\"true\"", xml);
        }

        [Test]
        public void ToFetchXmlString_WithCondition_ContainsFilterElement()
        {
            var query = new Query(ContactDefinition.EntityName);
            query.Criteria.AddCondition(ContactDefinition.Columns.Email, ConditionOperator.NotNull);

            var xml = query.ToFetchXmlString();

            StringAssert.Contains("<filter", xml);
            StringAssert.Contains($"attribute=\"{ContactDefinition.Columns.Email}\"", xml);
        }

        [Test]
        public void ToFetchXmlString_WithLink_ContainsLinkEntityElement()
        {
            var query = new Query(ContactDefinition.EntityName);
            query.AddLink(AccountDefinition.EntityName, ContactDefinition.Columns.AccountId, AccountDefinition.Columns.Id, addColumns: false);

            var xml = query.ToFetchXmlString();

            StringAssert.Contains($"<link-entity name=\"{AccountDefinition.EntityName}\"", xml);
        }

        [Test]
        public void ToFetchXmlString_WellFormedXml_HasOpenAndCloseEntityTags()
        {
            var query = new Query(ContactDefinition.EntityName);

            var xml = query.ToFetchXmlString();

            StringAssert.Contains("<entity name=", xml);
            StringAssert.Contains("</entity></fetch>", xml);
        }
    }
}
