// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace XrmFramework.DeployUtils.TableSync;

/// <summary>
/// Transformation des noms Dataverse (SchemaName, libellés d'options) en identifiants C#.
///
/// Port de <c>XrmFramework.DefinitionManager.TextHelper</c> et de
/// <c>DataAccessManager.RemovePrefix</c>, rendu utilisable hors WinForms afin que le CLI
/// produise exactement les mêmes noms que le DefinitionManager historique.
/// </summary>
public static class NameFormatter
{
    /// <summary>
    /// Caractères remplacés par une espace avant la mise en PascalCase.
    /// Les non-ASCII sont écrits en échappement Unicode : l'original comportait des caractères
    /// invisibles (apostrophes typographiques, espace insécable) qu'un éditeur normalise
    /// silencieusement, ce qui changerait les noms générés sans que la diff ne le montre.
    /// </summary>
    private static readonly string[] SeparatorCharacters =
    {
        "'",
        "\u2018", // apostrophe typographique ouvrante
        "\u2019", // apostrophe typographique fermante
        "_", ",", "-", "(", ")", ":", "/", "\\", "&",
        "\u00a0"  // espace insécable : ressemble à une espace mais n'en est pas une
    };

    /// <summary>
    /// Convertit un libellé Dataverse en identifiant C# PascalCase.
    /// </summary>
    /// <remarks>
    /// La mise en casse est épinglée sur <see cref="CultureInfo.InvariantCulture" /> alors que
    /// l'implémentation d'origine utilisait la culture courante : sans cela, le CLI produirait
    /// des noms différents selon la culture du poste ou de l'agent d'intégration continue.
    ///
    /// Conformément au comportement de <see cref="TextInfo.ToTitleCase" />, un mot entièrement
    /// en majuscules est laissé tel quel (« ID » reste « ID », « id » devient « Id »).
    /// </remarks>
    public static string FormatText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        foreach (var separator in SeparatorCharacters)
            text = text.Replace(separator, " ");

        // Ces deux symboles se prononcent : les supprimer produirait des noms ambigus
        // (« %Remise » et « Remise » donneraient le même identifiant).
        text = text.Replace("%", " Pourcent ").Replace("+", " Plus ");

        text = RemoveDiacritics(text);
        text = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text);

        return text.Replace(" ", string.Empty);
    }

    /// <summary>
    /// Retire le préfixe d'éditeur d'un nom de schéma (<c>ftp_contrat</c> → <c>Contrat</c>)
    /// et force la première lettre en majuscule.
    /// </summary>
    /// <param name="name">Nom de schéma Dataverse.</param>
    /// <param name="publisherPrefixes">
    /// Préfixes de personnalisation des éditeurs de l'environnement, sans le séparateur.
    /// </param>
    /// <remarks>
    /// La comparaison est ordinale et insensible à la casse, là où l'implémentation d'origine
    /// s'appuyait sur un <c>StartsWith</c> culturel et sensible à la casse — donc dépendant de
    /// la culture du poste et incapable de reconnaître un <c>Ftp_Contrat</c>.
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
    /// Décompose puis retire les signes diacritiques (« Réf. Société » → « Ref. Societe »).
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
