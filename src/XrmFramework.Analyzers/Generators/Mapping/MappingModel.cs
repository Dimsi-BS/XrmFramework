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

    public MappingModel(string className, string? ns, string entityNameRef, bool isBindingModelBase,
                     ImmutableArray<MappingProperty> properties, ImmutableArray<MappingExtension> extensions)
    {
        ClassName          = className;
        Namespace          = ns;
        EntityNameRef      = entityNameRef;
        IsBindingModelBase = isBindingModelBase;
        Properties         = properties;
        Extensions         = extensions;
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
    public AttributeTypeCode AttrType         { get; }
    public bool              IsValidForUpdate { get; }
    public string?           LookupTargetRef  { get; }

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
