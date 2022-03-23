using System;
using System.Linq;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

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

        private static readonly DiagnosticDescriptor Xrm0200 = new DiagnosticDescriptor(DiagnosticIds.Xrm0200Id, Xrm0200Title, Xrm0200MessageFormat, Category, DiagnosticSeverity.Error, true, description: Xrm0200Description);
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
                .FirstOrDefault(a => a.Name.ToString() == "CrmEntity" || a.Name.ToString() == "CrmEntityAttribute");


            if (attributesSyntax == null)
            {
                return;
            }

            if (attributeArgumentSyntax.Expression is LiteralExpressionSyntax)
            {
                var diag = Diagnostic.Create(Xrm0200, attributeArgumentSyntax.GetLocation(), attributeArgumentSyntax.GetText());
                context.ReportDiagnostic(diag);
            }
        }
    }
}
