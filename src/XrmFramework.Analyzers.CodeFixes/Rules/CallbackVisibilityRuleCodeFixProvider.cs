using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XrmFramework.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CallbackVisibilityRuleCodeFixProvider)), Shared]
    public class CallbackVisibilityRuleCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticIds.Xrm0002Id);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var title = "Make method public";
            foreach (var diagnostic in context.Diagnostics)
            {
                var sourceTree = diagnostic.Location.SourceTree;

                if (sourceTree == null)
                {
                    continue;
                }

                var methodRoot = await sourceTree.GetRootAsync(context.CancellationToken).ConfigureAwait(false);
                var methodDeclaration = methodRoot.FindNode(diagnostic.Location.SourceSpan) as MethodDeclarationSyntax;

                context.RegisterCodeFix(
                    CodeAction.Create(title, c => MakePublic(context.Document, methodDeclaration, c), title),
                    diagnostic);
            }
        }

        public override FixAllProvider GetFixAllProvider() => null;

        private async Task<Document> MakePublic(Document document, MethodDeclarationSyntax methodDecl, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            if (root == null)
            {
                return document;
            }

            var newModifiers = SyntaxFactory.TokenList(new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) });
            var newMethodDeclaration = methodDecl.WithModifiers(newModifiers);

            var newRoot = root.ReplaceNode(methodDecl, newMethodDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }
    }
}
