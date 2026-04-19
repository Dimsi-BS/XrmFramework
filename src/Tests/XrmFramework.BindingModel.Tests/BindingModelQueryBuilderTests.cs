// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XrmFramework.BindingModel.Tests.Fakes;

namespace XrmFramework.BindingModel.Tests
{
    /// <summary>
    /// Unit tests for <see cref="BindingModelHelper.GetRetrieveAllQuery{T}"/> and the
    /// underlying <c>BindingModelQueryBuilder</c> logic.
    /// </summary>
    [TestClass]
    public class BindingModelQueryBuilderTests
    {
        // ------------------------------------------------------------------
        // Entity name
        // ------------------------------------------------------------------

        [TestMethod]
        public void GetRetrieveAllQuery_UsesCorrectEntityName()
        {
            var query = BindingModelHelper.GetRetrieveAllQuery<ContactModel>();

            Assert.AreEqual(ContactDefinition.EntityName, query.EntityName);
        }

        // ------------------------------------------------------------------
        // Column set — scalar attributes
        // ------------------------------------------------------------------

        [TestMethod]
        public void GetRetrieveAllQuery_IncludesAllMappedColumns()
        {
            var query = BindingModelHelper.GetRetrieveAllQuery<ContactModel>();
            var columns = query.ColumnSet.Columns;

            CollectionAssert.Contains(columns, ContactDefinition.Columns.FullName);
            CollectionAssert.Contains(columns, ContactDefinition.Columns.Email);
            CollectionAssert.Contains(columns, ContactDefinition.Columns.IsActive);
            CollectionAssert.Contains(columns, ContactDefinition.Columns.BirthDate);
            CollectionAssert.Contains(columns, ContactDefinition.Columns.Revenue);
            CollectionAssert.Contains(columns, ContactDefinition.Columns.StatusCode);
            CollectionAssert.Contains(columns, ContactDefinition.Columns.AccountId);
        }

        [TestMethod]
        public void GetRetrieveAllQuery_DoesNotDuplicateColumns()
        {
            var query = BindingModelHelper.GetRetrieveAllQuery<ContactModel>();
            var columns = query.ColumnSet.Columns;

            // All column names must be unique.
            var distinct = columns.Distinct().ToList();
            Assert.AreEqual(distinct.Count, columns.Count,
                "Column set should not contain duplicate entries.");
        }

        // ------------------------------------------------------------------
        // Lookup column — link entity
        // ------------------------------------------------------------------

        [TestMethod]
        public void GetRetrieveAllQuery_LookupAttribute_NullObjectTypedAsGuid_NoLinkEntityAdded()
        {
            // ContactModel.AccountId is a Guid-typed lookup without an explicit CrmLookupAttribute
            // on the property.  The query builder should still add the column but skip the link entity.
            var query = BindingModelHelper.GetRetrieveAllQuery<ContactModel>();

            // The column should be present.
            CollectionAssert.Contains(query.ColumnSet.Columns, ContactDefinition.Columns.AccountId);
        }

        // ------------------------------------------------------------------
        // Non-generic overload
        // ------------------------------------------------------------------

        [TestMethod]
        public void GetRetrieveAllQuery_NonGenericOverload_ReturnsSameQueryAsGeneric()
        {
            var generic = BindingModelHelper.GetRetrieveAllQuery<ContactModel>();
            var nonGeneric = BindingModelHelper.GetRetrieveAllQuery(typeof(ContactModel));

            Assert.AreEqual(generic.EntityName, nonGeneric.EntityName);
            CollectionAssert.AreEquivalent(
                generic.ColumnSet.Columns.ToList(),
                nonGeneric.ColumnSet.Columns.ToList());
        }

        // ------------------------------------------------------------------
        // Simple model with no lookup — no link entities
        // ------------------------------------------------------------------

        [TestMethod]
        public void GetRetrieveAllQuery_ModelWithNoLookup_HasNoLinkEntities()
        {
            var query = BindingModelHelper.GetRetrieveAllQuery<ContactModelWithBase>();

            Assert.AreEqual(0, query.LinkEntities.Count,
                "A model without IBindingModel-typed lookup properties should produce no link entities.");
        }
    }
}
