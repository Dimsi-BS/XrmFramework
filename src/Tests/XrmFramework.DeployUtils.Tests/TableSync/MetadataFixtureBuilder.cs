// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using SdkLabel = Microsoft.Xrm.Sdk.Label;
using SdkLocalizedLabel = Microsoft.Xrm.Sdk.LocalizedLabel;

// Same precaution as in MetadataTableFactory: the XrmFramework namespace declares its own
// AttributeMetadata / EntityMetadata / OptionMetadata, and enclosing namespaces take precedence
// over using directives. The "Sdk" alias would likewise be captured, by XrmFramework.Sdk.
using DataverseMetadata = Microsoft.Xrm.Sdk.Metadata;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// Factory for Dataverse metadata used in tests.
/// </summary>
/// <remarks>
/// Most SDK properties only expose an <c>internal</c> write accessor: they are normally
/// populated by deserializing service responses. The tests therefore assign them via
/// reflection, which avoids having to have a real environment available.
/// </remarks>
internal static class MetadataFixtureBuilder
{
    /// <summary>
    /// Assigns a property regardless of its write accessor's access level, falling back
    /// to the backing field if the property doesn't declare a settable accessor.
    /// </summary>
    internal static T Set<T>(this T target, string propertyName, object value)
    {
        var type = target!.GetType();

        var setter = type.GetProperty(propertyName,
                             BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                         ?.GetSetMethod(nonPublic: true);

        if (setter != null)
        {
            setter.Invoke(target, new[] { value });
            return target;
        }

        var field = FindBackingField(type, propertyName)
                    ?? throw new InvalidOperationException(
                        $"Unable to assign \"{propertyName}\" on {type.Name}: " +
                        "neither a write accessor nor a backing field was found.");

        field.SetValue(target, value);
        return target;
    }

    private static FieldInfo? FindBackingField(Type type, string propertyName)
    {
        // SDK types sometimes use the auto-generated field "<Prop>k__BackingField",
        // sometimes a private field named "_prop".
        var candidates = new[]
        {
            $"<{propertyName}>k__BackingField",
            "_" + char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1),
            "_" + propertyName
        };

        for (var current = type; current != null; current = current.BaseType)
        {
            var field = candidates
                .Select(name => current.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance))
                .FirstOrDefault(f => f != null);

            if (field != null)
                return field;
        }

        return null;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Entities
    // ══════════════════════════════════════════════════════════════════════════

