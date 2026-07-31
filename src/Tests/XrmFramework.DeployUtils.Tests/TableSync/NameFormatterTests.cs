// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// Parity of <see cref="NameFormatter" /> with the legacy DefinitionManager: the names it
/// produces feed generated code, so any drift would break the build of consuming projects.
/// </summary>
[TestFixture]
public class NameFormatterTests
{
    // ══════════════════════════════════════════════════════════════════════════
    // FormatText — separators and casing
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
        // A non-breaking space is visually indistinguishable from an ordinary space; without
        // handling, it would survive the splitting and produce an invalid C# identifier.
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
        // Documented behavior of ToTitleCase, which already-generated names rely on.
        Assert.AreEqual("ID", NameFormatter.FormatText("ID"));
        Assert.AreEqual("Id", NameFormatter.FormatText("id"));
    }

    [TestCase(null)]
    [TestCase("")]
    public void FormatText_ReturnsInput_WhenNullOrEmpty(string? input)
        => Assert.AreEqual(input, NameFormatter.FormatText(input!));

    // ══════════════════════════════════════════════════════════════════════════
    // FormatText — culture invariance
    // ══════════════════════════════════════════════════════════════════════════

    [TestCase("fr-FR")]
    [TestCase("en-US")]
    [TestCase("tr-TR")]
    public void FormatText_ProducesSameResult_WhateverTheCurrentCulture(string cultureName)
    {
        // Turkish is the critical case: its casing rule for "i" would produce "İd" instead
        // of "Id" if the current culture were used. A continuous integration agent must not
        // generate names different from those on the development machine.
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
        // Dataverse exposes the SchemaName with arbitrary casing ("Ftp_Contrat") while
        // the publisher's customizationprefix is lowercase.
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
        // "ftp_" alone must not produce an empty string, which would make capitalization fail.
        Assert.AreEqual("Ftp_", NameFormatter.RemovePrefix("ftp_", new[] { "ftp" }));
    }

    [TestCase(null)]
    [TestCase("")]
    public void RemovePrefix_ReturnsInput_WhenNullOrEmpty(string? input)
        => Assert.AreEqual(input, NameFormatter.RemovePrefix(input!, new[] { "ftp" }));
}
