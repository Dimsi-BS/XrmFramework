using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace XrmFramework.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PluginVisibilityRuleCodeFixProvider)), Shared]
    public class PluginVisibilityRuleCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticIds.Xrm0003Id);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var title = "Make class public";

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

            var declaration = token.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(title, _ => MakePublic(context.Document, root, declaration), title),
                diagnostic);
        }

        private Task<Solution> MakePublic(Document document, SyntaxNode root, ClassDeclarationSyntax typeDecl)
        {
            var modifiers = typeDecl.Modifiers;

            var listModifiers = new List<SyntaxToken> { SyntaxFactory.Token(SyntaxKind.PublicKeyword) };

            foreach (var token in modifiers)
            {
                if (token.IsKind(SyntaxKind.InternalKeyword) || token.IsKind(SyntaxKind.PrivateKeyword) || token.IsKind(SyntaxKind.ProtectedKeyword))
                {
                    continue;
                }
                listModifiers.Add(token);
            }

            var newModifiers = SyntaxFactory.TokenList(listModifiers);

            var newClassDeclaration = typeDecl.WithModifiers(newModifiers);

            var newRoot = root.ReplaceNode(typeDecl, newClassDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return Task.FromResult(newDocument.Project.Solution);
        }
    }
}
