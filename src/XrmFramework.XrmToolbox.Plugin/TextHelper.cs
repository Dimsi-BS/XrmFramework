using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmFramework.XrmToolbox
{
    internal static class TextHelper
    {
        internal static string FormatText(this string text)
        {
            text = text.Replace("'", " ").Replace("‘", " ").Replace("’", " ").Replace("_", " ").Replace(",", " ").Replace("-", " ").Replace("(", " ").Replace(")", " ").Replace(":", " ").Replace("/", " ").Replace("\\", " ").Replace("%", " Pourcent ").Replace("+", " Plus ").Replace("&", " ")
                // Characters looking like spaces but not spaces
                .Replace(" ", " ").Replace(" ", " ");
            text = RemoveDiacritics(text);

            var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;

            text = textInfo.ToTitleCase(text);

            text = text.Replace(" ", "");

            return text;
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        internal static string StrongFormat(this string text)
        {
            text = text.Replace("'", " ").Replace("‘", " ").Replace("’", " ").Replace("_", " ").Replace(",", " ").Replace("-", " ").Replace("(", " ").Replace(")", " ").Replace(":", " ").Replace("/", " ").Replace("\\", " ").Replace("%", " Pourcent ").Replace("+", " Plus ").Replace("&", " ")
                // Characters looking like spaces but not spaces
                .Replace(" ", " ").Replace(" ", " ").Replace("."," ");
            var splitText = text.Split(' ');
            for(int i = 0; i < splitText.Length; i++)
            {
                var word = RemoveDiacritics(splitText[i]);
                var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
                var textInfo = cultureInfo.TextInfo;
                word = textInfo.ToTitleCase(word);
                splitText[i] = word;

            }
            text = string.Join("",splitText);
            return text;
        }
    }
}
