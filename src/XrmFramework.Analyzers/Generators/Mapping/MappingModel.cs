// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#nullable enable

using System.Collections.Immutable;
using Model.Sdk;

namespace XrmFramework.Analyzers.Generators.Mapping;

/// <summary>
/// The neutral description <see cref="MappingEmitter"/> works from: a binding model reduced to
/// what emitting its mapping needs, with no Roslyn symbol left in it.
///
/// Two producers build it. <see cref="MappingSourceGenerator"/> reads a hand-written class through
/// the semantic model; <see cref="ModelSourceFileGenerator"/> reads a <c>.model</c> file and the
/// <c>.table</c> it names. The second cannot use symbols at all: the definition class it would
/// need is itself being generated in the same pass, and generators never see one another's output.
/// </summary>
internal sealed class MappingModel
{
    public string                   ClassName          { get; }
    public string?                  Namespace          { get; }
    public string                   EntityNameRef      { get; }
    public bool                     IsBindingModelBase { get; }
    public ImmutableArray<MappingProperty> Properties         { get; }
    public ImmutableArray<MappingExtension>  Extensions         { get; }
    public ImmutableArray<MappingRelationship> Relationships    { get; }

    /// <summary>
    /// Name of the generated definition class the table was named through, read lexically from
    /// <c>[CrmEntity(typeof(AccountDefinition))]</c>. Lets the tables be consulted for column
    /// metadata the semantic model cannot supply, the definition being generated in the same pass.
    /// </summary>
    public string? DefinitionName { get; set; }

    /// <summary>
    /// The table's alternate keys, each as the ordered logical names of its member columns, in the
    /// order the <c>.table</c> file declares them. <see cref="MappingModelFactory"/> reads this
    /// straight from the table; <see cref="MappingMetadataFallback"/> fills it in the same way for
    /// a hand-written class, whose own table is only known once the <c>.table</c> AdditionalFiles
    /// are read.
    /// </summary>
    public ImmutableArray<ImmutableArray<string>> AlternateKeys { get; set; } = ImmutableArray<ImmutableArray<string>>.Empty;

    public MappingModel(string className, string? ns, string entityNameRef, bool isBindingModelBase,
                     ImmutableArray<MappingProperty> properties, ImmutableArray<MappingExtension> extensions,
                     ImmutableArray<MappingRelationship> relationships = default)
    {
        ClassName          = className;
        Namespace          = ns;
        EntityNameRef      = entityNameRef;
        IsBindingModelBase = isBindingModelBase;
        Properties         = properties;
        Extensions         = extensions;
        Relationships      = relationships.IsDefault ? ImmutableArray<MappingRelationship>.Empty : relationships;
    }
}

internal sealed class MappingProperty
{
    public string            Name             { get; }
    public string            TypeName         { get; }
    public string            InnerTypeName    { get; }
    public bool              IsNullable       { get; }
    public bool              IsEnum           { get; }
    public bool              IsList           { get; }
    public string?           ListElemTypeName { get; }
    public bool              HasSetter        { get; }
    public string            ColumnRef        { get; }
    public AttributeTypeCode AttrType         { get; set; }
    public bool              IsValidForUpdate { get; }
    public string?           LookupTargetRef  { get; set; }

    /// <summary>
    ///     C# expression naming the aliased field a projection is read from, or
    ///     <see langword="null"/> for an ordinary property.
    ///
    ///     A property projecting a column of the record behind a lookup does not live in the
    ///     entity itself: the query builder adds a LinkEntity aliased on the lookup column, so
    ///     the value arrives under "&lt;lookup column&gt;.&lt;projected column&gt;". Reading the
    ///     lookup column instead hands back the EntityReference, and writing to it would put the
    ///     projected value where the reference belongs — a projection is read-only.
    /// </summary>
    public string? AliasedValueRef { get; set; }

    /// <summary>Definition class the column constant was written against, read lexically.</summary>
    public string? DefinitionName { get; set; }

