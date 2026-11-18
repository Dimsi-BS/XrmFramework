// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

using NUnit.Framework;
using XrmFramework.RemoteDebugger;
using XrmFramework.Workflows.Tests.Fakes;

namespace XrmFramework.Workflows.Tests
{
    /// <summary>
    /// Which kind of context the remote debugger says it captured.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>IsWorkflowContext</c> is the only thing that tells the debugger client what to
    /// instantiate: a custom workflow activity is a <c>CodeActivity</c> run through a
    /// <c>WorkflowInvoker</c>, a plugin is an <c>IPlugin</c> whose <c>Execute</c> is called. A
    /// workflow activity reaching the client unflagged was cast to <c>IPlugin</c>, which it does
    /// not implement, and the session failed with
    /// <c>Unable to cast object of type 'Workflows.GetDashboardData' to type
    /// 'Microsoft.Xrm.Sdk.IPlugin'</c>.
    /// </para>
    /// <para>
    /// These live in the workflow assembly's own test project because this is the only assembly
    /// where <c>RemoteDebugExecutionContext</c> has both of its partials — the shared one from
    /// <c>XrmFramework.Plugin</c> and the workflow one that recognises an <c>IWorkflowContext</c>.
    /// A plugin assembly compiles the first alone, so the branch cannot be reached from there.
    /// </para>
    /// </remarks>
    [TestFixture]
    public class RemoteDebugExecutionContextTests
    {
        // ─────────────────────────────────────────────────────────────────────
        //  A workflow context
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void AWorkflowContext_IsFlaggedAsOne()
        {
            var context = new RemoteDebugExecutionContext(new FakeWorkflowContext());

            Assert.IsTrue(context.IsWorkflowContext,
                "Without this the client casts the CodeActivity to IPlugin.");
        }

        [Test]
        public void AWorkflowContext_CarriesItsWorkflowMembers()
        {
            var source = new FakeWorkflowContext
            {
                StageName = "PostOperation",
                WorkflowCategory = 3,
                WorkflowMode = 1
            };

            var context = new RemoteDebugExecutionContext(source);

            Assert.AreEqual("PostOperation", context.StageName);
            Assert.AreEqual(3, context.WorkflowCategory);
            Assert.AreEqual(1, context.WorkflowMode);
        }

        /// <summary>
        /// The members every context carries are read whichever branch is taken — the base
        /// assignments run before the branch, and must keep doing so.
        /// </summary>
        [Test]
        public void AWorkflowContext_CarriesTheSharedExecutionMembers()
        {
            var userId = Guid.NewGuid();
            var source = new FakeWorkflowContext
            {
                MessageName = "dpp_GetDashboardData",
                PrimaryEntityName = "dpp_cra",
                UserId = userId,
                Depth = 2
            };

            var context = new RemoteDebugExecutionContext(source);

            Assert.AreEqual("dpp_GetDashboardData", context.MessageName);
            Assert.AreEqual("dpp_cra", context.PrimaryEntityName);
            Assert.AreEqual(userId, context.UserId);
            Assert.AreEqual(2, context.Depth);
        }

        /// <summary>
        /// A workflow activity called from another one: the parent is captured through the same
        /// constructor, so it has to be recognised as a workflow context too.
        /// </summary>
        [Test]
        public void AWorkflowParentContext_IsCapturedAsAWorkflowContext()
        {
            var source = new FakeWorkflowContext
            {
                ParentContext = new FakeWorkflowContext { MessageName = "dpp_Parent" }
            };

            var context = new RemoteDebugExecutionContext(source);

            Assert.IsNotNull(context.RemoteParentContext);
            Assert.AreEqual("dpp_Parent", context.RemoteParentContext!.MessageName);
            Assert.IsTrue(context.RemoteParentContext.IsWorkflowContext);
        }

        /// <summary>
        /// The flag is what crosses the hybrid connection, so it has to survive the round trip the
        /// context actually makes — it is read back off the deserialized context, not the one the
        /// activity built.
        /// </summary>
        [Test]
        public void TheWorkflowFlag_SurvivesTheMessageRoundTrip()
        {
            var context = new RemoteDebugExecutionContext(new FakeWorkflowContext())
            {
                Id = Guid.NewGuid()
            };

            // The message is what actually crosses the hybrid connection, with the contract
            // resolver the debugger really serializes through.
            var message = new RemoteDebuggerMessage(
                RemoteDebuggerMessageType.Context, context, context.Id);

            var received = message.GetContext<RemoteDebugExecutionContext>();

            Assert.IsTrue(received.IsWorkflowContext,
                "The client reads the flag off the deserialized context, not the one the activity built.");
        }

        // ─────────────────────────────────────────────────────────────────────
        //  A plugin context
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// The other direction, which the fix must not cost: a plugin context is not a workflow
        /// one, and still takes the branch reading its stage.
        /// </summary>
        [Test]
        public void APluginContext_IsNotFlaggedAsAWorkflow()
        {
            var context = new RemoteDebugExecutionContext(new FakePluginContext { Stage = 20 });

            Assert.IsFalse(context.IsWorkflowContext);
            Assert.AreEqual(20, context.Stage);
        }

        [Test]
        public void APluginParentContext_IsStillCaptured()
        {
            var source = new FakePluginContext
            {
                Stage = 40,
                ParentContext = new FakePluginContext { Stage = 20, MessageName = "Update" }
            };

            var context = new RemoteDebugExecutionContext(source);

            Assert.IsNotNull(context.RemoteParentContext);
            Assert.AreEqual("Update", context.RemoteParentContext!.MessageName);
            Assert.AreEqual(20, context.RemoteParentContext.Stage);
            Assert.IsFalse(context.RemoteParentContext.IsWorkflowContext);
        }
    }
}
