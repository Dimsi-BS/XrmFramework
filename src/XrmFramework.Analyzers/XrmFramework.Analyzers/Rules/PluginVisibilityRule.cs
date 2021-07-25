using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Composition;
using System.Threading;
using Microsoft.CodeAnalysis.CodeActions;

namespace XrmFramework.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PluginVisibilityRuleAnalyzer : DiagnosticAnalyzer // CodeFixProvider
    {
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.PluginVisibilityTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.PluginVisibilityMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.PluginVisibilityDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Syntax";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(DiagnosticIds.CheckPluginVisibility, Title, MessageFormat, Category, DiagnosticSeverity.Error, true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ClassDeclarationSyntax classDeclaration)
            {
                var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);

                if (classSymbol.AllInterfaces.All(i => i.Name != "IPlugin"))
                {
                    return;
                }

                var modifiers = classDeclaration.Modifiers;

                if (!modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                {
                    var diag = Diagnostic.Create(_rule, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier.Text);
                    context.ReportDiagnostic(diag);
                }
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PluginVisibilityRuleCodeFixProvider)), Shared]
    public class PluginVisibilityRuleCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticIds.CheckPluginVisibility);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var title = "Make class public";

            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;// Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(title, createChangedSolution: c => MakePublic(context.Document, root, declaration, c), equivalenceKey: title),
                diagnostic);

        }

        private Task<Solution> MakePublic(Document document, SyntaxNode root, ClassDeclarationSyntax typeDecl, CancellationToken cancellationToken)
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
