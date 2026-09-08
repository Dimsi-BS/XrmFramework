// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model.Sdk;
using XrmFramework.Core;

namespace XrmFramework.Analyzers.Generators.Mapping;

/// <summary>
/// Builds a <see cref="MappingModel"/> from a <c>.model</c> file and the <c>.table</c> it names.
///
/// This is the second producer for <see cref="MappingEmitter"/>, beside the symbol-based one in
/// <see cref="MappingSourceGenerator"/>. It reads everything from the table description rather
/// than from Roslyn symbols, because the <c>…Definition</c> class it would otherwise interrogate
/// is generated in the same pass and is therefore not resolvable.
/// </summary>
internal static class MappingModelFactory
{
    /// <summary>
    /// C# types a property can carry that are never a generated option-set enum. Anything else
    /// on an option-set column is taken to be the enum the table generator emits.
    /// </summary>
    private static readonly HashSet<string> PrimitiveTypeNames = new(StringComparer.Ordinal)
    {
        "bool", "byte", "sbyte", "char", "decimal", "double", "float", "int", "uint",
        "long", "ulong", "short", "ushort", "object", "string",
        "Boolean", "Byte", "Decimal", "Double", "Guid", "Int16", "Int32", "Int64",
        "Single", "String", "DateTime", "OptionSetValue", "EntityReference", "Money"
    };

    /// <summary>Column types whose value is an option set, and so may map to a generated enum.</summary>
    private static bool IsOptionSet(AttributeTypeCode type)
        => type is AttributeTypeCode.Picklist or AttributeTypeCode.State or AttributeTypeCode.Status;

    public sealed class Result
    {
        public MappingModel? Model { get; }
        public ImmutableArray<MappingFailure> Failures { get; }

        public Result(MappingModel? model, ImmutableArray<MappingFailure> failures)
        {
            Model = model;
            Failures = failures;
        }
    }

