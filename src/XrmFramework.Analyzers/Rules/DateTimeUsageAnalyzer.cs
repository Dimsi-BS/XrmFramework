// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

// ReSharper disable ArrangeObjectCreationWhenTypeEvident

namespace XrmFramework.Analyzers
{
    /// <summary>
    /// XRM0300 — Forbids the direct use of <c>DateTime.Now</c>, <c>DateTime.UtcNow</c>
    /// and <c>DateTime.Today</c> in classes that inherit from <c>XrmFramework.Plugin</c>
    /// or implement <c>Microsoft.Xrm.Sdk.IPlugin</c>.
    /// Recommends injecting <c>IDateTimeProvider</c> as a method parameter.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DateTimeUsageAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Usage";

        #region XRM0300: Use IDateTimeProvider instead of DateTime.Now / DateTime.UtcNow / DateTime.Today

        private static readonly LocalizableString Xrm0300Title =
            new LocalizableResourceString(nameof(Resources.Xrm0300_Title), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString Xrm0300MessageFormat =
            new LocalizableResourceString(nameof(Resources.Xrm0300_MessageFormat), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString Xrm0300Description =
            new LocalizableResourceString(nameof(Resources.Xrm0300_Description), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Xrm0300 = new DiagnosticDescriptor(
            DiagnosticIds.Xrm0300Id,
            Xrm0300Title,
            Xrm0300MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Xrm0300Description,
            helpLinkUri: DiagnosticIds.HelpLink(DiagnosticIds.Xrm0300Id));

        #endregion

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Xrm0300);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(
                GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            // Hook into member access expressions (DateTime.Now, DateTime.UtcNow, DateTime.Today)
            context.RegisterSyntaxNodeAction(
                AnalyzeMemberAccess,
                SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;

            // Quick filter on the member name before touching the semantic model
            var memberName = memberAccess.Name.Identifier.Text;
            if (memberName != "Now" && memberName != "UtcNow" && memberName != "Today")
            {
                return;
            }

            // Verify via the semantic model that the property actually belongs to System.DateTime
            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol;
            if (symbol == null)
            {
                return;
            }

            if (symbol.Kind != SymbolKind.Property)
            {
                return;
            }

            var containingType = symbol.ContainingType;
            if (containingType == null ||
                containingType.SpecialType != SpecialType.System_DateTime)
            {
                return;
            }

            // Verify that we are inside a plugin class
            var classDeclaration = memberAccess
                .Ancestors()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();

            if (classDeclaration == null)
            {
                return;
            }

            var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
            if (classSymbol == null)
            {
                return;
            }

            if (!IsPluginOrServiceClass(classSymbol))
            {
                return;
            }

            // Build the display text of the full expression (e.g. "DateTime.Now")
            var fullExpression = memberAccess.ToFullString().Trim();

            var diagnostic = Diagnostic.Create(
                Xrm0300,
                memberAccess.GetLocation(),
                fullExpression,          // {0} — the expression used
                memberName);             // {1} — the property name (Now / UtcNow / Today)

            context.ReportDiagnostic(diagnostic);
        }

        /// <summary>
        /// Returns <c>true</c> if the class (or one of its ancestors) inherits from
        /// <c>XrmFramework.Plugin</c> or implements <c>Microsoft.Xrm.Sdk.IPlugin</c>.
        /// </summary>
        private static bool IsPluginOrServiceClass(INamedTypeSymbol classSymbol)
        {
            var current = classSymbol;
            while (current != null)
            {
                // Directly inherits from XrmFramework.Plugin
                if (current.Name == "Plugin" &&
                    current.ContainingNamespace?.ToDisplayString() == "XrmFramework")
                {
                    return true;
                }

                current = current.BaseType;
            }

            // Implements Microsoft.Xrm.Sdk.IPlugin
            return classSymbol.AllInterfaces.Any(i =>
                (i.Name == "IPlugin" &&
                i.ContainingNamespace?.ToDisplayString() == "Microsoft.Xrm.Sdk")
                ||
                (i.Name == "IService" &&
                i.ContainingNamespace?.ToDisplayString() == "XrmFramework"));
        }
    }
}
