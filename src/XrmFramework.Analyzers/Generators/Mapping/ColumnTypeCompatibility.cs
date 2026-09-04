// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Model.Sdk;
using XrmFramework.Core;

namespace XrmFramework.Analyzers.Generators.Mapping;

/// <summary>
/// Checks the C# type a <c>.model</c> gives a property against the type of the column it maps to.
///
/// <see cref="MappingEmitter"/> branches on the property's type name, so a type that does not
/// match the column does not fail — it falls through to the generic path and produces code that
/// compiles and is wrong. An <c>int</c> on a <c>Money</c> column emits
/// <c>entity.GetAttributeValue&lt;int&gt;(…)</c>, which reads zero forever because the attribute
/// holds a <see cref="Microsoft.Xrm.Sdk.Money"/>.
/// </summary>
/// <remarks>
/// Reported as a warning rather than an error. The accepted sets below cover what the emitter
/// actually special-cases plus the natural type of each column, but a project may have a
/// legitimate mapping this does not know about, and a false positive on an error would stop a
/// build over a judgement call.
/// </remarks>
internal static class ColumnTypeCompatibility
{
    /// <summary>C# types that are never a generated option-set enum.</summary>
    private static readonly HashSet<string> KnownPrimitives = new(StringComparer.Ordinal)
    {
        "bool", "Boolean", "byte", "sbyte", "char", "decimal", "Decimal", "double", "Double",
        "float", "Single", "int", "Int32", "uint", "long", "Int64", "ulong", "short", "Int16",
        "ushort", "object", "string", "String", "Guid", "DateTime", "OptionSetValue",
        "EntityReference", "Money"
    };

    /// <summary>
    /// The types each column kind accepts. A column kind absent from this map is not checked —
    /// <c>PartyList</c>, <c>CalendarRules</c>, <c>ManagedProperty</c> and a non-multi-select
    /// <c>Virtual</c> have no single natural mapping.
    /// </summary>
    private static readonly Dictionary<AttributeTypeCode, string[]> Accepted = new()
    {
        [AttributeTypeCode.Money] = new[] { "decimal", "Decimal", "Money" },
        [AttributeTypeCode.Lookup] = new[] { "Guid", "EntityReference" },
        [AttributeTypeCode.Customer] = new[] { "Guid", "EntityReference" },
        [AttributeTypeCode.Owner] = new[] { "Guid", "EntityReference" },
        [AttributeTypeCode.DateTime] = new[] { "DateTime" },
        [AttributeTypeCode.Boolean] = new[] { "bool", "Boolean" },
        [AttributeTypeCode.Integer] = new[] { "int", "Int32" },
        [AttributeTypeCode.BigInt] = new[] { "long", "Int64" },
        [AttributeTypeCode.Double] = new[] { "double", "Double" },
        [AttributeTypeCode.Decimal] = new[] { "decimal", "Decimal" },
        [AttributeTypeCode.String] = new[] { "string", "String" },
        [AttributeTypeCode.Memo] = new[] { "string", "String" },
        [AttributeTypeCode.Uniqueidentifier] = new[] { "Guid" },
    };

    /// <summary>Option-set columns also accept the enum the table generator emits for them.</summary>
    private static bool IsOptionSet(AttributeTypeCode type)
        => type is AttributeTypeCode.Picklist or AttributeTypeCode.State or AttributeTypeCode.Status;

    /// <summary>
    /// Returns the expected types when <paramref name="declaredType"/> does not fit the column,
    /// or <see langword="null"/> when it does — or when the column kind is not checked.
    /// </summary>
    public static string? Describe(Column column, string declaredType, bool isList, string? listElementType)
    {
        // A list is a multi-select: what matters is the element type, and it has to be an enum.
        if (isList)
        {
            var element = Leaf(listElementType ?? string.Empty);

            if (element.Length == 0 || KnownPrimitives.Contains(element))
            {
                return column.IsMultiSelect || IsOptionSet(column.Type)
                    ? "a generated option set enum"
                    : null;
            }

            return null;
        }

        var declared = Leaf(declaredType);

        if (declared.Length == 0) return null;

        if (column.IsMultiSelect)
        {
            // A multi-select column read into a single value loses every option but one.
            return $"List<{(IsOptionSet(column.Type) ? "TheOptionSetEnum" : "T")}>";
        }

        if (IsOptionSet(column.Type))
        {
            var acceptsEnum = !KnownPrimitives.Contains(declared);

            return acceptsEnum || declared is "int" or "Int32" or "OptionSetValue"
                ? null
                : "the generated option set enum, int, or OptionSetValue";
        }

        if (!Accepted.TryGetValue(column.Type, out var accepted))
        {
            return null;
        }

        return accepted.Contains(declared, StringComparer.Ordinal)
            ? null
            : Humanize(accepted);
    }

    /// <summary>Strips a namespace qualifier and a trailing <c>?</c>.</summary>
    private static string Leaf(string typeName)
    {
        var text = typeName.Trim();

        if (text.EndsWith("?", StringComparison.Ordinal))
            text = text.Substring(0, text.Length - 1).Trim();

        return text.Substring(text.LastIndexOf('.') + 1);
    }

    private static string Humanize(string[] accepted)
    {
        var distinct = accepted
            .GroupBy(a => a, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToArray();

        return distinct.Length == 1
            ? distinct[0]
            : string.Join(" or ", distinct);
    }
}
