// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Core;
using CoreTable = XrmFramework.Core.Table;
using CoreLocalizedLabel = XrmFramework.Core.LocalizedLabel;
using SdkLabel = Microsoft.Xrm.Sdk.Label;

// L'espace de noms XrmFramework déclare ses propres AttributeMetadata, EntityMetadata et
// OptionMetadata. Comme les espaces de noms englobants l'emportent sur les directives using,
// un « using Microsoft.Xrm.Sdk.Metadata » serait silencieusement ignoré pour ces trois types.
// L'alias d'espace de noms rend l'origine explicite à chaque usage.
using DataverseMetadata = Microsoft.Xrm.Sdk.Metadata;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Résultat de la conversion d'une entité Dataverse.
    /// </summary>
    public sealed class MetadataConversionResult
    {
        internal MetadataConversionResult(CoreTable table, IReadOnlyList<OptionSetEnum> globalEnums)
        {
            Table = table;
            GlobalEnums = globalEnums;
        }

        /// <summary>Table convertie, option sets locaux inclus.</summary>
        public CoreTable Table { get; }

        /// <summary>
        /// Option sets globaux référencés par l'entité. Ils ne sont jamais écrits dans le .table de
        /// l'entité mais rassemblés dans le fichier <c>OptionSet.table</c> partagé.
        /// </summary>
        public IReadOnlyList<OptionSetEnum> GlobalEnums { get; }
    }

    /// <summary>
    /// Convertit les métadonnées Dataverse d'une entité en <see cref="CoreTable" />.
    /// </summary>
    /// <remarks>
    /// Extraction de la partie pure de <c>DefinitionManager.DataAccessManager.DoRetrieveEntities</c>,
    /// qui construisait en parallèle le modèle WinForms et le modèle <c>XrmFramework.Core</c> ;
    /// seul le second est conservé ici, ce qui rend la conversion utilisable sans interface
    /// graphique et testable.
    /// </remarks>
    public static class MetadataTableFactory
    {
        /// <summary>
        /// Colonnes systèmes systématiquement sélectionnées à la création d'un .table : elles sont
        /// utilisées par la quasi-totalité des plugins, et les activer une par une via
        /// <c>tables sync</c> serait une friction inutile.
        /// </summary>
        private static readonly HashSet<string> AlwaysSelectedColumns =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "createdon",
                "modifiedon",
                "statecode",
                "statuscode"
            };

        /// <summary>
        /// Convertit une entité et ses attributs, relations, clés alternatives et option sets.
        /// </summary>
        /// <param name="entity">
        /// Métadonnées récupérées avec <c>EntityFilters.Entity | Attributes | Relationships</c>.
        /// </param>
        /// <param name="publisherPrefixes">Préfixes d'éditeur à retirer des noms de schéma.</param>
        public static MetadataConversionResult Convert(
            DataverseMetadata.EntityMetadata entity, IEnumerable<string> publisherPrefixes)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var prefixes = publisherPrefixes as IList<string> ?? publisherPrefixes?.ToList() ?? new List<string>();

            var table = new CoreTable
            {
                LogicalName = entity.LogicalName,
                CollectionName = entity.LogicalCollectionName,
                Name = NameFormatter.FormatText(NameFormatter.RemovePrefix(entity.SchemaName, prefixes))
            };

            AddKeys(entity, table);
            AddRelationships(entity, table);

            var globalEnums = AddColumns(entity, table, prefixes);

            return new MetadataConversionResult(table, globalEnums);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Clés alternatives
        // ──────────────────────────────────────────────────────────────────────

        private static void AddKeys(DataverseMetadata.EntityMetadata entity, CoreTable table)
        {
            if (entity.Keys == null)
                return;

            foreach (var key in entity.Keys)
            {
                // Certains noms logiques remontent entourés de guillemets selon la version du SDK.
                var logicalName = key.LogicalName?.Trim('"');

                var newKey = new Key
                {
                    LogicalName = logicalName,
                    Name = NameFormatter.FormatText(GetUserLabel(key.DisplayName) ?? logicalName)
                };

                if (key.KeyAttributes != null)
                    newKey.FieldNames.AddRange(key.KeyAttributes);

                table.Keys.Add(newKey);
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Relations
        // ──────────────────────────────────────────────────────────────────────

        private static void AddRelationships(DataverseMetadata.EntityMetadata entity, CoreTable table)
        {
            foreach (var relationship in entity.OneToManyRelationships ?? new DataverseMetadata.OneToManyRelationshipMetadata[0])
            {
                table.OneToManyRelationships.Add(new Relation
                {
                    Name = relationship.SchemaName,
                    Role = EntityRole.Referenced,
                    EntityName = relationship.ReferencingEntity,
                    NavigationPropertyName = relationship.ReferencedEntityNavigationPropertyName,
                    LookupFieldName = relationship.ReferencingAttribute
                });
            }

            foreach (var relationship in entity.ManyToOneRelationships ?? new DataverseMetadata.OneToManyRelationshipMetadata[0])
            {
                table.ManyToOneRelationships.Add(new Relation
                {
                    Name = relationship.SchemaName,
                    Role = EntityRole.Referencing,
                    EntityName = relationship.ReferencedEntity,
                    NavigationPropertyName = relationship.ReferencingEntityNavigationPropertyName,
                    LookupFieldName = relationship.ReferencingAttribute
                });
            }

            foreach (var relationship in entity.ManyToManyRelationships ?? new DataverseMetadata.ManyToManyRelationshipMetadata[0])
            {
                // Une relation N-N est symétrique : on retient toujours le bout opposé à l'entité
                // courante, y compris pour une auto-relation où les deux bouts sont identiques.
                var isEntity1 = string.Equals(relationship.Entity1LogicalName, entity.LogicalName,
                    StringComparison.OrdinalIgnoreCase);

                table.ManyToManyRelationships.Add(new Relation
                {
                    Name = relationship.SchemaName,
                    Role = EntityRole.Referencing,
                    EntityName = isEntity1 ? relationship.Entity2LogicalName : relationship.Entity1LogicalName,
                    NavigationPropertyName = relationship.IntersectEntityName,
                    LookupFieldName = isEntity1
                        ? relationship.Entity2IntersectAttribute
                        : relationship.Entity1IntersectAttribute
                });
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Colonnes et option sets
        // ──────────────────────────────────────────────────────────────────────

        private static IReadOnlyList<OptionSetEnum> AddColumns(
            DataverseMetadata.EntityMetadata entity, CoreTable table, IList<string> prefixes)
        {
            var globalEnums = new List<OptionSetEnum>();

            // Les colonnes participant à une clé alternative sont sélectionnées d'office :
            // sans elles, le code généré ne peut pas exprimer la clé.
            var keyFieldNames = new HashSet<string>(
                table.Keys.SelectMany(k => k.FieldNames), StringComparer.OrdinalIgnoreCase);

            var attributes = (entity.Attributes ?? new DataverseMetadata.AttributeMetadata[0])
                .Where(IsConvertible)
                .OrderBy(a => a.LogicalName, StringComparer.Ordinal);

            foreach (var attributeMetadata in attributes)
            {
                var attributeType = attributeMetadata.AttributeType.Value;
                var isMultiSelect = attributeType == DataverseMetadata.AttributeTypeCode.Virtual
                                    && attributeMetadata is DataverseMetadata.MultiSelectPicklistAttributeMetadata;

                string enumLogicalName = null;

                if (IsEnumAttribute(attributeType, isMultiSelect))
                {
                    var optionSetEnum = BuildEnum(entity, table, attributeMetadata, attributeType, out enumLogicalName);

                    // Un option set dépourvu de libellé dans la langue de l'utilisateur ne peut pas
                    // produire de nom de type C# : l'attribut entier est écarté, comme le faisait
                    // le DefinitionManager.
                    if (optionSetEnum == null)
                        continue;

                    if (optionSetEnum.IsGlobal)
                    {
                        if (globalEnums.All(e => e.LogicalName != optionSetEnum.LogicalName))
                            globalEnums.Add(optionSetEnum);
                    }
                    else
                    {
                        table.Enums.Add(optionSetEnum);
                    }
                }

                table.Columns.Add(BuildColumn(
                    entity, attributeMetadata, attributeType, isMultiSelect, enumLogicalName, prefixes, keyFieldNames));
            }

            return globalEnums;
        }

        /// <summary>
        /// Écarte les attributs inexploitables : sans aucune validité CRUD, discriminants de lookup
        /// polymorphe (<c>EntityName</c>) et attributs dérivés d'un autre (<c>AttributeOf</c>,
        /// typiquement les compagnons de valeur formatée).
        /// </summary>
        private static bool IsConvertible(DataverseMetadata.AttributeMetadata attributeMetadata)
        {
            if (attributeMetadata.AttributeType == null)
                return false;

            if (!attributeMetadata.IsValidForCreate.GetValueOrDefault()
                && !attributeMetadata.IsValidForRead.GetValueOrDefault()
                && !attributeMetadata.IsValidForUpdate.GetValueOrDefault())
                return false;

            if (attributeMetadata.AttributeType.Value == DataverseMetadata.AttributeTypeCode.EntityName)
                return false;

            return string.IsNullOrEmpty(attributeMetadata.AttributeOf);
        }

        private static bool IsEnumAttribute(DataverseMetadata.AttributeTypeCode attributeType, bool isMultiSelect)
            => attributeType == DataverseMetadata.AttributeTypeCode.Picklist
               || attributeType == DataverseMetadata.AttributeTypeCode.State
               || attributeType == DataverseMetadata.AttributeTypeCode.Status
               || isMultiSelect;

        private static OptionSetEnum BuildEnum(
            DataverseMetadata.EntityMetadata entity,
            CoreTable table,
            DataverseMetadata.AttributeMetadata attributeMetadata,
            DataverseMetadata.AttributeTypeCode attributeType,
            out string enumLogicalName)
        {
            enumLogicalName = null;

            var optionSet = (attributeMetadata as DataverseMetadata.EnumAttributeMetadata)?.OptionSet;
            if (optionSet == null)
                return null;

            var isGlobal = optionSet.IsGlobal.GetValueOrDefault();

            // Un option set local est identifié par « entité|attribut » : deux entités peuvent
            // définir des choix homonymes sans collision dans le fichier partagé.
            var logicalName = isGlobal
                ? optionSet.Name
                : entity.LogicalName + "|" + attributeMetadata.LogicalName;

            string name;
            if (attributeType == DataverseMetadata.AttributeTypeCode.State)
                name = table.Name + "State";
            else if (attributeType == DataverseMetadata.AttributeTypeCode.Status)
                name = table.Name + "Status";
            else
                name = NameFormatter.FormatText(GetUserLabel(optionSet.DisplayName));

            if (string.IsNullOrEmpty(name))
                return null;

            enumLogicalName = logicalName;

            var result = new OptionSetEnum
            {
                LogicalName = logicalName,
                Name = name,
                IsGlobal = isGlobal,
                // Un Picklist dont aucune option ne vaut 0 peut être nul côté CRM : le type C#
                // généré doit alors exposer une valeur nulle explicite.
                HasNullValue = attributeType == DataverseMetadata.AttributeTypeCode.Picklist
                               && (optionSet.Options?.All(o => o.Value.GetValueOrDefault() != 0) ?? false)
            };

            foreach (var option in optionSet.Options ?? Enumerable.Empty<DataverseMetadata.OptionMetadata>())
            {
                var optionLabel = GetUserLabel(option.Label);

                // Sans libellé utilisateur, aucun nom de membre d'énumération ne peut être produit.
                if (string.IsNullOrEmpty(optionLabel))
                    continue;

                var value = new OptionSetEnumValue
                {
                    Name = NameFormatter.FormatText(optionLabel),
                    Value = option.Value.GetValueOrDefault(),
                    ExternalValue = option.ExternalValue
                };

                AddLabels(option.Label, value.Labels);
                result.Values.Add(value);
            }

            return result;
        }

        private static Column BuildColumn(
            DataverseMetadata.EntityMetadata entity,
            DataverseMetadata.AttributeMetadata attributeMetadata,
            DataverseMetadata.AttributeTypeCode attributeType,
            bool isMultiSelect,
            string enumLogicalName,
            IList<string> prefixes,
            HashSet<string> keyFieldNames)
        {
            var primaryType = GetPrimaryType(entity, attributeMetadata);

            // La clé primaire est toujours exposée sous le nom « Id », quel que soit son nom de
            // schéma : c'est la convention sur laquelle s'appuie le code généré.
            var name = primaryType == PrimaryType.Id
                ? "Id"
                : NameFormatter.RemovePrefix(attributeMetadata.SchemaName, prefixes);

            var column = new Column
            {
                LogicalName = attributeMetadata.LogicalName,
                Name = name,
                Type = isMultiSelect
                    ? AttributeTypeCode.Picklist
                    : (AttributeTypeCode)attributeType,
                IsMultiSelect = isMultiSelect,
                PrimaryType = primaryType,
                StringLength = GetStringLength(attributeMetadata, attributeType),
                EnumName = enumLogicalName,
                Capabilities = GetCapabilities(attributeMetadata),
                Selected = IsSelectedByDefault(attributeMetadata, primaryType, keyFieldNames)
            };

            ApplyRange(column, attributeMetadata, attributeType);

            if (attributeType == DataverseMetadata.AttributeTypeCode.DateTime)
                column.DateTimeBehavior = ToFrameworkDateTimeBehavior(
                    (attributeMetadata as DataverseMetadata.DateTimeAttributeMetadata)?.DateTimeBehavior, attributeMetadata);

            AddLabels(attributeMetadata.DisplayName, column.Labels);

            return column;
        }

        /// <summary>
        /// Politique de sélection appliquée aux colonnes nouvellement découvertes : le minimum
        /// exploitable. Les autres colonnes sont bien écrites dans le .table avec toutes leurs
        /// métadonnées, mais restent inactives jusqu'à ce que <c>tables sync</c> les active parce
        /// que le code les référence — ce qui évite de générer des milliers de constantes inutiles.
        /// </summary>
        private static bool IsSelectedByDefault(
            DataverseMetadata.AttributeMetadata attributeMetadata, PrimaryType primaryType, HashSet<string> keyFieldNames)
            => primaryType != PrimaryType.None
               || keyFieldNames.Contains(attributeMetadata.LogicalName)
               || AlwaysSelectedColumns.Contains(attributeMetadata.LogicalName);

        private static PrimaryType GetPrimaryType(DataverseMetadata.EntityMetadata entity, DataverseMetadata.AttributeMetadata attributeMetadata)
        {
            if (attributeMetadata.LogicalName == entity.PrimaryIdAttribute)
                return PrimaryType.Id;

            if (attributeMetadata.LogicalName == entity.PrimaryNameAttribute)
                return PrimaryType.Name;

            if (attributeMetadata.LogicalName == entity.PrimaryImageAttribute)
                return PrimaryType.Image;

            return PrimaryType.None;
        }

        private static AttributeCapabilities GetCapabilities(DataverseMetadata.AttributeMetadata attributeMetadata)
        {
            var capabilities = AttributeCapabilities.None;

            // Contrairement aux autres indicateurs, celui-ci est une propriété managée (elle peut
            // être verrouillée par une solution) et non un simple booléen nullable.
            if (attributeMetadata.IsValidForAdvancedFind?.Value == true)
                capabilities |= AttributeCapabilities.AdvancedFind;

            if (attributeMetadata.IsValidForCreate.GetValueOrDefault())
                capabilities |= AttributeCapabilities.Create;

            if (attributeMetadata.IsValidForRead.GetValueOrDefault())
                capabilities |= AttributeCapabilities.Read;

            if (attributeMetadata.IsValidForUpdate.GetValueOrDefault())
                capabilities |= AttributeCapabilities.Update;

            return capabilities;
        }

        private static int? GetStringLength(
            DataverseMetadata.AttributeMetadata attributeMetadata, DataverseMetadata.AttributeTypeCode attributeType)
        {
            switch (attributeType)
            {
                case DataverseMetadata.AttributeTypeCode.String:
                    return (attributeMetadata as DataverseMetadata.StringAttributeMetadata)?.MaxLength;
                case DataverseMetadata.AttributeTypeCode.Memo:
                    return (attributeMetadata as DataverseMetadata.MemoAttributeMetadata)?.MaxLength;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Reporte les bornes numériques déclarées côté CRM, qui alimentent l'attribut
        /// <c>[Range]</c> du code généré. Le générateur exige les deux bornes : elles sont donc
        /// toujours renseignées ensemble ou laissées nulles ensemble.
        /// </summary>
        private static void ApplyRange(
            Column column, DataverseMetadata.AttributeMetadata attributeMetadata, DataverseMetadata.AttributeTypeCode attributeType)
        {
            switch (attributeType)
            {
                case DataverseMetadata.AttributeTypeCode.Money:
                    var money = attributeMetadata as DataverseMetadata.MoneyAttributeMetadata;
                    column.MinRange = money?.MinValue;
                    column.MaxRange = money?.MaxValue;
                    break;

                case DataverseMetadata.AttributeTypeCode.Integer:
                    var integer = attributeMetadata as DataverseMetadata.IntegerAttributeMetadata;
                    column.MinRange = integer?.MinValue;
                    column.MaxRange = integer?.MaxValue;
                    break;

                case DataverseMetadata.AttributeTypeCode.Double:
                    var doubleMetadata = attributeMetadata as DataverseMetadata.DoubleAttributeMetadata;
                    column.MinRange = doubleMetadata?.MinValue;
                    column.MaxRange = doubleMetadata?.MaxValue;
                    break;

                case DataverseMetadata.AttributeTypeCode.Decimal:
                    var decimalMetadata = attributeMetadata as DataverseMetadata.DecimalAttributeMetadata;
                    column.MinRange = (double?)decimalMetadata?.MinValue;
                    column.MaxRange = (double?)decimalMetadata?.MaxValue;
                    break;
            }
        }

        private static DateTimeBehavior ToFrameworkDateTimeBehavior(
            DataverseMetadata.DateTimeBehavior behavior, DataverseMetadata.AttributeMetadata attributeMetadata)
        {
            var value = behavior?.Value ?? nameof(DateTimeBehavior.UserLocal);

            if (value == nameof(DateTimeBehavior.UserLocal))
                return DateTimeBehavior.UserLocal;

            if (value == nameof(DateTimeBehavior.DateOnly))
                return DateTimeBehavior.DateOnly;

            if (value == nameof(DateTimeBehavior.TimeZoneIndependent))
                return DateTimeBehavior.TimeZoneIndependent;

            // Échouer bruyamment plutôt que d'étiqueter la colonne avec un comportement erroné,
            // qui produirait des conversions de fuseau silencieusement fausses dans les plugins.
            throw new NotSupportedException(
                $"Comportement de date/heure inconnu « {value} » sur l'attribut " +
                $"« {attributeMetadata.LogicalName} ».");
        }

        // ──────────────────────────────────────────────────────────────────────
        // Libellés
        // ──────────────────────────────────────────────────────────────────────

        private static void AddLabels(SdkLabel label, ICollection<CoreLocalizedLabel> target)
        {
            if (label?.LocalizedLabels == null)
                return;

            foreach (var localizedLabel in label.LocalizedLabels)
                target.Add(new CoreLocalizedLabel
                {
                    Label = localizedLabel.Label,
                    LangId = localizedLabel.LanguageCode
                });
        }

        private static string GetUserLabel(SdkLabel label) => label?.UserLocalizedLabel?.Label;
    }
}
