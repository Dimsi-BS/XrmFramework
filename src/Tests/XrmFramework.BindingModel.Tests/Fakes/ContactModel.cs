// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace XrmFramework.BindingModel.Tests.Fakes
{
    /// <summary>
    /// Simple binding model implementing <see cref="IBindingModel"/> directly (no <see cref="BindingModelBase"/>).
    /// Used to test the entity/XML mappers without the InitializedProperties tracking layer.
    /// </summary>
    [CrmEntity(ContactDefinition.EntityName)]
    public class ContactModel : IBindingModel
    {
        public Guid Id { get; set; }

        [CrmMapping(ContactDefinition.Columns.FullName)]
        public string? FullName { get; set; }

        [CrmMapping(ContactDefinition.Columns.Email)]
        public string? Email { get; set; }

        [CrmMapping(ContactDefinition.Columns.IsActive)]
        public bool? IsActive { get; set; }

        [CrmMapping(ContactDefinition.Columns.BirthDate)]
        public DateTime? BirthDate { get; set; }

        [CrmMapping(ContactDefinition.Columns.Revenue)]
        public decimal? Revenue { get; set; }

        [CrmMapping(ContactDefinition.Columns.StatusCode)]
        public ContactStatus StatusCode { get; set; }

        [CrmMapping(ContactDefinition.Columns.Interests)]
        public List<ContactInterest> Interests { get; } = new();

        [CrmMapping(ContactDefinition.Columns.AccountId)]
        public Guid AccountId { get; set; }
    }

    /// <summary>Status picklist for <see cref="ContactModel"/>.</summary>
    public enum ContactStatus
    {
        Null = 0,
        Active = 1,
        Inactive = 2,
    }

    /// <summary>Multi-select interests picklist for <see cref="ContactModel"/>.</summary>
    public enum ContactInterest
    {
        Null = 0,
        Sports = 1,
        Music = 2,
        Travel = 3,
    }
}
