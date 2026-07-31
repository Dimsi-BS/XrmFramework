// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using DataverseMetadata = Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;
using XrmFramework.Core;
using XrmFramework.DeployUtils.TableSync;
using static XrmFramework.DeployUtils.Tests.TableSync.MetadataFixtureBuilder;
using CoreAttributeTypeCode = XrmFramework.AttributeTypeCode;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// Conversion of Dataverse metadata into a <see cref="Table" />. These rules must remain
/// identical to those of the legacy DefinitionManager: generated code depends on them.
/// </summary>
[TestFixture]
public class MetadataTableFactoryTests
{
    private static readonly string[] Prefixes = { "ftp" };

    // ══════════════════════════════════════════════════════════════════════════
    // Table identity
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Convert_StripsPublisherPrefix_FromTableName()
    {
        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid");

        var result = MetadataTableFactory.Convert(entity, Prefixes);

        Assert.AreEqual("ftp_contrat", result.Table.LogicalName);
        Assert.AreEqual("Contrat", result.Table.Name);
        Assert.AreEqual("ftp_contrats", result.Table.CollectionName);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Columns
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Convert_NamesPrimaryIdColumn_Id()
    {
        // Convention that all generated code relies on, regardless of the schema name.
        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid",
            attributes: Attribute("ftp_contratid", "ftp_ContratId",
                DataverseMetadata.AttributeTypeCode.Uniqueidentifier));

        var column = MetadataTableFactory.Convert(entity, Prefixes).Table.Columns.Single();

        Assert.AreEqual("Id", column.Name);
        Assert.AreEqual(PrimaryType.Id, column.PrimaryType);
    }

    [Test]
    public void Convert_StripsPublisherPrefix_FromColumnName()
    {
        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid",
            attributes: StringAttribute("ftp_numerocontrat", "ftp_NumeroContrat", 100));

        var column = MetadataTableFactory.Convert(entity, Prefixes).Table.Columns
            .Single(c => c.LogicalName == "ftp_numerocontrat");

