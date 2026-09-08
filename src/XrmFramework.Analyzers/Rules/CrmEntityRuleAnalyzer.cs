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
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CrmEntityRuleAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Syntax";

        #region XRM0200 : Use DefinitionClass.EntityName in CrmEntityAttribute
        private static readonly LocalizableString Xrm0200Title = new LocalizableResourceString(nameof(Resources.Xrm0200_Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0200MessageFormat = new LocalizableResourceString(nameof(Resources.Xrm0200_MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0200Description = new LocalizableResourceString(nameof(Resources.Xrm0200_Description), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Xrm0200 = new DiagnosticDescriptor(DiagnosticIds.Xrm0200Id, Xrm0200Title, Xrm0200MessageFormat, Category, DiagnosticSeverity.Error, true, description: Xrm0200Description, helpLinkUri: DiagnosticIds.HelpLink(DiagnosticIds.Xrm0200Id));
        #endregion

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Xrm0200);

        public ImmutableArray<DiagnosticDescriptor> Rules => ImmutableArray.Create(Xrm0200);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.AttributeArgument);
        }

        private static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var attributeArgumentSyntax = (AttributeArgumentSyntax)context.Node;
            var attributesSyntax = context.Node.AncestorsAndSelf().OfType<AttributeSyntax>()
                .FirstOrDefault(a => IsCrmEntityAttribute(a.Name));


            if (attributesSyntax == null || !ReferenceEquals(attributesSyntax.ArgumentList?.Arguments.FirstOrDefault(), attributeArgumentSyntax))
            {
                return;
            }

            // Only a string literal is wrong. Both AccountDefinition.EntityName and
            // typeof(AccountDefinition) name the generated definition, which is the point of
            // the rule.
            if (attributeArgumentSyntax.Expression is LiteralExpressionSyntax)
            {
                var diag = Diagnostic.Create(Xrm0200, attributeArgumentSyntax.GetLocation(), attributeArgumentSyntax.GetText());
                context.ReportDiagnostic(diag);
            }
        }

        /// <summary>
        /// Matches the attribute on its leaf name, so a qualified usage such as
        /// <c>[XrmFramework.CrmEntity(...)]</c> is caught too — comparing the whole name missed it.
        /// </summary>
        private static bool IsCrmEntityAttribute(NameSyntax name)
        {
            var leaf = LeafName(name);

            return leaf == "CrmEntity" || leaf == "CrmEntityAttribute";
        }

        private static string LeafName(NameSyntax name)
        {
            switch (name)
            {
                case IdentifierNameSyntax identifier:
                    return identifier.Identifier.Text;
                case QualifiedNameSyntax qualified:
                    return LeafName(qualified.Right);
                case AliasQualifiedNameSyntax aliased:
                    return LeafName(aliased.Name);
                default:
                    return name.ToString();
            }
        }
    }
}