    /// <summary>
    /// Converts <paramref name="model"/> against <paramref name="table"/>. Anything the pair does
    /// not allow to be mapped is reported rather than skipped: a property silently missing from a
    /// generated class is the hardest kind of authoring mistake to notice.
    /// </summary>
    public static Result Create(Core.Model model, Table table, TableCollection tables, ICollection<Core.Model> models)
    {
        var failures = ImmutableArray.CreateBuilder<MappingFailure>();
        var properties = ImmutableArray.CreateBuilder<MappingProperty>();
        var extensions = ImmutableArray.CreateBuilder<MappingExtension>();
        var relationships = ImmutableArray.CreateBuilder<MappingRelationship>();

        foreach (var property in model.Properties)
        {
            if (property.ExtendBindingModel)
            {
                var extended = ResolveExtension(model, property, models, failures);

                if (extended != null)
                {
                    extensions.Add(extended);
                }

                continue;
            }

            var column = table.Columns.FirstOrDefault(c => c.LogicalName == property.LogicalName);

            if (column == null)
            {
                var oneToMany = table.OneToManyRelationships.FirstOrDefault(r => r.Name == property.LogicalName);

                if (oneToMany != null)
                {
                    var relationship = ResolveRelationship(model, property, table, oneToMany, failures);

                    if (relationship != null)
                    {
                        relationships.Add(relationship);
                    }

                    continue;
                }

                failures.Add(MappingFailure.UnknownColumn(model.Name, property.Name, property.LogicalName, table.LogicalName));
                continue;
            }

            if (!column.Selected)
            {
                failures.Add(MappingFailure.ColumnNotSelected(model.Name, property.Name, column.LogicalName, table.LogicalName));
                continue;
            }

            string? lookupTargetRef = null;
            Table? targetTable = null;
            List<Relation>? lookupRelations = null;
            Relation? lookupRelation = null;

            if (IsLookup(column.Type))
            {
                var relations = table.ManyToOneRelationships
                    .Where(r => r.LookupFieldName == column.LogicalName)
                    .ToList();
                lookupRelations = relations;

                if (relations.Count == 0)
                {
                    failures.Add(MappingFailure.LookupWithoutRelationship(model.Name, property.Name, column.LogicalName, table.LogicalName));
                    continue;
                }

                Relation? relation;

                if (!string.IsNullOrEmpty(property.LookupTargetTableLogicalName))
                {
                    relation = relations.FirstOrDefault(r => r.EntityName == property.LookupTargetTableLogicalName);

                    // An Owner column relates to the "owner" pseudo-entity, never to the tables an
                    // owner actually is. Naming systemuser or team is the only way to say which,
                    // and is what the hand-written models do.
                    if (relation == null && IsOwnerAlias(column.Type, relations, property.LookupTargetTableLogicalName!))
                    {
                        relation = new Relation
                        {
                            // The real relationship the platform reports, kept so an embedded
                            // LookupTargetModel can still be matched against RelatedEntities by its
                            // actual schema name rather than the systemuser/team alias, which names
                            // no relationship of its own.
                            Name = relations.First(r => r.EntityName == OwnerEntityName).Name,
                            EntityName = property.LookupTargetTableLogicalName,
                            LookupFieldName = column.LogicalName
                        };
                    }

                    if (relation == null)
                    {
                        failures.Add(MappingFailure.UnknownLookupTarget(
                            model.Name, property.Name, property.LookupTargetTableLogicalName!, column.LogicalName,
                            relations.Select(r => r.EntityName)));
                        continue;
                    }
                }
                else if (relations.Count > 1)
                {
                    // A polymorphic lookup — customerid, regardingobjectid — reaches several
                    // tables. Picking one arbitrarily would emit an EntityReference pointing at
                    // the wrong table half the time, so the model has to say which.
                    failures.Add(MappingFailure.AmbiguousLookupTarget(
                        model.Name, property.Name, column.LogicalName, relations.Select(r => r.EntityName)));
                    continue;
                }
                else
                {
                    relation = relations[0];
                }

                lookupRelation = relation;
                targetTable = tables.FirstOrDefault(t => t.LogicalName == relation.EntityName);

                // A target table the project does not track still yields a correct mapping; the
                // entity name is simply written as a literal instead of a definition constant.
                lookupTargetRef = targetTable != null
                    ? $"{targetTable.Name}Definition.EntityName"
                    : $"\"{relation.EntityName}\"";
            }

            var mapped = BuildProperty(table, column, property, lookupTargetRef);

            if (property.LookupTargetModel)
            {
                mapped.IsEmbeddedLookupModel = true;
                mapped.EmbeddedModelTypeName = property.TypeFullName;
                mapped.EmbeddedIsPolymorphic = (lookupRelations?.Count ?? 0) > 1;
                mapped.EmbeddedRelationshipName = lookupRelation?.Name;
            }

            if (!string.IsNullOrEmpty(property.LookupTargetColumnLogicalName))
            {
                var projected = targetTable?.Columns
                    .FirstOrDefault(c => c.LogicalName == property.LookupTargetColumnLogicalName);

                // Alias the query builder gives the link: the lookup column's logical name,
                // then the projected column. Built from the definition constants so a rename
                // of either follows.
                var projectedRef = projected != null && targetTable != null
                    ? $"{targetTable.Name}Definition.Columns.{projected.Name}"
                    : $"\"{property.LookupTargetColumnLogicalName}\"";

                mapped.AliasedValueRef = $"{table.Name}Definition.Columns.{column.Name} + \".\" + {projectedRef}";
            }

            ReportTypeIncompatibility(failures, model, property, column, mapped, targetTable);

            properties.Add(mapped);
        }

        var mappingModel = new MappingModel(
            model.Name,
            string.IsNullOrEmpty(model.ModelNamespace) ? null : model.ModelNamespace,
            $"{table.Name}Definition.EntityName",
            isBindingModelBase: true,
            properties.ToImmutable(),
            extensions.ToImmutable(),
            relationships.ToImmutable())
        {
            AlternateKeys = ResolveAlternateKeys(model, table)
        };

        return new Result(mappingModel, failures.ToImmutable());
    }

