// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XrmFramework.PluginManifest.Generator
{
    /// <summary>
    /// Source generator qui analyse les plugins XrmFramework à la compilation et émet un
    /// manifeste de steps embarqué (<c>XrmFramework.Generated.PluginManifest.Json</c>).
    /// Le CLI net8.0 lit ce const sans instancier les types net462.
    /// </summary>
    /// <remarks>
    /// Lot 1 : couvre plugins + steps (+ attributs de méthode) + workflows. Les custom APIs
    /// sont émises vides pour l'instant (raffinement ultérieur).
    /// Implémentation volontairement basée sur <c>CompilationProvider</c> (simple et correcte) ;
    /// une optimisation en pipeline incrémental fin est possible plus tard.
    /// </remarks>
    [Generator(LanguageNames.CSharp)]
    public sealed class PluginManifestGenerator : IIncrementalGenerator
    {
        private const string PluginTypeName = "XrmFramework.Plugin";
        private const string CustomApiTypeName = "XrmFramework.CustomApi";
        private const string WorkflowTypeName = "XrmFramework.Workflow.CustomWorkflowActivity";

        // Enregistrement dynamique : le déploiement passe exclusivement par le manifeste,
        // donc un AddStep non analysable statiquement = step non déployable → erreur de build.
        private static readonly DiagnosticDescriptor NonStaticStep = new DiagnosticDescriptor(
            id: "XRMMAN001",
            title: "AddStep non analysable statiquement",
            messageFormat: "L'appel AddStep dans '{0}.AddSteps()' n'est pas analysable statiquement ({1}). Le déploiement XrmFramework lit le manifeste généré à la compilation : déclarez les steps via des appels AddStep directs à arguments constants (littéraux, enums, nameof).",
            category: "XrmFramework.Manifest",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        // [UnsecureConfig(ResourceType, PropertyName)] : config lue au runtime, non résoluble
        // statiquement → omise du manifeste. Avertissement (la fonctionnalité reste légitime).
        private static readonly DiagnosticDescriptor UnsecureConfigNotResolved = new DiagnosticDescriptor(
            id: "XRMMAN002",
            title: "Configuration de step non résoluble statiquement",
            messageFormat: "La configuration [UnsecureConfig(ResourceType, ...)] de '{0}' n'est pas résoluble statiquement ; elle sera absente du manifeste. Utilisez [UnsecureConfig(\"...\")] avec une chaîne littérale pour qu'elle soit déployée.",
            category: "XrmFramework.Manifest",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterSourceOutput(context.CompilationProvider, Execute);
        }

        private static void Execute(SourceProductionContext spc, Compilation compilation)
        {
            var pluginType = compilation.GetTypeByMetadataName(PluginTypeName);
            if (pluginType is null)
                return; // pas un projet plugin

            var customApiType = compilation.GetTypeByMetadataName(CustomApiTypeName);
            var workflowType = compilation.GetTypeByMetadataName(WorkflowTypeName);

            var plugins = new List<PluginInfo>();
            var workflows = new List<WorkflowInfo>();
            var customApis = new List<CustomApiInfo>();

            foreach (var type in EnumerateTypes(compilation.Assembly.GlobalNamespace))
            {
                if (type.IsAbstract || type.TypeKind != TypeKind.Class)
                    continue;
                if (type.DeclaredAccessibility != Accessibility.Public &&
                    type.DeclaredAccessibility != Accessibility.Internal)
                    continue;

                if (workflowType != null && InheritsFrom(type, workflowType))
                {
                    workflows.Add(ExtractWorkflow(compilation, type));
                    continue;
                }

                if (customApiType != null && InheritsFrom(type, customApiType))
                {
                    customApis.Add(ExtractCustomApi(type));
                    continue;
                }

                if (!InheritsFrom(type, pluginType))
                    continue;

                plugins.Add(ExtractPlugin(spc, compilation, type));
            }

            var json = BuildJson(plugins, workflows, customApis);
            spc.AddSource("PluginManifest.g.cs", BuildSource(json));
        }

        // ──────────────────────────────────────────────────────────────────────
        // Extraction d'un plugin
        // ──────────────────────────────────────────────────────────────────────

        private static PluginInfo ExtractPlugin(SourceProductionContext spc, Compilation compilation, INamedTypeSymbol type)
        {
            var steps = new List<StepInfo>();

            var addStepsMethod = FindMethod(type, "AddSteps");
            var syntaxRef = addStepsMethod?.DeclaringSyntaxReferences.FirstOrDefault();
            if (syntaxRef?.GetSyntax() is MethodDeclarationSyntax addStepsSyntax && addStepsSyntax.Body != null)
            {
                var model = compilation.GetSemanticModel(addStepsSyntax.SyntaxTree);

                foreach (var invocation in addStepsSyntax.Body.DescendantNodes().OfType<InvocationExpressionSyntax>())
                {
                    if (!IsCall(invocation, "AddStep"))
                        continue;

                    var step = ExtractStep(spc, model, type, invocation);
                    if (step != null)
                        steps.Add(step);
                }
            }

            return new PluginInfo(type.ToDisplayString(), steps);
        }

        private static StepInfo? ExtractStep(SourceProductionContext spc, SemanticModel model, INamedTypeSymbol pluginType, InvocationExpressionSyntax invocation)
        {
            var args = invocation.ArgumentList.Arguments;
            if (args.Count < 5)
                return null;

            // Garde-fou : AddStep dans une boucle / condition → non déroulable.
            if (IsInsideControlFlow(invocation))
            {
                spc.ReportDiagnostic(Diagnostic.Create(NonStaticStep, invocation.GetLocation(),
                    pluginType.Name, "appel dans une boucle/condition"));
                return null;
            }

            var stage = ResolveMemberName(model, args[0].Expression);
            var message = ResolveMemberName(model, args[1].Expression);
            var mode = ResolveMemberName(model, args[2].Expression);
            var entityName = ResolveConstString(model, args[3].Expression);
            var methodName = ResolveConstString(model, args[4].Expression);

            if (stage is null || message is null || mode is null || entityName is null || methodName is null)
            {
                spc.ReportDiagnostic(Diagnostic.Create(NonStaticStep, invocation.GetLocation(),
                    pluginType.Name, "argument non constant"));
                return null;
            }

            var columns = new List<string>();
            for (var i = 5; i < args.Count; i++)
            {
                var col = ResolveConstString(model, args[i].Expression);
                if (col != null) columns.Add(col);
            }

            var method = FindMethod(pluginType, methodName);
            if (method is null)
            {
                spc.ReportDiagnostic(Diagnostic.Create(NonStaticStep, invocation.GetLocation(),
                    pluginType.Name, $"méthode '{methodName}' introuvable"));
                return null;
            }

            return BuildStep(spc, message, stage, mode, entityName, methodName, columns, method);
        }

        private static StepInfo BuildStep(SourceProductionContext spc, string message, string stage, string mode,
            string entityName, string methodName, List<string> columns, IMethodSymbol method)
        {
            // Réplique la logique de Step.cs (fallback sur columns).
            var preImageAttr = GetAttribute(method, "XrmFramework.PreImageAttribute");
            var postImageAttr = GetAttribute(method, "XrmFramework.PostImageAttribute");
            var filteringAttr = GetAttribute(method, "XrmFramework.FilteringAttributesAttribute");
            var orderAttr = GetAttribute(method, "XrmFramework.ExecutionOrderAttribute");
            var impersonationAttr = GetAttribute(method, "XrmFramework.ImpersonationAttribute");
            var unsecureAttr = GetAttribute(method, "XrmFramework.UnsecureConfigAttribute");

            // Filtering : seulement pour le message Update.
            var filtering = new List<string>();
            if (message == "Update")
            {
                if (filteringAttr != null)
                    filtering.AddRange(GetStringArrayArg(filteringAttr));
                else
                    filtering.AddRange(columns);
            }

            var (preAll, preCols) = ReadImage(preImageAttr, columns);
            var (postAll, postCols) = ReadImage(postImageAttr, columns);

            var order = 1;
            if (orderAttr != null && orderAttr.ConstructorArguments.Length == 1 &&
                orderAttr.ConstructorArguments[0].Value is int o)
                order = o;

            var impersonation = "";
            if (impersonationAttr != null && impersonationAttr.ConstructorArguments.Length == 1 &&
                impersonationAttr.ConstructorArguments[0].Value is string u)
                impersonation = u;

            string? unsecureConfig = null;
            if (unsecureAttr != null)
            {
                // Ctor (string) → inline ; ctor (Type, string) → non résoluble statiquement.
                if (unsecureAttr.ConstructorArguments.Length == 1 &&
                    unsecureAttr.ConstructorArguments[0].Value is string cfg)
                    unsecureConfig = cfg;
                else
                    spc.ReportDiagnostic(Diagnostic.Create(UnsecureConfigNotResolved, method.Locations.FirstOrDefault(),
                        $"{method.ContainingType.Name}.{method.Name}"));
            }

            return new StepInfo
            {
                Message = message,
                Stage = stage,
                Mode = mode,
                EntityName = entityName,
                MethodName = methodName,
                MethodNames = new List<string> { methodName },
                FilteringAttributes = filtering,
                PreImageAll = preAll,
                PreImageAttributes = preCols,
                PostImageAll = postAll,
                PostImageAttributes = postCols,
                Order = order,
                ImpersonationUsername = impersonation,
                UnsecureConfig = unsecureConfig
            };
        }

        private static (bool all, List<string> cols) ReadImage(AttributeData? imageAttr, List<string> fallbackColumns)
        {
            if (imageAttr == null)
                return (false, new List<string>(fallbackColumns));

            // Ctor (bool allColumns) vs (params string[] columns).
            var ctorParams = imageAttr.AttributeConstructor?.Parameters;
            if (ctorParams != null && ctorParams.Value.Length == 1 &&
                ctorParams.Value[0].Type.SpecialType == SpecialType.System_Boolean)
            {
                var all = imageAttr.ConstructorArguments[0].Value is bool b && b;
                return (all, new List<string>());
            }

            return (false, GetStringArrayArg(imageAttr));
        }

        // ──────────────────────────────────────────────────────────────────────
        // Extraction d'un workflow
        // ──────────────────────────────────────────────────────────────────────

        private static WorkflowInfo ExtractWorkflow(Compilation compilation, INamedTypeSymbol type)
        {
            // DisplayName par défaut (comportement System.Activities) = nom court du type ;
            // surchargé si un SetDisplayName("...") littéral est présent dans un constructeur.
            var displayName = type.Name;

            foreach (var ctor in type.Constructors)
            {
                var syntaxRef = ctor.DeclaringSyntaxReferences.FirstOrDefault();
                if (syntaxRef?.GetSyntax() is not ConstructorDeclarationSyntax ctorSyntax || ctorSyntax.Body == null)
                    continue;

                var model = compilation.GetSemanticModel(ctorSyntax.SyntaxTree);
                foreach (var invocation in ctorSyntax.Body.DescendantNodes().OfType<InvocationExpressionSyntax>())
                {
                    if (!IsCall(invocation, "SetDisplayName") || invocation.ArgumentList.Arguments.Count < 1)
                        continue;

                    var literal = ResolveConstString(model, invocation.ArgumentList.Arguments[0].Expression);
                    if (literal != null)
                        displayName = literal;
                }
            }

            return new WorkflowInfo(type.ToDisplayString(), displayName);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Extraction d'un custom API
        // ──────────────────────────────────────────────────────────────────────

        private static CustomApiInfo ExtractCustomApi(INamedTypeSymbol type)
        {
            var attr = type.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == "XrmFramework.CustomApiAttribute");

            var name = GetNamedString(attr, "Name");
            var info = new CustomApiInfo
            {
                FullName = type.ToDisplayString(),
                Name = string.IsNullOrEmpty(name) ? type.Name : name!,
                DisplayName = GetNamedString(attr, "DisplayName"),
                Description = GetNamedString(attr, "Description"),
                BoundEntityLogicalName = GetNamedString(attr, "BoundEntityLogicalName"),
                ExecutePrivilegeName = GetNamedString(attr, "ExecutePrivilegeName"),
                IsFunction = GetNamedBool(attr, "IsFunction"),
                IsPrivate = GetNamedBool(attr, "IsPrivate"),
                WorkflowSdkStepEnabled = GetNamedBool(attr, "WorkflowSdkStepEnabled"),
                // BindingType est l'argument positionnel du constructeur.
                BindingType = attr != null && attr.ConstructorArguments.Length >= 1
                    ? EnumMemberName(attr.ConstructorArguments[0]) : null,
                AllowedCustomProcessing = GetNamedEnumName(attr, "AllowedCustomProcessing"),
            };

            foreach (var prop in EnumerateProperties(type))
            {
                var inAttr = GetPropertyAttribute(prop, "XrmFramework.CustomApiInputAttribute");
                var outAttr = GetPropertyAttribute(prop, "XrmFramework.CustomApiOutputAttribute");
                var argAttr = inAttr ?? outAttr;
                if (argAttr == null)
                    continue;

                // Type générique T de CustomApiIn/OutArgument<T>.
                var typeArg = (prop.Type as INamedTypeSymbol)?.TypeArguments.FirstOrDefault();

                info.Arguments.Add(new ArgInfo
                {
                    IsIn = inAttr != null,
                    Name = GetNamedString(argAttr, "Name") ?? prop.Name,
                    TypeFullName = typeArg?.ToDisplayString(FullyQualifiedNoGlobal) ?? "",
                    IsEnum = typeArg?.TypeKind == TypeKind.Enum,
                    DisplayName = GetNamedString(argAttr, "DisplayName"),
                    Description = GetNamedString(argAttr, "Description"),
                    LogicalEntityName = GetNamedString(argAttr, "LogicalEntityName"),
                    IsOptional = GetNamedBool(argAttr, "IsOptional"),
                });
            }

            return info;
        }

        private static IEnumerable<IPropertySymbol> EnumerateProperties(INamedTypeSymbol type)
        {
            for (var current = type; current != null; current = current.BaseType)
                foreach (var prop in current.GetMembers().OfType<IPropertySymbol>())
                    yield return prop;
        }

        private static AttributeData? GetPropertyAttribute(IPropertySymbol prop, string attributeFullName)
            => prop.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == attributeFullName);

        private static string? GetNamedString(AttributeData? attr, string name)
        {
            if (attr == null) return null;
            foreach (var na in attr.NamedArguments)
                if (na.Key == name) return na.Value.Value as string;
            return null;
        }

        private static bool GetNamedBool(AttributeData? attr, string name)
        {
            if (attr == null) return false;
            foreach (var na in attr.NamedArguments)
                if (na.Key == name && na.Value.Value is bool b) return b;
            return false;
        }

        private static string? GetNamedEnumName(AttributeData? attr, string name)
        {
            if (attr == null) return null;
            foreach (var na in attr.NamedArguments)
                if (na.Key == name) return EnumMemberName(na.Value);
            return null;
        }

        private static string? EnumMemberName(TypedConstant tc)
        {
            if (tc.Type is INamedTypeSymbol enumType && enumType.TypeKind == TypeKind.Enum)
            {
                foreach (var member in enumType.GetMembers().OfType<IFieldSymbol>())
                    if (member.HasConstantValue && Equals(member.ConstantValue, tc.Value))
                        return member.Name;
            }
            return tc.Value?.ToString();
        }

        // Noms de types pleinement qualifiés CLR (System.String, pas le mot-clé "string"),
        // attendus par le mapper côté DeployUtils (Lot 2).
        private static readonly SymbolDisplayFormat FullyQualifiedNoGlobal =
            SymbolDisplayFormat.FullyQualifiedFormat
                .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)
                .WithMiscellaneousOptions(
                    SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions
                    & ~SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        // ──────────────────────────────────────────────────────────────────────
        // Helpers Roslyn
        // ──────────────────────────────────────────────────────────────────────

        private static IEnumerable<INamedTypeSymbol> EnumerateTypes(INamespaceSymbol ns)
        {
            foreach (var member in ns.GetMembers())
            {
                if (member is INamespaceSymbol childNs)
                {
                    foreach (var t in EnumerateTypes(childNs))
                        yield return t;
                }
                else if (member is INamedTypeSymbol type)
                {
                    yield return type;
                    foreach (var nested in type.GetTypeMembers())
                        yield return nested;
                }
            }
        }

        private static bool InheritsFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
        {
            for (var current = type.BaseType; current != null; current = current.BaseType)
            {
                if (SymbolEqualityComparer.Default.Equals(current, baseType))
                    return true;
            }
            return false;
        }

        private static IMethodSymbol? FindMethod(INamedTypeSymbol type, string name)
        {
            for (var current = type; current != null; current = current.BaseType)
            {
                var method = current.GetMembers(name).OfType<IMethodSymbol>().FirstOrDefault();
                if (method != null)
                    return method;
            }
            return null;
        }

        private static bool IsCall(InvocationExpressionSyntax invocation, string methodName)
        {
            switch (invocation.Expression)
            {
                case IdentifierNameSyntax id:
                    return id.Identifier.Text == methodName;
                case MemberAccessExpressionSyntax member:
                    return member.Name.Identifier.Text == methodName;
                default:
                    return false;
            }
        }

        private static bool IsInsideControlFlow(SyntaxNode node)
        {
            for (var current = node.Parent; current != null; current = current.Parent)
            {
                switch (current)
                {
                    case MethodDeclarationSyntax:
                        return false; // remonté jusqu'à la méthode sans control flow
                    case ForStatementSyntax:
                    case ForEachStatementSyntax:
                    case WhileStatementSyntax:
                    case DoStatementSyntax:
                    case IfStatementSyntax:
                    case SwitchStatementSyntax:
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Résout le nom d'un membre statique référencé en argument : membre d'enum
        /// (Stages.PreOperation) OU propriété/champ statique de classe (Messages.Create,
        /// le type Messages étant une classe à membres statiques, pas un enum).
        /// </summary>
        private static string? ResolveMemberName(SemanticModel model, ExpressionSyntax expr)
        {
            var symbol = model.GetSymbolInfo(expr).Symbol;
            return symbol switch
            {
                IFieldSymbol field when field.IsStatic => field.Name,
                IPropertySymbol property when property.IsStatic => property.Name,
                _ => null,
            };
        }

        private static string? ResolveConstString(SemanticModel model, ExpressionSyntax expr)
        {
            var constant = model.GetConstantValue(expr);
            return constant.HasValue ? constant.Value as string : null;
        }

        private static AttributeData? GetAttribute(IMethodSymbol method, string attributeFullName)
        {
            return method.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == attributeFullName);
        }

        private static List<string> GetStringArrayArg(AttributeData attr)
        {
            var result = new List<string>();
            if (attr.ConstructorArguments.Length >= 1)
            {
                var arg = attr.ConstructorArguments[0];
                if (arg.Kind == TypedConstantKind.Array)
                {
                    foreach (var v in arg.Values)
                        if (v.Value is string s) result.Add(s);
                }
            }
            return result;
        }

        // ──────────────────────────────────────────────────────────────────────
        // Sérialisation JSON (à la main, sans dépendance)
        // ──────────────────────────────────────────────────────────────────────

        private static string BuildJson(List<PluginInfo> plugins, List<WorkflowInfo> workflows, List<CustomApiInfo> customApis)
        {
            var sb = new StringBuilder();
            sb.Append("{\"plugins\":[");
            for (var pi = 0; pi < plugins.Count; pi++)
            {
                var p = plugins[pi];
                if (pi > 0) sb.Append(',');
                sb.Append("{\"fullName\":").Append(JsonStr(p.FullName)).Append(",\"steps\":[");
                for (var si = 0; si < p.Steps.Count; si++)
                {
                    var s = p.Steps[si];
                    if (si > 0) sb.Append(',');
                    sb.Append("{\"message\":").Append(JsonStr(s.Message))
                      .Append(",\"stage\":").Append(JsonStr(s.Stage))
                      .Append(",\"mode\":").Append(JsonStr(s.Mode))
                      .Append(",\"entityName\":").Append(JsonStr(s.EntityName))
                      .Append(",\"methodName\":").Append(JsonStr(s.MethodName))
                      .Append(",\"methodNames\":").Append(JsonArr(s.MethodNames))
                      .Append(",\"filteringAttributes\":").Append(JsonArr(s.FilteringAttributes))
                      .Append(",\"order\":").Append(s.Order)
                      .Append(",\"impersonationUsername\":").Append(JsonStr(s.ImpersonationUsername))
                      .Append(",\"unsecureConfig\":").Append(s.UnsecureConfig == null ? "null" : JsonStr(s.UnsecureConfig))
                      .Append(",\"preImage\":{\"allAttributes\":").Append(s.PreImageAll ? "true" : "false")
                      .Append(",\"attributes\":").Append(JsonArr(s.PreImageAttributes)).Append('}')
                      .Append(",\"postImage\":{\"allAttributes\":").Append(s.PostImageAll ? "true" : "false")
                      .Append(",\"attributes\":").Append(JsonArr(s.PostImageAttributes)).Append('}')
                      .Append('}');
                }
                sb.Append("]}");
            }
            sb.Append("],\"workflows\":[");
            for (var wi = 0; wi < workflows.Count; wi++)
            {
                if (wi > 0) sb.Append(',');
                sb.Append("{\"fullName\":").Append(JsonStr(workflows[wi].FullName))
                  .Append(",\"displayName\":").Append(JsonStr(workflows[wi].DisplayName)).Append('}');
            }
            sb.Append("],\"customApis\":[");
            for (var ci = 0; ci < customApis.Count; ci++)
            {
                var c = customApis[ci];
                if (ci > 0) sb.Append(',');
                sb.Append("{\"fullName\":").Append(JsonStr(c.FullName))
                  .Append(",\"name\":").Append(JsonStr(c.Name))
                  .Append(",\"displayName\":").Append(JsonStrOrNull(c.DisplayName))
                  .Append(",\"description\":").Append(JsonStrOrNull(c.Description))
                  .Append(",\"bindingType\":").Append(JsonStrOrNull(c.BindingType))
                  .Append(",\"boundEntityLogicalName\":").Append(JsonStrOrNull(c.BoundEntityLogicalName))
                  .Append(",\"isFunction\":").Append(c.IsFunction ? "true" : "false")
                  .Append(",\"isPrivate\":").Append(c.IsPrivate ? "true" : "false")
                  .Append(",\"allowedCustomProcessing\":").Append(JsonStrOrNull(c.AllowedCustomProcessing))
                  .Append(",\"executePrivilegeName\":").Append(JsonStrOrNull(c.ExecutePrivilegeName))
                  .Append(",\"workflowSdkStepEnabled\":").Append(c.WorkflowSdkStepEnabled ? "true" : "false")
                  .Append(",\"arguments\":[");
                for (var ai = 0; ai < c.Arguments.Count; ai++)
                {
                    var a = c.Arguments[ai];
                    if (ai > 0) sb.Append(',');
                    sb.Append("{\"isInArgument\":").Append(a.IsIn ? "true" : "false")
                      .Append(",\"name\":").Append(JsonStr(a.Name))
                      .Append(",\"typeFullName\":").Append(JsonStr(a.TypeFullName))
                      .Append(",\"isEnum\":").Append(a.IsEnum ? "true" : "false")
                      .Append(",\"displayName\":").Append(JsonStrOrNull(a.DisplayName))
                      .Append(",\"description\":").Append(JsonStrOrNull(a.Description))
                      .Append(",\"logicalEntityName\":").Append(JsonStrOrNull(a.LogicalEntityName))
                      .Append(",\"isOptional\":").Append(a.IsOptional ? "true" : "false")
                      .Append('}');
                }
                sb.Append("]}");
            }
            sb.Append("]}");
            return sb.ToString();
        }

        private static string JsonStrOrNull(string? value) => value == null ? "null" : JsonStr(value);

        private static string JsonArr(IReadOnlyList<string> items)
        {
            var sb = new StringBuilder("[");
            for (var i = 0; i < items.Count; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append(JsonStr(items[i]));
            }
            sb.Append(']');
            return sb.ToString();
        }

        private static string JsonStr(string? value)
        {
            if (value == null) return "null";
            var sb = new StringBuilder("\"");
            foreach (var c in value)
            {
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 0x20) sb.Append("\\u").Append(((int)c).ToString("x4"));
                        else sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

        private static string BuildSource(string json)
        {
            // Le manifeste est encodé en Base64 pour éviter tout souci d'échappement dans le const.
            var bytes = Encoding.UTF8.GetBytes(json);
            var base64 = System.Convert.ToBase64String(bytes);

            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated/>");
            sb.AppendLine("#nullable enable");
            sb.AppendLine("namespace XrmFramework.Generated");
            sb.AppendLine("{");
            sb.AppendLine("    [System.CodeDom.Compiler.GeneratedCode(\"XrmFramework.PluginManifest.Generator\", \"1.0\")]");
            sb.AppendLine("    public static class PluginManifest");
            sb.AppendLine("    {");
            sb.AppendLine("        /// <summary>Manifeste JSON des composants déployables (UTF-8).</summary>");
            sb.Append("        public const string Json = ").Append(JsonStr(json)).AppendLine(";");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Manifeste JSON encodé en Base64 (robustesse de lecture cross-outillage).</summary>");
            sb.Append("        public const string JsonBase64 = \"").Append(base64).AppendLine("\";");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        // ──────────────────────────────────────────────────────────────────────
        // Modèles internes
        // ──────────────────────────────────────────────────────────────────────

        private sealed class PluginInfo
        {
            public PluginInfo(string fullName, List<StepInfo> steps) { FullName = fullName; Steps = steps; }
            public string FullName { get; }
            public List<StepInfo> Steps { get; }
        }

        private sealed class WorkflowInfo
        {
            public WorkflowInfo(string fullName, string displayName) { FullName = fullName; DisplayName = displayName; }
            public string FullName { get; }
            public string DisplayName { get; }
        }

        private sealed class CustomApiInfo
        {
            public string FullName = "";
            public string Name = "";
            public string? DisplayName;
            public string? Description;
            public string? BindingType;
            public string? BoundEntityLogicalName;
            public bool IsFunction;
            public bool IsPrivate;
            public string? AllowedCustomProcessing;
            public string? ExecutePrivilegeName;
            public bool WorkflowSdkStepEnabled;
            public List<ArgInfo> Arguments = new List<ArgInfo>();
        }

        private sealed class ArgInfo
        {
            public bool IsIn;
            public string Name = "";
            public string TypeFullName = "";
            public bool IsEnum;
            public string? DisplayName;
            public string? Description;
            public string? LogicalEntityName;
            public bool IsOptional;
        }

        private sealed class StepInfo
        {
            public string Message = "";
            public string Stage = "";
            public string Mode = "";
            public string EntityName = "";
            public string MethodName = "";
            public List<string> MethodNames = new List<string>();
            public List<string> FilteringAttributes = new List<string>();
            public bool PreImageAll;
            public List<string> PreImageAttributes = new List<string>();
            public bool PostImageAll;
            public List<string> PostImageAttributes = new List<string>();
            public int Order = 1;
            public string ImpersonationUsername = "";
            public string? UnsecureConfig;
        }
    }
}
