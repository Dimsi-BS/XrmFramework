using System.Linq;
using NUnit.Framework;

namespace XrmFramework.Core.Tests
{
    [TestFixture]
    public class KeySelectorTests
    {
        [Test]
        public void Declared_NullTable_ReturnsEmpty()
        {
            Assert.AreEqual(0, KeySelector.Declared(null).Count);
        }

        [Test]
        public void Declared_TableWithoutKeys_ReturnsEmpty()
        {
            Assert.AreEqual(0, KeySelector.Declared(new Table { LogicalName = "account" }).Count);
        }

        [Test]
        public void Declared_Key_IsDeclared()
        {
            var table = new Table { LogicalName = "account" };
            table.Keys.Add(new Key { LogicalName = "eco_reference", Name = "Reference" });

            var declared = KeySelector.Declared(table);

            Assert.AreEqual(1, declared.Count);
            Assert.AreEqual("Reference", declared[0].MemberName);
            Assert.AreEqual("eco_reference", declared[0].EffectiveLogicalName);
        }

        /// <summary>
        /// A key the CRM is known under no name at all stands for nothing to query on, so there is
        /// no constant to declare for it.
        /// </summary>
        [Test]
        public void Declared_KeyWithoutAnyName_IsSkipped()
        {
            var table = new Table { LogicalName = "account" };
            table.Keys.Add(new Key());

            Assert.AreEqual(0, KeySelector.Declared(table).Count);
        }

        /// <summary>
        /// A file written before <see cref="Key.LogicalName" /> existed carries the logical name in
        /// <see cref="Key.Name" />, and still declares its key.
        /// </summary>
        [Test]
        public void Declared_LegacyKeyCarryingItsLogicalNameInName_IsDeclared()
        {
            var table = new Table { LogicalName = "account" };
            table.Keys.Add(new Key { Name = "eco_reference" });

            var declared = KeySelector.Declared(table);

            Assert.AreEqual(1, declared.Count);
            Assert.AreEqual("eco_reference", declared[0].EffectiveLogicalName);
        }

        /// <summary>
        /// One constant per name: two keys claiming it would declare it twice.
        /// </summary>
        [Test]
        public void Declared_TwoKeysClaimingOneMemberName_DeclaresItOnce()
        {
            var table = new Table { LogicalName = "account" };
            table.Keys.Add(new Key { LogicalName = "eco_reference", Name = "Reference" });
            table.Keys.Add(new Key { LogicalName = "eco_reference_2", Name = "Reference" });

            var declared = KeySelector.Declared(table);

            Assert.AreEqual(1, declared.Count);
            Assert.AreEqual("eco_reference", declared[0].EffectiveLogicalName,
                            "the first claimant keeps the name");
        }

        /// <summary>
        /// Two names differing by case alone are two C# identifiers, so neither shadows the other.
        /// </summary>
        [Test]
        public void Declared_NamesDifferingByCaseOnly_AreBothDeclared()
        {
            var table = new Table { LogicalName = "account" };
            table.Keys.Add(new Key { LogicalName = "eco_reference", Name = "Reference" });
            table.Keys.Add(new Key { LogicalName = "eco_reference_2", Name = "reference" });

            var declared = KeySelector.Declared(table);

            Assert.AreEqual(2, declared.Count);
            Assert.That(declared.Select(k => k.MemberName), Is.EquivalentTo(new[] { "Reference", "reference" }));
        }
    }
}
