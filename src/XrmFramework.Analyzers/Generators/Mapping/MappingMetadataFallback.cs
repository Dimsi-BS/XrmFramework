// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model.Sdk;
using Newtonsoft.Json;
using XrmFramework.Core;

namespace XrmFramework.Analyzers.Generators.Mapping;

/// <summary>
/// Fills in the column metadata the semantic model could not supply.
///
/// <see cref="MappingSourceGenerator"/> normally reads a column's type off the
/// <c>[AttributeMetadata]</c> attribute carried by the generated definition constant. In the
/// project that owns the <c>.table</c> files that constant does not resolve — its class is being
/// generated in the same pass — and every column silently fell back to
/// <see cref="AttributeTypeCode.String"/>, producing mappings that compiled and were wrong for
/// picklists, lookups, money and dates.
///
/// The tables say the same thing and are available as <c>AdditionalFiles</c>, so they are read
/// directly. Matching is by the <em>C# name</em>: a model writes
/// <c>AccountDefinition.Columns.Name</c>, and the table's column carries <c>Name</c> as its
/// <see cref="Column.Name"/>.
/// </summary>
internal static class MappingMetadataFallback
{
    public static TableCollection ReadTables(ImmutableArray<string> contents)
    {
        var tables = new TableCollection();

        foreach (var content in contents)
        {
            if (string.IsNullOrWhiteSpace(content)) continue;

            Table? table;
            try
            {
                table = JsonConvert.DeserializeObject<Table>(content);
            }
            catch (JsonException)
            {
                // A malformed .table is the table generator's business to report, not this one's.
                continue;
            }

            if (table != null && !string.IsNullOrEmpty(table.Name))
            {
                tables.Add(table);
            }
        }

        return tables;
    }

    /// <summary>
    /// Completes <paramref name="model"/> in place. Properties whose metadata already came from a
    /// resolved symbol are left alone: when the definition is a referenced assembly the semantic
    /// model is the better source, and this must not override it.
    /// </summary>
    public static void Complete(MappingModel model, TableCollection tables)
    {
        if (tables.Count == 0) return;

        foreach (var property in model.Properties)
        {
            if (property.MetadataResolved) continue;

            var definitionName = property.DefinitionName ?? model.DefinitionName;
            if (definitionName == null || property.ColumnLeafName == null) continue;

            var table = FindTable(tables, definitionName);
            if (table == null) continue;

            var column = table.Columns.FirstOrDefault(c => c.Name == property.ColumnLeafName);
            if (column == null) continue;

            property.AttrType = column.Type;

            if (property.LookupTargetRef == null && IsLookup(column.Type))
            {
                var relation = table.ManyToOneRelationships
                    .FirstOrDefault(r => r.LookupFieldName == column.LogicalName);

                if (relation != null)
                {
                    var target = tables.FirstOrDefault(t => t.LogicalName == relation.EntityName);

                    property.LookupTargetRef = target != null
                        ? $"{target.Name}Definition.EntityName"
                        : $"\"{relation.EntityName}\"";
                }
            }
        }
    }

    private static bool IsLookup(AttributeTypeCode type)
        => type is AttributeTypeCode.Lookup or AttributeTypeCode.Customer or AttributeTypeCode.Owner;

    /// <summary>
    /// <c>AccountDefinition</c> is the definition of the table whose <see cref="Table.Name"/> is
    /// <c>Account</c>. The suffix is what the table generator appends, so it is what gets removed.
    /// </summary>
    private static Table? FindTable(IEnumerable<Table> tables, string definitionName)
    {
        var tableName = definitionName.EndsWith("Definition", StringComparison.Ordinal)
            ? definitionName.Substring(0, definitionName.Length - "Definition".Length)
            : definitionName;

        return tables.FirstOrDefault(t => string.Equals(t.Name, tableName, StringComparison.Ordinal));
    }
}