    /// <summary>Leaf name of the column constant — <c>Name</c> in <c>AccountDefinition.Columns.Name</c>.</summary>
    public string? ColumnLeafName { get; set; }

    /// <summary>Whether <see cref="AttrType"/> came from a resolved symbol rather than the fallback.</summary>
    public bool MetadataResolved { get; set; }

    /// <summary>
    ///     Whether this property embeds another binding model behind a lookup, filled from the
    ///     record the lookup points at rather than from the lookup value itself — a
    ///     <c>LookupTargetModel</c> property. Read-only: <see cref="Name"/>'s type is that model's
    ///     class, so there is no value shaped like the column to write back.
    /// </summary>
    public bool IsEmbeddedLookupModel { get; set; }

    /// <summary>The embedded model's class name, e.g. <c>Contoso.Core.Model.AccountModel</c>.</summary>
    public string? EmbeddedModelTypeName { get; set; }

    /// <summary>
    ///     Whether the lookup column reaches more than one table. The join a query brings the
    ///     embedded columns back under is then aliased with the actual target's logical name as a
    ///     suffix, to keep one candidate's columns apart from another's.
    /// </summary>
    public bool EmbeddedIsPolymorphic { get; set; }

    /// <summary>
    ///     Schema name of the many-to-one relationship the lookup was resolved to, checked against
    ///     <c>RelatedEntities</c> when the query did not alias the embedded columns.
    /// </summary>
    public string? EmbeddedRelationshipName { get; set; }

    /// <summary>
    ///     Definition class the embedded model's own <c>[CrmEntity]</c> names, read lexically —
    ///     <c>AccountDefinition</c> for a property typed <c>AccountModel</c>. A hand-written
    ///     property carries no explicit target of its own: this is what says which of the lookup
    ///     column's relationships is the one to embed. Only needed to resolve
    ///     <see cref="EmbeddedIsPolymorphic"/>/<see cref="EmbeddedRelationshipName"/> from the
    ///     <c>.table</c> metadata; unused once those are set.
    /// </summary>
    public string? EmbeddedTargetDefinitionName { get; set; }

    public MappingProperty(string name, string typeName, string innerTypeName,
                    bool isNullable, bool isEnum, bool isList, string? listElemTypeName,
                    bool hasSetter, string columnRef, AttributeTypeCode attrType,
                    bool isValidForUpdate, string? lookupTargetRef)
    {
        Name             = name;
        TypeName         = typeName;
        InnerTypeName    = innerTypeName;
        IsNullable       = isNullable;
        IsEnum           = isEnum;
        IsList           = isList;
        ListElemTypeName = listElemTypeName;
        HasSetter        = hasSetter;
        ColumnRef        = columnRef;
        AttrType         = attrType;
        IsValidForUpdate = isValidForUpdate;
        LookupTargetRef  = lookupTargetRef;
    }
}

internal sealed class MappingExtension
{
    public string Name     { get; }
    public string TypeName { get; }

    public MappingExtension(string name, string typeName) { Name = name; TypeName = typeName; }
}

/// <summary>
/// A <c>[ChildRelationship]</c> property: a <c>List&lt;T&gt;</c> of another binding model,
/// populated from the entities a one-to-many relationship returns rather than from the entity's
/// own attribute bag. Maps no column of its own.
/// </summary>
internal sealed class MappingRelationship
{
    public string Name             { get; }
    public string ElementTypeName  { get; }

    /// <summary>C# expression naming the relationship's schema name, e.g. <c>AccountDefinition.OneToManyRelationships.contact_account</c>.</summary>
    public string RelationshipRef  { get; }
    public bool   IsValidForUpdate { get; }

    public MappingRelationship(string name, string elementTypeName, string relationshipRef, bool isValidForUpdate)
    {
        Name             = name;
        ElementTypeName  = elementTypeName;
        RelationshipRef  = relationshipRef;
        IsValidForUpdate = isValidForUpdate;
    }
}
