// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;
using NUnit.Framework.Legacy;
using XrmFramework.Sdk.Queries;
using XrmFramework.Tests.Sdk.Queries.Fakes;

namespace XrmFramework.Tests.Sdk.Queries
{
    [TestFixture]
    public class LinkTests
    {
        [Test]
        public void ToFetchXmlString_Basic_ContainsLinkEntityAttributes()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.AddLink(FakeAccountDefinition.EntityName, FakeContactDefinition.Columns.FakeAccountId, FakeAccountDefinition.Columns.Id, addColumns: false);

            var xml = query.Links[0].ToFetchXmlString();

            StringAssert.Contains($"name=\"{FakeAccountDefinition.EntityName}\"", xml);
            StringAssert.Contains($"from=\"{FakeAccountDefinition.Columns.Id}\"", xml);
            StringAssert.Contains($"to=\"{FakeContactDefinition.Columns.FakeAccountId}\"", xml);
        }

        [Test]
        public void ToFetchXmlString_InnerJoin_ContainsInnerLinkType()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.AddLink(FakeAccountDefinition.EntityName, FakeContactDefinition.Columns.FakeAccountId, FakeAccountDefinition.Columns.Id, JoinOperator.Inner, addColumns: false);

            var xml = query.Links[0].ToFetchXmlString();

            StringAssert.Contains("link-type=\"inner\"", xml);
        }

        [Test]
        public void ToFetchXmlString_OuterJoin_ContainsOuterLinkType()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.AddLink(FakeAccountDefinition.EntityName, FakeContactDefinition.Columns.FakeAccountId, FakeAccountDefinition.Columns.Id, JoinOperator.LeftOuter, addColumns: false);

            var xml = query.Links[0].ToFetchXmlString();

            StringAssert.Contains("link-type=\"outer\"", xml);
        }

        [Test]
        public void ToFetchXmlString_WithAlias_ContainsAliasAttribute()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            var link = query.AddLink(FakeAccountDefinition.EntityName, FakeContactDefinition.Columns.FakeAccountId, FakeAccountDefinition.Columns.Id, addColumns: false);
            link.EntityAlias = "acc";

            var xml = link.ToFetchXmlString();

            StringAssert.Contains("alias=\"acc\"", xml);
        }

        [Test]
        public void ToFetchXmlString_WithColumns_ContainsAttributeElements()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            var link = query.AddLink(FakeAccountDefinition.EntityName, FakeContactDefinition.Columns.FakeAccountId, FakeAccountDefinition.Columns.Id, addColumns: false);
            link.AddColumn(FakeAccountDefinition.Columns.Name);

            var xml = link.ToFetchXmlString();

            StringAssert.Contains($"<attribute name=\"{FakeAccountDefinition.Columns.Name}\"", xml);
        }

        [Test]
        public void ToFetchXmlString_WithLinkCriteria_ContainsFilterElement()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            var link = query.AddLink(FakeAccountDefinition.EntityName, FakeContactDefinition.Columns.FakeAccountId, FakeAccountDefinition.Columns.Id, addColumns: false);
            link.Criteria.AddCondition(FakeAccountDefinition.Columns.City, ConditionOperator.NotNull);

            var xml = link.ToFetchXmlString();

            StringAssert.Contains("<filter", xml);
            StringAssert.Contains(FakeAccountDefinition.Columns.City, xml);
        }

        [Test]
        public void ToFetchXmlString_WellFormed_HasCloseTag()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.AddLink(FakeAccountDefinition.EntityName, FakeContactDefinition.Columns.FakeAccountId, FakeAccountDefinition.Columns.Id, addColumns: false);

            var xml = query.Links[0].ToFetchXmlString();

            StringAssert.EndsWith("</link-entity>", xml);
        }

        [Test]
        public void IsWebApiFriendly_WithValidDefinition_ReturnsTrue()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            var link = query.AddLink(FakeAccountDefinition.EntityName, FakeContactDefinition.Columns.FakeAccountId, FakeAccountDefinition.Columns.Id, addColumns: true);

            Assert.IsTrue(link.IsWebApiFriendly);
        }

        [Test]
        public void IsWebApiFriendly_WithCriteria_ReturnsFalse()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            var link = query.AddLink(FakeAccountDefinition.EntityName, FakeContactDefinition.Columns.FakeAccountId, FakeAccountDefinition.Columns.Id, addColumns: false);
            link.Criteria.AddCondition(FakeAccountDefinition.Columns.City, ConditionOperator.NotNull);

            Assert.IsFalse(link.IsWebApiFriendly);
        }

        [Test]
        public void ToWebApiString_WithRelationship_ReturnsNavigationPropertyName()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            var link = query.AddLink(FakeAccountDefinition.EntityName, FakeContactDefinition.Columns.FakeAccountId, FakeAccountDefinition.Columns.Id, addColumns: false);
            link.AddColumn(FakeAccountDefinition.Columns.Name);

            var result = link.ToWebApiString();

            StringAssert.Contains("contact_customer_accounts_nav", result);
        }

        [Test]
        public void CleanLinks_RemovableLink_GetsRemovedFromQuery()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            query.AddLink(FakeAccountDefinition.EntityName, FakeContactDefinition.Columns.FakeAccountId, FakeAccountDefinition.Columns.Id, addColumns: true);

            query.CleanLinks();

            Assert.AreEqual(0, query.Links.Count);
        }

        [Test]
        public void CleanLinks_LinkWithCriteria_IsNotRemoved()
        {
            var query = new Query(FakeContactDefinition.EntityName);
            var link = query.AddLink(FakeAccountDefinition.EntityName, FakeContactDefinition.Columns.FakeAccountId, FakeAccountDefinition.Columns.Id, addColumns: true);
            link.Criteria.AddCondition(FakeAccountDefinition.Columns.City, ConditionOperator.NotNull);

            query.CleanLinks();

            Assert.AreEqual(1, query.Links.Count);
        }
    }
}