    internal static DataverseMetadata.EntityMetadata Entity(
        string logicalName,
        string schemaName,
        string primaryIdAttribute,
        string? primaryNameAttribute = null,
        params DataverseMetadata.AttributeMetadata[] attributes)
    {
        var entity = new DataverseMetadata.EntityMetadata()
            .Set(nameof(DataverseMetadata.EntityMetadata.LogicalName), logicalName)
            .Set(nameof(DataverseMetadata.EntityMetadata.SchemaName), schemaName)
            .Set(nameof(DataverseMetadata.EntityMetadata.LogicalCollectionName), logicalName + "s")
            .Set(nameof(DataverseMetadata.EntityMetadata.PrimaryIdAttribute), primaryIdAttribute)
            .Set(nameof(DataverseMetadata.EntityMetadata.IsCustomEntity), (bool?)true);

        if (primaryNameAttribute != null)
            entity.Set(nameof(DataverseMetadata.EntityMetadata.PrimaryNameAttribute), primaryNameAttribute);

        if (attributes.Length > 0)
            entity.Set(nameof(DataverseMetadata.EntityMetadata.Attributes), attributes);

        return entity;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Attributes
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Creates an attribute valid for create, read, and update.
    /// </summary>
    /// <remarks>
    /// <c>AttributeMetadata</c> is abstract on the SDK side: the concrete type is inferred from
    /// the requested <paramref name="type" />, so that the casts performed by the conversion
    /// (max length, bounds, date behavior) find what they expect.
    /// </remarks>
    internal static DataverseMetadata.AttributeMetadata Attribute(
        string logicalName,
        string schemaName,
        DataverseMetadata.AttributeTypeCode type,
        string? frenchLabel = null)
        => Configure(CreateConcrete(type), logicalName, schemaName, type, frenchLabel);

    /// <summary>
    /// Variant for tests that need to manipulate a specific concrete type (for example to
    /// populate an <c>OptionSet</c>).
    /// </summary>
    internal static T Attribute<T>(
        string logicalName,
        string schemaName,
        DataverseMetadata.AttributeTypeCode type,
        string? frenchLabel = null)
        where T : DataverseMetadata.AttributeMetadata, new()
        => (T)Configure(new T(), logicalName, schemaName, type, frenchLabel);

    private static DataverseMetadata.AttributeMetadata Configure(
        DataverseMetadata.AttributeMetadata attribute,
        string logicalName,
        string schemaName,
        DataverseMetadata.AttributeTypeCode type,
        string? frenchLabel)
        => attribute
            .Set(nameof(DataverseMetadata.AttributeMetadata.LogicalName), logicalName)
            .Set(nameof(DataverseMetadata.AttributeMetadata.SchemaName), schemaName)
            .Set(nameof(DataverseMetadata.AttributeMetadata.AttributeType),
                (DataverseMetadata.AttributeTypeCode?)type)
            .Set(nameof(DataverseMetadata.AttributeMetadata.IsValidForCreate), (bool?)true)
            .Set(nameof(DataverseMetadata.AttributeMetadata.IsValidForRead), (bool?)true)
            .Set(nameof(DataverseMetadata.AttributeMetadata.IsValidForUpdate), (bool?)true)
            .Set(nameof(DataverseMetadata.AttributeMetadata.DisplayName), Label(frenchLabel ?? schemaName));

    private static DataverseMetadata.AttributeMetadata CreateConcrete(
        DataverseMetadata.AttributeTypeCode type)
        => type switch
        {
            DataverseMetadata.AttributeTypeCode.String => new DataverseMetadata.StringAttributeMetadata(),
            DataverseMetadata.AttributeTypeCode.Memo => new DataverseMetadata.MemoAttributeMetadata(),
            DataverseMetadata.AttributeTypeCode.DateTime => new DataverseMetadata.DateTimeAttributeMetadata(),
            DataverseMetadata.AttributeTypeCode.Money => new DataverseMetadata.MoneyAttributeMetadata(),
            DataverseMetadata.AttributeTypeCode.Integer => new DataverseMetadata.IntegerAttributeMetadata(),
            DataverseMetadata.AttributeTypeCode.Double => new DataverseMetadata.DoubleAttributeMetadata(),
            DataverseMetadata.AttributeTypeCode.Decimal => new DataverseMetadata.DecimalAttributeMetadata(),
            DataverseMetadata.AttributeTypeCode.Boolean => new DataverseMetadata.BooleanAttributeMetadata(),
            DataverseMetadata.AttributeTypeCode.Picklist => new DataverseMetadata.PicklistAttributeMetadata(),
            DataverseMetadata.AttributeTypeCode.State => new DataverseMetadata.StateAttributeMetadata(),
            DataverseMetadata.AttributeTypeCode.Status => new DataverseMetadata.StatusAttributeMetadata(),
            DataverseMetadata.AttributeTypeCode.Lookup => new DataverseMetadata.LookupAttributeMetadata(),
            DataverseMetadata.AttributeTypeCode.Uniqueidentifier =>
                new DataverseMetadata.UniqueIdentifierAttributeMetadata(),
            DataverseMetadata.AttributeTypeCode.EntityName => new DataverseMetadata.EntityNameAttributeMetadata(),
            _ => new DataverseMetadata.StringAttributeMetadata()
        };

    internal static DataverseMetadata.AttributeMetadata StringAttribute(
        string logicalName, string schemaName, int maxLength)
        => Attribute<DataverseMetadata.StringAttributeMetadata>(
                logicalName, schemaName, DataverseMetadata.AttributeTypeCode.String)
            .Set(nameof(DataverseMetadata.StringAttributeMetadata.MaxLength), (int?)maxLength);

    // ══════════════════════════════════════════════════════════════════════════
    // Labels, option sets, keys
    // ══════════════════════════════════════════════════════════════════════════

    internal static SdkLabel Label(string text, int languageCode = 1036)
    {
        var localized = new SdkLocalizedLabel(text, languageCode);

        var label = new SdkLabel();
        label.LocalizedLabels.Add(localized);
        label.Set(nameof(SdkLabel.UserLocalizedLabel), localized);

        return label;
    }

    internal static DataverseMetadata.OptionSetMetadata OptionSet(
        string name,
        bool isGlobal,
        string displayLabel,
        params DataverseMetadata.OptionMetadata[] options)
    {
        var optionSet = new DataverseMetadata.OptionSetMetadata()
            .Set(nameof(DataverseMetadata.OptionSetMetadata.Name), name)
            .Set(nameof(DataverseMetadata.OptionSetMetadata.IsGlobal), (bool?)isGlobal)
            .Set(nameof(DataverseMetadata.OptionSetMetadata.DisplayName), Label(displayLabel));

        foreach (var option in options)
            optionSet.Options.Add(option);

        return optionSet;
    }

    internal static DataverseMetadata.OptionMetadata Option(int value, string label)
        => new DataverseMetadata.OptionMetadata(Label(label), value);

    internal static DataverseMetadata.EntityKeyMetadata Key(
        string logicalName, string displayLabel, params string[] fields)
        => new DataverseMetadata.EntityKeyMetadata()
            .Set(nameof(DataverseMetadata.EntityKeyMetadata.LogicalName), logicalName)
            .Set(nameof(DataverseMetadata.EntityKeyMetadata.KeyAttributes), fields)
            .Set(nameof(DataverseMetadata.EntityKeyMetadata.DisplayName), Label(displayLabel));
}
