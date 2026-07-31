// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace XrmFramework.DeployUtils.TableSync;

/// <summary>
/// Transformation of Dataverse names (SchemaName, option labels) into C# identifiers.
///
/// Port of <c>XrmFramework.DefinitionManager.TextHelper</c> and
/// <c>DataAccessManager.RemovePrefix</c>, made usable outside WinForms so that the CLI
/// produces exactly the same names as the historical DefinitionManager.
/// </summary>
public static class NameFormatter
{
    /// <summary>
    /// Characters replaced by a space before conversion to PascalCase.
    /// Non-ASCII characters are written as Unicode escapes: the original contained
    /// invisible characters (typographic apostrophes, non-breaking space) that an editor silently
    /// normalizes, which would change the generated names without the diff showing it.
    /// </summary>
    private static readonly string[] SeparatorCharacters =
    {
        "'",
        "\u2018", // opening typographic apostrophe
        "\u2019", // closing typographic apostrophe
        "_", ",", "-", "(", ")", ":", "/", "\\", "&",
        "\u00a0"  // non-breaking space: looks like a space but isn't one
    };

    /// <summary>
    /// Converts a Dataverse label into a C# PascalCase identifier.
    /// </summary>
    /// <remarks>
    /// Casing is pinned to <see cref="CultureInfo.InvariantCulture" /> whereas
    /// the original implementation used the current culture: without this, the CLI would produce
    /// different names depending on the machine's culture or the continuous integration agent's.
    ///
    /// In accordance with the behavior of <see cref="TextInfo.ToTitleCase" />, a word entirely
    /// in uppercase is left as-is ("ID" stays "ID", "id" becomes "Id").
    /// </remarks>
    public static string FormatText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        foreach (var separator in SeparatorCharacters)
            text = text.Replace(separator, " ");

        // These two symbols are pronounced: removing them would produce ambiguous names
        // ("%Remise" and "Remise" would yield the same identifier).
        text = text.Replace("%", " Pourcent ").Replace("+", " Plus ");

        text = RemoveDiacritics(text);
        text = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text);

        return text.Replace(" ", string.Empty);
    }

    /// <summary>
    /// Removes the publisher prefix from a schema name (<c>ftp_contrat</c> → <c>Contrat</c>)
    /// and forces the first letter to uppercase.
    /// </summary>
    /// <param name="name">Dataverse schema name.</param>
    /// <param name="publisherPrefixes">
    /// Customization prefixes of the environment's publishers, without the separator.
    /// </param>
    /// <remarks>
    /// The comparison is ordinal and case-insensitive, whereas the original implementation
    /// relied on a culture-sensitive, case-sensitive <c>StartsWith</c> — thus dependent on
    /// the machine's culture and unable to recognize a <c>Ftp_Contrat</c>.
    /// </remarks>
    public static string RemovePrefix(string name, IEnumerable<string> publisherPrefixes)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        if (publisherPrefixes != null)
        {
            foreach (var publisherPrefix in publisherPrefixes)
            {
                if (string.IsNullOrWhiteSpace(publisherPrefix))
                    continue;

                var prefix = publisherPrefix + "_";

                if (name.Length > prefix.Length &&
                    name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(prefix.Length);
                    break;
                }
            }
        }

        return name.Substring(0, 1).ToUpperInvariant() + name.Substring(1);
    }

    /// <summary>
    /// Decomposes then removes diacritical marks ("Réf. Société" → "Ref. Societe").
    /// </summary>
    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(normalizedString.Length);

        foreach (var c in normalizedString)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                stringBuilder.Append(c);
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
