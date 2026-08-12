// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Core;
using CoreTable = XrmFramework.Core.Table;
using CoreLocalizedLabel = XrmFramework.Core.LocalizedLabel;
using SdkLabel = Microsoft.Xrm.Sdk.Label;

// The XrmFramework namespace declares its own AttributeMetadata, EntityMetadata and
// OptionMetadata. Since enclosing namespaces take precedence over using directives,
// a "using Microsoft.Xrm.Sdk.Metadata" would be silently ignored for these three types.
// The namespace alias makes the origin explicit at every use.
using DataverseMetadata = Microsoft.Xrm.Sdk.Metadata;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Result of converting a Dataverse entity.
    /// </summary>
    public sealed class MetadataConversionResult
    {
        internal MetadataConversionResult(CoreTable table, IReadOnlyList<OptionSetEnum> globalEnums)
        {
            Table = table;
            GlobalEnums = globalEnums;
        }

        /// <summary>Converted table, including local option sets.</summary>
        public CoreTable Table { get; }

        /// <summary>
        /// Global option sets referenced by the entity. They are never written to the entity's
        /// .table but gathered in the shared <c>OptionSets.table</c> file.
        /// </summary>
        public IReadOnlyList<OptionSetEnum> GlobalEnums { get; }
    }

    /// <summary>
    /// Converts an entity's Dataverse metadata into a <see cref="CoreTable" />.
    /// </summary>
    /// <remarks>
    /// Extraction of the pure part of <c>DefinitionManager.DataAccessManager.DoRetrieveEntities</c>,
    /// which used to build the WinForms model and the <c>XrmFramework.Core</c> model in parallel;
    /// only the latter is kept here, which makes the conversion usable without a graphical
    /// interface and testable.
    /// </remarks>
    public static class MetadataTableFactory
    {
        /// <summary>
        /// System columns systematically selected when creating a .table: they are
        /// used by nearly all plugins, and activating them one by one via
        /// <c>tables sync</c> would be needless friction.
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
        /// Converts an entity along with its attributes, relationships, alternate keys and option sets.
        /// </summary>
        /// <param name="entity">
        /// Metadata retrieved with <c>EntityFilters.Entity | Attributes | Relationships</c>.
        /// </param>
        /// <param name="publisherPrefixes">Publisher prefixes to strip from schema names.</param>
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
        // Alternate keys
        // ──────────────────────────────────────────────────────────────────────

        private static void AddKeys(DataverseMetadata.EntityMetadata entity, CoreTable table)
        {
            if (entity.Keys == null)
                return;

            foreach (var key in entity.Keys)
            {
                // Some logical names come back wrapped in quotes depending on the SDK version.
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
        // Relationships
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
                // An N-N relationship is symmetric: we always retain the end opposite to the
                // current entity, including for a self-relationship where both ends are identical.
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
        // Columns and option sets
        // ──────────────────────────────────────────────────────────────────────

        private static IReadOnlyList<OptionSetEnum> AddColumns(
            DataverseMetadata.EntityMetadata entity, CoreTable table, IList<string> prefixes)
        {
            var globalEnums = new List<OptionSetEnum>();

            // Columns participating in an alternate key are automatically selected:
            // without them, the generated code cannot express the key.
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
                    var optionSetEnum = BuildEnum(entity, table, attributeMetadata, attributeType, isMultiSelect,
                                                  out enumLogicalName);

                    // An option set with no label in the user's language cannot
                    // produce a C# type name: the whole attribute is discarded, as the
                    // DefinitionManager used to do.
                    if (optionSetEnum == null)
                        continue;

                    if (optionSetEnum.IsGlobal)
                    {
                        var known = globalEnums.FirstOrDefault(
                            e => string.Equals(e.LogicalName, optionSetEnum.LogicalName,
                                               StringComparison.OrdinalIgnoreCase));

                        // Two columns of the same entity may rest on the same global choice. They
                        // describe one and the same option set, so the nullability either of them
                        // establishes is kept — see TableMerger.MergeGlobalOptionSets, which does
                        // the same across entities.
                        if (known == null)
                            globalEnums.Add(optionSetEnum);
                        else
                            known.HasNullValue |= optionSetEnum.HasNullValue;
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
        /// Discards unusable attributes: those with no CRUD validity at all, polymorphic lookup
        /// discriminators (<c>EntityName</c>), and attributes derived from another one (<c>AttributeOf</c>,
        /// typically formatted-value companions).
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
            bool isMultiSelect,
            out string enumLogicalName)
        {
            enumLogicalName = null;

            var optionSet = (attributeMetadata as DataverseMetadata.EnumAttributeMetadata)?.OptionSet;
            if (optionSet == null)
                return null;

            var isGlobal = optionSet.IsGlobal.GetValueOrDefault();

            // A local option set is identified by "entity|attribute": two entities can
            // define choices with the same name without collision in the shared file.
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
                // A choice with no option valued at 0 can be null on the CRM side: the generated
                // C# type must then expose an explicit null value. A multi-select choice qualifies
                // just as much — an empty one is null too — and it reaches us as a Virtual
                // attribute, which is why the type alone cannot answer for it.
                HasNullValue = (attributeType == DataverseMetadata.AttributeTypeCode.Picklist || isMultiSelect)
                               && (optionSet.Options?.All(o => o.Value.GetValueOrDefault() != 0) ?? false)
            };

            foreach (var option in optionSet.Options ?? Enumerable.Empty<DataverseMetadata.OptionMetadata>())
            {
                var optionLabel = GetUserLabel(option.Label);

                // Without a user label, no enumeration member name can be produced.
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

            // The primary key is always exposed under the name "Id", regardless of its
            // schema name: this is the convention the generated code relies on.
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
        /// Selection policy applied to newly discovered columns: the usable minimum.
        /// The other columns are indeed written to the .table with all their
        /// metadata, but remain inactive until <c>tables sync</c> activates them because
        /// the code references them — which avoids generating thousands of useless constants.
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

            // Unlike the other flags, this one is a managed property (it can
            // be locked by a solution) rather than a plain nullable boolean.
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
        /// Carries over the numeric bounds declared on the CRM side, which feed the generated
        /// code's <c>[Range]</c> attribute. The generator requires both bounds: they are therefore
        /// always set together or left null together.
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

            // Fail loudly rather than label the column with an incorrect behavior,
            // which would produce silently wrong timezone conversions in the plugins.
            throw new NotSupportedException(
                $"Unknown date/time behavior \"{value}\" on attribute " +
                $"\"{attributeMetadata.LogicalName}\".");
        }

        // ──────────────────────────────────────────────────────────────────────
        // Labels
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
