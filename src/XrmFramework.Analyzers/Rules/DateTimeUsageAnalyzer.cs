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
    /// XRM0300 — Interdit l'usage direct de <c>DateTime.Now</c> et <c>DateTime.UtcNow</c>
    /// dans les classes qui héritent de <c>XrmFramework.Plugin</c> ou implémentent
    /// <c>Microsoft.Xrm.Sdk.IPlugin</c>.
    /// Recommande d'injecter <c>IDateTimeProvider</c> comme paramètre de méthode.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DateTimeUsageAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Usage";

        #region XRM0300 : Utiliser IDateTimeProvider plutôt que DateTime.Now / DateTime.UtcNow

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

            // On s'accroche sur les accès de membre (DateTime.Now, DateTime.UtcNow)
            context.RegisterSyntaxNodeAction(
                AnalyzeMemberAccess,
                SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;

            // Filtre rapide sur le nom du membre avant de toucher au modèle sémantique
            var memberName = memberAccess.Name.Identifier.Text;
            if (memberName != "Now" && memberName != "UtcNow")
            {
                return;
            }

            // Vérifie via le modèle sémantique que la propriété appartient bien à System.DateTime
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

            // Vérifie que l'on se trouve dans une classe plugin
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

            // Construit le texte d'affichage de l'expression complète (ex. "DateTime.Now")
            var fullExpression = memberAccess.ToFullString().Trim();

            var diagnostic = Diagnostic.Create(
                Xrm0300,
                memberAccess.GetLocation(),
                fullExpression,          // {0} — expression utilisée
                memberName);             // {1} — nom de la propriété (Now / UtcNow)

            context.ReportDiagnostic(diagnostic);
        }

        /// <summary>
        /// Retourne <c>true</c> si la classe (ou l'un de ses ancêtres) hérite de
        /// <c>XrmFramework.Plugin</c> ou implémente <c>Microsoft.Xrm.Sdk.IPlugin</c>.
        /// </summary>
        private static bool IsPluginOrServiceClass(INamedTypeSymbol classSymbol)
        {
            var current = classSymbol;
            while (current != null)
            {
                // Hérite directement de XrmFramework.Plugin
                if (current.Name == "Plugin" &&
                    current.ContainingNamespace?.ToDisplayString() == "XrmFramework")
                {
                    return true;
                }

                current = current.BaseType;
            }

            // Implémente Microsoft.Xrm.Sdk.IPlugin
            return classSymbol.AllInterfaces.Any(i =>
                (i.Name == "IPlugin" &&
                i.ContainingNamespace?.ToDisplayString() == "Microsoft.Xrm.Sdk")
                ||
                (i.Name == "IService" &&
                i.ContainingNamespace?.ToDisplayString() == "XrmFramework"));
        }
    }
}
