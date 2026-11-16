// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace XrmFramework.BindingModel.Tests.Fakes
{
    /// <summary>
    /// Hand-written binding model over a multi-select choice, in the shape the bug was reported in:
    /// a <c>[CrmMapping]</c> property typed <see cref="ICollection{T}" /> of <see cref="int" />,
    /// read through <c>RetrieveAll&lt;T&gt;</c>.
    /// </summary>
    /// <remarks>
    /// The definitions this maps to are generated here from <c>Definitions/*.table</c>, so what the
    /// mapper branches on is the metadata the table generator really emits — the reason this used to
    /// throw <c>InvalidCastException</c> was that generator declaring the column a plain
    /// <c>Picklist</c>, and a fake definition hand-written with the right attribute would have hidden
    /// exactly that.
    /// </remarks>
    [CrmEntity(MultiSelectActivityDefinition.EntityName)]
    public class MultiSelectActivityModel : IBindingModel
    {
        public Guid Id { get; set; }

        [CrmMapping(MultiSelectActivityDefinition.Columns.Name)]
        public string? Name { get; set; }

        /// <summary>The multi-select column read as the raw option values.</summary>
        [CrmMapping(MultiSelectActivityDefinition.Columns.ReportTypes)]
        public ICollection<int>? ReportTypes { get; set; }

        /// <summary>
        /// The same column on a second model property would need a second column: this one reaches a
        /// multi-select declared on the table at the far end of a lookup, which the query brings back
        /// as an aliased value.
        /// </summary>
        [CrmMapping(MultiSelectActivityDefinition.Columns.ProjectId, IsValidForUpdate = false)]
        [CrmLookup(MultiSelectProjectDefinition.EntityName,
            MultiSelectProjectDefinition.Columns.BillingTypes)]
        public ICollection<int>? ProjectBillingTypes { get; set; }
    }

    /// <summary>
    /// The same column typed on the enum the table generator emits for it, settable rather than
    /// get-only: a property whose assignment is what the boxed <c>ICollection&lt;object&gt;</c>
    /// behind <c>ToEnumCollection</c> used to make impossible.
    /// </summary>
    [CrmEntity(MultiSelectActivityDefinition.EntityName)]
    public class MultiSelectActivityEnumModel : IBindingModel
    {
        public Guid Id { get; set; }

        [CrmMapping(MultiSelectActivityDefinition.Columns.ReportTypes)]
        public ICollection<MultiSelectReportType>? ReportTypes { get; set; }
    }
}
