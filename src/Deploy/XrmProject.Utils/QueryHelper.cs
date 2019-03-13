// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;

namespace Plugins.Tests
{
    public static class AssertHelper
    {
        public static void AssertCondition(this QueryExpression query, ConditionOperator conditionOperator, string columnName, params object[] values)
        {
            Assert.IsTrue(query.Criteria.Conditions.Any(c => c.AttributeName == columnName));

            var condition = query.Criteria.Conditions.First(c => c.AttributeName == columnName);

            Assert.ReferenceEquals(conditionOperator, condition.Operator);
            CollectionAssert.ReferenceEquals(condition.Values, values);
        }
        public static void AssertCondition(this QueryExpression query, string columnName, params object[] values)
        {
            AssertCondition(query, ConditionOperator.Equal, columnName, values);
        }

        public static void AssertColumns(this ColumnSet cs, params string[] columns)
        {
            CollectionAssert.AreEquivalent(cs.Columns, columns, "Columns are not OK");
        }

        public static void AssertColumn<T>(this Entity e, string columnName, T value)
        {
            var val = e.GetAttributeValue<T>(columnName);

            Assert.AreEqual(value, val);
        }
    }
}
