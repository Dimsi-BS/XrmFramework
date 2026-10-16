// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.BindingModel.Tests.Fakes
{
    /// <summary>
    /// A writable (default <c>IsValidForUpdate</c>) <see cref="Guid"/> property mapped to
    /// <see cref="ContactDefinition.Columns.BackupCustomerId"/>, a lookup reaching both account and
    /// contact. Building this type's <see cref="ModelDefinition"/> must fail — a bare Guid cannot say
    /// which of the two tables it means.
    /// </summary>
    [CrmEntity(ContactDefinition.EntityName)]
    public class ContactWithWritableGuidOnPolymorphicLookupModel : IBindingModel
    {
        public Guid Id { get; set; }

        [CrmMapping(ContactDefinition.Columns.BackupCustomerId)]
        public Guid BackupCustomerId { get; set; }
    }

    /// <summary>
    /// Same ambiguous column, but the mapping is explicitly read-only: nothing is ever written back
    /// through it, so there is nothing to guess and no problem to report.
    /// </summary>
    [CrmEntity(ContactDefinition.EntityName)]
    public class ContactWithReadOnlyGuidOnPolymorphicLookupModel : IBindingModel
    {
        public Guid Id { get; set; }

        [CrmMapping(ContactDefinition.Columns.BackupCustomerId, IsValidForUpdate = false)]
        public Guid BackupCustomerId { get; set; }
    }

    /// <summary>
    /// Same ambiguous column, but typed <see cref="EntityReference"/>: it already carries its own
    /// target table, so there is nothing to guess either.
    /// </summary>
    [CrmEntity(ContactDefinition.EntityName)]
    public class ContactWithEntityReferenceOnPolymorphicLookupModel : IBindingModel
    {
        public Guid Id { get; set; }

        [CrmMapping(ContactDefinition.Columns.BackupCustomerId)]
        public EntityReference? BackupCustomerId { get; set; }
    }

    /// <summary>
    /// A writable Guid mapped to <see cref="ContactDefinition.Columns.AccountId"/>, an ordinary
    /// single-target lookup — the overwhelmingly common case, which must keep working exactly as
    /// before this check was added.
    /// </summary>
    [CrmEntity(ContactDefinition.EntityName)]
    public class ContactWithWritableGuidOnSingleTargetLookupModel : IBindingModel
    {
        public Guid Id { get; set; }

        [CrmMapping(ContactDefinition.Columns.AccountId)]
        public Guid AccountId { get; set; }
    }
}
