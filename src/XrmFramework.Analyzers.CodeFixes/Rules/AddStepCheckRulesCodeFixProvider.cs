using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using InvocationExpressionSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax;

namespace XrmFramework.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddStepCheckRulesCodeFixProvider)), Shared]
    public class AddStepCheckRulesCodeFixProvider : CodeFixProvider
    {
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var title = "Use nameof";

            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            if (root == null)
            {
                return;
            }

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var token = root.FindToken(diagnosticSpan.Start).Parent;

            if (token == null)
            {
                return;
            }

            var argument = token.AncestorsAndSelf().OfType<ArgumentSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(title, _ => UseNameof(context.Document, root, argument), title),
                diagnostic);
        }

        private Task<Document> UseNameof(Document document, SyntaxNode root, ArgumentSyntax argument)
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
