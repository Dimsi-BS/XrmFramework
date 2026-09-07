// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using XrmFramework;
using XrmFramework.BindingModel;

namespace XrmFramework.BindingModel.Tests.Models
{
    /// <summary>
    /// Hand-written binding model embedding <see cref="PluginTypeManualModel"/> behind a lookup,
    /// with no <c>[CrmLookup]</c> of its own — proves <c>MappingSourceGenerator</c> resolves this
    /// the same way <c>ModelSourceFileGenerator</c> resolves a <c>LookupTargetModel</c> property in
    /// a <c>.model</c> file, from the embedded model's own <c>[CrmEntity]</c> rather than from any
    /// explicit lookup attribute.
    ///
    /// The embedded model is hand-written too, not <c>.model</c>-generated: a generator never sees
    /// another generator's output, so a property here typed with a <c>.model</c>-driven class in
    /// the same compilation pass would resolve to an error type instead.
    /// </summary>
    [CrmEntity(typeof(SdkMessageProcessingStepDefinition))]
    public partial class SdkMessageProcessingStepManualModel : IBindingModel
    {
        public Guid Id { get; set; }

        [CrmMapping(SdkMessageProcessingStepDefinition.Columns.PluginTypeId)]
        public PluginTypeManualModel PluginType { get; set; }
    }
}
