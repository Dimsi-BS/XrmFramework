using NUnit.Framework;

namespace XrmFramework.Core.Tests
{
    [TestFixture]
    public class OptionSetEnumValueTests
    {
        [Test]
        public void ObjectInitialization()
        {
            var obj = new OptionSetEnumValue();

            Assert.IsNull(obj.ExternalValue);
            obj.ExternalValue = "ExternalValue";
            Assert.AreEqual("ExternalValue", obj.ExternalValue);

            Assert.IsNull(obj.Name);
            obj.Name = "Name";
            Assert.AreEqual("Name", obj.Name);

            Assert.AreEqual(0, obj.Value);
            obj.Value = 543;
            Assert.AreEqual(543, obj.Value);

            Assert.IsNotNull(obj.Labels);
            Assert.AreEqual(0, obj.Labels.Count);
        }

        /// <summary>
        /// A file written before the labels were kept carries none: the name is all that is left to
        /// describe the option with.
        /// </summary>
        [Test]
        public void WithoutLabel_TheNameStandsForTheLabel()
        {
            var value = new OptionSetEnumValue { Name = "PourInvest.Jeanbrun", Value = 1 };

            Assert.AreEqual("PourInvest.Jeanbrun", value.EffectiveLabel);
        }

        /// <summary>
        /// The label is what the CRM displays, and what the name was stripped down from: it is the
        /// label, not the identifier, that describes the option.
        /// </summary>
        [Test]
        public void TheLabelDescribesTheOption_NotTheName()
        {
            var value = new OptionSetEnumValue { Name = "LectureEcriture", Value = 3 };
            value.Labels.Add(new LocalizedLabel { Label = "Lecture-écriture", LangId = 1036 });

            Assert.AreEqual("Lecture-écriture", value.EffectiveLabel);
        }

        /// <summary>
        /// One label per provisioned language reaches the file, in whichever order the CRM returned
        /// them. The one the name was derived from is the language the project works in.
        /// </summary>
        [Test]
        public void SeveralLanguages_TheNameSaysWhichLabelDescribesTheOption()
        {
            var value = new OptionSetEnumValue { Name = "ValeurParDefaut", Value = 1 };
            value.Labels.Add(new LocalizedLabel { Label = "Default Value", LangId = 1033 });
            value.Labels.Add(new LocalizedLabel { Label = "Valeur par défaut", LangId = 1036 });

            Assert.AreEqual("Valeur par défaut", value.EffectiveLabel);
        }

        /// <summary>
        /// A name no label can be matched to — the project renamed it — is still no description: the
        /// first label the file carries is, for lack of anything saying which language to prefer.
        /// </summary>
        [Test]
        public void ARenamedMember_IsStillDescribedByALabel()
        {
            var value = new OptionSetEnumValue { Name = "Modifiable", Value = 3 };
            value.Labels.Add(new LocalizedLabel { Label = "Lecture-écriture", LangId = 1036 });
            value.Labels.Add(new LocalizedLabel { Label = "Read-write", LangId = 1033 });

            Assert.AreEqual("Lecture-écriture", value.EffectiveLabel);
        }

        /// <summary>
        /// The formatter spells out the two symbols it keeps hearing in a name, so the label holding
        /// them must still be recognized as the one that name came from.
        /// </summary>
        [Test]
        public void ALabelHoldingAPronouncedSymbol_IsMatchedToItsName()
        {
            var value = new OptionSetEnumValue { Name = "PourcentRemise", Value = 7 };
            value.Labels.Add(new LocalizedLabel { Label = "Discount rate", LangId = 1033 });
            value.Labels.Add(new LocalizedLabel { Label = "%Remise", LangId = 1036 });

            Assert.AreEqual("%Remise", value.EffectiveLabel);
        }
    }
}