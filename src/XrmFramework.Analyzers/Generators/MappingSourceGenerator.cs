// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#nullable enable

using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Model.Sdk;

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

    private static ModelInfo? ExtractModelInfo(GeneratorSyntaxContext ctx, CancellationToken ct)
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

        var properties = ImmutableArray.CreateBuilder<PropInfo>();
        var extensions = ImmutableArray.CreateBuilder<ExtInfo>();

        CollectMappings(symbol, ctx.SemanticModel, ct, properties, extensions);

        return new ModelInfo(symbol.Name, ns, entityNameRef, isBindingBase,
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
        ImmutableArray<PropInfo>.Builder props,
        ImmutableArray<ExtInfo>.Builder exts)
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
                    exts.Add(new ExtInfo(
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

    private static PropInfo? BuildPropInfo(
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

        // ── Resolve column field → [AttributeMetadata] + [CrmLookup] ─────────
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

        return new PropInfo(
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

    private static void EmitSources(SourceProductionContext ctx, ImmutableArray<ModelInfo?> models)
    {
        foreach (var model in models)
        {
            if (model is null) continue;
            try
            {
                var code = GenerateCode(model);
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

    // ─────────────────────────────────────────────────────────────────────────
    //  Code generation
    // ─────────────────────────────────────────────────────────────────────────

    private static string GenerateCode(ModelInfo model)
    {
        var sb = new CodeWriter();

        sb.Line("// <auto-generated />");
        sb.Line();
        sb.Line("using System;");
        sb.Line("using System.CodeDom.Compiler;");
        sb.Line("using System.Collections.Generic;");
        sb.Line("using System.Diagnostics.CodeAnalysis;");
        sb.Line("using System.Linq;");
        sb.Line("using Microsoft.Xrm.Sdk;");
        sb.Line("using XrmFramework;");
        sb.Line("using XrmFramework.BindingModel;");
        sb.Line();

        if (model.Namespace is not null)
        {
            sb.Line($"namespace {model.Namespace}");
            sb.OpenBrace();
        }

        sb.Line("[GeneratedCode(\"XrmFramework.MappingGenerator\", \"1.0\")]");
        sb.Line("[ExcludeFromCodeCoverage]");
        sb.Line($"partial class {model.ClassName}");
        sb.OpenBrace();

        WriteToBindingModelMethod(sb, model);
        sb.Line();
        WriteToEntityMethod(sb, model);

        sb.CloseBrace(); // class

        if (model.Namespace is not null)
            sb.CloseBrace(); // namespace

        return sb.ToString();
    }

    // ── ToBindingModel ────────────────────────────────────────────────────────

    private static void WriteToBindingModelMethod(CodeWriter sb, ModelInfo model)
    {
        sb.Line($"public static {model.ClassName} ToBindingModel(Entity entity)");
        sb.OpenBrace();

        sb.Line("if (entity == null)");
        sb.Indent(); sb.Line("return null;"); sb.Dedent();
        sb.Line();
        sb.Line($"if (entity.LogicalName != {model.EntityNameRef})");
        sb.Indent(); sb.Line("return null;"); sb.Dedent();
        sb.Line();
        sb.Line($"var model = new {model.ClassName}();");
        sb.Line("model.Id = entity.Id;");

        foreach (var ext in model.Extensions)
        {
            sb.Line();
            sb.Line($"// {ext.Name} — extension of the same entity");
            sb.Line($"model.{ext.Name} = {ext.TypeName}.ToBindingModel(entity);");
        }

        foreach (var prop in model.Properties)
        {
            sb.Line();
            WriteReadFromEntity(sb, prop);
        }

        sb.Line();
        sb.Line("return model;");
        sb.CloseBrace();
    }

    private static void WriteReadFromEntity(CodeWriter sb, PropInfo prop)
    {
        var attrLabel = prop.IsList ? "MultiSelectPicklist" : prop.AttrType.ToString();
        sb.Line($"// {prop.Name} ({attrLabel})");

        if (prop.IsList)
        {
            WriteListRead(sb, prop);
            return;
        }

        sb.Line($"if (entity.Contains({prop.ColumnRef}))");
        sb.Indent();
        sb.Line($"model.{prop.Name} = {BuildReadExpr(prop)};");
        sb.Dedent();
    }

    private static void WriteListRead(CodeWriter sb, PropInfo prop)
    {
        var elemType = prop.ListElemTypeName ?? "object";
        sb.Line($"if (entity.Contains({prop.ColumnRef}))");
        sb.OpenBrace();
        sb.Line($"foreach (var item in entity.GetOptionSetValues<{elemType}>({prop.ColumnRef}))");
        sb.Indent(); sb.Line($"model.{prop.Name}.Add(item);"); sb.Dedent();
        sb.CloseBrace();
    }

    private static string BuildReadExpr(PropInfo prop) => prop.AttrType switch
    {
        AttributeTypeCode.Lookup   or
        AttributeTypeCode.Customer or
        AttributeTypeCode.Owner    => BuildLookupRead(prop),

        AttributeTypeCode.Money    => BuildMoneyRead(prop),

        AttributeTypeCode.Picklist or
        AttributeTypeCode.State    or
        AttributeTypeCode.Status   => BuildPicklistRead(prop),

        _ => $"entity.GetAttributeValue<{prop.TypeName}>({prop.ColumnRef})",
    };

    private static string BuildLookupRead(PropInfo prop)
    {
        if (prop.InnerTypeName == "Guid")
        {
            var fallback = prop.IsNullable ? "" : " ?? Guid.Empty";
            return $"entity.GetAttributeValue<EntityReference>({prop.ColumnRef})?.Id{fallback}";
        }

        return $"entity.GetAttributeValue<{prop.TypeName}>({prop.ColumnRef})";
    }

    private static string BuildMoneyRead(PropInfo prop)
    {
        if (prop.InnerTypeName is "decimal" || prop.TypeName is "decimal" or "decimal?")
        {
            var fallback = prop.IsNullable ? "" : " ?? default";
            return $"entity.GetAttributeValue<Money>({prop.ColumnRef})?.Value{fallback}";
        }

        return $"entity.GetAttributeValue<{prop.TypeName}>({prop.ColumnRef})";
    }

    private static string BuildPicklistRead(PropInfo prop)
    {
        if (prop.IsEnum)
        {
            var enumType = prop.IsNullable ? prop.InnerTypeName : prop.TypeName;
            return $"entity.GetOptionSetValue<{enumType}>({prop.ColumnRef})";
        }

        if (prop.InnerTypeName is "int" || prop.TypeName is "int" or "int?")
        {
            var fallback = prop.IsNullable ? "" : " ?? default";
            return $"entity.GetAttributeValue<OptionSetValue>({prop.ColumnRef})?.Value{fallback}";
        }

        return $"entity.GetAttributeValue<{prop.TypeName}>({prop.ColumnRef})";
    }

    // ── ToEntity ──────────────────────────────────────────────────────────────

    private static void WriteToEntityMethod(CodeWriter sb, ModelInfo model)
    {
        sb.Line("public Entity ToEntity(IOrganizationService service = null)");
        sb.OpenBrace();

        sb.Line($"var entity = new Entity({model.EntityNameRef}, Id);");

        foreach (var ext in model.Extensions)
        {
            sb.Line();
            sb.Line($"// {ext.Name} — extension of the same entity");
            sb.Line($"entity.MergeWith({ext.Name}?.ToEntity(service));");
        }

        foreach (var prop in model.Properties.Where(p => p.IsValidForUpdate))
        {
            sb.Line();
            WriteSetOnEntity(sb, prop, model.IsBindingModelBase);
        }

        sb.Line();
        sb.Line("return entity;");
        sb.CloseBrace();
    }

    private static void WriteSetOnEntity(CodeWriter sb, PropInfo prop, bool isBindingModelBase)
    {
        var attrLabel = prop.IsList ? "MultiSelectPicklist" : prop.AttrType.ToString();
        sb.Line($"// {prop.Name} ({attrLabel})");

        if (isBindingModelBase)
        {
            sb.Line($"if (InitializedProperties.Contains(nameof({prop.Name})))");
            sb.OpenBrace();
        }

        WriteEntityAssignment(sb, prop);

        if (isBindingModelBase)
            sb.CloseBrace();
    }

    private static void WriteEntityAssignment(CodeWriter sb, PropInfo prop)
    {
        if (prop.IsList)
        {
            sb.Line($"entity.SetOptionSetValues({prop.ColumnRef}, {prop.Name});");
            return;
        }

        switch (prop.AttrType)
        {
            case AttributeTypeCode.Lookup:
            case AttributeTypeCode.Customer:
            case AttributeTypeCode.Owner:
                WriteLookupAssignment(sb, prop);
                break;

            case AttributeTypeCode.Money:
                WriteMoneyAssignment(sb, prop);
                break;

            case AttributeTypeCode.Picklist:
            case AttributeTypeCode.State:
            case AttributeTypeCode.Status:
                WritePicklistAssignment(sb, prop);
                break;

            case AttributeTypeCode.DateTime:
                WriteDateTimeAssignment(sb, prop);
                break;

            default:
                sb.Line($"entity[{prop.ColumnRef}] = {prop.Name};");
                break;
        }
    }

    private static void WriteLookupAssignment(CodeWriter sb, PropInfo prop)
    {
        if (prop.InnerTypeName == "Guid")
        {
            var target = prop.LookupTargetRef ?? "\"unknown\"";
            if (prop.IsNullable)
            {
                sb.Line($"entity[{prop.ColumnRef}] = {prop.Name}.HasValue");
                sb.Indent();
                sb.Line($"? new EntityReference({target}, {prop.Name}.Value)");
                sb.Line(": null;");
                sb.Dedent();
            }
            else
            {
                sb.Line($"entity[{prop.ColumnRef}] = {prop.Name} != Guid.Empty");
                sb.Indent();
                sb.Line($"? new EntityReference({target}, {prop.Name})");
                sb.Line(": null;");
                sb.Dedent();
            }
        }
        else
        {
            sb.Line($"entity[{prop.ColumnRef}] = {prop.Name};");
        }
    }

    private static void WriteMoneyAssignment(CodeWriter sb, PropInfo prop)
    {
        if (prop.InnerTypeName is "decimal" || prop.TypeName is "decimal" or "decimal?")
        {
            sb.Line(prop.IsNullable
                ? $"entity[{prop.ColumnRef}] = {prop.Name}.HasValue ? new Money({prop.Name}.Value) : null;"
                : $"entity[{prop.ColumnRef}] = new Money({prop.Name});");
        }
        else
        {
            sb.Line($"entity[{prop.ColumnRef}] = {prop.Name};");
        }
    }

    private static void WritePicklistAssignment(CodeWriter sb, PropInfo prop)
    {
        if (prop.IsEnum)
        {
            sb.Line(prop.IsNullable
                ? $"entity[{prop.ColumnRef}] = {prop.Name}.HasValue ? new OptionSetValue((int){prop.Name}.Value) : null;"
                : $"entity[{prop.ColumnRef}] = {prop.Name} != default ? new OptionSetValue((int){prop.Name}) : null;");
        }
        else if (prop.InnerTypeName is "int" || prop.TypeName is "int" or "int?")
        {
            sb.Line(prop.IsNullable
                ? $"entity[{prop.ColumnRef}] = {prop.Name}.HasValue ? new OptionSetValue({prop.Name}.Value) : null;"
                : $"entity[{prop.ColumnRef}] = new OptionSetValue({prop.Name});");
        }
        else
        {
            sb.Line($"entity.SetOptionSetValue({prop.ColumnRef}, {prop.Name});");
        }
    }

    private static void WriteDateTimeAssignment(CodeWriter sb, PropInfo prop)
    {
        if (prop.IsNullable)
            sb.Line($"entity[{prop.ColumnRef}] = {prop.Name};");
        else
            sb.Line($"entity[{prop.ColumnRef}] = {prop.Name} != DateTime.MinValue ? {prop.Name} : (DateTime?)null;");
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Data classes  (plain classes – records need IsExternalInit unavailable
    //  in netstandard2.0 without a polyfill)
    // ─────────────────────────────────────────────────────────────────────────

    private sealed class ModelInfo
    {
        public string                   ClassName          { get; }
        public string?                  Namespace          { get; }
        public string                   EntityNameRef      { get; }
        public bool                     IsBindingModelBase { get; }
        public ImmutableArray<PropInfo> Properties         { get; }
        public ImmutableArray<ExtInfo>  Extensions         { get; }

        public ModelInfo(string className, string? ns, string entityNameRef, bool isBindingModelBase,
                         ImmutableArray<PropInfo> properties, ImmutableArray<ExtInfo> extensions)
        {
            ClassName          = className;
            Namespace          = ns;
            EntityNameRef      = entityNameRef;
            IsBindingModelBase = isBindingModelBase;
            Properties         = properties;
            Extensions         = extensions;
        }
    }

    private sealed class PropInfo
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

        public PropInfo(string name, string typeName, string innerTypeName,
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

    private sealed class ExtInfo
    {
        public string Name     { get; }
        public string TypeName { get; }

        public ExtInfo(string name, string typeName) { Name = name; TypeName = typeName; }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Minimal indented string builder (local, so no external dependency)
    // ─────────────────────────────────────────────────────────────────────────

    private sealed class CodeWriter
    {
        private readonly StringBuilder _sb     = new();
        private int                    _indent;
        private bool                   _pendingIndent = true;

        public void Line(string text = "")
        {
            if (text.Length > 0) DoIndent();
            _sb.AppendLine(text);
            _pendingIndent = true;
        }

        public void Indent()  => _indent++;
        public void Dedent()  => _indent = Math.Max(0, _indent - 1);

        public void OpenBrace()  { Line("{"); Indent(); }
        public void CloseBrace() { Dedent(); Line("}"); }

        private void DoIndent()
        {
            if (_pendingIndent && _indent > 0)
                _sb.Append(new string(' ', _indent * 4));
            _pendingIndent = false;
        }

        public override string ToString() => _sb.ToString();
    }
}
