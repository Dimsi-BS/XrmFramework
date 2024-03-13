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
    [CLSCompliant(false)]
    public class DateTimeNowRuleAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Syntax";

        #region XRM0300 : Use DefinitionClass.EntityName in CrmEntityAttribute
        private static readonly LocalizableString Xrm0300Title = new LocalizableResourceString(nameof(Resources.Xrm0300_Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0300MessageFormat = new LocalizableResourceString(nameof(Resources.Xrm0300_MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0300Description = new LocalizableResourceString(nameof(Resources.Xrm0300_Description), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Xrm0300 = new DiagnosticDescriptor(DiagnosticIds.Xrm0300Id, Xrm0300Title, Xrm0300MessageFormat, Category, DiagnosticSeverity.Warning, true, description: Xrm0300Description);
        #endregion

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Xrm0300);

        public ImmutableArray<DiagnosticDescriptor> Rules => ImmutableArray.Create(Xrm0300);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var memberAccessExpressionSyntax = (MemberAccessExpressionSyntax)context.Node;

            var firstIdentifier = memberAccessExpressionSyntax.ChildNodes().First() as IdentifierNameSyntax;
            var secondIdentifier = memberAccessExpressionSyntax.ChildNodes().Last() as IdentifierNameSyntax;

            if (firstIdentifier == null || secondIdentifier == null || firstIdentifier.Identifier.Text != "DateTime" || (secondIdentifier.Identifier.Text != "Now" && secondIdentifier.Identifier.Text != "UtcNow"))
            {
                return;
            }

            var objectSymbol = context.SemanticModel.GetSymbolInfo(firstIdentifier);

            if (objectSymbol is { Symbol: { ContainingNamespace: { Name: "System" }, Name: "DateTime" } })
            {
                var diag = Diagnostic.Create(Xrm0300, memberAccessExpressionSyntax.GetLocation(), memberAccessExpressionSyntax.GetText());
                context.ReportDiagnostic(diag);
            }
        }
    }
}
