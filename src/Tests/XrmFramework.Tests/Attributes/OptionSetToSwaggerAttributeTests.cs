// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;

namespace XrmFramework.Tests.Attributes
{
    [TestFixture]
    public class OptionSetToSwaggerAttributeTests
    {
        private enum SimpleState { Active = 1, Inactive = 2, Pending = 3 }

        private enum StateWithNull { Null = 0, Active = 1, Inactive = 2 }

        private enum SingleValue { Only = 5 }

        // ─────────────────────────────────────────────────────────────
        //  addEmptyValue = true (default)
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void AddEmptyValue_True_PatternStartsWithSpace()
        {
            var attr = new OptionSetToSwaggerAttribute(typeof(SimpleState));

            Assert.AreEqual("( |1|2|3)", attr.Pattern);
        }

        [Test]
        public void AddEmptyValue_True_NullValueZero_IsSkipped()
        {
            var attr = new OptionSetToSwaggerAttribute(typeof(StateWithNull));

            Assert.AreEqual("( |1|2)", attr.Pattern);
        }

        // ─────────────────────────────────────────────────────────────
        //  addEmptyValue = false
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void AddEmptyValue_False_PatternHasNoLeadingSpace()
        {
            var attr = new OptionSetToSwaggerAttribute(typeof(SimpleState), addEmptyValue: false);

            Assert.AreEqual("(1|2|3)", attr.Pattern);
        }

        [Test]
        public void AddEmptyValue_False_NullValueZero_IsSkipped()
        {
            var attr = new OptionSetToSwaggerAttribute(typeof(StateWithNull), addEmptyValue: false);

            Assert.AreEqual("(1|2)", attr.Pattern);
        }

        // ─────────────────────────────────────────────────────────────
        //  Single-value enum
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void SingleValue_AddEmptyTrue_CorrectPattern()
        {
            var attr = new OptionSetToSwaggerAttribute(typeof(SingleValue));

            Assert.AreEqual("( |5)", attr.Pattern);
        }

        [Test]
        public void SingleValue_AddEmptyFalse_CorrectPattern()
        {
            var attr = new OptionSetToSwaggerAttribute(typeof(SingleValue), addEmptyValue: false);

            Assert.AreEqual("(5)", attr.Pattern);
        }
    }
}