    /// <summary>
    ///     The table's alternate keys whose every column this model actually maps — a key none of
    ///     whose columns the model writes could never be satisfied, so resolving it at runtime
    ///     would only ever be dead code.
    /// </summary>
    private static ImmutableArray<ImmutableArray<string>> ResolveAlternateKeys(Core.Model model, Table table)
    {
        var mappedColumnNames = new HashSet<string>(
            model.Properties
                .Where(p => !p.ExtendBindingModel && p.LogicalName != null)
                .Select(p => p.LogicalName)
                .Where(n => table.Columns.Any(c => c.LogicalName == n)),
            StringComparer.Ordinal);

        return table.Keys
            .Select(k => k.FieldNames.ToImmutableArray())
            .Where(k => k.Length > 0 && k.All(mappedColumnNames.Contains))
            .ToImmutableArray();
    }

    /// <summary>
    /// Checks the declared C# type against the column whose value the property actually carries.
    ///
    /// On a projection that is <em>not</em> the lookup column. A model reaching a field of the
    /// targeted record — the pattern behind <c>[CrmMapping(lookupColumn)]</c> paired with
    /// <c>[CrmLookup(target, targetColumn)]</c> — declares the type of the <em>projected</em>
    /// column, so comparing it to the lookup would report every such property as expecting a
    /// <c>Guid</c>.
    /// </summary>
    private static void ReportTypeIncompatibility(
        ImmutableArray<MappingFailure>.Builder failures,
        Core.Model model,
        ModelProperty property,
        Column column,
        MappingProperty mapped,
        Table? targetTable)
    {
        // The property is a whole related model: its type is that model's class, and nothing
        // about a column describes it.
        if (property.LookupTargetModel)
        {
            return;
        }

        var checkedColumn = column;

        if (!string.IsNullOrEmpty(property.LookupTargetColumnLogicalName))
        {
            // Unknown target table: the projected column's type cannot be established, and
            // guessing would be worse than staying quiet.
            var projected = targetTable?.Columns
                .FirstOrDefault(c => c.LogicalName == property.LookupTargetColumnLogicalName);

            if (projected == null)
            {
                return;
            }

            checkedColumn = projected;
        }

        var expected = ColumnTypeCompatibility.Describe(
            checkedColumn, property.TypeFullName ?? string.Empty, mapped.IsList, mapped.ListElemTypeName);

        if (expected != null)
        {
            failures.Add(MappingFailure.IncompatibleType(
                model.Name, property.Name, property.TypeFullName ?? "?", checkedColumn.LogicalName, checkedColumn.Type, expected));
        }
    }

    /// <summary>
    ///     Whether <paramref name="requested"/> is one of the tables an Owner column can point at.
    ///     The platform declares such a column against the <c>owner</c> pseudo-entity, so the
    ///     relationships never mention systemuser or team even though those are what it holds.
    /// </summary>
    private static bool IsOwnerAlias(AttributeTypeCode type, List<Relation> relations, string requested)
        => type == AttributeTypeCode.Owner
        && relations.All(r => r.EntityName == OwnerEntityName)
        && (requested == "systemuser" || requested == "team");

    private const string OwnerEntityName = "owner";

    /// <summary>
    ///     Resolves an [ExtendBindingModel] property against the other .model files. The model it
    ///     names has to exist and to target the same table: both halves are filled from one row,
    ///     so a different table would have nothing to fill the extension from.
    /// </summary>
    private static MappingExtension? ResolveExtension(
        Core.Model model,
        ModelProperty property,
        ICollection<Core.Model> models,
        ImmutableArray<MappingFailure>.Builder failures)
    {
        var typeName = property.TypeFullName?.Trim();

        if (string.IsNullOrEmpty(typeName))
        {
            failures.Add(MappingFailure.ExtensionWithoutType(model.Name, property.Name));
            return null;
        }

        var leaf = typeName!.Substring(typeName.LastIndexOf('.') + 1);
        var extended = models.FirstOrDefault(m => m.Name == leaf);

        if (extended == null)
        {
            failures.Add(MappingFailure.UnknownExtensionModel(model.Name, property.Name, typeName));
            return null;
        }

        if (extended.TableLogicalName != model.TableLogicalName)
        {
            failures.Add(MappingFailure.ExtensionOnAnotherTable(
                model.Name, property.Name, extended.Name, extended.TableLogicalName, model.TableLogicalName));
            return null;
        }

        return new MappingExtension(property.Name, typeName);
    }

