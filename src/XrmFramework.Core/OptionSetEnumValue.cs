using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace XrmFramework.Core
{
    [JsonObject(MemberSerialization.OptOut)]
    public class OptionSetEnumValue
    {
        public int Value { get; set; }

        public string Name { get; set; }

        public ICollection<LocalizedLabel> Labels { get; } = new List<LocalizedLabel>();

        [JsonProperty("ExtVal")]
        public string ExternalValue { get; set; }

        /// <summary>
        /// The CRM label standing for this option, which the generated member carries in its
        /// <c>[Description]</c> attribute.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <see cref="Name" /> is no answer to this: it is the C# identifier, derived from the label
        /// by stripping whatever an identifier cannot hold — a space, a dash, a diacritic — and a
        /// project may since have renamed it. The label is the one thing the CRM displays, which is
        /// what <c>GetDescription()</c> is asked for and what <c>GetValueFromDescription()</c>
        /// matches a CRM value against.
        /// </para>
        /// <para>
        /// The CRM answers with one label per provisioned language, and nothing in the
        /// <c>.table</c> says which one the project works in. The one <see cref="Name" /> was derived
        /// from does: that is the label of the language the pull ran in, so it is preferred over the
        /// order the labels happen to be written in. A file written before the labels were kept
        /// carries none, and then <see cref="Name" /> is all there is to go on.
        /// </para>
        /// </remarks>
        [JsonIgnore]
        public string EffectiveLabel
        {
            get
            {
                if (Labels == null || Labels.Count == 0)
                {
                    return Name;
                }

                var derivedFrom = string.IsNullOrEmpty(Name)
                    ? null
                    : Labels.FirstOrDefault(l => Fingerprint(l.Label) == Fingerprint(Name));

                return (derivedFrom ?? Labels.First()).Label;
            }
        }

        /// <summary>
        /// What a label and the name derived from it have in common: the letters and digits they
        /// hold, diacritics folded and case dropped.
        /// </summary>
        /// <remarks>
        /// Deliberately looser than the name formatting of the pull tool, which this comparison must
        /// not become a second implementation of — it only has to tell one language's label from
        /// another's. The two pronounced symbols the formatter spells out are spelled out here too,
        /// without which a "%Remise" label would match no name at all.
        /// </remarks>
        private static string Fingerprint(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var spelled = text.Replace("%", "Pourcent").Replace("+", "Plus");
            var decomposed = spelled.Normalize(NormalizationForm.FormD);
            var fingerprint = new StringBuilder(decomposed.Length);

            foreach (var c in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                if (char.IsLetterOrDigit(c))
                {
                    fingerprint.Append(char.ToUpperInvariant(c));
                }
            }

            return fingerprint.ToString();
        }
    }
}
