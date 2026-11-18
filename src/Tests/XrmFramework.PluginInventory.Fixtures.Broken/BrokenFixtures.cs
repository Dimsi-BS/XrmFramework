// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using XrmFramework.Workflow;

namespace XrmFramework.PluginInventory.Fixtures.Broken
{
    /// <summary>
    ///     Custom Workflow Activity whose action names a method that does not exist, so the framework
    ///     throws from the constructor.
    /// </summary>
    /// <remarks>
    ///     The inventory used to swallow this and report the activity under its type name, which
    ///     meant a broken activity deployed quietly under a plausible-looking friendly name. It now
    ///     has to fail and say which type it was.
    /// </remarks>
    public sealed class BrokenActivity : CustomWorkflowActivity
    {
        public BrokenActivity() => SetAction("ThisMethodDoesNotExist");
    }
}