    /// <summary>
    ///     Resolves a <c>[ChildRelationship]</c> property against the one-to-many relationship
    ///     <paramref name="relation"/> names. The property maps no column: it is a
    ///     <c>List&lt;T&gt;</c> populated from the entities the relationship returns, so its
    ///     declared type has to say what <c>T</c> is.
    /// </summary>
    private static MappingRelationship? ResolveRelationship(
        Core.Model model,
        ModelProperty property,
        Table table,
        Relation relation,
        ImmutableArray<MappingFailure>.Builder failures)
    {
        var typeName = property.TypeFullName?.Trim();

        if (string.IsNullOrEmpty(typeName)
            || !typeName!.StartsWith("List<", StringComparison.Ordinal)
            || !typeName.EndsWith(">", StringComparison.Ordinal))
        {
            failures.Add(MappingFailure.RelationshipNotAList(model.Name, property.Name, typeName ?? "?"));
            return null;
        }

        var elementTypeName = typeName.Substring(5, typeName.Length - 6).Trim();

        if (elementTypeName.Length == 0)
        {
            failures.Add(MappingFailure.RelationshipNotAList(model.Name, property.Name, typeName));
            return null;
        }

        var relationshipRef = $"{table.Name}Definition.OneToManyRelationships.{relation.Name}";

        return new MappingRelationship(property.Name, elementTypeName, relationshipRef, property.IsValidForUpdate);
    }

    private static bool IsLookup(AttributeTypeCode type)
        => type is AttributeTypeCode.Lookup or AttributeTypeCode.Customer or AttributeTypeCode.Owner;

    private static MappingProperty BuildProperty(Table table, Column column, ModelProperty property, string? lookupTargetRef)
    {
        AnalyzeTypeName(property.TypeFullName, column,
            out var typeName, out var innerTypeName,
            out var isNullable, out var isEnum,
            out var isList, out var listElemTypeName);

        return new MappingProperty(
            property.Name,
            typeName,
            innerTypeName,
            isNullable,
            isEnum,
            isList,
            listElemTypeName,
            hasSetter: true,
            columnRef: $"{table.Name}Definition.Columns.{column.Name}",
            attrType: column.Type,
            isValidForUpdate: property.IsValidForUpdate,
            lookupTargetRef: lookupTargetRef);
    }

    /// <summary>
    /// The <c>.model</c> carries a property type as text, so the shape has to be read off the
    /// string. Mirrors what <c>MappingSourceGenerator.AnalyzeType</c> derives from a symbol:
    /// <c>List&lt;T&gt;</c> is a list, a trailing <c>?</c> is <see cref="Nullable{T}"/>, and the
    /// inner name is the type with that wrapper removed.
    ///
    /// Whether the type is an enum cannot be read from the text — <c>ContactStatus</c> looks like
    /// any other identifier — so it is decided by the column: an option-set column whose property
    /// is not a known primitive maps to the enum the table generator emits for it.
    /// </summary>
    private static void AnalyzeTypeName(
        string rawTypeName,
        Column column,
        out string typeName,
        out string innerTypeName,
        out bool isNullable,
        out bool isEnum,
        out bool isList,
        out string? listElemTypeName)
    {
        typeName = (rawTypeName ?? "string").Trim();
        innerTypeName = typeName;
        listElemTypeName = null;
        isNullable = false;
        isList = false;

        if (typeName.StartsWith("List<", StringComparison.Ordinal) && typeName.EndsWith(">", StringComparison.Ordinal))
        {
            isList = true;
            listElemTypeName = typeName.Substring(5, typeName.Length - 6).Trim();
            isEnum = IsEnumType(listElemTypeName, column);
            return;
        }

        if (typeName.EndsWith("?", StringComparison.Ordinal))
        {
            isNullable = true;
            innerTypeName = typeName.Substring(0, typeName.Length - 1).Trim();
        }

        isEnum = IsEnumType(innerTypeName, column);
    }