        Assert.AreEqual("NumeroContrat", column.Name);
        Assert.AreEqual(100, column.StringLength);
        Assert.AreEqual(CoreAttributeTypeCode.String, column.Type);
    }

    [Test]
    public void Convert_MapsLocalizedLabels()
    {
        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid",
            attributes: Attribute("ftp_libelle", "ftp_Libelle",
                DataverseMetadata.AttributeTypeCode.String, "Libellé du contrat"));

        var column = MetadataTableFactory.Convert(entity, Prefixes).Table.Columns
            .Single(c => c.LogicalName == "ftp_libelle");

        Assert.AreEqual(1, column.Labels.Count);
        Assert.AreEqual("Libellé du contrat", column.Labels.Single().Label);
        Assert.AreEqual(1036, column.Labels.Single().LangId);
    }

    [Test]
    public void Convert_MapsCrudCapabilities()
    {
        var attribute = Attribute("ftp_lecture", "ftp_Lecture", DataverseMetadata.AttributeTypeCode.String)
            .Set(nameof(DataverseMetadata.AttributeMetadata.IsValidForCreate), (bool?)false)
            .Set(nameof(DataverseMetadata.AttributeMetadata.IsValidForUpdate), (bool?)false);

        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid", attributes: attribute);

        var column = MetadataTableFactory.Convert(entity, Prefixes).Table.Columns.Single();

        Assert.AreEqual(AttributeCapabilities.Read, column.Capabilities);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Attribute filtering
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Convert_SkipsAttributes_WithoutAnyCrudValidity()
    {
        var unusable = Attribute("ftp_interne", "ftp_Interne", DataverseMetadata.AttributeTypeCode.String)
            .Set(nameof(DataverseMetadata.AttributeMetadata.IsValidForCreate), (bool?)false)
            .Set(nameof(DataverseMetadata.AttributeMetadata.IsValidForRead), (bool?)false)
            .Set(nameof(DataverseMetadata.AttributeMetadata.IsValidForUpdate), (bool?)false);

        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid", attributes: unusable);

        Assert.AreEqual(0, MetadataTableFactory.Convert(entity, Prefixes).Table.Columns.Count);
    }

    [Test]
    public void Convert_SkipsEntityNameDiscriminators()
    {
        // Textual companion of a polymorphic lookup: unusable as a column.
        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid",
            attributes: Attribute("regardingobjecttypecode", "RegardingObjectTypeCode",
                DataverseMetadata.AttributeTypeCode.EntityName));

        Assert.AreEqual(0, MetadataTableFactory.Convert(entity, Prefixes).Table.Columns.Count);
    }

    [Test]
    public void Convert_SkipsDerivedAttributes()
    {
        var derived = Attribute("ftp_montant_base", "ftp_Montant_Base", DataverseMetadata.AttributeTypeCode.Money)
            .Set(nameof(DataverseMetadata.AttributeMetadata.AttributeOf), "ftp_montant");

        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid", attributes: derived);

        Assert.AreEqual(0, MetadataTableFactory.Convert(entity, Prefixes).Table.Columns.Count);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Selection policy
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Convert_SelectsPrimaryColumns_AndCommonSystemColumns()
    {
        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid", "ftp_nom",
            Attribute("ftp_contratid", "ftp_ContratId", DataverseMetadata.AttributeTypeCode.Uniqueidentifier),
            StringAttribute("ftp_nom", "ftp_Nom", 100),
            Attribute("createdon", "CreatedOn", DataverseMetadata.AttributeTypeCode.DateTime),
            Attribute("modifiedon", "ModifiedOn", DataverseMetadata.AttributeTypeCode.DateTime),
            StringAttribute("ftp_commentaire", "ftp_Commentaire", 2000));

        var columns = MetadataTableFactory.Convert(entity, Prefixes).Table.Columns;

        Assert.IsTrue(columns.Single(c => c.LogicalName == "ftp_contratid").Selected, "primary key");
        Assert.IsTrue(columns.Single(c => c.LogicalName == "ftp_nom").Selected, "name column");
        Assert.IsTrue(columns.Single(c => c.LogicalName == "createdon").Selected, "createdon");
        Assert.IsTrue(columns.Single(c => c.LogicalName == "modifiedon").Selected, "modifiedon");

        Assert.IsFalse(columns.Single(c => c.LogicalName == "ftp_commentaire").Selected,
            "An ordinary column stays inactive: it is tables sync that activates it once the code references it.");
    }

    [Test]
    public void Convert_SelectsColumnsParticipatingInAlternateKeys()
    {
        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid",
            attributes: StringAttribute("ftp_reference", "ftp_Reference", 50));

        entity.Set(nameof(DataverseMetadata.EntityMetadata.Keys),
            new[] { Key("ftp_reference_key", "Reference Key", "ftp_reference") });

        var column = MetadataTableFactory.Convert(entity, Prefixes).Table.Columns
            .Single(c => c.LogicalName == "ftp_reference");

        Assert.IsTrue(column.Selected,
            "Without it, the generated code could not express the alternate key.");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Alternate keys
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Convert_MapsAlternateKey_WithLogicalNameAndFormattedName()
    {
        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid");
        entity.Set(nameof(DataverseMetadata.EntityMetadata.Keys),
            new[] { Key("ftp_reference_key", "Reference Lookup Key", "ftp_reference", "ftp_annee") });

        var key = MetadataTableFactory.Convert(entity, Prefixes).Table.Keys.Single();

        Assert.AreEqual("ftp_reference_key", key.LogicalName);
        Assert.AreEqual("ReferenceLookupKey", key.Name);
        Assert.AreEqual(new[] { "ftp_reference", "ftp_annee" }, key.FieldNames.ToArray());
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Option sets
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Convert_PlacesLocalOptionSet_InTable_WithScopedLogicalName()
    {
        var picklist = Attribute<DataverseMetadata.PicklistAttributeMetadata>("ftp_type", "ftp_Type", DataverseMetadata.AttributeTypeCode.Picklist)
            .Set(nameof(DataverseMetadata.PicklistAttributeMetadata.OptionSet),
                OptionSet("ftp_type", isGlobal: false, "Type de contrat",
                    Option(1, "Location"), Option(2, "Vente")));

        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid", attributes: picklist);

        var result = MetadataTableFactory.Convert(entity, Prefixes);

        Assert.AreEqual(0, result.GlobalEnums.Count);

        var local = result.Table.Enums.Single();
        Assert.AreEqual("ftp_contrat|ftp_type", local.LogicalName,
            "A local option set is scoped to \"entity|attribute\" to avoid collisions.");
        Assert.AreEqual("TypeDeContrat", local.Name);
        Assert.AreEqual(new[] { "Location", "Vente" }, local.Values.Select(v => v.Name).ToArray());
        Assert.AreEqual("ftp_contrat|ftp_type", result.Table.Columns.Single().EnumName);
    }

    [Test]
    public void Convert_PlacesGlobalOptionSet_OutsideTable()
    {
        var picklist = Attribute<DataverseMetadata.PicklistAttributeMetadata>("ftp_devise", "ftp_Devise", DataverseMetadata.AttributeTypeCode.Picklist)
            .Set(nameof(DataverseMetadata.PicklistAttributeMetadata.OptionSet),
                OptionSet("ftp_deviseglobale", isGlobal: true, "Devise", Option(1, "Euro")));

        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid", attributes: picklist);

        var result = MetadataTableFactory.Convert(entity, Prefixes);

        Assert.AreEqual(0, result.Table.Enums.Count,
            "A global option set is never written into the entity's .table.");
        Assert.AreEqual("ftp_deviseglobale", result.GlobalEnums.Single().LogicalName);
        Assert.IsTrue(result.GlobalEnums.Single().IsGlobal);
    }

    [Test]
    public void Convert_NamesStateAndStatusOptionSets_AfterTable()
    {
        var state = Attribute<DataverseMetadata.StateAttributeMetadata>("statecode", "StateCode", DataverseMetadata.AttributeTypeCode.State)
            .Set(nameof(DataverseMetadata.StateAttributeMetadata.OptionSet),
                OptionSet("ftp_contrat_statecode", false, "Statut", Option(0, "Actif")));

        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid", attributes: state);

        var result = MetadataTableFactory.Convert(entity, Prefixes);

        Assert.AreEqual("ContratState", result.Table.Enums.Single().Name);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Relationships
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Convert_MapsManyToOneRelationship()
    {
        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid");
        entity.Set(nameof(DataverseMetadata.EntityMetadata.ManyToOneRelationships), new[]
        {
            new DataverseMetadata.OneToManyRelationshipMetadata()
                .Set(nameof(DataverseMetadata.OneToManyRelationshipMetadata.SchemaName), "ftp_contrat_account")
                .Set(nameof(DataverseMetadata.OneToManyRelationshipMetadata.ReferencedEntity), "account")
                .Set(nameof(DataverseMetadata.OneToManyRelationshipMetadata.ReferencingAttribute), "ftp_accountid")
                .Set(nameof(DataverseMetadata.OneToManyRelationshipMetadata.ReferencingEntityNavigationPropertyName), "ftp_accountid")
        });

        var relation = MetadataTableFactory.Convert(entity, Prefixes).Table.ManyToOneRelationships.Single();

        Assert.AreEqual("ftp_contrat_account", relation.Name);
        Assert.AreEqual("account", relation.EntityName);
        Assert.AreEqual("ftp_accountid", relation.LookupFieldName);
        Assert.AreEqual(EntityRole.Referencing, relation.Role);
    }

    [Test]
    public void Convert_MapsOneToManyRelationship_AsReferenced()
    {
        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid");
        entity.Set(nameof(DataverseMetadata.EntityMetadata.OneToManyRelationships), new[]
        {
            new DataverseMetadata.OneToManyRelationshipMetadata()
                .Set(nameof(DataverseMetadata.OneToManyRelationshipMetadata.SchemaName), "ftp_contrat_lignes")
                .Set(nameof(DataverseMetadata.OneToManyRelationshipMetadata.ReferencingEntity), "ftp_ligne")
                .Set(nameof(DataverseMetadata.OneToManyRelationshipMetadata.ReferencingAttribute), "ftp_contratid")
                .Set(nameof(DataverseMetadata.OneToManyRelationshipMetadata.ReferencedEntityNavigationPropertyName), "ftp_contrat_lignes")
        });

        var relation = MetadataTableFactory.Convert(entity, Prefixes).Table.OneToManyRelationships.Single();

        Assert.AreEqual("ftp_ligne", relation.EntityName);
        Assert.AreEqual(EntityRole.Referenced, relation.Role);
    }

    [Test]
    public void Convert_MapsManyToManyRelationship_ToOppositeEnd()
    {
        var entity = Entity("ftp_contrat", "ftp_Contrat", "ftp_contratid");
        entity.Set(nameof(DataverseMetadata.EntityMetadata.ManyToManyRelationships), new[]
        {
            new DataverseMetadata.ManyToManyRelationshipMetadata()
                .Set(nameof(DataverseMetadata.ManyToManyRelationshipMetadata.SchemaName), "ftp_contrat_tag")
                .Set(nameof(DataverseMetadata.ManyToManyRelationshipMetadata.Entity1LogicalName), "ftp_contrat")
                .Set(nameof(DataverseMetadata.ManyToManyRelationshipMetadata.Entity2LogicalName), "ftp_tag")
                .Set(nameof(DataverseMetadata.ManyToManyRelationshipMetadata.Entity1IntersectAttribute), "ftp_contratid")
                .Set(nameof(DataverseMetadata.ManyToManyRelationshipMetadata.Entity2IntersectAttribute), "ftp_tagid")
                .Set(nameof(DataverseMetadata.ManyToManyRelationshipMetadata.IntersectEntityName), "ftp_contrat_tag")
        });

        var relation = MetadataTableFactory.Convert(entity, Prefixes).Table.ManyToManyRelationships.Single();

        Assert.AreEqual("ftp_tag", relation.EntityName, "We always retain the opposite end.");
        Assert.AreEqual("ftp_tagid", relation.LookupFieldName);
    }
}
