// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using SdkLabel = Microsoft.Xrm.Sdk.Label;
using SdkLocalizedLabel = Microsoft.Xrm.Sdk.LocalizedLabel;

// Même précaution que dans MetadataTableFactory : l'espace de noms XrmFramework déclare ses propres
// AttributeMetadata / EntityMetadata / OptionMetadata, et les espaces de noms englobants l'emportent
// sur les directives using. L'alias « Sdk » serait lui aussi capté, par XrmFramework.Sdk.
using DataverseMetadata = Microsoft.Xrm.Sdk.Metadata;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// Fabrique de métadonnées Dataverse pour les tests.
/// </summary>
/// <remarks>
/// La plupart des propriétés du SDK n'exposent qu'un accesseur d'écriture <c>internal</c> : elles
/// sont normalement alimentées par la désérialisation des réponses du service. Les tests les
/// affectent donc par réflexion, ce qui évite d'avoir à disposer d'un vrai environnement.
/// </remarks>
internal static class MetadataFixtureBuilder
{
    /// <summary>
    /// Affecte une propriété quel que soit le niveau d'accès de son accesseur d'écriture, en
    /// retombant sur le champ de stockage si la propriété n'en déclare aucun.
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
                        $"Impossible d'affecter « {propertyName} » sur {type.Name} : " +
                        "ni accesseur d'écriture ni champ de stockage trouvé.");

        field.SetValue(target, value);
        return target;
    }

    private static FieldInfo? FindBackingField(Type type, string propertyName)
    {
        // Les types du SDK utilisent tantôt le champ auto-généré « <Prop>k__BackingField »,
        // tantôt un champ privé nommé « _prop ».
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
    // Entités
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
    // Attributs
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Crée un attribut valide en création, lecture et mise à jour.
    /// </summary>
    /// <remarks>
    /// <c>AttributeMetadata</c> est abstraite côté SDK : le type concret est déduit du
    /// <paramref name="type" /> demandé, afin que les transtypages effectués par la conversion
    /// (longueur maximale, bornes, comportement de date) trouvent bien ce qu'ils attendent.
    /// </remarks>
    internal static DataverseMetadata.AttributeMetadata Attribute(
        string logicalName,
        string schemaName,
        DataverseMetadata.AttributeTypeCode type,
        string? frenchLabel = null)
        => Configure(CreateConcrete(type), logicalName, schemaName, type, frenchLabel);

    /// <summary>
    /// Variante pour les tests devant manipuler un type concret précis (par exemple pour
    /// renseigner un <c>OptionSet</c>).
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
    // Libellés, option sets, clés
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
