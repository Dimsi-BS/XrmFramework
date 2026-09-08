// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using XrmFramework;
using XrmFramework.BindingModel;

namespace XrmFramework.BindingModel.Tests.Models
{
    /// <summary>
    /// Hand-written binding model over a table with a real alternate key — proves
    /// <c>MappingMetadataFallback</c> resolves it for a hand-written class the same way
    /// <c>MappingModelFactory</c> resolves it for a <c>.model</c> file.
    /// </summary>
    [CrmEntity(typeof(SystemUserDefinition))]
    public partial class SystemUserManualModel : IBindingModel
    {
        public Guid Id { get; set; }

        [CrmMapping(SystemUserDefinition.Columns.AzureActiveDirectoryObjectId)]
        public Guid? AzureActiveDirectoryObjectId { get; set; }
    }
}
