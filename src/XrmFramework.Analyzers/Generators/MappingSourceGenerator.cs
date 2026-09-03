// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#nullable enable

using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Model.Sdk;
using XrmFramework.Analyzers.Generators.Mapping;

namespace XrmFramework.Analyzers.Generators;

/// <summary>
/// Generates explicit, debuggable <c>ToBindingModel(Entity)</c> and <c>ToEntity(IOrganizationService?)</c>
/// partial methods for every non-abstract <c>partial</c> class decorated with <c>[CrmEntity]</c>
/// that implements <c>IBindingModel</c>.
///
/// Each property mapping becomes a visible, breakpoint-able line in the generated file,
/// eliminating the need to step through reflection-based mappers during debugging.
/// </summary>
[Generator]
public sealed class MappingSourceGenerator : IIncrementalGenerator
{
    private const string CrmEntityShort = "CrmEntity";
    private const string CrmEntityFull  = "CrmEntityAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.SyntaxProvider
            .CreateSyntaxProvider(CouldBeBindingModel, ExtractModelInfo)
            .Where(static m => m is not null)
            .Collect();

        context.RegisterSourceOutput(models, EmitSources);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  1. Syntax filter  (fast, no semantic model)
    // ─────────────────────────────────────────────────────────────────────────

    private static bool CouldBeBindingModel(SyntaxNode node, CancellationToken _)
    {
        if (node is not ClassDeclarationSyntax classDecl)
            return false;

        var modifiers = classDecl.Modifiers;
        if (!modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
            return false;

        if (modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword)))
            return false;

