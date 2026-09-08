// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using XrmFramework;
using XrmFramework.BindingModel;

namespace XrmFramework.BindingModel.Tests.Models
{
    /// <summary>Hand-written target for <see cref="SdkMessageProcessingStepManualModel.PluginType"/>.</summary>
    [CrmEntity(typeof(PluginTypeDefinition))]
    public partial class PluginTypeManualModel : IBindingModel
    {
        public Guid Id { get; set; }

        [CrmMapping(PluginTypeDefinition.Columns.TypeName)]
        public string TypeName { get; set; }
    }
}
