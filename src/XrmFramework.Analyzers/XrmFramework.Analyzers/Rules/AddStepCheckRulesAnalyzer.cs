using System.Linq;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using InvocationExpressionSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax;

namespace XrmFramework.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddStepCheckRulesAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Syntax";

        #region XRM0010 : Callback Method does not exist
        private static readonly LocalizableString Xrm0010Title = new LocalizableResourceString(nameof(Resources.Xrm0010_Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0010MessageFormat = new LocalizableResourceString(nameof(Resources.Xrm0010_MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0010Description = new LocalizableResourceString(nameof(Resources.Xrm0010_Description), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Xrm0010 = new DiagnosticDescriptor(DiagnosticIds.Xrm0010Id, Xrm0010Title, Xrm0010MessageFormat, Category, DiagnosticSeverity.Error, true, description: Xrm0010Description);
        #endregion

        #region XRM0011 : EntityName should be called through Definition class
        private static readonly LocalizableString Xrm0011Title = new LocalizableResourceString(nameof(Resources.Xrm0011_Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0011MessageFormat = new LocalizableResourceString(nameof(Resources.Xrm0011_MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0011Description = new LocalizableResourceString(nameof(Resources.Xrm0011_Description), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Xrm0011 = new DiagnosticDescriptor(DiagnosticIds.Xrm0011Id, Xrm0011Title, Xrm0011MessageFormat, Category, DiagnosticSeverity.Warning, true, description: Xrm0011Description);
        #endregion

        #region XRM0012 : Use nameof to reference callback
        private static readonly LocalizableString Xrm0012Title = new LocalizableResourceString(nameof(Resources.Xrm0012_Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0012MessageFormat = new LocalizableResourceString(nameof(Resources.Xrm0012_MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0012Description = new LocalizableResourceString(nameof(Resources.Xrm0012_Description), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Xrm0012 = new DiagnosticDescriptor(DiagnosticIds.Xrm0012Id, Xrm0012Title, Xrm0012MessageFormat, Category, DiagnosticSeverity.Warning, true, description: Xrm0012Description);
        #endregion

        #region XRM0013 : Invalid callback method
        private static readonly LocalizableString Xrm0013Title = new LocalizableResourceString(nameof(Resources.Xrm0013_Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0013MessageFormat = new LocalizableResourceString(nameof(Resources.Xrm0013_MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0013Description = new LocalizableResourceString(nameof(Resources.Xrm0013_Description), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Xrm0013 = new DiagnosticDescriptor(DiagnosticIds.Xrm0013Id, Xrm0013Title, Xrm0013MessageFormat, Category, DiagnosticSeverity.Error, true, description: Xrm0013Description);
        #endregion

        #region XRM0014 : AddStep should be called from AddSteps method
        private static readonly LocalizableString Xrm0014Title = new LocalizableResourceString(nameof(Resources.Xrm0014_Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0014MessageFormat = new LocalizableResourceString(nameof(Resources.Xrm0014_MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Xrm0014Description = new LocalizableResourceString(nameof(Resources.Xrm0014_Description), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Xrm0014 = new DiagnosticDescriptor(DiagnosticIds.Xrm0014Id, Xrm0014Title, Xrm0014MessageFormat, Category, DiagnosticSeverity.Error, true, description: Xrm0014Description);
        #endregion

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Xrm0010, Xrm0011, Xrm0012, Xrm0013, Xrm0014);

        public ImmutableArray<DiagnosticDescriptor> Rules => ImmutableArray.Create(Xrm0010, Xrm0011, Xrm0012, Xrm0013, Xrm0014);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpressionNode = (InvocationExpressionSyntax)context.Node;

            var symbol = context.SemanticModel.GetSymbolInfo(invocationExpressionNode).Symbol;

            if (symbol == null)
            {
                return;
            }

            if (symbol.Name != "AddStep" || symbol.ContainingType.Name != "Plugin" || symbol.ContainingNamespace.Name != "XrmFramework")
            {
                return;
            }

            if (!IsCalledFromAddSteps(invocationExpressionNode, context.SemanticModel))
            {
                var diag = Diagnostic.Create(Xrm0014, invocationExpressionNode.GetLocation());
                context.ReportDiagnostic(diag);
            }

            var argumentEntityName = invocationExpressionNode.ArgumentList.Arguments.ElementAt(3);

            if (argumentEntityName.Expression.IsKind(SyntaxKind.StringLiteralExpression))
            {
                var diag = Diagnostic.Create(Xrm0011, argumentEntityName.GetLocation());
                context.ReportDiagnostic(diag);
            }

            var argumentMethodName = invocationExpressionNode.ArgumentList.Arguments.ElementAt(4);

            if (argumentMethodName.Expression is InvocationExpressionSyntax invocationSyntax)
            {
                if (invocationSyntax.Expression is IdentifierNameSyntax ins)
                {
                    if (ins.Identifier.Text == "nameof")
                    {
                        var nameofArgumentSyntax = invocationSyntax.ArgumentList.Arguments.Single();

                        var targettedSymbols = context.SemanticModel.GetSymbolInfo(nameofArgumentSyntax.Expression);

                        var targettedSymbol = targettedSymbols.Symbol ?? targettedSymbols.CandidateSymbols.FirstOrDefault();

                        if (targettedSymbol is IMethodSymbol methodSymbol)
                        {
                            if (methodSymbol.MethodKind != MethodKind.Ordinary)
                            {

                                var diagnostic = Diagnostic.Create(Xrm0013, nameofArgumentSyntax.Expression.GetLocation(), nameofArgumentSyntax.GetText());
                                context.ReportDiagnostic(diagnostic);
                            }
                        }
                        else
                        {
                            var diagnostic = Diagnostic.Create(Xrm0013, nameofArgumentSyntax.Expression.GetLocation(), nameofArgumentSyntax.GetText());
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                    else
                    {
                        var diagnostic = Diagnostic.Create(Xrm0013, invocationSyntax.Expression.GetLocation(), invocationSyntax.GetText());
                        context.ReportDiagnostic(diagnostic);
                    }
                }
                else
                {
                    var diagnostic = Diagnostic.Create(Xrm0013, invocationSyntax.Expression.GetLocation(), invocationSyntax.GetText());
                    context.ReportDiagnostic(diagnostic);
                }

            }
            else if (argumentMethodName.Expression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
            {
                var result = context.SemanticModel.GetConstantValue(argumentMethodName.Expression);
                var diagnostic = Diagnostic.Create(Xrm0010, argumentMethodName.GetLocation(), "AddStep", symbol.ContainingType.Name);

                context.ReportDiagnostic(diagnostic);
            }
            else if (argumentMethodName.Expression.Kind() == SyntaxKind.StringLiteralExpression)
            {
                var classDeclarationNode = context.Node.Ancestors().First(n => n.Kind() == SyntaxKind.ClassDeclaration) as ClassDeclarationSyntax;

                var parentTypeSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationNode);

                var methodName = argumentMethodName.Expression.ToFullString().Replace("\"", string.Empty);

                if (!HasMember(parentTypeSymbol, methodName))
                {
                    var diag = Diagnostic.Create(Xrm0010, argumentMethodName.Expression.GetLocation(), methodName, parentTypeSymbol.Name);
                    context.ReportDiagnostic(diag);
                }
                else
                {
                    var diag = Diagnostic.Create(Xrm0012, argumentMethodName.Expression.GetLocation(), methodName);
                    context.ReportDiagnostic(diag);
                }

            }

        }

        private static bool IsCalledFromAddSteps(InvocationExpressionSyntax invocationExpressionNode, SemanticModel semanticModel)
        {
            var containingMethodSyntax = invocationExpressionNode.FirstAncestorOrSelf<MemberDeclarationSyntax>();

            var blockSyntax = (BlockSyntax)containingMethodSyntax.ChildNodes().Last();

            var controlFlow = semanticModel.AnalyzeControlFlow(blockSyntax.Statements.First(), blockSyntax.Statements.Last());

            if (controlFlow.Succeeded)
            {

            }

            var token = containingMethodSyntax.DescendantTokens().FirstOrDefault(f => f.IsKind(SyntaxKind.IdentifierToken));

            return token.Text == "AddSteps";
        }

        private static bool HasMember(ITypeSymbol type, string memberName)
        {
            var methods = type.GetMembers(memberName);
            if (methods.Any())
            {
                return true;
            }

            if (type.BaseType != null)
            {
                return HasMember(type.BaseType, memberName);
            }

            return false;
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddStepCheckRulesAnalyzer)), Shared]
    public class AddStepCheckRulesCodeFixProvider : CodeFixProvider
    {
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var title = "Use nameof";

            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;// Find the type declaration identified by the diagnostic.
            var argument = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ArgumentSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(title, c => UseNameof(context.Document, root, argument, c), equivalenceKey: title),
                diagnostic);

        }

        private Task<Document> UseNameof(Document document, SyntaxNode root, ArgumentSyntax argument, CancellationToken cancellationToken)
        {
            var nameToken = ((LiteralExpressionSyntax)argument.Expression).Token.Text;
            var newArgumentDeclaration = argument.WithExpression(SyntaxFactory.ParseExpression($"nameof({nameToken.Trim('"')})"));

            var newRoot = root.ReplaceNode(argument, newArgumentDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return Task.FromResult(newDocument);
        }

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticIds.Xrm0012Id);
    }
}
