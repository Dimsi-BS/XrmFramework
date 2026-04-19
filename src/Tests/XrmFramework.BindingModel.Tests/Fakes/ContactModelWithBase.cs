// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework.BindingModel.Tests.Fakes
{
    /// <summary>
    /// Binding model that extends <see cref="BindingModelBase"/> to test the <c>InitializedProperties</c>
    /// tracking feature: only properties whose setters are called will be written back to the CRM entity.
    /// </summary>
    [CrmEntity(ContactDefinition.EntityName)]
    public class ContactModelWithBase : BindingModelBase
    {
        [CrmMapping(ContactDefinition.Columns.FullName)]
        public string? FullName
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [CrmMapping(ContactDefinition.Columns.Email)]
        public string? Email
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [CrmMapping(ContactDefinition.Columns.IsActive)]
        public bool? IsActive
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [CrmMapping(ContactDefinition.Columns.Revenue)]
        public decimal? Revenue
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        [CrmMapping(ContactDefinition.Columns.StatusCode)]
        public ContactStatus StatusCode
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }
}
