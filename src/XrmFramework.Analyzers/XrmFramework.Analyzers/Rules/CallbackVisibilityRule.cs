using System.Linq;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using System.Threading;

namespace XrmFramework.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CallbackVisibilityRule : CodeFixProvider
    {
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.CheckCallbackVisibilityTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.CheckCallbackVisibilityMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.CheckCallbackVisibilityDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(DiagnosticIds.CheckCallbackVisibility, Title, MessageFormat, Category, DiagnosticSeverity.Error, true, description: Description);

        public ImmutableArray<DiagnosticDescriptor> Rules => ImmutableArray.Create(_rule);

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(_rule.Id);

        public void InitializeDiagnostics(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeMethodAnalysis, SyntaxKind.MethodDeclaration);
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var title = "Make method public";
            foreach (var diagnostic in context.Diagnostics)
            {
                var methodLocation = diagnostic.Location;

                var methodRoot = await methodLocation.SourceTree.GetRootAsync(context.CancellationToken).ConfigureAwait(false);
                var methodDeclaration = methodRoot.FindNode(methodLocation.SourceSpan) as MethodDeclarationSyntax;

                context.RegisterCodeFix(
                    CodeAction.Create(title, createChangedDocument: c => MakePublic(context.Document, methodDeclaration, c), equivalenceKey: title),
                    diagnostic);
            }
        }

        private async Task<Document> MakePublic(Document document, MethodDeclarationSyntax methodDecl, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var newModifiers = SyntaxFactory.TokenList(new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) });
            var newMethodDeclaration = methodDecl.WithModifiers(newModifiers);

            var newRoot = root.ReplaceNode(methodDecl, newMethodDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private static void AnalyzeMethodAnalysis(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = context.Node as MethodDeclarationSyntax;

            var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken) as IMethodSymbol;

            var typeSyntaxReferences = methodSymbol.ContainingType.DeclaringSyntaxReferences;

            var methodIsUsed = false;

            foreach (var typeSyntaxRef in typeSyntaxReferences)
            {
                var typeNode = typeSyntaxRef.GetSyntax(context.CancellationToken);

                var methodCalls = typeNode.DescendantNodes().OfType<InvocationExpressionSyntax>();

                foreach (var methodCall in methodCalls)
                {
                    var calledMethodSymbol = context.SemanticModel.Compilation.GetSemanticModel(methodCall.SyntaxTree).GetSymbolInfo(methodCall).Symbol as IMethodSymbol;

                    if (calledMethodSymbol == null || calledMethodSymbol.Name != "AddStep" || calledMethodSymbol.ContainingType.Name != "Plugin")
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
                            var fieldInfo = context.SemanticModel.GetSymbolInfo((MemberAccessExpressionSyntax)argumentExpression).Symbol as IFieldSymbol;
                            if (fieldInfo != null)
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
                if (methodSymbol.IsAbstract || methodSymbol.IsStatic || methodSymbol.DeclaredAccessibility != Accessibility.Public)
                {
                    var diag = Diagnostic.Create(_rule, methodDeclaration.Identifier.GetLocation(), methodSymbol.ContainingType.Name, methodSymbol.Name);
                    context.ReportDiagnostic(diag);
                }
            }
        }
    }
}
