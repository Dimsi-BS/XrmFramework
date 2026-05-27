// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;
using Microsoft.Xrm.Sdk.Query;

namespace XrmFramework.Tests.Extensions
{
    [TestFixture]
    public class QueryExpressionExtensionsTests
    {
        // ────────────────────────────────────────────────────────────
        //  GetRootFilterExpression
        // ────────────────────────────────────────────────────────────

        [Test]
        public void GetRootFilterExpression_CriteriaHasConditions_ReturnsCriteriaItself()
        {
            var query = new QueryExpression("contact");
            query.Criteria.AddCondition("firstname", ConditionOperator.Equal, "Alice");

            var result = query.GetRootFilterExpression();

            Assert.AreSame(query.Criteria, result);
        }

        [Test]
        public void GetRootFilterExpression_CriteriaHasNoConditionsButHasSubFilter_ReturnsFirstSubFilter()
        {
            var query = new QueryExpression("contact");
            var subFilter = new FilterExpression();
            subFilter.AddCondition("lastname", ConditionOperator.Equal, "Smith");
            query.Criteria.AddFilter(subFilter);

            var result = query.GetRootFilterExpression();

            Assert.AreSame(subFilter, result);
        }

        [Test]
        public void GetRootFilterExpression_EmptyCriteria_ReturnsNull()
        {
            var query = new QueryExpression("contact");

            var result = query.GetRootFilterExpression();

            Assert.IsNull(result);
        }

        // ────────────────────────────────────────────────────────────
        //  GetConditionValue<T>
        // ────────────────────────────────────────────────────────────

        [Test]
        public void GetConditionValue_ConditionPresentInRootFilter_ReturnsValue()
        {
            var filter = new FilterExpression();
            filter.AddCondition("firstname", ConditionOperator.Equal, "Alice");

            var result = filter.GetConditionValue<string>("firstname");

            Assert.AreEqual("Alice", result);
        }

        [Test]
        public void GetConditionValue_ConditionNotPresent_ReturnsDefault()
        {
            var filter = new FilterExpression();
            filter.AddCondition("lastname", ConditionOperator.Equal, "Smith");

            var result = filter.GetConditionValue<string>("firstname");

            Assert.IsNull(result);
        }

        [Test]
        public void GetConditionValue_IntDefault_ReturnsZeroWhenNotFound()
        {
            var filter = new FilterExpression();

            var result = filter.GetConditionValue<int>("age");

            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetConditionValue_ConditionInNestedFilter_ReturnsValue()
        {
            var rootFilter = new FilterExpression();
            var childFilter = new FilterExpression();
            childFilter.AddCondition("city", ConditionOperator.Equal, "Paris");
            rootFilter.AddFilter(childFilter);

            var result = rootFilter.GetConditionValue<string>("city");

            Assert.AreEqual("Paris", result);
        }

        [Test]
        public void GetConditionValue_ConditionInDeeplyNestedFilter_ReturnsValue()
        {
            var rootFilter   = new FilterExpression();
            var childFilter  = new FilterExpression();
            var grandChild   = new FilterExpression();
            grandChild.AddCondition("country", ConditionOperator.Equal, "France");
            childFilter.AddFilter(grandChild);
            rootFilter.AddFilter(childFilter);

            var result = rootFilter.GetConditionValue<string>("country");

            Assert.AreEqual("France", result);
        }

        // ────────────────────────────────────────────────────────────
        //  GetLinkCount
        // ────────────────────────────────────────────────────────────

        [Test]
        public void GetLinkCount_NoLinks_ReturnsZero()
        {
            var query = new QueryExpression("contact");

            Assert.AreEqual(0, query.GetLinkCount());
        }

        [Test]
        public void GetLinkCount_SingleFlatLink_ReturnsOne()
        {
            var query = new QueryExpression("contact");
            query.AddLink("account", "accountid", "accountid");

            Assert.AreEqual(1, query.GetLinkCount());
        }

        [Test]
        public void GetLinkCount_MultipleFlatLinks_ReturnsCorrectCount()
        {
            var query = new QueryExpression("contact");
            query.AddLink("account", "accountid", "accountid");
            query.AddLink("systemuser", "ownerid", "systemuserid");

            Assert.AreEqual(2, query.GetLinkCount());
        }

        [Test]
        public void GetLinkCount_NestedLinks_CountsAllLevels()
        {
            var query = new QueryExpression("contact");
            var link  = query.AddLink("account", "accountid", "accountid");
            link.AddLink("systemuser", "ownerid", "systemuserid");

            // 1 top-level link + 1 nested = 2
            Assert.AreEqual(2, query.GetLinkCount());
        }

        [Test]
        public void GetLinkCount_DeepNestedLinks_CountsAllLevels()
        {
            var query = new QueryExpression("contact");
            var link1 = query.AddLink("account", "accountid", "accountid");
            var link2 = link1.AddLink("systemuser", "ownerid", "systemuserid");
            link2.AddLink("businessunit", "businessunitid", "businessunitid");

            // 3 links total across 3 levels
            Assert.AreEqual(3, query.GetLinkCount());
        }
    }
}
