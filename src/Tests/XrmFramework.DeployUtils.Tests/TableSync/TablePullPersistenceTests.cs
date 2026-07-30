// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using XrmFramework.Core;
using XrmFramework.DeployUtils.TableSync;
using static XrmFramework.DeployUtils.Tests.TableSync.MetadataFixtureBuilder;
using DataverseMetadata = Microsoft.Xrm.Sdk.Metadata;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// Survie de la sélection à une récupération, vérifiée <b>de bout en bout</b> : métadonnées
/// Dataverse → conversion → fusion → écriture sur disque → relecture.
/// </summary>
/// <remarks>
/// Les tests de <see cref="TablePullMergeTests" /> couvrent la fusion en mémoire ; ceux-ci
/// couvrent ce qui atteint réellement le fichier versionné. La distinction n'est pas théorique :
/// la sérialisation omet les valeurs par défaut, donc <c>Select</c> disparaît du JSON dès qu'il
/// vaut <c>false</c>. Ils passent par <see cref="TablePullWriter" />, c'est-à-dire exactement le
/// chemin qu'emprunte la commande.
/// </remarks>
[TestFixture]
public class TablePullPersistenceTests
{
    private static readonly string[] Prefixes = { "ftp" };

    private string _tablesDir = string.Empty;

    [SetUp]
    public void SetUp()
    {
        _tablesDir = Path.Combine(Path.GetTempPath(),
            "XrmFramework.PullPersistence_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tablesDir);
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            if (Directory.Exists(_tablesDir))
                Directory.Delete(_tablesDir, recursive: true);
        }
        catch { /* meilleur effort */ }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Survie de la sélection
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Pull_KeepsColumnSelected_WhenTeamActivatedIt()
    {
        // Première récupération : ftp_commentaire n'est pas activée par défaut.
        Pull(Contrat());
        Assert.IsFalse(LoadTable().Columns.Single(c => c.LogicalName == "ftp_commentaire").Selected);

        // L'équipe l'active — soit à la main, soit via « tables sync » parce que le code la référence.
        Activate("ftp_commentaire");

        // Seconde récupération : la sélection doit survivre.
        Pull(Contrat());

        Assert.IsTrue(LoadTable().Columns.Single(c => c.LogicalName == "ftp_commentaire").Selected,
            "Une colonne activée doit survivre à une récupération.");
    }

    [Test]
    public void Pull_WritesSelectTrue_IntoJson()
    {
        // Contrôle sur le JSON réel et non sur le modèle objet : la sérialisation omet les valeurs
        // par défaut, un Select mal positionné disparaîtrait donc silencieusement du fichier.
        Pull(Contrat());
        Activate("ftp_commentaire");
        Pull(Contrat());

        var colonne = JObject.Parse(File.ReadAllText(TableFilePath()))["Cols"]!
            .Single(c => (string?)c["LogName"] == "ftp_commentaire");

        Assert.AreEqual(true, (bool?)colonne["Select"],
            "Le fichier écrit doit porter \"Select\": true.");
    }

    [Test]
    public void Pull_KeepsSelection_OfColumnRemovedFromEnvironment()
    {
        Pull(Contrat());
        Activate("ftp_commentaire");

        // La colonne disparaît de l'environnement : une récupération rafraîchit, elle ne détruit pas.
        Pull(ContratSansCommentaire());

        var orpheline = LoadTable().Columns.SingleOrDefault(c => c.LogicalName == "ftp_commentaire");

        Assert.IsNotNull(orpheline, "La colonne absente du CRM doit être conservée.");
        Assert.IsTrue(orpheline!.Selected, "Et conserver sa sélection.");
    }

    [Test]
    public void Pull_DoesNotReactivate_DeliberatelyDeselectedSystemColumn()
    {
        // createdon est activée d'office à la création ; si l'équipe la désactive,
        // une récupération ne doit pas revenir sur cette décision.
        Pull(Contrat());
        Deactivate("createdon");

        Pull(Contrat());

        Assert.IsFalse(LoadTable().Columns.Single(c => c.LogicalName == "createdon").Selected);
    }

    [Test]
    public void Pull_KeepsSelection_WhenFileWasRenamedByHand()
    {
        Pull(Contrat());
        Activate("ftp_commentaire");

        // L'équipe renomme le type C# et le fichier ; le rapprochement se fait sur le LogName.
        RenameTableFile("ContratDeLocation");

        Pull(Contrat());

        Assert.AreEqual(1, Directory.GetFiles(_tablesDir, "*.table").Length,
            "Le fichier renommé doit être mis à jour, pas dupliqué.");
        Assert.IsTrue(LoadTable().Columns.Single(c => c.LogicalName == "ftp_commentaire").Selected);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Idempotence
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Pull_IsIdempotent_WhenNothingChanged()
    {
        Pull(Contrat());
        Activate("ftp_commentaire");
        Pull(Contrat());

        var apresPremier = File.ReadAllText(TableFilePath());

        Pull(Contrat());

        Assert.AreEqual(apresPremier, File.ReadAllText(TableFilePath()),
            "Une seconde récupération identique doit produire une diff vide.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Helpers
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>Exécute le chemin réel de la commande : conversion, fusion, écriture.</summary>
    private void Pull(DataverseMetadata.EntityMetadata entity)
        => TablePullWriter.Write(_tablesDir, MetadataTableFactory.Convert(entity, Prefixes).Table);

    private string TableFilePath()
        => Directory.GetFiles(_tablesDir, "*.table").Single();

    private Table LoadTable() => TableFileStore.Load(TableFilePath());

    private void Activate(string logicalName) => SetSelected(logicalName, true);

    private void Deactivate(string logicalName) => SetSelected(logicalName, false);

    private void SetSelected(string logicalName, bool selected)
    {
        var path = TableFilePath();
        var table = TableFileStore.Load(path);

        table.Columns.Single(c => c.LogicalName == logicalName).Selected = selected;

        TableFileStore.Save(path, table);
    }

    private void RenameTableFile(string newName)
    {
        var path = TableFilePath();
        var table = TableFileStore.Load(path);

        table.Name = newName;

        File.Delete(path);
        TableFileStore.Save(TableFileStore.BuildTableFilePath(_tablesDir, newName), table);
    }

    // ── Métadonnées simulées ──────────────────────────────────────────────────

    private static DataverseMetadata.EntityMetadata Contrat()
        => Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid", "ftp_nom",
            Attribute("ftp_contratid", "ftp_ContratId", DataverseMetadata.AttributeTypeCode.Uniqueidentifier),
            StringAttribute("ftp_nom", "ftp_Nom", 100),
            Attribute("createdon", "CreatedOn", DataverseMetadata.AttributeTypeCode.DateTime),
            StringAttribute("ftp_commentaire", "ftp_Commentaire", 2000));

    private static DataverseMetadata.EntityMetadata ContratSansCommentaire()
        => Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid", "ftp_nom",
            Attribute("ftp_contratid", "ftp_ContratId", DataverseMetadata.AttributeTypeCode.Uniqueidentifier),
            StringAttribute("ftp_nom", "ftp_Nom", 100),
            Attribute("createdon", "CreatedOn", DataverseMetadata.AttributeTypeCode.DateTime));
}