    private static bool IsEnumType(string typeName, Column column)
    {
        if (!IsOptionSet(column.Type) && !column.IsMultiSelect)
        {
            return false;
        }

        var leaf = typeName.Substring(typeName.LastIndexOf('.') + 1);

        return !PrimitiveTypeNames.Contains(leaf);
    }
}

/// <summary>
/// Something a <c>.model</c> asked for that its <c>.table</c> does not allow, carried out of the
/// factory so the generator can turn it into a diagnostic at the right place.
/// </summary>
internal sealed class MappingFailure
{
    public string Id { get; }
    public string ModelName { get; }
    public string PropertyName { get; }
    public string Detail { get; }

    private MappingFailure(string id, string modelName, string propertyName, string detail)
    {
        Id = id;
        ModelName = modelName;
        PropertyName = propertyName;
        Detail = detail;
    }

    public static MappingFailure UnknownColumn(string model, string property, string column, string table)
        => new(MappingFailureIds.UnknownColumn, model, property,
               $"column '{column}' is not declared by table '{table}'");

    public static MappingFailure ColumnNotSelected(string model, string property, string column, string table)
        => new(MappingFailureIds.ColumnNotSelected, model, property,
               $"column '{column}' of table '{table}' is not selected, so no definition constant is generated for it");

    public static MappingFailure ExtensionWithoutType(string model, string property)
        => new(MappingFailureIds.Extension, model, property,
               "is an ExtendBindingModel property but names no model in its Type");

    public static MappingFailure UnknownExtensionModel(string model, string property, string typeName)
        => new(MappingFailureIds.Extension, model, property,
               $"extends '{typeName}', which no .model file declares");

    public static MappingFailure ExtensionOnAnotherTable(
        string model, string property, string extended, string extendedTable, string ownTable)
        => new(MappingFailureIds.Extension, model, property,
               $"extends '{extended}', which targets table '{extendedTable}' instead of '{ownTable}'; "
               + "an extension is filled from the same record");

    public static MappingFailure AmbiguousLookupTarget(
        string model, string property, string column, System.Collections.Generic.IEnumerable<string> candidates)
        => new(MappingFailureIds.AmbiguousLookupTarget, model, property,
               $"lookup column '{column}' reaches several tables ({string.Join(", ", candidates)}); "
               + "set LookupTargetTableLogicalName to say which one this property maps");

    public static MappingFailure UnknownLookupTarget(
        string model, string property, string requested, string column, System.Collections.Generic.IEnumerable<string> candidates)
        => new(MappingFailureIds.AmbiguousLookupTarget, model, property,
               $"LookupTargetTableLogicalName is '{requested}', which lookup column '{column}' does not reach "
               + $"(it reaches {string.Join(", ", candidates)})");
    public static MappingFailure IncompatibleType(
        string model, string property, string declaredType, string column, AttributeTypeCode columnType, string expected)
        => new(MappingFailureIds.IncompatibleType, model, property,
               $"declared as '{declaredType}' but column '{column}' is a {columnType}; expected {expected}");

    public static MappingFailure LookupWithoutRelationship(string model, string property, string column, string table)
        => new(MappingFailureIds.LookupWithoutRelationship, model, property,
               $"lookup column '{column}' has no many-to-one relationship in table '{table}'");

    public static MappingFailure RelationshipNotAList(string model, string property, string declaredType)
        => new(MappingFailureIds.RelationshipNotAList, model, property,
               $"is a one-to-many relationship and must be declared as 'List<T>', not '{declaredType}'");
}

internal static class MappingFailureIds
{
    public const string UnknownColumn = "XRM1006";
    public const string ColumnNotSelected = "XRM1006";
    public const string RelationshipNotAList = "XRM1006";
    public const string LookupWithoutRelationship = "XRM1007";
    public const string IncompatibleType = "XRM1009";
    public const string AmbiguousLookupTarget = "XRM1010";
    public const string Extension = "XRM1011";
}
