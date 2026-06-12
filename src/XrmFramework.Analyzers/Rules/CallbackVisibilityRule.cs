using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
// ReSharper disable ArrangeObjectCreationWhenTypeEvident

namespace XrmFramework.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CallbackVisibilityRuleAnalyzer : DiagnosticAnalyzer
    {
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.CheckCallbackVisibilityTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.CheckCallbackVisibilityMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.CheckCallbackVisibilityDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.Xrm0002Id, Title, MessageFormat, Category, DiagnosticSeverity.Error, true, description: Description, helpLinkUri: DiagnosticIds.HelpLink(DiagnosticIds.Xrm0002Id));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeMethodAnalysis, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMethodAnalysis(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not MethodDeclarationSyntax methodDeclaration)
            {
                return;
            }

            var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            if (methodSymbol == null)
            {
                return;
            }

            if (!methodSymbol.IsAbstract && !methodSymbol.IsStatic && methodSymbol.DeclaredAccessibility == Accessibility.Public)
            {
                return;
            }

            var typeSyntaxReferences = methodSymbol.ContainingType.DeclaringSyntaxReferences;

            var methodIsUsed = false;

            foreach (var typeSyntaxRef in typeSyntaxReferences)
            {
                var typeNode = typeSyntaxRef.GetSyntax(context.CancellationToken);

                var methodCalls = typeNode.DescendantNodes().OfType<InvocationExpressionSyntax>();

                foreach (var methodCall in methodCalls)
                {
#pragma warning disable RS1030 // Do not invoke Compilation.GetSemanticModel() method within a diagnostic analyzer
                    var semanticModel = context.SemanticModel.Compilation.GetSemanticModel(methodCall.SyntaxTree);
#pragma warning restore RS1030 // Do not invoke Compilation.GetSemanticModel() method within a diagnostic analyzer


                    if (semanticModel.GetSymbolInfo(methodCall).Symbol is not IMethodSymbol calledMethodSymbol || calledMethodSymbol.Name != "AddStep" || calledMethodSymbol.ContainingType.Name != "Plugin")
                    {
                        continue;
                    }
                    var argumentExpression = methodCall.ArgumentList.Arguments.ElementAt(4).Expression;

                    switch (argumentExpression.Kind())
                    {
                        case SyntaxKind.StringLiteralExpression:
                            methodIsUsed |= ((LiteralExpressionSyntax)argumentExpression).Token.ValueText == methodSymbol.Name;
                            break;
                        case SyntaxKind.PointerMemberAccessExpression:
                            if (context.SemanticModel.GetSymbolInfo((MemberAccessExpressionSyntax)argumentExpression).Symbol is IFieldSymbol fieldInfo)
                            {
                                methodIsUsed |= (fieldInfo.HasConstantValue && methodSymbol.Name.Equals(fieldInfo.ConstantValue));
                            }
                            break;
                        case SyntaxKind.InvocationExpression:
                            var syntax = argumentExpression as InvocationExpressionSyntax;
                            var identifier = syntax?.Expression as IdentifierNameSyntax;
                            if (identifier?.Identifier.Text == "nameof")
                            {
                                var callSyntax = syntax.ArgumentList.Arguments[0].Expression as IdentifierNameSyntax;
                                methodIsUsed |= callSyntax?.Identifier.Text == methodSymbol.Name;
                            }
                            break;
                    }
                    if (methodIsUsed)
                    {
                        break;
                    }
                }
                if (methodIsUsed)
                {
                    break;
                }
            }

            if (methodIsUsed)
            {
                var diagnostic = Diagnostic.Create(Rule, methodDeclaration.Identifier.GetLocation(), methodSymbol.ContainingType.Name, methodSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

}
