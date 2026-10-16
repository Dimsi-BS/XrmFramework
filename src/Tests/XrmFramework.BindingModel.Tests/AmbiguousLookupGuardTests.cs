// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using NUnit.Framework;
using XrmFramework.BindingModel.Tests.Fakes;

namespace XrmFramework.BindingModel.Tests
{
    /// <summary>
    /// A writable <see cref="Guid"/> property mapped to a lookup that reaches more than one table
    /// (Customer, Owner, or an occasionally polymorphic plain Lookup) cannot be written back
    /// correctly: the Guid alone carries no indication of which table it targets, so
    /// <c>BindingModelToEntityMapper</c> would otherwise have to silently guess. These tests check
    /// that the problem is reported clearly instead, as soon as the binding model type is used for
    /// anything — not only when a write happens to run into it.
    /// </summary>
    [TestFixture]
    public class AmbiguousLookupGuardTests
    {
        [Test]
        public void WritableGuidOnPolymorphicLookup_FailsFastWithAClearMessage()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
                DefinitionCache.GetModelDefinition(typeof(ContactWithWritableGuidOnPolymorphicLookupModel)));

            Assert.That(ex!.Message, Does.Contain("BackupCustomerId"),
                "The message must name the offending property.");
            Assert.That(ex.Message, Does.Contain("backupcustomerid"),
                "The message must name the CRM attribute behind it.");
        }

        [Test]
        public void ReadOnlyGuidOnPolymorphicLookup_IsNoProblem()
        {
            Assert.DoesNotThrow(() =>
                DefinitionCache.GetModelDefinition(typeof(ContactWithReadOnlyGuidOnPolymorphicLookupModel)),
                "IsValidForUpdate = false means nothing is ever written back through this property.");
        }

        [Test]
        public void EntityReferenceOnPolymorphicLookup_IsNoProblem()
        {
            Assert.DoesNotThrow(() =>
                DefinitionCache.GetModelDefinition(typeof(ContactWithEntityReferenceOnPolymorphicLookupModel)),
                "An EntityReference already carries its own target table.");
        }

        [Test]
        public void WritableGuidOnSingleTargetLookup_IsNoProblem()
        {
            // The overwhelmingly common case — a Guid mapped to an ordinary, single-target Lookup
            // column — must keep working exactly as before this guard was added.
            Assert.DoesNotThrow(() =>
                DefinitionCache.GetModelDefinition(typeof(ContactWithWritableGuidOnSingleTargetLookupModel)));
        }

        /// <summary>
        /// Every entry point that resolves a binding model's metadata — not just a direct
        /// <see cref="DefinitionCache.GetModelDefinition"/> call — must surface this same message,
        /// rather than let <see cref="DefinitionCache.TryGetModelDefinition"/>'s cached failure
        /// collapse it into a generic "not found".
        /// </summary>
        [Test]
        public void TheSameProblem_SurfacesThroughGetRetrieveAllQueryToo()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
                BindingModelHelper.GetRetrieveAllQuery<ContactWithWritableGuidOnPolymorphicLookupModel>());

            Assert.That(ex!.Message, Does.Contain("BackupCustomerId"));
        }
    }
}