        return classDecl.AttributeLists
            .SelectMany(al => al.Attributes)
            .Any(a => GetLeafName(a.Name) is CrmEntityShort or CrmEntityFull);
    }

    private static string? GetLeafName(NameSyntax? name) => name switch
    {
        IdentifierNameSyntax ins           => ins.Identifier.Text,
        QualifiedNameSyntax { Right: var r } => GetLeafName(r),
        _                                   => null,
    };

    // ─────────────────────────────────────────────────────────────────────────
    //  2. Semantic transform  (extracts plain string data for caching)
    // ─────────────────────────────────────────────────────────────────────────

    private static MappingModel? ExtractModelInfo(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        var classDecl = (ClassDeclarationSyntax)ctx.Node;

        if (ctx.SemanticModel.GetDeclaredSymbol(classDecl, ct) is not INamedTypeSymbol symbol)
            return null;

        if (symbol.AllInterfaces.All(i => i.Name != "IBindingModel"))
            return null;

        // [CrmEntity("contact")] – search on the class and its interfaces
        var crmEntityAttr = FindAttribute(symbol, "CrmEntityAttribute");
        if (crmEntityAttr == null)
            return null;

        if (crmEntityAttr.ConstructorArguments.FirstOrDefault().Value is not string entityName)
            return null;

        // Preserve the source expression so generated code references the constant
        // (e.g. "ContactDefinition.EntityName") instead of a bare string literal.
        var entityNameRef = GetArgText(crmEntityAttr, 0, ct) ?? $"\"{entityName}\"";

        var ns              = symbol.ContainingNamespace.IsGlobalNamespace ? null : symbol.ContainingNamespace.ToDisplayString();
        var isBindingBase   = HasAncestorNamed(symbol, "BindingModelBase");

        var properties = ImmutableArray.CreateBuilder<MappingProperty>();
        var extensions = ImmutableArray.CreateBuilder<MappingExtension>();

        CollectMappings(symbol, ctx.SemanticModel, ct, properties, extensions);

        return new MappingModel(symbol.Name, ns, entityNameRef, isBindingBase,
                             properties.ToImmutable(), extensions.ToImmutable());
    }

    private static AttributeData? FindAttribute(INamedTypeSymbol symbol, string attrClassName)
    {
        var found = symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == attrClassName);
        if (found != null) return found;

        foreach (var iface in symbol.AllInterfaces)
        {
            found = iface.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == attrClassName);
            if (found != null) return found;
        }

        return null;
    }

    private static bool HasAncestorNamed(INamedTypeSymbol symbol, string name)
    {
        for (var t = symbol.BaseType; t != null; t = t.BaseType)
            if (t.Name == name) return true;
        return false;
    }

    /// <summary>Returns the source text of positional attribute argument <paramref name="index"/>.</summary>
    private static string? GetArgText(AttributeData attr, int index, CancellationToken ct)
    {
        if (attr.ApplicationSyntaxReference?.GetSyntax(ct) is AttributeSyntax syn)
        {
            var args = syn.ArgumentList?.Arguments;
            if (args is { Count: > 0 } list && list.Count > index)
                return list[index].Expression.ToString();
        }
        return null;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Property collection
    // ─────────────────────────────────────────────────────────────────────────

    private static void CollectMappings(
        INamedTypeSymbol root,
        SemanticModel sem,
        CancellationToken ct,
        ImmutableArray<MappingProperty>.Builder props,
        ImmutableArray<MappingExtension>.Builder exts)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);

        // Walk the inheritance chain so we also pick up base-class properties.
        for (INamedTypeSymbol? type = root;
             type is { SpecialType: SpecialType.None };
             type = type.BaseType)
        {
            foreach (var member in type.GetMembers().OfType<IPropertySymbol>())
            {
                if (!seen.Add(member.Name)) continue;

                var attrs = member.GetAttributes();

                if (attrs.Any(a => a.AttributeClass?.Name == "ExtendBindingModelAttribute"))
                {
                    exts.Add(new MappingExtension(
                        member.Name,
                        member.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
                    continue;
                }

                var mappingAttr = attrs.FirstOrDefault(a => a.AttributeClass?.Name == "CrmMappingAttribute");
                if (mappingAttr is null) continue;

                var info = BuildPropInfo(member, mappingAttr, sem, ct);
                if (info is not null) props.Add(info);
            }
        }
    }

    private static MappingProperty? BuildPropInfo(
        IPropertySymbol prop,
        AttributeData  mappingAttr,
        SemanticModel  sem,
        CancellationToken ct)
    {
        // ── Column reference ──────────────────────────────────────────────────
        // Prefer the source expression (ContactDefinition.Columns.FullName) so the
        // generated code is readable; fall back to a string literal if unavailable.
        var columnRef = GetArgText(mappingAttr, 0, ct)
                        ?? $"\"{mappingAttr.ConstructorArguments[0].Value}\"";

        // ── IsValidForUpdate ──────────────────────────────────────────────────
        var isValidForUpdate = true;
        foreach (var na in mappingAttr.NamedArguments)
            if (na.Key == "IsValidForUpdate" && na.Value.Value is bool b) { isValidForUpdate = b; break; }

        // ── Resolve column field -> [AttributeMetadata] + [CrmLookup] ─────────
        IFieldSymbol? columnField = null;
        if (mappingAttr.ApplicationSyntaxReference?.GetSyntax(ct) is AttributeSyntax attrSyn)
        {
            var argExpr = attrSyn.ArgumentList?.Arguments.FirstOrDefault()?.Expression;
            if (argExpr is not null)
                columnField = sem.GetSymbolInfo(argExpr, ct).Symbol as IFieldSymbol;
        }

        var attrTypeCode    = ReadAttributeTypeCode(columnField);
        var lookupTargetRef = ReadLookupTargetRef(columnField, ct);

        // ── Property type analysis ────────────────────────────────────────────
        AnalyzeType(prop.Type,
            out var typeName, out var innerTypeName,
            out var isNullable, out var isEnum,
            out var isList,    out var listElemTypeName);

        var hasSetter = prop.SetMethod is not null;

        return new MappingProperty(
            prop.Name, typeName, innerTypeName,
            isNullable, isEnum, isList, listElemTypeName,
            hasSetter, columnRef, attrTypeCode,
            isValidForUpdate, lookupTargetRef);
    }

    private static AttributeTypeCode ReadAttributeTypeCode(IFieldSymbol? field)
    {
        if (field is null) return AttributeTypeCode.String;

        var meta = field.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "AttributeMetadataAttribute");

        if (meta is not null && meta.ConstructorArguments.Length > 0)
            return (AttributeTypeCode)(int)meta.ConstructorArguments[0].Value!;

        return AttributeTypeCode.String;
    }

    private static string? ReadLookupTargetRef(IFieldSymbol? field, CancellationToken ct)
    {
        if (field is null) return null;

        var lookupAttr = field.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "CrmLookupAttribute");
        if (lookupAttr is null) return null;

        // Try to use the source expression (e.g. "AccountDefinition.EntityName")
        if (lookupAttr.ApplicationSyntaxReference?.GetSyntax(ct) is AttributeSyntax syn)
        {
            var args = syn.ArgumentList?.Arguments;
            if (args is { Count: > 0 } list)
                return list[0].Expression.ToString();
        }

        var val = lookupAttr.ConstructorArguments.FirstOrDefault().Value;
        return val is null ? null : $"\"{val}\"";
    }

    private static void AnalyzeType(
        ITypeSymbol    type,
        out string     typeName,
        out string     innerTypeName,
        out bool       isNullable,
        out bool       isEnum,
        out bool       isList,
        out string?    listElemTypeName)
    {
        typeName         = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        listElemTypeName = null;
        isList           = false;
        isNullable       = false;
        isEnum           = type.TypeKind == TypeKind.Enum;
        innerTypeName    = typeName;

        // Nullable<T>
        if (type is INamedTypeSymbol { Name: "Nullable" } nullableType
            && nullableType.ContainingNamespace?.Name == "System"
            && nullableType.TypeArguments.Length == 1)
        {
            isNullable    = true;
            var inner     = nullableType.TypeArguments[0];
            innerTypeName = inner.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            isEnum        = inner.TypeKind == TypeKind.Enum;
            return;
        }

        // List<T>
        if (type is INamedTypeSymbol { Name: "List" } listType
            && listType.TypeArguments.Length == 1)
        {
            isList           = true;
            var elem         = listType.TypeArguments[0];
            listElemTypeName = elem.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            innerTypeName    = typeName;
            isEnum           = elem.TypeKind == TypeKind.Enum;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  3. Code emission
    // ─────────────────────────────────────────────────────────────────────────

    private static readonly DiagnosticDescriptor Xrm2001 = new(
        "XRM2001", "MappingGenerator failure",
        "Could not generate mapping for '{0}': {1}",
        "XrmFramework.Generators",
        DiagnosticSeverity.Warning, isEnabledByDefault: true,
        helpLinkUri: DiagnosticIds.HelpLink("XRM2001"));

    private static void EmitSources(SourceProductionContext ctx, ImmutableArray<MappingModel?> models)
    {
        foreach (var model in models)
        {
            if (model is null) continue;
            try
            {
                var code = MappingEmitter.Generate(model);
                var hint = model.Namespace is null
                    ? model.ClassName
                    : $"{model.Namespace}.{model.ClassName}";

                ctx.AddSource($"{hint}.Mapping.g.cs", code);
            }
            catch (Exception e)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(Xrm2001, null, model.ClassName, e.Message));
            }
        }
    }

}
