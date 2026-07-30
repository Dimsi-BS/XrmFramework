// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// Parité de <see cref="NameFormatter" /> avec le DefinitionManager historique : les noms produits
/// alimentent du code généré, donc toute dérive casserait la compilation des projets consommateurs.
/// </summary>
[TestFixture]
public class NameFormatterTests
{
    // ══════════════════════════════════════════════════════════════════════════
    // FormatText — séparateurs et casse
    // ══════════════════════════════════════════════════════════════════════════

    [TestCase("Nom du contrat", "NomDuContrat")]
    [TestCase("ftp_contrat", "FtpContrat")]
    [TestCase("Date de début", "DateDeDebut")]
    [TestCase("Montant (HT)", "MontantHT")]
    [TestCase("Client / Fournisseur", "ClientFournisseur")]
    [TestCase("Statut : actif", "StatutActif")]
    [TestCase("Nom, prénom", "NomPrenom")]
    [TestCase("Multi-devises", "MultiDevises")]
    [TestCase("Achats & ventes", "AchatsVentes")]
    public void FormatText_ReplacesSeparators_WithPascalCase(string input, string expected)
        => Assert.AreEqual(expected, NameFormatter.FormatText(input));

    [TestCase("Chiffre d'affaires", "ChiffreDAffaires")]
    [TestCase("Chiffre d’affaires", "ChiffreDAffaires")]
    [TestCase("Chiffre d‘affaires", "ChiffreDAffaires")]
    public void FormatText_TreatsTypographicApostrophes_LikePlainApostrophe(string input, string expected)
        => Assert.AreEqual(expected, NameFormatter.FormatText(input));

    [Test]
    public void FormatText_TreatsNonBreakingSpace_AsSeparator()
    {
        // Une espace insécable est visuellement indiscernable d'une espace ordinaire ; sans
        // traitement elle survivrait au découpage et produirait un identifiant C# invalide.
        Assert.AreEqual("NomComplet", NameFormatter.FormatText("Nom complet"));
    }

    [TestCase("% de remise", "PourcentDeRemise")]
    [TestCase("Prix + taxes", "PrixPlusTaxes")]
    public void FormatText_SpellsOutPronouncedSymbols(string input, string expected)
        => Assert.AreEqual(expected, NameFormatter.FormatText(input));

    [Test]
    public void FormatText_RemovesDiacritics()
        => Assert.AreEqual("ReferenceSociete", NameFormatter.FormatText("Référence Société"));

    [Test]
    public void FormatText_LeavesFullyUppercaseWords_Untouched()
    {
        // Comportement documenté de ToTitleCase, sur lequel s'appuient les noms déjà générés.
        Assert.AreEqual("ID", NameFormatter.FormatText("ID"));
        Assert.AreEqual("Id", NameFormatter.FormatText("id"));
    }

    [TestCase(null)]
    [TestCase("")]
    public void FormatText_ReturnsInput_WhenNullOrEmpty(string? input)
        => Assert.AreEqual(input, NameFormatter.FormatText(input!));

    // ══════════════════════════════════════════════════════════════════════════
    // FormatText — invariance culturelle
    // ══════════════════════════════════════════════════════════════════════════

    [TestCase("fr-FR")]
    [TestCase("en-US")]
    [TestCase("tr-TR")]
    public void FormatText_ProducesSameResult_WhateverTheCurrentCulture(string cultureName)
    {
        // Le turc est le cas critique : sa règle de casse sur le « i » produirait « İd » au lieu
        // de « Id » si la culture courante était utilisée. Un agent d'intégration continue ne doit
        // pas générer des noms différents de ceux du poste de développement.
        var previousCulture = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);

            Assert.AreEqual("Id", NameFormatter.FormatText("id"));
            Assert.AreEqual("IdentifiantInterne", NameFormatter.FormatText("identifiant interne"));
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = previousCulture;
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // RemovePrefix
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void RemovePrefix_StripsMatchingPublisherPrefix()
        => Assert.AreEqual("Contrat", NameFormatter.RemovePrefix("ftp_Contrat", new[] { "ftp" }));

    [Test]
    public void RemovePrefix_IsCaseInsensitive()
    {
        // Dataverse expose le SchemaName avec une casse arbitraire (« Ftp_Contrat ») alors que
        // le customizationprefix de l'éditeur est en minuscules.
        Assert.AreEqual("Contrat", NameFormatter.RemovePrefix("Ftp_Contrat", new[] { "ftp" }));
    }

    [Test]
    public void RemovePrefix_StopsAtFirstMatchingPrefix()
        => Assert.AreEqual("Contrat", NameFormatter.RemovePrefix("ftp_Contrat", new[] { "abc", "ftp", "ft" }));

    [Test]
    public void RemovePrefix_CapitalizesFirstLetter_WhenNoPrefixMatches()
        => Assert.AreEqual("Account", NameFormatter.RemovePrefix("account", new[] { "ftp" }));

    [Test]
    public void RemovePrefix_IgnoresEmptyPrefixes()
        => Assert.AreEqual("Account", NameFormatter.RemovePrefix("account", new[] { "", "  ", null! }));

    [Test]
    public void RemovePrefix_AcceptsNullPrefixCollection()
        => Assert.AreEqual("Account", NameFormatter.RemovePrefix("account", null!));

    [Test]
    public void RemovePrefix_DoesNotStrip_WhenNothingWouldRemain()
    {
        // « ftp_ » seul ne doit pas produire une chaîne vide, qui ferait échouer la mise en majuscule.
        Assert.AreEqual("Ftp_", NameFormatter.RemovePrefix("ftp_", new[] { "ftp" }));
    }

    [TestCase(null)]
    [TestCase("")]
    public void RemovePrefix_ReturnsInput_WhenNullOrEmpty(string? input)
        => Assert.AreEqual(input, NameFormatter.RemovePrefix(input!, new[] { "ftp" }));
}
